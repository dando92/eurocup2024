namespace TournamentManager.Requests
{
    public class PostAddMatch
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public string MatchName { get; set; }
        public string Group { get; set; }

        public string Levels { get; set; }
        public List<int> SongIds { get; set; }

        public int[] PlayerIds { get; set; }
    }

    public class PostAddSongToMatch
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public int MatchId { get; set; }

        public string Group { get; set; }
        public string Level { get; set; }
        public int SongId { get; set; }
    }

    public class PostEditSongToMatch
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public int MatchId { get; set; }

        public int EditSongId { get; set; }

        public string Group { get; set; }
        public string Level { get; set; }
        public int SongId { get; set; }
    }
}
