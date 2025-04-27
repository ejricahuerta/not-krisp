using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using NotKrisp.Core.Models;
using Newtonsoft.Json;

namespace NotKrisp.Core.Services
{
    public class GitHubApiService
    {
        private readonly HttpClient _httpClient;

        public GitHubApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NotKrispApp", "1.0"));
        }

        public async Task<GitHubUser?> GetUserAsync(string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.GetAsync("https://api.github.com/user");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();

            Console.WriteLine(json);
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject<GitHubUser>(json);
        }
    }

  
}