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
    public async Task<IActionResult> HandleCallback([FromQuery] string code)
    {
        _logger.LogInformation("Received GitHub callback with code: {Code}", code);

        try
        {
            _logger.LogInformation("Calling GitHubService to handle authorization callback");
            var success = await _gitHubService.HandleAuthorizationCallback(code);

            if (success)
            {
                _logger.LogInformation("Successfully completed GitHub integration");
                return Redirect(_configuration["FrontendUrl"] + "/auth/success");
            }

            _logger.LogWarning("Failed to complete GitHub integration");
            return Redirect(_configuration["FrontendUrl"] + "/auth/denied");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub callback. Code: {Code}", code);
            _logger.LogError(ex.Message);
            _logger.LogError(ex.StackTrace);
            _logger.LogError(ex.InnerException?.Message);
            return Redirect(_configuration["FrontendUrl"] + "/auth/denied");
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
            _logger.LogInformation("Calling GitHubService to get repositories");
            var repositories = await _gitHubService.GetRepositories();

            _logger.LogInformation("Successfully retrieved {Count} repositories", repositories.Count());
            return Ok(repositories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub repositories");
            return StatusCode(500, new { error = "Failed to get repositories", details = ex.Message });
        }
    }
}

public class CreateIssueRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}