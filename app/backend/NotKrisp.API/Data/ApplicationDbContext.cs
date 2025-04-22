using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NotKrisp.API.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace NotKrisp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Transcription> Transcriptions { get; set; }
        public DbSet<Summary> Summaries { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<IntegrationSettings> IntegrationSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Transcriptions)
                .WithOne(t => t.Meeting)
                .HasForeignKey(t => t.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Summaries)
                .WithOne(s => s.Meeting)
                .HasForeignKey(s => s.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Meeting>()
                .HasMany(m => m.Tickets)
                .WithOne(t => t.Meeting)
                .HasForeignKey(t => t.MeetingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Ticket entity
            modelBuilder.Entity<Ticket>()
                .Property(t => t.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null)
                )
                .Metadata.SetValueComparer(new ValueComparer<Dictionary<string, string>>(
                    (d1, d2) => d1.Count == d2.Count && !d1.Except(d2).Any(),
                    d => d.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    d => new Dictionary<string, string>(d)
                ));

            modelBuilder.Entity<IntegrationSettings>()
                .HasIndex(s => s.IntegrationType)
                .IsUnique();
        }
    }
} 