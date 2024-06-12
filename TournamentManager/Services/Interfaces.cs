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

}
