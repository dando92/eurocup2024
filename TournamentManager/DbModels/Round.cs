using System.Text.Json.Serialization;

namespace TournamentManager.DbModels
{
    public class Round
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SongId { get; set; }

        [JsonIgnore]
        public Match Match { get; set; }
        public ICollection<Standing> Standings { get; set; }
        
        public bool IsComplete() => Standings.Count >= Match.PlayerInMatches.Count;
    }

}
