namespace TournamentManager.DbModels
{
    public class StandingInRound
    {
        public int StandingId { get; set; }
        public int RoundId { get; set; }

        public virtual Standing? Standing { get; set; }
        public virtual Round? Round { get; set; }
    }
}
