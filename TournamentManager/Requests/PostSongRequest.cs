namespace TournamentManager.Requests
{
    public class PostSongRequest
    {
        public string Title { get; set; }
        public int? Difficulty { get; set; }
        public string Group { get; set; }
    }
}
