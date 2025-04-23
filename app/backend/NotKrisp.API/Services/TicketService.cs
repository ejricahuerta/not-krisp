using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotKrisp.API.Data;
using NotKrisp.API.Models;
using NotKrisp.API.Services.Interfaces;

namespace NotKrisp.API.Services
{
    public class TicketService : BaseService<Ticket>, ITicketService
    {
        public TicketService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByMeetingAsync(Guid meetingId)
        {
            return await _dbSet
                .Include(t => t.Meeting)
                .Where(t => t.MeetingId == meetingId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByPlatformAsync(string platform)
        {
            return await _dbSet
                .Where(t => t.Platform == platform)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket> UpdateTicketStatusAsync(Guid id, string status)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, ticket);
        }

        public async Task<Ticket> UpdateTicketUrlAsync(Guid id, string url)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            ticket.Url = url;
            ticket.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, ticket);
        }

        public async Task<Ticket> UpdateTicketMetadataAsync(Guid id, Dictionary<string, string> metadata)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket == null)
                throw new ArgumentException("Ticket not found");

            ticket.Metadata = metadata;
            ticket.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, ticket);
        }

        public override async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.Meeting)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Ticket> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Meeting)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}