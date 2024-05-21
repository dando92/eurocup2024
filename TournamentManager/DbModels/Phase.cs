namespace TournamentManager.DbModels
{
    public class Phase
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public required string Name { get; set; }

        public virtual Division? Division { get; set; }
        public virtual ICollection<Match>? Matches { get; set; }
    }
}
