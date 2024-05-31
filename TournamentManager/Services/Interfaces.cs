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
        bool DeleteStanding(int playerdId, int songId);
        bool EditStanding(int playerdId, int songId, double percentage, int score);
    }

    public interface IMatchUpdate
    {
        Task OnMatchUpdate(MatchUpdateDTO match);
    }

    public interface ITournamentCache
    {
        Round CurrentRound { get; }
        Match ActiveMatch { get; }
        Round AdvanceRound(bool forceRestart = false);
        void SetActiveMatch(Match match);
    }

}
