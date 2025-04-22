using System;
using System.Collections.Generic;

namespace NotKrisp.API.Models
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string MeetingId { get; set; } // External meeting ID (e.g., Zoom meeting ID)
        public string Platform { get; set; } // Zoom, Google Meet, etc.
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AudioUrl { get; set; }
        public string Status { get; set; } // Scheduled, In Progress, Completed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Transcription> Transcriptions { get; set; }
        public virtual ICollection<Summary> Summaries { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
} 