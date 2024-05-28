using TournamentManager.DbModels;

namespace TournamentManager.Tests
{
    public static class TournamentControllerDatabase
    {
        public static List<Match> Matches = new List<Match>()
        {

        };

        public static List<Phase> Phases = new List<Phase>()
        {
            new Phase()
            {
                DivisionId = 0,
                Id = 0,
                Name = "First round",
                Matches = TournamentControllerDatabase.Matches
            }
        };
        public static List<Division> Division = new List<Division>()
        {
            new Division()
            {
                Id = 0,
                Name="Lower",
                Phases = TournamentControllerDatabase.Phases
            },
            new Division()
            {
                Id = 1,
                Name="Mid",
                Phases = new List<Phase>()
                {
                    new Phase()
                    {
                        DivisionId = 1,
                        Id = 1,
                        Name = "First round",
                        Matches = new List<Match>()
                        { }
                    }
                }
            }
        };

        public static List<Player> Players = new List<Player>()
        {
            new Player(){ Id = 0, Name="p1"},
            new Player(){ Id = 1, Name="p2"},
            new Player(){ Id = 2, Name="p3"},
            new Player(){ Id = 3, Name="p4"},
            new Player(){ Id = 4, Name="p5"},
            new Player(){ Id = 5, Name="p6"},
            new Player(){ Id = 6, Name="p7"},
            new Player(){ Id = 7, Name="p8"},
            new Player(){ Id = 8, Name="p9"},
            new Player(){ Id = 9, Name="p10"},
            new Player(){ Id = 10, Name="p11"},
            new Player(){ Id = 11, Name="p12"}

        };

        public static List<Song> Songs = new List<Song>()
        {
            new Song(){ Id = 0, Title="s1",Group = "g1", Difficulty =8},
            new Song(){ Id = 1, Title="s2",Group = "g1", Difficulty =8},
            new Song(){ Id = 2, Title="s3",Group = "g1", Difficulty =8},
            new Song(){ Id = 3, Title="s4",Group = "g1", Difficulty =8},
            new Song(){ Id = 4, Title="s5",Group = "g1", Difficulty =9},
            new Song(){ Id = 5, Title="s6",Group = "g1", Difficulty =9},
            new Song(){ Id = 6, Title="s7",Group = "g2", Difficulty =9},
            new Song(){ Id = 7, Title="s8",Group = "g2", Difficulty =9},
            new Song(){ Id = 8, Title="s9",Group = "g2", Difficulty =10},
            new Song(){ Id = 9, Title="s10",Group = "g2", Difficulty =10},
            new Song(){ Id = 10, Title="s11",Group = "g2", Difficulty =10},
            new Song(){ Id = 11, Title="s12",Group = "g2", Difficulty =10}
        };
    }
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
            new Song(){ Id = 0, Title="s1",Group = "g1", Difficulty =8},
            new Song(){ Id = 1, Title="s2",Group = "g1", Difficulty =8},
            new Song(){ Id = 2, Title="s3",Group = "g2", Difficulty =8},
            new Song(){ Id = 3, Title="s4",Group = "g2", Difficulty =9},
            new Song(){ Id = 4, Title="s5",Group = "g2", Difficulty =9},
            new Song(){ Id = 5, Title="s6",Group = "g2", Difficulty =9}
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
            },
            SongInMatches = new List<SongInMatch>()
            {
                new SongInMatch(){ SongId = 0, MatchId = 0},
                new SongInMatch(){ SongId = 3, MatchId = 0},
                new SongInMatch(){ SongId = 2, MatchId = 0},
                new SongInMatch(){ SongId = 1, MatchId = 0}
            }
        };
    }
}