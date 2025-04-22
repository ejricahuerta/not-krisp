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
    public class SummaryService : BaseService<Summary>, ISummaryService
    {
        public SummaryService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Summary>> GetSummariesByMeetingAsync(Guid meetingId)
        {
            return await _dbSet
                .Where(s => s.MeetingId == meetingId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Summary>> GetSummariesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Summary> UpdateSummaryStatusAsync(Guid id, string status)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.Status = status;
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public async Task<Summary> UpdateSummaryContentAsync(Guid id, string content)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.Content = content;
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public async Task<Summary> UpdateActionItemsAsync(Guid id, List<string> actionItems)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.ActionItems = string.Join(", ", actionItems);
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public override async Task<IEnumerable<Summary>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Meeting)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Summary> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Meeting)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Summary?> GetSummaryById(int id)
        {
            return await _context.Summaries.FindAsync(id);
        }
    }
} 