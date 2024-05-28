using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public interface IRawStandingSubscriber
    {
        void OnNewStanding(RawStanding standing);
    }

    public interface IStandingSubscriber
    {
        void OnNewStanding(Standing standing);
    }

    public interface IMatchUpdate
    {
        Task OnMatchUpdate(Match match);
    }
}
