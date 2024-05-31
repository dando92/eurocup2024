using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentCache : ITournamentCache
    {
        private Match _activeMatch;

        public Match ActiveMatch { get => _activeMatch; }

        public TournamentCache()
        {
        }

        public void SetActiveMatch(Match match)
        {
            _activeMatch = match;
        }

        public Round GetRoundBySongId(int id)
        {
            if (_activeMatch == null)
                return null;

            return _activeMatch.Rounds.Where(r => r.SongId == id).FirstOrDefault();
        }
    }
}
