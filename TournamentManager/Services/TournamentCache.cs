using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentCache : ITournamentCache
    {
        private int _activeMatch;

        public int ActiveMatch { get => _activeMatch; }

        public TournamentCache()
        {
        }

        public void SetActiveMatch(Match match)
        {
            _activeMatch = match.Id;
        }

    }
}
