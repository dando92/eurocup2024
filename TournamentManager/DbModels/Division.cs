namespace TournamentManager.DbModels
{
    public class Division
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Phase> Phases { get; set; }
    }
}
