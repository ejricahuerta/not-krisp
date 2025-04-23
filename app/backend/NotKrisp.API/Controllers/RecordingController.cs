using Microsoft.AspNetCore.Mvc;
using NotKrisp.API.Services.Interfaces;
using NotKrisp.API.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace NotKrisp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecordingController : ControllerBase
    {
        private readonly IMeetingService _meetingService;
        private readonly ISummaryService _summaryService;
        private readonly ITicketService _ticketService;
        private readonly ILogger<RecordingController> _logger;

        public RecordingController(
            IMeetingService meetingService,
            ISummaryService summaryService,
            ITicketService ticketService,
            ILogger<RecordingController> logger)
        {
            _meetingService = meetingService;
            _summaryService = summaryService;
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpPost("process")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ProcessRecording([FromForm] ProcessRecordingRequest request)
        {
            try
            {
                if (request.AudioFile == null || request.AudioFile.Length == 0)
                {
                    return BadRequest("Audio file is required");
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest("Title is required");
                }

                _logger.LogInformation("Processing recording: {Title}", request.Title);

                // Create meeting
                var meeting = new Meeting
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Convert IFormFile to byte array
                using var memoryStream = new MemoryStream();
                await request.AudioFile.CopyToAsync(memoryStream);
                var audioBytes = memoryStream.ToArray();

                // Upload audio file to AssemblyAI
                var audioUrl = await _summaryService.UploadAudioAsync(audioBytes);
                meeting.AudioUrl = audioUrl;

                // Save meeting
                await _meetingService.CreateAsync(meeting);

                // Generate summary and tickets
                var result = await _summaryService.GenerateSummaryAndTicketsAsync(audioUrl, meeting.Id);

                return Ok(new
                {
                    Meeting = new
                    {
                        meeting.Id,
                        meeting.Title,
                        meeting.AudioUrl,
                        meeting.CreatedAt,
                        meeting.UpdatedAt
                    },
                    Summary = result.Summary,
                    Tickets = result.Tickets.Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.Status,
                        t.Platform,
                        t.CreatedAt,
                        t.UpdatedAt
                    })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recording: {Title}", request.Title);
                return StatusCode(500, "An error occurred while processing the recording");
            }
        }
    }

    public class ProcessRecordingRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public IFormFile AudioFile { get; set; } = null!;
    }
} 