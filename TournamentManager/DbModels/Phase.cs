namespace TournamentManager.DbModels
{
    public class Phase
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }

        public Division Division { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
