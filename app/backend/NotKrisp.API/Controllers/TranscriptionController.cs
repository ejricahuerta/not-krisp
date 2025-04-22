using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotKrisp.API.Models;
using NotKrisp.API.Services.Interfaces;

namespace NotKrisp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscriptionController : BaseController
    {
        private readonly ITranscriptionService _transcriptionService;

        public TranscriptionController(ITranscriptionService transcriptionService)
        {
            _transcriptionService = transcriptionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transcription>>> GetTranscriptions()
        {
            try
            {
                var transcriptions = await _transcriptionService.GetAllAsync();
                return Ok(transcriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transcription>> GetTranscription(Guid id)
        {
            try
            {
                var transcription = await _transcriptionService.GetByIdAsync(id);
                if (transcription == null)
                    return NotFound();

                return Ok(transcription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("meeting/{meetingId}")]
        public async Task<ActionResult<IEnumerable<Transcription>>> GetTranscriptionsByMeeting(Guid meetingId)
        {
            try
            {
                var transcriptions = await _transcriptionService.GetTranscriptionsByMeetingAsync(meetingId);
                return Ok(transcriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Transcription>>> GetTranscriptionsByStatus(string status)
        {
            try
            {
                var transcriptions = await _transcriptionService.GetTranscriptionsByStatusAsync(status);
                return Ok(transcriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Transcription>> CreateTranscription(Transcription transcription)
        {
            try
            {
                transcription.Id = Guid.NewGuid();
                transcription.CreatedAt = DateTime.UtcNow;
                transcription.UpdatedAt = DateTime.UtcNow;
                transcription.Status = "Processing";

                var createdTranscription = await _transcriptionService.CreateAsync(transcription);
                return CreatedAtAction(nameof(GetTranscription), new { id = createdTranscription.Id }, createdTranscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTranscription(Guid id, Transcription transcription)
        {
            try
            {
                if (id != transcription.Id)
                    return BadRequest();

                transcription.UpdatedAt = DateTime.UtcNow;
                await _transcriptionService.UpdateAsync(id, transcription);
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
        public async Task<ActionResult<Transcription>> UpdateTranscriptionStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var transcription = await _transcriptionService.UpdateTranscriptionStatusAsync(id, status);
                return Ok(transcription);
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

        [HttpPut("{id}/text")]
        public async Task<ActionResult<Transcription>> UpdateTranscriptionText(Guid id, [FromBody] string text)
        {
            try
            {
                var transcription = await _transcriptionService.UpdateTranscriptionTextAsync(id, text);
                return Ok(transcription);
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
        public async Task<IActionResult> DeleteTranscription(Guid id)
        {
            try
            {
                var result = await _transcriptionService.DeleteAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
} 