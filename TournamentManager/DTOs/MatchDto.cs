using TournamentManager.DbModels;

namespace TournamentManager.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Notes { get; set; }
        public double Multiplier { get; set; }
        public bool IsManualMatch { get; set; }
        public ICollection<Song> Songs { get; set; }
        public ICollection<Player> Players { get; set; }
        public ICollection<Round> Rounds { get; set; }
    }
}
