using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Standing
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int PlayerId { get; set; }
        public double Percentage { get; set; }
        public int RoundId { get; set; }
        public int Score { get; set; }
        public bool IsFailed { get; set; }

        public Player Player { get; set; }
    }
}
