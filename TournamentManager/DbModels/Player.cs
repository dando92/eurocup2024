using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? TeamId { get; set; }

        public int Score { get; set; }

        [JsonIgnore]
        public ICollection<PlayerInMatch> PlayerInMatches { get; set; }
        [JsonIgnore]
        public ICollection<Standing> Standings { get; set; }

        [JsonIgnore]
        public Team Team { get; set; }
    }

}
