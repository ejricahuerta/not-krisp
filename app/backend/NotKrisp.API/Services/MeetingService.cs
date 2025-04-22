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
    public class MeetingService : BaseService<Meeting>, IMeetingService
    {
        public MeetingService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Meeting>> GetMeetingsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(m => m.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Meeting>> GetMeetingsByPlatformAsync(string platform)
        {
            return await _dbSet
                .Where(m => m.Platform == platform)
                .ToListAsync();
        }

        public async Task<Meeting> StartMeetingAsync(Guid id)
        {
            var meeting = await GetByIdAsync(id);
            if (meeting == null)
                throw new ArgumentException("Meeting not found");

            meeting.Status = "In Progress";
            meeting.StartTime = DateTime.UtcNow;
            meeting.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, meeting);
        }

        public async Task<Meeting> EndMeetingAsync(Guid id)
        {
            var meeting = await GetByIdAsync(id);
            if (meeting == null)
                throw new ArgumentException("Meeting not found");

            meeting.Status = "Completed";
            meeting.EndTime = DateTime.UtcNow;
            meeting.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, meeting);
        }

        public async Task<string> GetMeetingAudioUrlAsync(Guid id)
        {
            var meeting = await GetByIdAsync(id);
            if (meeting == null)
                throw new ArgumentException("Meeting not found");

            return meeting.AudioUrl;
        }

        public override async Task<IEnumerable<Meeting>> GetAllAsync()
        {
            return await _dbSet
                .Include(m => m.Transcriptions)
                .Include(m => m.Summaries)
                .Include(m => m.Tickets)
                .ToListAsync();
        }

        public override async Task<Meeting> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(m => m.Transcriptions)
                .Include(m => m.Summaries)
                .Include(m => m.Tickets)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
} 