using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotKrisp.API.Models
{
    public class Meeting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string MeetingId { get; set; } = string.Empty; // External meeting ID (e.g., Zoom meeting ID)
        public string Platform { get; set; } = string.Empty; // Zoom, Google Meet, etc.
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Scheduled, In Progress, Completed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Transcription> Transcriptions { get; set; } = new List<Transcription>();
        [JsonIgnore]
        public virtual ICollection<Summary> Summaries { get; set; } = new List<Summary>();
        [JsonIgnore]
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
} 