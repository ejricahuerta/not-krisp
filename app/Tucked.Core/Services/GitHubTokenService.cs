using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Tucked.Core.Data;
using Tucked.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Tucked.Core.Services;

public class GitHubTokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GitHubTokenService> _logger;
    private readonly IConfiguration _configuration;

    public GitHubTokenService(
        IHttpClientFactory httpClientFactory,
        ApplicationDbContext dbContext,
        ILogger<GitHubTokenService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> ValidateTokenAsync(string accessToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Tucked", "1.0"));

            var response = await client.GetAsync("https://api.github.com/user");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating GitHub token");
            return false;
        }
    }

    public async Task<string?> GetValidTokenAsync(string userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.GitHubTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user?.GitHubTokens == null || !user.GitHubTokens.Any())
        {
            _logger.LogWarning("No tokens found for user {UserId}", userId);
            return null;
        }

        var token = user.GitHubTokens.OrderByDescending(t => t.CreatedAt).First();
        
        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogInformation("Token expired for user {UserId}, attempting refresh", userId);
            if (string.IsNullOrEmpty(token.RefreshToken))
            {
                _logger.LogWarning("No refresh token available for user {UserId}", userId);
                return null;
            }
            var newToken = await RefreshTokenAsync(token.RefreshToken);
            if (newToken != null)
            {
                await UpdateUserTokenAsync(userId, newToken);
                return newToken.AccessToken;
            }
            return null;
        }

        if (!await ValidateTokenAsync(token.AccessToken))
        {
            _logger.LogWarning("Token validation failed for user {UserId}", userId);
            return null;
        }

        return token.AccessToken;
    }

    private async Task<GitHubToken?> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new ArgumentNullException(nameof(refreshToken), "Refresh token cannot be null or empty.");
        try
        {
            var client = _httpClientFactory.CreateClient();
            var clientId = _configuration["GitHub:ClientId"];
            var clientSecret = _configuration["GitHub:ClientSecret"];

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            });

            var response = await client.PostAsync("https://github.com/login/oauth/access_token", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to refresh token. Status code: {StatusCode}", response.StatusCode);
                return null;
            }

            var result = await response.Content.ReadAsStringAsync();
            var values = result.Split('&')
                .Select(v => v.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);

            if (!values.ContainsKey("access_token"))
            {
                _logger.LogError("Refresh token response did not contain access_token");
                return null;
            }

            return new GitHubToken
            {
                AccessToken = values["access_token"],
                RefreshToken = values.GetValueOrDefault("refresh_token", refreshToken),
                TokenType = values.GetValueOrDefault("token_type", "bearer"),
                Scope = values.GetValueOrDefault("scope", ""),
                ExpiresAt = DateTime.UtcNow.AddHours(8) // GitHub tokens typically expire in 8 hours
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing GitHub token");
            return null;
        }
    }

    private async Task UpdateUserTokenAsync(string userId, GitHubToken newToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.GitHubTokens)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            _logger.LogError("User {UserId} not found when updating token", userId);
            return;
        }

        var token = new GitHubToken
        {
            UserId = userId,
            AccessToken = newToken.AccessToken,
            RefreshToken = newToken.RefreshToken,
            TokenType = newToken.TokenType,
            Scope = newToken.Scope,
            ExpiresAt = newToken.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        user.GitHubTokens.Add(token);
        await _dbContext.SaveChangesAsync();
    }
} 