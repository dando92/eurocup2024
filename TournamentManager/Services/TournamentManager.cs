using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentManager : IStandingSubscriber
    {
        private TorunamentCache _cache;
        private readonly IGenericRepository<Round> _roundRepository;
        private readonly IMatchUpdate _hub;

        public TournamentManager(TorunamentCache cache, IGenericRepository<Round> roundRepository, IMatchUpdate hub)
        {
            _cache = cache;
            _roundRepository = roundRepository;
            _hub = hub;
        }

        public void OnNewStanding(Standing standing)
        {
            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return;

            standing.RoundId = _cache.CurrentRound.Id;

            if (standing.Song == null || standing.Player == null)
                return;

            var playerInActiveMatch = _cache.ActiveMatch.PlayerInMatches.Where(pim => pim.PlayerId == standing.PlayerId).FirstOrDefault();
            var songInActiveMatch = _cache.ActiveMatch.SongInMatches.Where(pim => pim.SongId == standing.SongId).FirstOrDefault();
            
            if (playerInActiveMatch == null || songInActiveMatch == null)
                return;

            _cache.Standings.Add(standing);

            if (_cache.Standings.Count >= _cache.ActiveMatch.PlayerInMatches.Count)
            {
                _cache.Standings = _cache.Standings.Recalc();

                foreach (var std in _cache.Standings)
                    _cache.CurrentRound.Standings.Add(std);

                _roundRepository.Update(_cache.CurrentRound);
                
                _cache.AdvanceRound();

                _hub?.OnMatchUpdate(_cache.ActiveMatch);

                _cache.Standings.Clear();
            }
        }
    }
}
