using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tucked.Core.Models;

namespace Tucked.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<GitHubToken> GitHubTokens { get; set; } = null!;
        public DbSet<Meeting> Meetings { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure the GitHubToken entity
            builder.Entity<GitHubToken>()
                .HasOne(t => t.User)
                .WithMany(u => u.GitHubTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Meeting entity
            builder.Entity<Meeting>()
                .HasOne(m => m.CreatedBy)
                .WithMany()
                .HasForeignKey(m => m.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Meeting>()
                .HasMany(m => m.Tickets)
                .WithOne(t => t.RelatedMeeting)
                .HasForeignKey(t => t.RelatedMeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Ticket entity
            builder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany()
                .HasForeignKey(t => t.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 