using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotKrisp.Core.Models;

public class Meeting
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    public string? AudioFilePath { get; set; }

    public string? TranscriptionText { get; set; }

    public string? Summary { get; set; }

    [Required]
    public string CreatedById { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(CreatedById))]
    public virtual ApplicationUser CreatedBy { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
} 