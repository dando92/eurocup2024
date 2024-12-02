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


    public interface IScoreCalculator
    {
        void Recalc(ICollection<Standing> standings, double multiplier = 1);
    }

    public class MatchScoreCalculator : IScoreCalculator
    {
        public MatchScoreCalculator()
        {

        }

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

    public class TeamScoreCalculator : IScoreCalculator
    {
        public TeamScoreCalculator()
        {
        }

        public void RecalcRound(ICollection<Standing> standings, double multiplier = 1)
        {
            int disabledPlayers = standings.Count(s => s.IsDisabled());
            int maxPoints = standings.Count - disabledPlayers;
            var orderedStandings = standings.Where(s => !s.IsDisabled()).OrderByDescending(s => s.Percentage).OrderByDescending(s => s.IsFailed ? 0 : 1).ToList();
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
                        if(orderedStandings[i].IsFailed == orderedStandings[i + 1].IsFailed)
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

        public void Recalc(ICollection<Standing> standings, double multiplier = 1)
        {
            RecalcRound(standings, multiplier);

            foreach (var standing in standings)
            {
                standing.Player.Score += standing.Score;
                standing.Player.Team.Score += standing.Score;
            }
        }
    }
}
