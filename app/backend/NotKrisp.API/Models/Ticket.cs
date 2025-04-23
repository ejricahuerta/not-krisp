using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace NotKrisp.API.Models
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MeetingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty; // GitHub, Jira, Trello
        public string ExternalId { get; set; } = string.Empty; // ID from the external platform
        public string Status { get; set; } = string.Empty; // Created, Failed
        public string Url { get; set; } = string.Empty; // URL to the ticket in the external platform
        public Dictionary<string, string> Metadata { get; set; } = new(); // Additional platform-specific data
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        [JsonIgnore]
        public virtual Meeting Meeting { get; set; } = null!;
    }
}