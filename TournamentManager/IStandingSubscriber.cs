using TournamentManager.DbModels;

namespace TournamentManager
{
    public interface IStandingSubscriber
    {
        void OnNewStanding(Standing standing);
    }
}
