using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotKrisp.API.Models
{
    public class Summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Full, Brief, Action Items, etc.
        public string Status { get; set; } = string.Empty; // Processing, Completed, Failed
        public string ActionItems { get; set; } = string.Empty; // Comma-separated list of action items
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Foreign key
        public Guid MeetingId { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual Meeting Meeting { get; set; } = null!;
    }
}