namespace TournamentManager.DbModels
{
    public class Match
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public string Name { get; set; }

        public Phase Phase { get; set; }
        public ICollection<PlayerInMatch> PlayersInMatches { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public ICollection<SongInMatch> SongsInMatches { get; set; }
    }

}
