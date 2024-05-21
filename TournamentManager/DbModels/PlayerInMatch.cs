namespace TournamentManager.DbModels
{
    public class PlayerInMatch
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }

        public Player Player { get; set; }
        public Match Match { get; set; }
    }

}
