using System.ComponentModel.DataAnnotations;

namespace NotKrisp.API.Models;

public class IntegrationSettings
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string IntegrationType { get; set; } = string.Empty; // "Zoom" or "GitHub"
    
    [Required]
    public string ClientId { get; set; } = string.Empty;
    
    [Required]
    public string ClientSecret { get; set; } = string.Empty;
    
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiresAt { get; set; }
    
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 