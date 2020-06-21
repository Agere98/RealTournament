using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealTournament.Models;

namespace RealTournament.Data
{
    public class RealTournamentContext : DbContext
    {
        public RealTournamentContext(DbContextOptions<RealTournamentContext> options)
        : base(options)
        {
        }

        public virtual DbSet<Tournament> Tournament { get; set; }
        public virtual DbSet<Participant> Participant { get; set; }
        public virtual DbSet<Game> Game { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Time })
                    .IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Discipline).IsRequired();
                entity.Property(e => e.Organizer).IsRequired();
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(e => new { e.TournamentId, e.UserId });
                entity.HasIndex(e => new { e.TournamentId, e.LicenseNumber })
                    .IsUnique();
                entity.HasIndex(e => new { e.TournamentId, e.Ranking })
                    .IsUnique();
                entity.Property(e => e.LicenseNumber).IsRequired();
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasIndex(e => new
                {
                    e.TournamentId,
                    e.Phase,
                    e.FirstPlayerId,
                    e.SecondPlayerId
                }).IsUnique();
                entity.HasOne(e => e.FirstPlayer)
                    .WithMany(p => p.GamesAsFirst)
                    .HasForeignKey(e => new { e.TournamentId, e.FirstPlayerId })
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SecondPlayer)
                    .WithMany(p => p.GamesAsSecond)
                    .HasForeignKey(e => new { e.TournamentId, e.SecondPlayerId })
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.FirstPlayerId).IsRequired();
                entity.Property(e => e.SecondPlayerId).IsRequired();
            });
        }
    }
}
