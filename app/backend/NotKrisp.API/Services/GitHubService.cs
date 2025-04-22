using Microsoft.EntityFrameworkCore;
using NotKrisp.API.Data;
using NotKrisp.API.Models;
using Octokit;
using Microsoft.Extensions.Configuration;
using NotKrisp.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace NotKrisp.API.Services;

public interface IGitHubService
{
    Task<string> GetAuthorizationUrl(string redirectUri);
    Task<string> HandleAuthorizationCallback(string code);
    Task<Octokit.Issue> CreateIssue(string repositoryOwner, string repositoryName, string title, string body);
    Task<IEnumerable<Octokit.Repository>> GetRepositories(string accessToken);
    Task<Octokit.User> GetUserInfo(string accessToken);
    Task<IEnumerable<Models.Project>> GetProjects(string accessToken);
    Task<IEnumerable<Models.Issue>> GetIssues(string accessToken, string repositoryOwner, string repositoryName);
}

public class GitHubService : IGitHubService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubService> _logger;
    private readonly GitHubClient _client;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, Task<IEnumerable<Models.Issue>>> _issueFetchingTasks;

    public GitHubService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<GitHubService> logger,
        IMemoryCache cache)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _cache = cache;
        _client = new GitHubClient(new ProductHeaderValue("NotKrisp"));
        _issueFetchingTasks = new ConcurrentDictionary<string, Task<IEnumerable<Models.Issue>>>();
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
                Scopes = { "repo", "user", "user:email", "read:user" }
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

    public async Task<string> HandleAuthorizationCallback(string code)
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


            //return the access token
            return token.AccessToken;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GitHub authorization callback");
        }

        return string.Empty;

    }

    public async Task<Octokit.Issue> CreateIssue(string repositoryOwner, string repositoryName, string title, string body)
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

    public async Task<IEnumerable<Octokit.Repository>> GetRepositories(string accessToken)
    {
        _logger.LogInformation("Getting GitHub repositories");

        try
        {
            _client.Credentials = new Credentials(accessToken);
            _logger.LogInformation("Set GitHub client credentials");

            _logger.LogInformation("Requesting current user from GitHub");
            var user = await _client.User.Current();
            _logger.LogInformation("Current user: {Login}", user.Login);

            _logger.LogInformation("Requesting repositories from GitHub");
            var repositories = await _client.Repository.GetAllForUser(user.Login);
            _logger.LogInformation("Successfully retrieved {Count} repositories", repositories.Count());

            // Log each repository for debugging
            foreach (var repo in repositories)
            {
                _logger.LogInformation("Repository: {FullName} (ID: {Id})", repo.FullName, repo.Id);
            }

            return repositories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub repositories");
            throw;
        }
    }

    public async Task<Octokit.User> GetUserInfo(string accessToken)
    {
        _logger.LogInformation("Getting GitHub user information");

        try
        {
            _client.Credentials = new Credentials(accessToken);
            var user = await _client.User.Current();
            _logger.LogInformation("Successfully retrieved GitHub user information for {Username}", user.Login);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting GitHub user information");
            throw;
        }
    }

    public async Task<IEnumerable<Models.Project>> GetProjects(string accessToken)
    {
        var cacheKey = $"projects_{accessToken}";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Models.Project> cachedProjects))
        {
            return cachedProjects;
        }

        try
        {
            _client.Credentials = new Credentials(accessToken);
            var user = await _client.User.Current();
            var allProjects = new List<Models.Project>();
            var projectNumber = 1;

            // Get repositories in parallel
            var repositories = await _client.Repository.GetAllForUser(user.Login);
            var projectTasks = new List<Task>();

            foreach (var repo in repositories)
            {
                projectTasks.Add(ProcessRepositoryProjects(repo, allProjects, projectNumber));
            }

            // Get standalone GitHub Projects in parallel
            projectTasks.Add(ProcessStandaloneProjects(user.Login, allProjects, projectNumber));

            await Task.WhenAll(projectTasks);

            // Cache the results for 5 minutes
            _cache.Set(cacheKey, allProjects, TimeSpan.FromMinutes(5));

            return allProjects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects");
            throw;
        }
    }

    private async Task ProcessRepositoryProjects(Octokit.Repository repo, List<Models.Project> allProjects, int startProjectNumber)
    {
        try
        {
            var issues = await _client.Issue.GetAllForRepository(repo.Owner.Login, repo.Name, new RepositoryIssueRequest
            {
                Labels = { "project" }
            });

            var projects = issues.Select((i, index) => new Models.Project
            {
                Id = i.Id.ToString(),
                Name = i.Title,
                Body = i.Body,
                HtmlUrl = i.HtmlUrl,
                Number = startProjectNumber + index,
                State = i.State.StringValue,
                CreatedAt = i.CreatedAt.ToString(),
                UpdatedAt = i.UpdatedAt.ToString(),
                Labels = i.Labels.Select(l => new Models.Label
                {
                    Id = l.Id.ToString(),
                    Name = l.Name,
                    Color = l.Color
                }).ToList(),
                Assignees = i.Assignees.Select(a => new Models.Assignee
                {
                    Login = a.Login,
                    AvatarUrl = a.AvatarUrl
                }).ToList()
            });

            lock (allProjects)
            {
                allProjects.AddRange(projects);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get issues for {RepoName}", repo.FullName);
        }
    }

    private async Task ProcessStandaloneProjects(string login, List<Models.Project> allProjects, int startProjectNumber)
    {
        try
        {
            var projects = await _client.Repository.Project.GetAllForOrganization(login);
            var standaloneProjects = projects.Select((p, index) => new Models.Project
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Body = p.Body,
                HtmlUrl = p.HtmlUrl,
                Number = startProjectNumber + index,
                State = p.State.StringValue,
                CreatedAt = p.CreatedAt.ToString(),
                UpdatedAt = p.UpdatedAt.ToString()
            });

            lock (allProjects)
            {
                allProjects.AddRange(standaloneProjects);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get standalone projects");
        }
    }

    public async Task<IEnumerable<Models.Issue>> GetIssues(string accessToken, string repositoryOwner, string repositoryName)
    {
        var cacheKey = $"issues_{repositoryOwner}_{repositoryName}";
        
        // Check cache first
        if (_cache.TryGetValue(cacheKey, out IEnumerable<Models.Issue> cachedIssues))
        {
            return cachedIssues;
        }

        // Check if there's already a task fetching these issues
        var taskKey = $"{repositoryOwner}/{repositoryName}";
        if (_issueFetchingTasks.TryGetValue(taskKey, out var existingTask))
        {
            return await existingTask;
        }

        // Create new task to fetch issues
        var fetchTask = FetchIssuesInternal(accessToken, repositoryOwner, repositoryName);
        _issueFetchingTasks.TryAdd(taskKey, fetchTask);

        try
        {
            var issues = await fetchTask;
            
            // Cache the results for 5 minutes
            _cache.Set(cacheKey, issues, TimeSpan.FromMinutes(5));
            
            return issues;
        }
        finally
        {
            _issueFetchingTasks.TryRemove(taskKey, out _);
        }
    }

    private async Task<IEnumerable<Models.Issue>> FetchIssuesInternal(string accessToken, string repositoryOwner, string repositoryName)
    {
        try
        {
            _client.Credentials = new Credentials(accessToken);

            // First check if we can access the repository
            try
            {
                await _client.Repository.Get(repositoryOwner, repositoryName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to access repository: {Owner}/{Name}", repositoryOwner, repositoryName);
                throw;
            }

            var issues = await _client.Issue.GetAllForRepository(repositoryOwner, repositoryName, new RepositoryIssueRequest
            {
                State = ItemStateFilter.All,
                Filter = IssueFilter.All
            });

            // Filter out pull requests and map to our model
            var actualIssues = issues
                .Where(i => i.PullRequest == null)
                .Select(i => new Models.Issue
                {
                    Id = i.Id.ToString(),
                    Number = i.Number,
                    Title = i.Title,
                    Body = i.Body,
                    HtmlUrl = i.HtmlUrl,
                    State = i.State.StringValue,
                    CreatedAt = i.CreatedAt.ToString(),
                    UpdatedAt = i.UpdatedAt.ToString(),
                    Labels = i.Labels.Select(l => new Models.Label
                    {
                        Id = l.Id.ToString(),
                        Name = l.Name,
                        Color = l.Color
                    }).ToList(),
                    Assignees = i.Assignees.Select(a => new Models.Assignee
                    {
                        Login = a.Login,
                        AvatarUrl = a.AvatarUrl
                    }).ToList()
                });

            return actualIssues;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting issues for {Owner}/{Name}", repositoryOwner, repositoryName);
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