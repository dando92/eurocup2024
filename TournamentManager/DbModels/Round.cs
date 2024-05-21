namespace TournamentManager.DbModels
{
    public class Round
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public string Title { get; set; }

        public Match Match { get; set; }
        public ICollection<StandingInRound> StandingsInRounds { get; set; }
    }

}
