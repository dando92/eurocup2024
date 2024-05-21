namespace TournamentManager.DbModels
{
    public class SongInMatch
    {
        public int SongId { get; set; }
        public int MatchId { get; set; }

        public virtual Song? Song { get; set; }
        public virtual Match? Match { get; set; }
    }
}
