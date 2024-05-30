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
        void AddStanding(Score standing);
        void AddStanding(Standing standing);
        bool DeleteStanding(Func<Standing, bool> shallDelete);
    }

    public interface IMatchUpdate
    {
        Task OnMatchUpdate(Match match);
    }

    public interface ITournamentCache
    {
        Round CurrentRound { get; }
        Match ActiveMatch { get; }
        Round AdvanceRound(bool forceRestart = false);
        void SetActiveMatch(Match match);
    }

}
