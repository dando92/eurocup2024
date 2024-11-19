using Microsoft.EntityFrameworkCore;
using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public class TournamentDbContext(DbContextOptions<TournamentDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // give table names on every entity
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<Match>().ToTable("Matches");
            modelBuilder.Entity<Division>().ToTable("Divisions");
            modelBuilder.Entity<Phase>().ToTable("Phases");
            modelBuilder.Entity<Song>().ToTable("Songs");
            modelBuilder.Entity<PlayerInMatch>().ToTable("PlayersInMatches");
            modelBuilder.Entity<SongInMatch>().ToTable("SongsInMatches");
            modelBuilder.Entity<Round>().ToTable("Rounds");
            modelBuilder.Entity<Standing>().ToTable("Standings");

            modelBuilder.Entity<PlayerInMatch>()
                       .HasKey(pim => new { pim.PlayerId, pim.MatchId });
            
            
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

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
