namespace TournamentManager.DbModels
{
    public class Match
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public required string Name { get; set; }

        public virtual Phase? Phase { get; set; }
        public virtual ICollection<Round>? Rounds { get; set; }
        public virtual ICollection<Player>? Players { get; set; }
        public virtual ICollection<Song>? Songs { get; set; }
    }
}
