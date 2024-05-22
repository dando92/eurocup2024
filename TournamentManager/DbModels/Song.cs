namespace TournamentManager.DbModels
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Difficulty { get; set; }
        public string Group { get; set; }

        public ICollection<SongInMatch> SongsInMatches { get; set; }
        public ICollection<Standing> Standings { get; set; }
    }

}
