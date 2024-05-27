using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public interface IRawStandingSubscriber
    {
        void OnNewStanding(PostStandingRequest standing);
    }

    public interface IStandingSubscriber
    {
        void OnNewStanding(Standing standing);
    }

    public interface IMatchUpdate
    {
        void OnMatchUpdate(Match match);
    }
}
