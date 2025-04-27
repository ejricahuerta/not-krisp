using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotKrisp.Core.Models;

public class GitHubToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [Required]
    public string AccessToken { get; set; } = null!;

    public string? RefreshToken { get; set; }

    [Required]
    public string TokenType { get; set; } = "bearer";

    [Required]
    public string Scope { get; set; } = "";

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser User { get; set; } = null!;
} 