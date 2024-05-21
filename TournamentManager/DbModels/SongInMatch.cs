namespace TournamentManager.DbModels
{
    public class SongInMatch
    {
        public int SongId { get; set; }
        public int MatchId { get; set; }

        public Song Song { get; set; }
        public Match Match { get; set; }
    }

}
