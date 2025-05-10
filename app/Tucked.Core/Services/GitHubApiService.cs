using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Tucked.Core.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Tucked.Core.Services
{
    public class GitHubApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubApiService> _logger;

        public GitHubApiService(HttpClient httpClient, ILogger<GitHubApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TuckedApp", "1.0"));
        }

        /// <summary>
        /// Gets the authenticated GitHub user.
        /// </summary>
        public virtual async Task<GitHubUser?> GetUserAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("https://api.github.com/user");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("GitHub GetUserAsync failed: {Status} {Reason}", response.StatusCode, response.ReasonPhrase);
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<GitHubUser>(json);
        }

        /// <summary>
        /// Creates a new GitHub issue.
        /// </summary>
        public virtual async Task<string?> CreateIssueAsync(string repoOwner, string repoName, string title, string body, string accessToken)
        {
            return await SendWithRetryAsync(
                () => {
                    var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/issues";
                    var payload = new { title, body };
                    var json = JsonConvert.SerializeObject(payload);
                    var request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("TuckedApp", "1.0"));
                    return request;
                },
                responseBody => {
                    dynamic result = JsonConvert.DeserializeObject(responseBody);
                    return result?.number?.ToString();
                });
        }

        /// <summary>
        /// Updates an existing GitHub issue.
        /// </summary>
        public virtual async Task<bool> UpdateIssueAsync(string repoOwner, string repoName, int issueNumber, string title, string body, string accessToken)
        {
            return await SendWithRetryAsync(
                () => {
                    var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/issues/{issueNumber}";
                    var payload = new { title, body };
                    var json = JsonConvert.SerializeObject(payload);
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("TuckedApp", "1.0"));
                    return request;
                },
                _ => true) != null;
        }

        /// <summary>
        /// Closes a GitHub issue.
        /// </summary>
        public virtual async Task<bool> CloseIssueAsync(string repoOwner, string repoName, int issueNumber, string accessToken)
        {
            return await SendWithRetryAsync(
                () => {
                    var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/issues/{issueNumber}";
                    var payload = new { state = "closed" };
                    var json = JsonConvert.SerializeObject(payload);
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("TuckedApp", "1.0"));
                    return request;
                },
                _ => true) != null;
        }

        /// <summary>
        /// Gets a GitHub issue by number.
        /// </summary>
        public virtual async Task<dynamic?> GetIssueAsync(string repoOwner, string repoName, int issueNumber, string accessToken)
        {
            return await SendWithRetryAsync(
                () => {
                    var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/issues/{issueNumber}";
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.UserAgent.Add(new ProductInfoHeaderValue("TuckedApp", "1.0"));
                    return request;
                },
                responseBody => JsonConvert.DeserializeObject(responseBody));
        }

        /// <summary>
        /// Sends an HTTP request with retry and exponential backoff for rate limits.
        /// </summary>
        protected virtual async Task<T?> SendWithRetryAsync<T>(Func<HttpRequestMessage> requestFactory, Func<string, T?> parseResponse, int maxRetries = 3)
        {
            int delay = 2000;
            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                using (var request = requestFactory())
                {
                    var response = await _httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        return parseResponse(responseBody);
                    }
                    if ((int)response.StatusCode == 403 && response.Headers.TryGetValues("X-RateLimit-Remaining", out var values) && values.Contains("0"))
                    {
                        _logger.LogWarning("GitHub API rate limit hit. Attempt {Attempt}. Waiting {Delay}ms before retrying.", attempt + 1, delay);
                        await Task.Delay(delay);
                        delay *= 2;
                        continue;
                    }
                    _logger.LogError("GitHub API request failed: {Status} {Reason}. Attempt {Attempt}", response.StatusCode, response.ReasonPhrase, attempt + 1);
                    if (attempt == maxRetries) return default;
                    await Task.Delay(delay);
                    delay *= 2;
                }
            }
            return default;
        }
    }
}