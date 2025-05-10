using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tucked.Core.Models;
using Tucked.Core.Services;

namespace Tucked.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MeetingController : ControllerBase
{
    private readonly IMeetingService _meetingService;
    private readonly ITicketService _ticketService;
    public MeetingController(IMeetingService meetingService, ITicketService ticketService)
    {
        _meetingService = meetingService;
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var meetings = await _meetingService.GetAllAsync();
        return Ok(meetings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var meeting = await _meetingService.GetByIdAsync(id);
        if (meeting == null) return NotFound();
        return Ok(meeting);
    }

    [HttpGet("{meetingId}/tickets")]
    public async Task<IActionResult> GetTicketsForMeeting(int meetingId)
    {
        var meeting = await _meetingService.GetByIdAsync(meetingId);
        if (meeting == null) return NotFound();
        var tickets = (await _ticketService.GetAllAsync()).Where(t => t.RelatedMeetingId == meetingId);
        return Ok(tickets);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Meeting meeting)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _meetingService.CreateAsync(meeting);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, Meeting meeting)
    {
        if (id != meeting.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _meetingService.UpdateAsync(meeting);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _meetingService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
} 