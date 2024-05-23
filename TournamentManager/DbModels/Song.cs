namespace TournamentManager.DbModels
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Difficulty { get; set; }
        public string Group { get; set; }

        public ICollection<SongInMatch> SongInMatches { get; set; }
        public ICollection<Standing> Standings { get; set; }
        public string GetTitleWithFolder() => Path.Combine(Group, Title);
    }

}
