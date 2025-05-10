using System.Collections.Generic;

namespace Tucked.Core.Services
{
    public class TicketExtractionService
    {
        // Extract lines starting with 'TODO' or containing 'Action:'
        public List<(string Title, string Description)> ExtractTickets(string transcription)
        {
            var tickets = new List<(string, string)>();
            var lines = transcription.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("TODO") || line.Contains("Action:"))
                {
                    tickets.Add((line.Trim(), "Extracted from meeting transcription"));
                }
            }
            return tickets;
        }
    }
} 