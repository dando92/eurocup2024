using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Match
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PhaseId { get; set; }

        [JsonIgnore]
        public Phase Phase { get; set; }
        public ICollection<Round> Rounds { get; set; }
        public ICollection<PlayerInMatch> PlayerInMatches { get; set; }
        public ICollection<SongInMatch> SongInMatches { get; set; }
    }
}
