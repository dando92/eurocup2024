using Microsoft.AspNetCore.SignalR;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class MatchUpdateDTO
    {
        public int DivisionId { get; set; }
        public int PhaseId { get; set; }
        public int MatchId { get; set; }

    }
    public class MatchUpdateHub : Hub<IMatchUpdate>
    { }

    public class NotificationHub(IHubContext<MatchUpdateHub, IMatchUpdate> context) : IMatchUpdate
    {
        IHubContext<MatchUpdateHub, IMatchUpdate> _context = context;

        public async Task OnMatchUpdate(MatchUpdateDTO match)
        {
            await _context.Clients.All.OnMatchUpdate(match);
        }
    }
}
