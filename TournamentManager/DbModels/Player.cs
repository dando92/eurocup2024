namespace TournamentManager.DbModels
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PlayerInMatch> PlayersInMatches { get; set; }
        public ICollection<Standing> Standings { get; set; }
    }

}
