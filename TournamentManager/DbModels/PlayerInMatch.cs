namespace TournamentManager.DbModels
{
    public class PlayerInMatch
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }

        public virtual Player? Player { get; set; }
        public virtual Match? Match { get; set; }
    }
}
