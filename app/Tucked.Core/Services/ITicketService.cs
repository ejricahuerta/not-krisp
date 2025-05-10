using Tucked.Core.Models;

namespace Tucked.Core.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket?> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(int id);
    Task<List<Ticket>> GetAllWithGitHubIssueIdAsync();
    Task<List<Ticket>> CreateFromTranscriptionAsync(string transcription, string repoOwner, string repoName, string githubAccessToken, TicketExtractionService extractionService, GitHubApiService githubApi, string assignedToId, int relatedMeetingId);
} 