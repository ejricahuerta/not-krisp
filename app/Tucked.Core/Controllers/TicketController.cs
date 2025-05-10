using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tucked.Core.Models;
using Tucked.Core.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Tucked.Core.Data;
using Microsoft.Extensions.Logging;

namespace Tucked.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TicketExtractionService _extractionService;
    private readonly GitHubApiService _githubApi;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<TicketController> _logger;

    public TicketController(ITicketService ticketService, UserManager<ApplicationUser> userManager, TicketExtractionService extractionService, GitHubApiService githubApi, ApplicationDbContext dbContext, ILogger<TicketController> logger)
    {
        _ticketService = ticketService;
        _userManager = userManager;
        _extractionService = extractionService;
        _githubApi = githubApi;
        _dbContext = dbContext;
        _logger = logger;
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

    [HttpPost("test-github-issue")]
    public async Task<IActionResult> TestGitHubIssue()
    {
        _logger.LogInformation("TestGitHubIssue endpoint hit");
        try
        {
            string transcription = "TODO: Implement OAuth2 PKCE flow\nAction: Add dark mode to dashboard\nTODO: Optimize database indexes\nAction: Refactor notification service \nTODO: Implement OAuth2 PKCE flow\nAction: Add dark mode to dashboard\nTODO: Optimize database indexes\nAction: Refactor notification service \nTODO: Implement OAuth2 PKCE flow\nAction: Add dark mode to dashboard\nTODO: Optimize database indexes\nAction: Refactor notification service \nTODO: Implement OAuth2 PKCE flow\nAction: Add dark mode to dashboard\nTODO: Optimize database indexes\nAction: Refactor notification service ";
            string repoOwner = "ejricahuerta";
            string repoName = "not-krisp";
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("No user in request");
                return BadRequest("No user in request");
            }
            string githubAccessToken = user.GitHubAccessToken;

            var meeting = _dbContext.Meetings.FirstOrDefault();
            if (meeting == null)
            {
                meeting = new Tucked.Core.Models.Meeting
                {
                    Title = "Demo Meeting",
                    Date = DateTime.UtcNow,
                    Duration = TimeSpan.FromMinutes(30),
                    CreatedById = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.Meetings.Add(meeting);
                _dbContext.SaveChanges();
            }

            var tickets = await _ticketService.CreateFromTranscriptionAsync(
                transcription,
                repoOwner,
                repoName,
                githubAccessToken,
                _extractionService,
                _githubApi,
                user.Id,
                meeting.Id);
            _logger.LogInformation("Created {Count} tickets", tickets.Count);
            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in TestGitHubIssue endpoint");
            return StatusCode(500, ex.Message);
        }
    }
}