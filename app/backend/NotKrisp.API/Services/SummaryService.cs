using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotKrisp.API.Data;
using NotKrisp.API.Models;
using NotKrisp.API.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace NotKrisp.API.Services
{
    public class SummaryService : BaseService<Summary>, ISummaryService
    {
        private readonly ILogger<SummaryService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public SummaryService(
            ILogger<SummaryService> logger,
            IOptions<AppSettings> settings,
            ApplicationDbContext context) : base(context)
        {
            _logger = logger;
            _context = context;
            _apiKey = settings.Value.AssemblyAI.ApiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.assemblyai.com/v2/")
            };
            _httpClient.DefaultRequestHeaders.Add("Authorization", _apiKey);
        }

        public async Task<IEnumerable<Summary>> GetSummariesByMeetingAsync(Guid meetingId)
        {
            return await _dbSet
                .Where(s => s.MeetingId == meetingId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Summary>> GetSummariesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<Summary> UpdateSummaryStatusAsync(Guid id, string status)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.Status = status;
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public async Task<Summary> UpdateSummaryContentAsync(Guid id, string content)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.Content = content;
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public async Task<Summary> UpdateActionItemsAsync(Guid id, List<string> actionItems)
        {
            var summary = await GetByIdAsync(id);
            if (summary == null)
                throw new ArgumentException("Summary not found");

            summary.ActionItems = string.Join(", ", actionItems);
            summary.UpdatedAt = DateTime.UtcNow;

            return await UpdateAsync(id, summary);
        }

        public override async Task<IEnumerable<Summary>> GetAllAsync()
        {
            return await _dbSet
                .Include(s => s.Meeting)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Summary> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(s => s.Meeting)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Summary?> GetSummaryById(int id)
        {
            return await _context.Summaries.FindAsync(id);
        }

        public async Task<SummaryResult> GenerateSummaryAndTicketsAsync(string transcription, Guid meetingId)
        {
            try
            {
                _logger.LogInformation("Generating summary and tickets for meeting: {MeetingId}", meetingId);

                // Ensure API key is set
                if (string.IsNullOrEmpty(_apiKey))
                {
                    throw new Exception("AssemblyAI API key is not configured");
                }

                // Check if the input is a URL or transcription text
                bool isUrl = Uri.TryCreate(transcription, UriKind.Absolute, out Uri? uriResult) 
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                TranscriptResponse transcriptResponse;

                if (isUrl)
                {
                    _logger.LogInformation("Processing audio URL: {AudioUrl}", transcription);
                    
                    // Start the transcription
                    var request = new
                    {
                        audio_url = transcription,
                        summarization = true,
                        summary_model = "conversational",
                        summary_type = "bullets_verbose",
                        format_text = true,
                        speaker_labels = true
                    };

                    var response = await _httpClient.PostAsJsonAsync("transcript", request);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("AssemblyAI API error: {Error}. Status code: {StatusCode}", errorContent, response.StatusCode);
                        throw new Exception($"AssemblyAI API error: {errorContent}");
                    }

                    transcriptResponse = await response.Content.ReadFromJsonAsync<TranscriptResponse>();
                    if (transcriptResponse == null)
                    {
                        throw new Exception("Failed to parse AssemblyAI response");
                    }

                    _logger.LogInformation("Started transcription with ID: {TranscriptId}", transcriptResponse.Id);

                    // Poll for completion
                    while (true)
                    {
                        var statusResponse = await _httpClient.GetAsync($"transcript/{transcriptResponse.Id}");
                        if (!statusResponse.IsSuccessStatusCode)
                        {
                            var errorContent = await statusResponse.Content.ReadAsStringAsync();
                            _logger.LogError("AssemblyAI status check error: {Error}. Status code: {StatusCode}", errorContent, statusResponse.StatusCode);
                            throw new Exception($"AssemblyAI status check error: {errorContent}");
                        }

                        var status = await statusResponse.Content.ReadFromJsonAsync<TranscriptResponse>();
                        if (status == null)
                        {
                            throw new Exception("Failed to parse AssemblyAI status response");
                        }

                        if (status.Status == "completed")
                        {
                            _logger.LogInformation("Transcription completed successfully");
                            transcriptResponse = status;
                            break;
                        }
                        else if (status.Status == "error")
                        {
                            _logger.LogError("Transcription failed: {Error}", status.Error);
                            throw new Exception($"Transcription failed: {status.Error}");
                        }

                        _logger.LogInformation("Transcription status: {Status}", status.Status);
                        await Task.Delay(1000);
                    }
                }
                else
                {
                    _logger.LogInformation("Processing transcription text directly");
                    
                    // Use the summarize endpoint for text input
                    var request = new
                    {
                        text = transcription,
                        summarization = true,
                        summary_model = "conversational",
                        summary_type = "bullets_verbose",
                        format_text = true,
                        speaker_labels = true
                    };

                    var response = await _httpClient.PostAsJsonAsync("v2/summarize", request);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("AssemblyAI API error: {Error}. Status code: {StatusCode}", errorContent, response.StatusCode);
                        throw new Exception($"AssemblyAI API error: {errorContent}");
                    }

                    var summaryResponse = await response.Content.ReadFromJsonAsync<SummaryResponse>();
                    if (summaryResponse == null)
                    {
                        throw new Exception("Failed to parse AssemblyAI response");
                    }

                    transcriptResponse = new TranscriptResponse
                    {
                        Id = Guid.NewGuid().ToString(),
                        Status = "completed",
                        Summary = summaryResponse.Summary,
                        Error = string.Empty
                    };
                }

                return CreateSummaryResult(meetingId, transcriptResponse.Summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary and tickets for meeting: {MeetingId}", meetingId);
                throw;
            }
        }

        public async Task<string> UploadAudioAsync(byte[] fileBytes)
        {
            using var content = new ByteArrayContent(fileBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.PostAsync("upload", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("AssemblyAI upload error: {Error}. Status code: {StatusCode}", errorContent, response.StatusCode);
                throw new Exception($"AssemblyAI upload error: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("AssemblyAI upload response: {Response}", responseContent);

            var uploadResponse = await response.Content.ReadFromJsonAsync<UploadResponse>();
            if (uploadResponse == null || string.IsNullOrEmpty(uploadResponse.UploadUrl))
            {
                throw new Exception($"Failed to parse AssemblyAI upload response: {responseContent}");
            }

            return uploadResponse.UploadUrl;
        }

        private SummaryResult CreateSummaryResult(Guid meetingId, string? summary)
        {
            var summaryResult = new SummaryResult
            {
                MeetingId = meetingId,
                Summary = summary ?? "No summary generated",
                Tickets = ParseBulletsIntoTickets(summary ?? string.Empty)
            };

            // Save to database
            _ = SaveSummaryAndTicketsAsync(summaryResult);

            return summaryResult;
        }

        private List<Ticket> ParseBulletsIntoTickets(string summary)
        {
            var tickets = new List<Ticket>();
            if (string.IsNullOrEmpty(summary)) return tickets;

            // Split the summary into bullet points
            var bullets = summary.Split(new[] { "\n-", "- " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var bullet in bullets)
            {
                // Create a ticket for each actionable item
                // We'll consider any bullet point that suggests an action
                var trimmedBullet = bullet.Trim();
                if (IsActionItem(trimmedBullet))
                {
                    tickets.Add(new Ticket
                    {
                        Title = trimmedBullet,
                        Description = "Generated from meeting transcript",
                        Status = "Open",
                        Platform = "Internal",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return tickets;
        }

        private bool IsActionItem(string bullet)
        {
            // Simple heuristic to identify action items
            // Look for common action verbs or phrases
            var actionPhrases = new[]
            {
                "need to", "should", "will", "must", "todo", "to do",
                "follow up", "review", "create", "update", "implement",
                "develop", "research", "investigate", "plan", "schedule"
            };

            return actionPhrases.Any(phrase =>
                bullet.ToLower().Contains(phrase));
        }

        private class UploadResponse
        {
            [JsonPropertyName("upload_url")]
            public string UploadUrl { get; set; } = string.Empty;
        }

        private class TranscriptResponse
        {
            public string Id { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string Summary { get; set; } = string.Empty;
            public string Error { get; set; } = string.Empty;
        }

        private class SummaryResponse
        {
            public string Summary { get; set; } = string.Empty;
        }

        private async Task SaveSummaryAndTicketsAsync(SummaryResult result)
        {
            try
            {
                // Save summary
                var summary = new Summary
                {
                    MeetingId = result.MeetingId,
                    Content = result.Summary,
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Summaries.Add(summary);

                // Save tickets
                foreach (var ticket in result.Tickets)
                {
                    ticket.MeetingId = result.MeetingId;
                    _context.Tickets.Add(ticket);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving summary and tickets to database");
                throw;
            }
        }
    }

    public class SummaryResult
    {
        public Guid MeetingId { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<Ticket> Tickets { get; set; } = new();
    }
}