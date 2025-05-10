using Microsoft.EntityFrameworkCore;
using Tucked.Core.Data;
using Tucked.Core.Models;

namespace Tucked.Core.Services;

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

    public bool IsDuplicateTicket(string title)
    {
        // Simple duplicate check by title (case-insensitive, not closed)
        return _db.Tickets.Any(t => t.Title.ToLower() == title.ToLower() && t.Status != TicketStatus.Closed);
    }

    public async Task<List<Ticket>> CreateFromTranscriptionAsync(string transcription, string repoOwner, string repoName, string githubAccessToken, TicketExtractionService extractionService, GitHubApiService githubApi, string assignedToId, int relatedMeetingId)
    {
        var createdTickets = new List<Ticket>();
        var candidates = extractionService.ExtractTickets(transcription);
        foreach (var (title, description) in candidates)
        {
            if (IsDuplicateTicket(title))
                continue;
            var ticket = new Ticket { Title = title, Description = description, Status = TicketStatus.Open, CreatedAt = DateTime.UtcNow, AssignedToId = assignedToId, RelatedMeetingId = relatedMeetingId };
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();
            // Create GitHub issue
            if (!string.IsNullOrEmpty(githubAccessToken))
            {
                var issueId = await githubApi.CreateIssueAsync(repoOwner, repoName, title, description, githubAccessToken);
                if (issueId != null)
                {
                    ticket.GitHubIssueId = issueId;
                    await _db.SaveChangesAsync();
                }
            }
            createdTickets.Add(ticket);
        }
        return createdTickets;
    }

    public async Task<List<Ticket>> GetAllWithGitHubIssueIdAsync()
    {
        return await _db.Tickets.Where(t => t.GitHubIssueId != null).ToListAsync();
    }
} 