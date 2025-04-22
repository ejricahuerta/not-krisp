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
    public class TicketController : BaseController
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            try
            {
                var tickets = await _ticketService.GetAllAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(Guid id)
        {
            try
            {
                var ticket = await _ticketService.GetByIdAsync(id);
                if (ticket == null)
                    return NotFound();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("meeting/{meetingId}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByMeeting(Guid meetingId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByMeetingAsync(meetingId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("platform/{platform}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByPlatform(string platform)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByPlatformAsync(platform);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicketsByStatus(string status)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByStatusAsync(status);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Ticket>> CreateTicket(Ticket ticket)
        {
            try
            {
                ticket.Id = Guid.NewGuid();
                ticket.CreatedAt = DateTime.UtcNow;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.Status = "Created";

                var createdTicket = await _ticketService.CreateAsync(ticket);
                return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, createdTicket);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(Guid id, Ticket ticket)
        {
            try
            {
                if (id != ticket.Id)
                    return BadRequest();

                ticket.UpdatedAt = DateTime.UtcNow;
                await _ticketService.UpdateAsync(id, ticket);
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
        public async Task<ActionResult<Ticket>> UpdateTicketStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var ticket = await _ticketService.UpdateTicketStatusAsync(id, status);
                return Ok(ticket);
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

        [HttpPut("{id}/url")]
        public async Task<ActionResult<Ticket>> UpdateTicketUrl(Guid id, [FromBody] string url)
        {
            try
            {
                var ticket = await _ticketService.UpdateTicketUrlAsync(id, url);
                return Ok(ticket);
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

        [HttpPut("{id}/metadata")]
        public async Task<ActionResult<Ticket>> UpdateTicketMetadata(Guid id, [FromBody] Dictionary<string, string> metadata)
        {
            try
            {
                var ticket = await _ticketService.UpdateTicketMetadataAsync(id, metadata);
                return Ok(ticket);
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
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            try
            {
                var result = await _ticketService.DeleteAsync(id);
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