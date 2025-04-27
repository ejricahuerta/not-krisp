using Microsoft.EntityFrameworkCore;
using NotKrisp.Core.Data;
using NotKrisp.Core.Models;

namespace NotKrisp.Core.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _db;
    public TicketService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync()
        => await _db.Tickets.Include(t => t.AssignedTo).Include(t => t.RelatedMeeting).ToListAsync();

    public async Task<Ticket?> GetByIdAsync(int id)
        => await _db.Tickets.Include(t => t.AssignedTo).Include(t => t.RelatedMeeting).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket?> UpdateAsync(Ticket ticket)
    {
        var existing = await _db.Tickets.FindAsync(ticket.Id);
        if (existing == null) return null;
        _db.Entry(existing).CurrentValues.SetValues(ticket);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null) return false;
        _db.Tickets.Remove(ticket);
        await _db.SaveChangesAsync();
        return true;
    }
} 