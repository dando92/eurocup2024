namespace TournamentManager.DbModels
{
    public class Division
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public virtual ICollection<Phase>? Phases { get; set; }
    }
}
