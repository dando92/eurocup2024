using Microsoft.AspNetCore.SignalR;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class MatchUpdateHub : Hub<IMatchUpdate>
    { }

    public class NotificationHub(IHubContext<MatchUpdateHub, IMatchUpdate> context) : IMatchUpdate
    {
        IHubContext<MatchUpdateHub, IMatchUpdate> _context = context;

        public async Task OnMatchUpdate(Match match)
        {
            await _context.Clients.All.OnMatchUpdate(match);
        }
    }
}
