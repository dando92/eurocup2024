using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Phase
    {
        public int Id { get; set; }
        public int DivisionId { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public Division Division { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
