namespace TournamentManager.Requests
{
    public class PostActiveMatchRequest
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public int MatchId { get; set; }
    }
}
