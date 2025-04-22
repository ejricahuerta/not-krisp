using Microsoft.AspNetCore.Mvc;
using NotKrisp.API.Services;

namespace NotKrisp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GitHubController : ControllerBase
{
    private readonly IGitHubService _gitHubService;
    private readonly ILogger<GitHubController> _logger;
    private readonly IConfiguration _configuration;
    public GitHubController(
        IGitHubService gitHubService,
        ILogger<GitHubController> logger,
        IConfiguration configuration)
    {
        _gitHubService = gitHubService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("authorize")]
    public async Task<IActionResult> GetAuthorizationUrl([FromQuery] string redirectUri)
    {
        _logger.LogInformation("Received request to get GitHub authorization URL with redirectUri: {RedirectUri}", redirectUri);

        try
        {
            _logger.LogInformation("Calling GitHubService to get authorization URL");
            var url = await _gitHubService.GetAuthorizationUrl(redirectUri);
            _logger.LogInformation("Successfully generated GitHub authorization URL: {Url}", url);

            return Ok(new { url });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub authorization URL. RedirectUri: {RedirectUri}", redirectUri);
            return StatusCode(500, new { error = "Failed to get authorization URL", details = ex.Message });
        }

    }

    [HttpGet("callback")]
    public async Task<IActionResult> HandleCallback([FromQuery] string code, [FromQuery] string redirectUri)
    {
        _logger.LogInformation("Received GitHub callback with code: {Code}", code);

        try
        {
            _logger.LogInformation("Calling GitHubService to handle authorization callback");
            var accessToken = await _gitHubService.HandleAuthorizationCallback(code);

            if (!string.IsNullOrEmpty(accessToken))
            {
                _logger.LogInformation("Successfully completed GitHub integration");
                var userInfo = await _gitHubService.GetUserInfo(accessToken);
                return Ok(new
                {
                    success = true,
                    accessToken,
                    userInfo = new
                    {
                        username = userInfo.Login,
                        email = userInfo.Email,
                        name = userInfo.Name,
                        avatarUrl = userInfo.AvatarUrl
                    }
                });
            }

            _logger.LogWarning("Failed to complete GitHub integration");
            return Unauthorized(new { success = false, message = "Failed to complete GitHub integration" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub callback. Code: {Code}", code);
            _logger.LogError(ex.Message);
            _logger.LogError(ex.StackTrace);
            _logger.LogError(ex.InnerException?.Message);
            return Redirect(redirectUri);
        }
    }

    [HttpPost("issues")]
    public async Task<IActionResult> CreateIssue(
        [FromQuery] string repositoryOwner,
        [FromQuery] string repositoryName,
        [FromBody] CreateIssueRequest request)
    {
        _logger.LogInformation(
            "Received request to create GitHub issue. Repository: {Owner}/{Name}, Title: {Title}",
            repositoryOwner,
            repositoryName,
            request.Title
        );

        try
        {
            _logger.LogInformation("Calling GitHubService to create issue");
            var issue = await _gitHubService.CreateIssue(
                repositoryOwner,
                repositoryName,
                request.Title,
                request.Body);

            _logger.LogInformation("Successfully created GitHub issue: {IssueUrl}", issue.HtmlUrl);
            return Ok(issue);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating GitHub issue. Repository: {Owner}/{Name}, Title: {Title}",
                repositoryOwner,
                repositoryName,
                request.Title
            );
            return StatusCode(500, new { error = "Failed to create issue", details = ex.Message });
        }
    }

    [HttpGet("repositories")]
    public async Task<IActionResult> GetRepositories()
    {
        _logger.LogInformation("Received request to get GitHub repositories");

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized(new { error = "Missing or invalid authorization header" });
            }

            var accessToken = authHeader.Substring("Bearer ".Length).Trim();

            _logger.LogInformation("Access token: {AccessToken}", accessToken);
            var maskedToken = accessToken.Length > 8
                ? $"{accessToken.Substring(0, 4)}...{accessToken.Substring(accessToken.Length - 4)}"
                : "***";
            _logger.LogInformation("Extracted access token from authorization header: {MaskedToken}", maskedToken);

            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Access token is empty after extraction");
                return Unauthorized(new { error = "Access token is empty" });
            }

            _logger.LogInformation("Calling GitHubService to get repositories");
            var repositories = await _gitHubService.GetRepositories(accessToken);

            _logger.LogInformation("Successfully retrieved {Count} repositories", repositories.Count());
            return Ok(repositories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub repositories");
            return StatusCode(500, new { error = "Failed to get repositories", details = ex.Message });
        }
    }

    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects()
    {
        _logger.LogInformation("Received request to get GitHub projects");

        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                return Unauthorized(new { error = "Missing or invalid authorization header" });
            }

            var accessToken = authHeader.Substring("Bearer ".Length).Trim();

            _logger.LogInformation("Calling GitHubService to get projects");
            var projects = await _gitHubService.GetProjects(accessToken);

            _logger.LogInformation("Successfully retrieved {Count} projects", projects.Count());
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub projects");
            return StatusCode(500, new { error = "Failed to get projects", details = ex.Message });
        }
    }

    [HttpGet("issues")]
    public async Task<IActionResult> GetIssues([FromQuery] string repositoryOwner, [FromQuery] string repositoryName)
    {
        _logger.LogInformation(
            "Received request to get issues for repository: {Owner}/{Name}",
            repositoryOwner,
            repositoryName
        );

        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(repositoryOwner))
            {
                return BadRequest(new { error = "Repository owner is required" });
            }

            if (string.IsNullOrWhiteSpace(repositoryName))
            {
                return BadRequest(new { error = "Repository name is required" });
            }

            // Get access token from Authorization header
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "Missing or invalid authorization token" });
            }

            var accessToken = authHeader.Substring("Bearer ".Length);

            _logger.LogInformation("Calling GitHubService to get issues");
            var issues = await _gitHubService.GetIssues(accessToken, repositoryOwner, repositoryName);
            _logger.LogInformation("Successfully retrieved {Count} issues", issues.Count());

            return Ok(issues);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error getting GitHub issues for repository: {Owner}/{Name}",
                repositoryOwner,
                repositoryName
            );
            return StatusCode(500, new { error = "Failed to get issues", details = ex.Message });
        }
    }
}

public class CreateIssueRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}