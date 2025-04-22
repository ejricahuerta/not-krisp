using Microsoft.EntityFrameworkCore;
using NotKrisp.API.Data;
using NotKrisp.API.Models;
using Octokit;
using Microsoft.Extensions.Configuration;
using NotKrisp.API.Services.Interfaces;

namespace NotKrisp.API.Services;

public interface IGitHubService
{
    Task<string> GetAuthorizationUrl(string redirectUri);
    Task<bool> HandleAuthorizationCallback(string code);
    Task<Issue> CreateIssue(string repositoryOwner, string repositoryName, string title, string body);
    Task<IEnumerable<Repository>> GetRepositories();
}

public class GitHubService : IGitHubService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubService> _logger;
    private readonly GitHubClient _client;

    public GitHubService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<GitHubService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _client = new GitHubClient(new ProductHeaderValue("NotKrisp"));
    }

    public async Task<string> GetAuthorizationUrl(string redirectUri)
    {
        _logger.LogInformation("Getting GitHub authorization URL for redirectUri: {RedirectUri}", redirectUri);

        try
        {
            var clientId = _configuration["GitHub:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("GitHub ClientId is not configured");
            }

            _logger.LogInformation("Using GitHub ClientId from configuration");

            var request = new OauthLoginRequest(clientId)
            {
                RedirectUri = new Uri(redirectUri),
                Scopes = { "repo" }
            };

            var url = _client.Oauth.GetGitHubLoginUrl(request).ToString();
            _logger.LogInformation("Generated GitHub authorization URL: {Url}", url);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub authorization URL");
            throw;
        }
    }

    public async Task<bool> HandleAuthorizationCallback(string code)
    {
        _logger.LogInformation("Handling GitHub authorization callback with code: {Code}", code);

        try
        {
            var clientId = _configuration["GitHub:ClientId"];
            var clientSecret = _configuration["GitHub:ClientSecret"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new InvalidOperationException("GitHub credentials are not configured");
            }

            _logger.LogInformation("Using GitHub credentials from configuration");

            var request = new OauthTokenRequest(clientId, clientSecret, code);

            _logger.LogInformation("Requesting access token from GitHub");
            var token = await _client.Oauth.CreateAccessToken(request);
            _logger.LogInformation("Successfully received access token from GitHub");

            // Store the access token in the database
            var settings = await GetGitHubSettings();
            settings.AccessToken = token.AccessToken;
            settings.IsEnabled = true;
            settings.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Saving updated GitHub settings");
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully saved GitHub settings");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub authorization callback");
            return false;
        }
    }

    public async Task<Issue> CreateIssue(string repositoryOwner, string repositoryName, string title, string body)
    {
        _logger.LogInformation(
            "Creating GitHub issue. Repository: {Owner}/{Name}, Title: {Title}",
            repositoryOwner,
            repositoryName,
            title
        );

        try
        {
            var settings = await GetGitHubSettings();
            _logger.LogInformation("Retrieved GitHub settings. Access token available: {HasToken}", !string.IsNullOrEmpty(settings.AccessToken));

            _client.Credentials = new Credentials(settings.AccessToken);

            var newIssue = new NewIssue(title)
            {
                Body = body
            };

            _logger.LogInformation("Sending issue creation request to GitHub");
            var issue = await _client.Issue.Create(repositoryOwner, repositoryName, newIssue);
            _logger.LogInformation("Successfully created GitHub issue: {IssueUrl}", issue.HtmlUrl);

            return issue;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating GitHub issue. Repository: {Owner}/{Name}, Title: {Title}",
                repositoryOwner,
                repositoryName,
                title
            );
            throw;
        }
    }

    public async Task<IEnumerable<Repository>> GetRepositories()
    {
        _logger.LogInformation("Getting GitHub repositories");

        try
        {
            var settings = await GetGitHubSettings();
            _logger.LogInformation("Retrieved GitHub settings. Access token available: {HasToken}", !string.IsNullOrEmpty(settings.AccessToken));

            _client.Credentials = new Credentials(settings.AccessToken);

            _logger.LogInformation("Requesting repositories from GitHub");
            var repositories = await _client.Repository.GetAllForCurrent();
            _logger.LogInformation("Successfully retrieved {Count} repositories", repositories.Count());

            return repositories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub repositories");
            throw;
        }
    }

    private async Task<IntegrationSettings> GetGitHubSettings()
    {
        _logger.LogInformation("Getting GitHub integration settings from database");

        try
        {
            var settings = await _context.IntegrationSettings
                .FirstOrDefaultAsync(s => s.IntegrationType == "GitHub");

            if (settings == null)
            {
                _logger.LogInformation("Creating new GitHub integration settings");
                settings = new IntegrationSettings
                {
                    IntegrationType = "GitHub",
                    IsEnabled = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.IntegrationSettings.Add(settings);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Successfully retrieved GitHub settings. IsEnabled: {IsEnabled}", settings.IsEnabled);
            return settings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub settings from database");
            throw;
        }
    }
}