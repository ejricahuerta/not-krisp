using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotKrisp.API.Models;

namespace NotKrisp.API.Services.Interfaces
{
    public interface ITicketService : IBaseService<Ticket>
    {
        Task<IEnumerable<Ticket>> GetTicketsByMeetingAsync(Guid meetingId);
        Task<IEnumerable<Ticket>> GetTicketsByPlatformAsync(string platform);
        Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status);
        Task<Ticket> UpdateTicketStatusAsync(Guid id, string status);
        Task<Ticket> UpdateTicketUrlAsync(Guid id, string url);
        Task<Ticket> UpdateTicketMetadataAsync(Guid id, Dictionary<string, string> metadata);
    }
} 