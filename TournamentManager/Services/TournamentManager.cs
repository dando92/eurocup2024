using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentManager : IStandingSubscriber
    {
        private TournamentCache _cache;
        private readonly IMatchUpdate _hub;

        public TournamentManager(TournamentCache cache, IMatchUpdate hub)
        {
            _cache = cache;
            _hub = hub;
        }

        public void OnNewStanding(Standing standing)
        {
            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return;

            if (standing.Song == null || standing.Player == null)
                return;

            var playerInActiveMatch = _cache.ActiveMatch.PlayerInMatches.Where(pim => pim.PlayerId == standing.PlayerId).FirstOrDefault();
            var songInActiveMatch = _cache.ActiveMatch.SongInMatches.Where(pim => pim.SongId == standing.SongId).FirstOrDefault();
            
            if (playerInActiveMatch == null || songInActiveMatch == null)
                return;

            standing.RoundId = _cache.CurrentRound.Id;

            if (_cache.CurrentRound.Standings.Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.SongId).Count() > 0)
                return;

            _cache.CurrentRound.Standings.Add(standing);

            if (_cache.CurrentRound.Standings.Count >= _cache.ActiveMatch.PlayerInMatches.Count)
            {
                _cache.CurrentRound.Standings.Recalc();
                _cache.AdvanceRound();

                _hub?.OnMatchUpdate(_cache.ActiveMatch);
            }
        }

        public bool DeleteStanding(Func<Standing, bool> shallDelete)
        {
            bool removed = false;

            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return removed;
            
            Round round = _cache.CurrentRound;

            foreach (var standing in round.Standings)
            {
                if (shallDelete(standing))
                {
                    round.Standings.Remove(standing);
                    removed = true;
                }
            }

            return removed;
        }
    }
}
