using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotKrisp.API.Models;

namespace NotKrisp.API.Services.Interfaces
{
    public interface ITranscriptionService : IBaseService<Transcription>
    {
        Task<IEnumerable<Transcription>> GetTranscriptionsByMeetingAsync(Guid meetingId);
        Task<IEnumerable<Transcription>> GetTranscriptionsByStatusAsync(string status);
        Task<Transcription> UpdateTranscriptionStatusAsync(Guid id, string status);
        Task<Transcription> UpdateTranscriptionTextAsync(Guid id, string text);
    }
} 