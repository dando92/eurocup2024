namespace TournamentManager.Requests
{
    public class PostDeleteStanding
    {
        public int SongId { get; set; }
        public int PlayerId { get; set; }
    }

    public class PostEditStanding : PostDeleteStanding
    {
        public double Percentage { get; set; }
        public int Score { get; set; }
    }
}
