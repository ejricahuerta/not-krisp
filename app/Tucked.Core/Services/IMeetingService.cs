using Tucked.Core.Models;

namespace Tucked.Core.Services;

public interface IMeetingService
{
    Task<IEnumerable<Meeting>> GetAllAsync();
    Task<Meeting?> GetByIdAsync(int id);
    Task<Meeting> CreateAsync(Meeting meeting);
    Task<Meeting?> UpdateAsync(Meeting meeting);
    Task<bool> DeleteAsync(int id);
} 