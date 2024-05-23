using TournamentManager.DbModels;

namespace TournamentManager.Tests
{
    public class MoqTorunamentContainer : ITournamentInfoContainer
    {
        public Round round1;
        public Round round2;
        public Match match;

        public MoqTorunamentContainer()
        {

            round1 = new Round()
            {
                Standings = new List<Standing>(),
            };
            round2 = new Round()
            {
                Standings = new List<Standing>(),
            };
            match = new Match()
            {
                Rounds = new List<Round>() { round1, round2 },
                PlayerInMatches = new List<PlayerInMatch>()
                {
                    new PlayerInMatch(),
                    new PlayerInMatch(),
                    new PlayerInMatch(),
                    new PlayerInMatch()
                }
                
            };
        }

        public void AddStanding(Standing standing) => throw new NotImplementedException();
        public Match GetActiveMatch() => match;
        public Round GetCurrentRound() => round1;
        public Player GetPlayerById(int id) =>  new Player();
        public Song GetSongByName(string name) => new Song();
        public void SetActiveMatch(Match match) => throw new NotImplementedException();
        public Player GetPlayerByName(string name)
        {
            return new Player();
        }

        public Song GetSongById(int id)
        {
            return new Song();
        }
    }
}