using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotKrisp.API.Models;

namespace NotKrisp.API.Services.Interfaces
{
    public interface ISummaryService : IBaseService<Summary>
    {
        Task<IEnumerable<Summary>> GetSummariesByMeetingAsync(Guid meetingId);
        Task<IEnumerable<Summary>> GetSummariesByStatusAsync(string status);
        Task<Summary> UpdateSummaryStatusAsync(Guid id, string status);
        Task<Summary> UpdateSummaryContentAsync(Guid id, string content);
        Task<Summary> UpdateActionItemsAsync(Guid id, List<string> actionItems);
    }
} 