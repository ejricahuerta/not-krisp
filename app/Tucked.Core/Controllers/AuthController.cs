using Microsoft.AspNetCore.Authentication;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tucked.Core.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Tucked.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Tucked.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly GitHubApiService _gitHubApiService;
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, GitHubApiService gitHubApiService)
        {
            _userManager = userManager;
            _config = config;
            _gitHubApiService = gitHubApiService;
        }

        [HttpGet("github")]
        [AllowAnonymous]
        public IActionResult GitHubLogin(string? returnUrl = "/")
        {
            var host = HttpContext.Request.Host.Value;
            var scheme = HttpContext.Request.Scheme;
            var redirectUri = HttpUtility.UrlEncode($"{scheme}://{host}/api/auth/github/callback");
            var url = $"https://github.com/login/oauth/authorize?client_id={_config["GitHub:ClientId"]}&scope=user:email&redirect_uri={redirectUri}";
            // redirect to github oauth
            return Redirect(url);
        }

        [HttpGet("github/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GitHubCallback([FromQuery] string code)
        {
            ApplicationUser? user = null;
            string? accessToken = null;
            string? refreshToken = null;
            GitHubUser? githubUser = null;

            try
            {
                (accessToken, refreshToken) = await ExchangeCodeForTokenAsync(code);
                githubUser = await _gitHubApiService.GetUserAsync(accessToken);
                if (githubUser == null)
                {
                    return StatusCode(500, "Failed to fetch GitHub user profile.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in GitHubCallback: " + ex.Message);
                return StatusCode(500, ex.Message);
            }

            try
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.GitHubId == githubUser.Id.ToString());
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = githubUser.Login,
                        Email = githubUser.Email,
                        GitHubId = githubUser.Id.ToString(),
                        AvatarUrl = githubUser.AvatarUrl,
                        GitHubAccessToken = accessToken,
                        GitHubRefreshToken = refreshToken
                    };
                    await _userManager.CreateAsync(user);
                }
                else
                {
                    user.GitHubAccessToken = accessToken;
                    user.GitHubRefreshToken = refreshToken;
                    user.AvatarUrl = githubUser.AvatarUrl;
                    user.Email = githubUser.Email;
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in User Fetching: " + ex.Message);
                return StatusCode(500, ex.Message);
            }

            try
            {
                var jwt = GenerateJwtToken(user);
                return Ok(new
                {
                    token = jwt,
                    user = new
                    {
                        user.UserName,
                        user.Email,
                        user.AvatarUrl,
                        user.GitHubId
                    }
                });
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error in JWT Generation: " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var keyString = _config["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("avatar_url", user.AvatarUrl ?? ""),
                new Claim("github_id", user.GitHubId ?? "")
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<(string accessToken, string refreshToken)> ExchangeCodeForTokenAsync(string code)
        {
            var clientId = _config["GitHub:ClientId"] ?? throw new InvalidOperationException("GitHub ClientId is not configured.");
            var clientSecret = _config["GitHub:ClientSecret"] ?? throw new InvalidOperationException("GitHub ClientSecret is not configured.");
            var redirectUri = "http://localhost:5149/api/auth/github/callback";
            using var httpClient = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("code", code ?? string.Empty),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                })
            };
            tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var tokenResponse = await httpClient.SendAsync(tokenRequest);
            if (!tokenResponse.IsSuccessStatusCode)
            {
                System.Console.WriteLine("Failed to exchange code for access token");
                throw new Exception("Failed to exchange code for access token");
            }
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            using var tokenDoc = JsonDocument.Parse(tokenJson);
            var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString();
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("No access token returned from GitHub.");
            var refreshToken = tokenDoc.RootElement.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : string.Empty;
            return (accessToken, refreshToken ?? string.Empty);
        }
    }
}