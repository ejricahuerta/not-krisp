using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotKrisp.Core.Models;
using NotKrisp.Core.Services;

namespace NotKrisp.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tickets = await _ticketService.GetAllAsync();
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _ticketService.GetByIdAsync(id);
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Ticket ticket)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _ticketService.CreateAsync(ticket);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, Ticket ticket)
    {
        if (id != ticket.Id) return BadRequest();
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await _ticketService.UpdateAsync(ticket);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _ticketService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
} 