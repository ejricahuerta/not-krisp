using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tucked.Core.Services
{
    public class TicketSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TicketSyncService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                    var githubApiService = scope.ServiceProvider.GetRequiredService<GitHubApiService>();
                    var tickets = await ticketService.GetAllWithGitHubIssueIdAsync();
                    foreach (var ticket in tickets)
                    {
                        // You will need to get repoOwner, repoName, accessToken for each ticket/user
                        // For demo, use placeholders:
                        string repoOwner = "your-org";
                        string repoName = "your-repo";
                        string accessToken = "user-access-token";
                        if (int.TryParse(ticket.GitHubIssueId, out var issueNumber))
                        {
                            var issue = await githubApiService.GetIssueAsync(repoOwner, repoName, issueNumber, accessToken);
                            if (issue != null)
                            {
                                // Example: update local ticket status if closed on GitHub
                                if ((string)issue.state == "closed" && ticket.Status != Models.TicketStatus.Closed)
                                {
                                    ticket.Status = Models.TicketStatus.Closed;
                                    // Optionally update comments, etc.
                                    // Save changes to DB as needed
                                }
                            }
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
} 