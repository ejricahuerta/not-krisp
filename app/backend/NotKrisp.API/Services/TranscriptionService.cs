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
    public class TranscriptionService : BaseService<Transcription>, ITranscriptionService
    {
        public TranscriptionService(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transcription>> GetTranscriptionsByMeetingAsync(Guid meetingId)
        {
            return await _dbSet
                .Where(t => t.MeetingId == meetingId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transcription>> GetTranscriptionsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Transcription> UpdateTranscriptionStatusAsync(Guid id, string status)
        {
            var transcription = await GetByIdAsync(id);
            if (transcription == null)
                throw new ArgumentException("Transcription not found");

            transcription.Status = status;
            transcription.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, transcription);
        }

        public async Task<Transcription> UpdateTranscriptionTextAsync(Guid id, string text)
        {
            var transcription = await GetByIdAsync(id);
            if (transcription == null)
                throw new ArgumentException("Transcription not found");

            transcription.Content = text;
            transcription.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, transcription);
        }

        public override async Task<IEnumerable<Transcription>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.Meeting)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Transcription> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Meeting)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
} 