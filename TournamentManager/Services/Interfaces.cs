using System.Runtime.CompilerServices;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

    public interface IScoringSystemProvider
    {
        IScoringSystem GetScoringSystem(string key);

        IEnumerable<string> GetPossibleScoringSystem();
    }

    public class ManualScoringSystemProvider : IScoringSystemProvider
    {
        public Dictionary<string, IScoringSystem> _systems;

        public ManualScoringSystemProvider(IEnumerable<IScoringSystem> scoringSystems)
        {
            _systems = new Dictionary<string, IScoringSystem>();

            foreach (IScoringSystem system in scoringSystems)
                Add(system);
        }

        public void Add(IScoringSystem scoreCalculator)
        {
            _systems.Add(scoreCalculator.Name, scoreCalculator);
        }

        public IEnumerable<string> GetPossibleScoringSystem()
        {
            return _systems.Keys;
        }

        public IScoringSystem GetScoringSystem(string key)
        {
            return _systems[key];
        }
    }

    public interface IScoringSystem
    {
        string Name { get; }
        string Description { get; }

        void Recalc(ICollection<Standing> standings, double multiplier = 1);
    }

    public class EurocupScoreCalculator : IScoringSystem
    {
        public EurocupScoreCalculator()
        {

        }

        public string Name => "EurocupScoreCalculator";

        public string Description => "Fail count 0";

        public void Recalc(ICollection<Standing> standings, double multiplier = 1)
        {
            int maxPoints = standings.Count;
            var orderedStandings = standings.Where(s => !s.IsFailed).OrderByDescending(s => s.Percentage).ToList();
            int tieCount = 0;

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Score = (int)(maxPoints * multiplier);

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

    public class TagTeamScoreCalculatorFailCount : IScoringSystem
    {
        public string Name => "TagTeamScoreCalculatorFailCount";

        public string Description => "Fail count points";

        public TagTeamScoreCalculatorFailCount()
        {
        }

        public void Recalc(ICollection<Standing> standings, double multiplier = 1)
        {
            int disabledPlayers = standings.Count(s => s.IsDisabled());
            int maxPoints = standings.Count - disabledPlayers;
            var orderedStandings = standings.Where(s => !s.IsDisabled()).OrderByDescending(s => s.Percentage).OrderByDescending(s => s.IsFailed ? 0 : 1).ToList();
            int tieCount = 0;

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].SetScore((int)(maxPoints * multiplier));

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
                    else if (orderedStandings[i].Percentage < orderedStandings[i + 1].Percentage)
                    {
                        if (orderedStandings[i].IsFailed != orderedStandings[i + 1].IsFailed)
                        {
                            if (tieCount > 0)
                            {
                                maxPoints -= tieCount;
                                tieCount = 0;
                            }

                            maxPoints--;
                        }
                    }
                    else if (orderedStandings[i].Percentage == orderedStandings[i + 1].Percentage)
                    {
                        if (orderedStandings[i].IsFailed == orderedStandings[i + 1].IsFailed)
                            tieCount++;
                        else
                        {
                            maxPoints -= tieCount;
                            tieCount = 0;
                            maxPoints--;
                        }
                    }
                }
            }
        }
    }


    public class TagTeamScoreCalculatorFailNotCount : IScoringSystem
    {
        public string Name => "TagTeamScoreCalculatorFailNotCount";

        public string Description => "Fail count 0 points";

        public TagTeamScoreCalculatorFailNotCount()
        {
        }

        public void Recalc(ICollection<Standing> standings, double multiplier = 1)
        {
            int disabledPlayers = standings.Count(s => s.IsFailed && s.Percentage == (double)-1);
            int maxPoints = standings.Count - disabledPlayers;
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
}
