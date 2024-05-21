namespace TournamentManager.DbModels
{
    public class Standing
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int PlayerId { get; set; }
        public required string Percentage { get; set; }
        public int Score { get; set; }

        public virtual Song? Song { get; set; }
        public virtual Player? Player { get; set; }
    }
}
