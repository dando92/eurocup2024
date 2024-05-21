namespace TournamentManager.DbModels
{
    public class Round
    {
        public int Id { get; set; }
        public int MatchId { get; set; }

        public virtual ICollection<Standing>? Standings { get; set; }
    }
}
