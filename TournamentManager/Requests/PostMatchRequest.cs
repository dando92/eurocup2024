namespace TournamentManager.Requests
{
    public class PostMatchRequest
    {
        public int MatchId { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Notes { get; set; }
        public double Multiplier { get; set; }
        public string ScoringSystem { get; set; }
    }
}
