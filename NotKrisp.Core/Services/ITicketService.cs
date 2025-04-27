using NotKrisp.Core.Models;

namespace NotKrisp.Core.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket?> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(int id);
} 