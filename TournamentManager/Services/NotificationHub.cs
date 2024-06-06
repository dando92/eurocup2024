using Microsoft.AspNetCore.SignalR;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class ScoreUpdateDTO
    {
        public Score Score { get; set; }
    }

    public class ScoreUpdateHub : Hub<IScoreUpdate>
    { }

    public class LogUpdateDTO
    {
        public string Message { get; set; }
        public string Error { get; set; }
    }

    public class LogUpdateHub : Hub<ILogUpdate>
    { }

    public class MatchUpdateDTO
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public int MatchId { get; set; }

    }
    public class MatchUpdateHub : Hub<IMatchUpdate>
    { }

    public class NotificationHub(IHubContext<MatchUpdateHub, IMatchUpdate> matchContext, 
        IHubContext<LogUpdateHub, ILogUpdate> logContext,
        IHubContext<ScoreUpdateHub, IScoreUpdate> scoreUpdateContext) : IMatchUpdate, ILogUpdate, IScoreUpdate
    {
        IHubContext<MatchUpdateHub, IMatchUpdate> _matchContext = matchContext;
        IHubContext<ScoreUpdateHub, IScoreUpdate> _scoreUpdateContext = scoreUpdateContext;
        IHubContext<LogUpdateHub, ILogUpdate> _logContext = logContext;

        public async Task OnMatchUpdate(MatchUpdateDTO match)
        {
            await _matchContext.Clients.All.OnMatchUpdate(match);
        }
        
        public async Task OnLogUpdate(LogUpdateDTO match)
        {
            await _logContext.Clients.All.OnLogUpdate(match);
        }

        public async Task OnScoreUpdate(ScoreUpdateDTO score)
        {
            await _scoreUpdateContext.Clients.All.OnScoreUpdate(score);
        }
    }
}
