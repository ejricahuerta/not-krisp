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
    public class MeetingController : BaseController
    {
        private readonly IMeetingService _meetingService;

        public MeetingController(IMeetingService meetingService)
        {
            _meetingService = meetingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetings()
        {
            try
            {
                var meetings = await _meetingService.GetAllAsync();
                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meeting>> GetMeeting(Guid id)
        {
            try
            {
                var meeting = await _meetingService.GetByIdAsync(id);
                if (meeting == null)
                    return NotFound();

                return Ok(meeting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Meeting>> CreateMeeting(Meeting meeting)
        {
            try
            {
                meeting.Id = Guid.NewGuid();
                meeting.CreatedAt = DateTime.UtcNow;
                meeting.UpdatedAt = DateTime.UtcNow;
                meeting.Status = "Created";

                var createdMeeting = await _meetingService.CreateAsync(meeting);
                return CreatedAtAction(nameof(GetMeeting), new { id = createdMeeting.Id }, createdMeeting);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeeting(Guid id, Meeting meeting)
        {
            try
            {
                if (id != meeting.Id)
                    return BadRequest();

                meeting.UpdatedAt = DateTime.UtcNow;
                await _meetingService.UpdateAsync(id, meeting);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeeting(Guid id)
        {
            try
            {
                var result = await _meetingService.DeleteAsync(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetingsByStatus(string status)
        {
            try
            {
                var meetings = await _meetingService.GetMeetingsByStatusAsync(status);
                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("platform/{platform}")]
        public async Task<ActionResult<IEnumerable<Meeting>>> GetMeetingsByPlatform(string platform)
        {
            try
            {
                var meetings = await _meetingService.GetMeetingsByPlatformAsync(platform);
                return Ok(meetings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult<Meeting>> StartMeeting(Guid id)
        {
            try
            {
                var meeting = await _meetingService.StartMeetingAsync(id);
                return Ok(meeting);
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

        [HttpPost("{id}/end")]
        public async Task<ActionResult<Meeting>> EndMeeting(Guid id)
        {
            try
            {
                var meeting = await _meetingService.EndMeetingAsync(id);
                return Ok(meeting);
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

        [HttpGet("{id}/audio")]
        public async Task<ActionResult<string>> GetMeetingAudioUrl(Guid id)
        {
            try
            {
                var audioUrl = await _meetingService.GetMeetingAudioUrlAsync(id);
                return Ok(audioUrl);
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
    }
} 