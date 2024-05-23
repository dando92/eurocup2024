using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public class TournamentDbContext : DbContext
    {
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerInMatch>()
                       .HasKey(pim => new { pim.PlayerId, pim.MatchId });

            modelBuilder.Entity<PlayerInMatch>()
                .HasOne(pim => pim.Player)
                .WithMany(p => p.PlayerInMatches)
                .HasForeignKey(pim => pim.PlayerId);

            modelBuilder.Entity<PlayerInMatch>()
                .HasOne(pim => pim.Match)
                .WithMany(m => m.PlayerInMatches)
                .HasForeignKey(pim => pim.MatchId);

            modelBuilder.Entity<SongInMatch>()
                .HasKey(sim => new { sim.SongId, sim.MatchId });

            modelBuilder.Entity<SongInMatch>()
                .HasOne(sim => sim.Song)
                .WithMany(s => s.SongInMatches)
                .HasForeignKey(sim => sim.SongId);

            modelBuilder.Entity<SongInMatch>()
                .HasOne(sim => sim.Match)
                .WithMany(m => m.SongInMatches)
                .HasForeignKey(sim => sim.MatchId);
        }
    }
}
