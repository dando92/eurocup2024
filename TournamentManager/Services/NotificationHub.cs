using Microsoft.AspNetCore.SignalR;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
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

    public class NotificationHub(IHubContext<MatchUpdateHub, IMatchUpdate> matchContext, IHubContext<LogUpdateHub, ILogUpdate> logContext) : IMatchUpdate, ILogUpdate
    {
        IHubContext<MatchUpdateHub, IMatchUpdate> _matchContext = matchContext;
        IHubContext<LogUpdateHub, ILogUpdate> _logContext = logContext;

        public async Task OnMatchUpdate(MatchUpdateDTO match)
        {
            await _matchContext.Clients.All.OnMatchUpdate(match);
        }
        

        public async Task OnLogUpdate(LogUpdateDTO match)
        {
            await _logContext.Clients.All.OnLogUpdate(match);
        }
    }
}
