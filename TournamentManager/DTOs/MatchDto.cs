using TournamentManager.DbModels;

namespace TournamentManager.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public ICollection<Song> Songs { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<Round> Rounds { get; set; }
    }
}
