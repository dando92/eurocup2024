using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentManager : IStandingSubscriber
    {
        private TorunamentCache _cache;
        private readonly IGenericRepository<Round> _roundRepository;

        public TournamentManager(TorunamentCache cache, IGenericRepository<Round> roundRepository)
        {
            _cache = cache;
            _roundRepository = roundRepository;
        }

        public void OnNewStanding(Standing standing)
        {
            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return;

            standing.RoundId = _cache.CurrentRound.Id;

            if (standing.Song == null || standing.Player == null)
                return;

            //TODO: check player in matches and song in matches

            _cache.Standings.Add(standing);

            if (_cache.Standings.Count >= _cache.ActiveMatch.PlayerInMatches.Count)
            {
                _cache.Standings = _cache.Standings.Recalc();

                foreach (var std in _cache.Standings)
                    _cache.CurrentRound.Standings.Add(std);

                _roundRepository.Update(_cache.CurrentRound);
                _cache.AdvanceRound();
                _cache.Standings.Clear();
            }
        }
    }
}
