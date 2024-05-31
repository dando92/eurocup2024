using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Match
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Notes { get; set; }
        public int PhaseId { get; set; }
        public bool IsManualMatch { get; set; }

        [JsonIgnore]
        public Phase Phase { get; set; }
        public ICollection<Round> Rounds { get; set; }
        [JsonIgnore]
        public ICollection<PlayerInMatch> PlayerInMatches { get; set; }
        [JsonIgnore]
        public ICollection<SongInMatch> SongInMatches { get; set; }
    }
}
