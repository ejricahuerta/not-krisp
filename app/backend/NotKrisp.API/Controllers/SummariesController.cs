using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotKrisp.API.Models;
using NotKrisp.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using NotKrisp.API.Services;

namespace NotKrisp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : BaseController
    {
        private readonly ISummaryService _summaryService;
        private readonly ILogger<SummaryController> _logger;

        public SummaryController(
            ISummaryService summaryService,
            ILogger<SummaryController> logger)
        {
            _summaryService = summaryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Summary>>> GetSummaries()
        {
            try
            {
                var summaries = await _summaryService.GetAllAsync();
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Summary>> GetSummary(Guid id)
        {
            try
            {
                var summary = await _summaryService.GetByIdAsync(id);
                if (summary == null)
                    return NotFound();

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("meeting/{meetingId}")]
        public async Task<ActionResult<IEnumerable<Summary>>> GetByMeeting(Guid meetingId)
        {
            try
            {
                var summaries = await _summaryService.GetSummariesByMeetingAsync(meetingId);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting summaries for meeting: {MeetingId}", meetingId);
                return StatusCode(500, "An error occurred while retrieving summaries");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Summary>>> GetSummariesByStatus(string status)
        {
            try
            {
                var summaries = await _summaryService.GetSummariesByStatusAsync(status);
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Summary>> CreateSummary(Summary summary)
        {
            try
            {
                summary.Id = Guid.NewGuid();
                summary.CreatedAt = DateTime.UtcNow;
                summary.UpdatedAt = DateTime.UtcNow;
                summary.Status = "Processing";

                var createdSummary = await _summaryService.CreateAsync(summary);
                return CreatedAtAction(nameof(GetSummary), new { id = createdSummary.Id }, createdSummary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSummary(Guid id, Summary summary)
        {
            try
            {
                if (id != summary.Id)
                    return BadRequest();

                summary.UpdatedAt = DateTime.UtcNow;
                await _summaryService.UpdateAsync(id, summary);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<Summary>> UpdateSummaryStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var summary = await _summaryService.UpdateSummaryStatusAsync(id, status);
                return Ok(summary);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/content")]
        public async Task<ActionResult<Summary>> UpdateSummaryContent(Guid id, [FromBody] string content)
        {
            try
            {
                var summary = await _summaryService.UpdateSummaryContentAsync(id, content);
                return Ok(summary);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/action-items")]
        public async Task<ActionResult<Summary>> UpdateActionItems(Guid id, [FromBody] List<string> actionItems)
        {
            try
            {
                var summary = await _summaryService.UpdateActionItemsAsync(id, actionItems);
                return Ok(summary);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSummary(Guid id)
        {
            try
            {
                var result = await _summaryService.DeleteAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSummaryAndTickets([FromBody] GenerateSummaryRequest request)
        {
            try
            {
                _logger.LogInformation("Generating summary and tickets for meeting: {MeetingId}", request.MeetingId);
                
                var result = await _summaryService.GenerateSummaryAndTicketsAsync(
                    request.Transcription,
                    request.MeetingId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating summary and tickets");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class GenerateSummaryRequest
    {
        public Guid MeetingId { get; set; }
        public string Transcription { get; set; } = string.Empty;
    }
} 