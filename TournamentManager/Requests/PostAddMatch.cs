﻿namespace TournamentManager.Requests
{
    public class PostAddMatch
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public string MatchName { get; set; }
        public string Group { get; set; }
        public string Subtitle { get; set; }
        public bool IsManualMatch { get; set; }
        public double Multiplier { get; set; }
        public string Notes { get; set; }
        public string Levels { get; set; }
        public List<int> SongIds { get; set; }
        public string ScoringSystem { get; set; }
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

    public class PostEditSongToMatch : PostAddSongToMatch
    {
        public int EditSongId { get; set; }
    }
}
