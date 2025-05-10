using Microsoft.AspNetCore.Identity;

namespace Tucked.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? GitHubId { get; set; }
        public string? GitHubUsername { get; set; }
        public string? GitHubAvatarUrl { get; set; }
        public string? GitHubAccessToken { get; set; }
        public string? GitHubRefreshToken { get; set; }
        public string? AvatarUrl { get; set; }
        // Add other custom fields as needed

        // Navigation property for GitHub tokens
        public ICollection<GitHubToken> GitHubTokens { get; set; } = new List<GitHubToken>();
    }
} 