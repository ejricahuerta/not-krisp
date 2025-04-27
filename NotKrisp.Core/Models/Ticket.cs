using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotKrisp.Core.Models;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? GitHubIssueId { get; set; }

    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    [Required]
    public string AssignedToId { get; set; } = null!;

    [Required]
    public int RelatedMeetingId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey(nameof(AssignedToId))]
    public virtual ApplicationUser AssignedTo { get; set; } = null!;

    [ForeignKey(nameof(RelatedMeetingId))]
    public virtual Meeting RelatedMeeting { get; set; } = null!;
} 