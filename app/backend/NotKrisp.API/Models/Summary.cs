using System;

namespace NotKrisp.API.Models
{
    public class Summary
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Content { get; set; }
        public string ActionItems { get; set; }
        public string Status { get; set; } // Processing, Completed, Failed
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual Meeting Meeting { get; set; }
    }
} 