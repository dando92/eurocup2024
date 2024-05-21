using Microsoft.EntityFrameworkCore;
using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public class TournamentDbContext : DbContext
    {
        public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }

        // General lists
        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Song> Songs { get; set; }

        //Tournament
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<Match> Matches { get; set; }
        public virtual DbSet<Phase> Phases { get; set; }
        public virtual DbSet<Round> Rounds { get; set; }
        public virtual DbSet<Standing> Standings { get; set; }

        // n-to-n relations
        public virtual DbSet<PlayerInMatch> PlayersInMatches { get; set; }
        public virtual DbSet<SongInMatch> SongsInMatches { get; set; }
        public virtual DbSet<StandingInRound> StandingInRounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(e =>
            {
                e.ToTable("Players");
                e.HasKey(p => p.Id);
            });

            modelBuilder.Entity<Song>(e =>
            {
                e.ToTable("Songs");
                e.HasKey(s => s.Id);
            });

            modelBuilder.Entity<Division>(e =>
            {
                e.ToTable("Divisions");
                e.HasKey(d => d.Id);
            });

            modelBuilder.Entity<Match>(e =>
            {
                e.ToTable("Matches");
                e.HasKey(m => m.Id);
            });

            modelBuilder.Entity<Phase>(e =>
            {
                e.ToTable("Phases");
                e.HasKey(p => p.Id);
            });

            modelBuilder.Entity<Round>(e =>
            {
                e.ToTable("Rounds");
                e.HasKey(r => r.Id);
            });

            modelBuilder.Entity<Standing>(e =>
            {
                e.ToTable("Standings");
                e.HasKey(s => s.Id);
            });

            modelBuilder.Entity<PlayerInMatch>(e =>
            {
                e.ToTable("PlayersInMatches");
                e.HasKey(pim => new { pim.PlayerId, pim.MatchId });
            });

            modelBuilder.Entity<SongInMatch>(e =>
            {
                e.ToTable("SongsInMatches");
                e.HasKey(sim => new { sim.SongId, sim.MatchId });
            });

            modelBuilder.Entity<StandingInRound>(e =>
            {
                e.ToTable("StandingsInRounds");
                e.HasKey(sir => new { sir.StandingId, sir.RoundId });
            });

            // Division has multiple phases
            modelBuilder.Entity<Phase>()
                .HasOne(p => p.Division)
                .WithMany(d => d.Phases)
                .HasForeignKey(p => p.DivisionId);

            // Phase has multiple matches
            modelBuilder.Entity<Match>()
                .HasOne(m => m.Phase)
                .WithMany(p => p.Matches)
                .HasForeignKey(m => m.PhaseId);

        }
    }
}
