using System;
using System.Collections.Generic;

namespace NotKrisp.API.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid MeetingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Platform { get; set; } // GitHub, Jira, Trello
        public string ExternalId { get; set; } // ID from the external platform
        public string Status { get; set; } // Created, Failed
        public string Url { get; set; } // URL to the ticket in the external platform
        public Dictionary<string, string> Metadata { get; set; } // Additional platform-specific data
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual Meeting Meeting { get; set; }
    }
} 