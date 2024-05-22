using Microsoft.EntityFrameworkCore;
using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public class TournamentDbContext : DbContext
    {
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<PlayerInMatch> PlayersInMatches { get; set; }
        public DbSet<SongInMatch> SongsInMatches { get; set; }
        public DbSet<Standing> Standings { get; set; }

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
