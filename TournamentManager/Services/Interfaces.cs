using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public interface ISongRoller
    {
        List<int> RollSongs(int divisionId, string group, string levels);
        int RollSong(int divisionId, string group, int level);
    }

    public interface IStandingManager
    {
        bool AddStanding(Score standing);
        bool AddStanding(Standing standing);
        bool DeleteStanding(int playerdId, int songId);
        bool EditStanding(int playerdId, int songId, double percentage, int score);
    }

    public interface IMatchUpdate
    {
        Task OnMatchUpdate(MatchUpdateDTO match);
    }

    public interface ILogUpdate
    {
        Task OnLogUpdate(LogUpdateDTO log);
    }

    public interface IScoreUpdate
    {
        Task OnScoreUpdate(ScoreUpdateDTO score);
    }

    public interface ITournamentCache
    {
        int ActiveMatch { get; }
        void SetActiveMatch(Match match);
    }


    public interface IScoreCalculator
    {
        void Recalc(ICollection<Standing> standings);
    }

    public class MatchScoreCalculator : IScoreCalculator
    {
        public void Recalc(ICollection<Standing> standings)
        {
            int maxPoints = standings.Count;
            var orderedStandings = standings.Where(s => !s.IsFailed).OrderByDescending(s => s.Percentage).ToList();
            int tieCount = 0;

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Score = maxPoints;

                if (i + 1 < orderedStandings.Count)
                {
                    if (orderedStandings[i].Percentage > orderedStandings[i + 1].Percentage)
                    {
                        if (tieCount > 0)
                        {
                            maxPoints -= tieCount;
                            tieCount = 0;
                        }

                        maxPoints--;
                    }
                    else if (orderedStandings[i].Percentage == orderedStandings[i + 1].Percentage)
                        tieCount++;
                }
            }
        }
    }

    public class TeamScoreCalculator : IScoreCalculator
    {
        IScoreCalculator _matchScoreCalculator;
        IGenericRepository<Team> _teamRepo;
        IGenericRepository<Match> _matchRepo;

        public TeamScoreCalculator(IGenericRepository<Team> teamRepo, IGenericRepository<Match> matchRepo)
        {
            _teamRepo = teamRepo;
            _matchRepo = matchRepo;

            _matchScoreCalculator = new MatchScoreCalculator();
        }

        public void Recalc(ICollection<Standing> standings)
        {
            _matchScoreCalculator.Recalc(standings);

            var matches = _matchRepo.GetAll();
            
            var teams = _teamRepo.GetAll();

            foreach (var team in teams)
                team.Score = 0;

            foreach (var match in matches)
            {
                foreach(var round in match.Rounds)
                {
                    foreach (var standing in round.Standings)
                    {
                        standing.Player.Team.Score += standing.Score;
                    }
                }
            }
        }
    }
}
