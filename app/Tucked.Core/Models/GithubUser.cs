using System.Text.Json.Serialization;

namespace Tucked.Core.Models;

public class GitHubUser
{
    // Required fields from GitHub API
    [JsonPropertyName("login")]
    public required string Login { get; set; }

    [JsonPropertyName("id")]
    public required long Id { get; set; }

    [JsonPropertyName("avatar_url")]
    public required string AvatarUrl { get; set; }
    
    // Optional fields that may be null
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }
    
    // Numeric fields with sensible defaults
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    // Add more fields as needed
}