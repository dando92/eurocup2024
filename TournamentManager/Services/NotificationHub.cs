using Microsoft.AspNetCore.SignalR;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class MatchUpdateHub : Hub<IMatchUpdate>
    { }

    public class NotificationHub(IHubContext<MatchUpdateHub, IMatchUpdate> context) : IMatchUpdate
    { 
        IHubContext<MatchUpdateHub, IMatchUpdate> _context = context;

        public void OnMatchUpdate(Match match)
        {
            _context.Clients.All.OnMatchUpdate(match);
        }
    }
}
