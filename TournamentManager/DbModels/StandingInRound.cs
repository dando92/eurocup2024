namespace TournamentManager.DbModels
{
    public class StandingInRound
    {
        public int StandingId { get; set; }
        public int RoundId { get; set; }

        public Standing Standing { get; set; }
        public Round Round { get; set; }
    }

}
