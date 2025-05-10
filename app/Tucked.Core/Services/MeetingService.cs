using Microsoft.EntityFrameworkCore;
using Tucked.Core.Data;
using Tucked.Core.Models;

namespace Tucked.Core.Services;

public class MeetingService : IMeetingService
{
    private readonly ApplicationDbContext _db;
    public MeetingService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Meeting>> GetAllAsync()
        => await _db.Meetings.Include(m => m.Tickets).ToListAsync();

    public async Task<Meeting?> GetByIdAsync(int id)
        => await _db.Meetings.Include(m => m.Tickets).FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Meeting> CreateAsync(Meeting meeting)
    {
        _db.Meetings.Add(meeting);
        await _db.SaveChangesAsync();
        return meeting;
    }

    public async Task<Meeting?> UpdateAsync(Meeting meeting)
    {
        var existing = await _db.Meetings.FindAsync(meeting.Id);
        if (existing == null) return null;
        _db.Entry(existing).CurrentValues.SetValues(meeting);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var meeting = await _db.Meetings.FindAsync(id);
        if (meeting == null) return false;
        _db.Meetings.Remove(meeting);
        await _db.SaveChangesAsync();
        return true;
    }
} 