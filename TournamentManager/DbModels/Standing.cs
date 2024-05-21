namespace TournamentManager.DbModels
{
    public class Standing
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int PlayerId { get; set; }
        public string Percentage { get; set; }
        public int Score { get; set; }

        public Player Player { get; set; }
        public Song Song { get; set; }
        public ICollection<StandingInRound> StandingsInRounds { get; set; }
    }

}
