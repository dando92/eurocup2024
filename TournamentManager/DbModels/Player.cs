using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<PlayerInMatch> PlayerInMatches { get; set; }
        public ICollection<Standing> Standings { get; set; }
    }

}
