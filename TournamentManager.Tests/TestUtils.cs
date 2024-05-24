using TournamentManager.DbModels;

namespace TournamentManager.Tests
{
    public static class TestUtils
    {
        public static List<Player> PlayersInMatch = new List<Player>()
        {
            new Player(){ Id = 0, Name="p1"},
            new Player(){ Id = 1, Name="p2"},
            new Player(){ Id = 2, Name="p3"},
            new Player(){ Id = 3, Name="p4"}
        };

        public static List<Song> SongsInMatch = new List<Song>()
        {
            new Song(){ Id = 0, Title="s1", Difficulty =7},
            new Song(){ Id = 1, Title="s2", Difficulty =8},
            new Song(){ Id = 2, Title="s3", Difficulty =9},
            new Song(){ Id = 3, Title="s4", Difficulty =10}
        };

        public static DbModels.Match Match = new DbModels.Match()
        {
            Id = 0,
            Name = "GironeA",
            Rounds = new List<Round>()
            {
                new Round(){Id = 0, Standings = new List<Standing>(), MatchId= 0 },
                new Round(){Id = 1, Standings = new List<Standing>(), MatchId= 0 },
                new Round(){Id = 2, Standings = new List<Standing>(), MatchId= 0 },
                new Round(){Id = 3, Standings = new List<Standing>(), MatchId= 0 }
            },
            PlayerInMatches = new List<PlayerInMatch>()
            {
                new PlayerInMatch(){ MatchId = 0, PlayerId = PlayersInMatch[0].Id },
                new PlayerInMatch(){ MatchId = 0, PlayerId = PlayersInMatch[1].Id },
                new PlayerInMatch(){ MatchId = 0, PlayerId = PlayersInMatch[2].Id },
                new PlayerInMatch(){ MatchId = 0, PlayerId = PlayersInMatch[3].Id }
            }
        };
    }
}