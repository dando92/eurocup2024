namespace TournamentManager.Requests
{
    public class PostAddMatch
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public string MatchName { get; set; }
        public string Group { get; set; }
        public string Levels { get; set; }
        public int[] PlayerIds { get; set; }
    }
}
