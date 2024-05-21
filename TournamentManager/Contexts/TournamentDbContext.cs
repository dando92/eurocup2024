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
        public DbSet<PlayerInMatch> PlayersInMatches { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongInMatch> SongsInMatches { get; set; }
        public DbSet<Standing> Standings { get; set; }
        public DbSet<StandingInRound> StandingsInRounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerInMatch>()
                .HasKey(pm => new { pm.PlayerId, pm.MatchId });

            modelBuilder.Entity<PlayerInMatch>()
                .HasOne(pm => pm.Player)
                .WithMany(p => p.PlayersInMatches)
                .HasForeignKey(pm => pm.PlayerId);

            modelBuilder.Entity<PlayerInMatch>()
                .HasOne(pm => pm.Match)
                .WithMany(m => m.PlayersInMatches)
                .HasForeignKey(pm => pm.MatchId);

            modelBuilder.Entity<SongInMatch>()
                .HasKey(sm => new { sm.SongId, sm.MatchId });

            modelBuilder.Entity<SongInMatch>()
                .HasOne(sm => sm.Song)
                .WithMany(s => s.SongsInMatches)
                .HasForeignKey(sm => sm.SongId);

            modelBuilder.Entity<SongInMatch>()
                .HasOne(sm => sm.Match)
                .WithMany(m => m.SongsInMatches)
                .HasForeignKey(sm => sm.MatchId);

            modelBuilder.Entity<StandingInRound>()
                .HasKey(sr => new { sr.StandingId, sr.RoundId });

            modelBuilder.Entity<StandingInRound>()
                .HasOne(sr => sr.Standing)
                .WithMany(s => s.StandingsInRounds)
                .HasForeignKey(sr => sr.StandingId);

            modelBuilder.Entity<StandingInRound>()
                .HasOne(sr => sr.Round)
                .WithMany(r => r.StandingsInRounds)
                .HasForeignKey(sr => sr.RoundId);
        }
    }
}
