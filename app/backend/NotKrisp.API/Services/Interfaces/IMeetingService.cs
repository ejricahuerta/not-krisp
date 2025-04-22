using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotKrisp.API.Models;

namespace NotKrisp.API.Services.Interfaces
{
    public interface IMeetingService : IBaseService<Meeting>
    {
        Task<IEnumerable<Meeting>> GetMeetingsByStatusAsync(string status);
        Task<IEnumerable<Meeting>> GetMeetingsByPlatformAsync(string platform);
        Task<Meeting> StartMeetingAsync(Guid id);
        Task<Meeting> EndMeetingAsync(Guid id);
        Task<string> GetMeetingAudioUrlAsync(Guid id);
    }
} 