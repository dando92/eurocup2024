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

            _cache.Standings.Add(standing);

            if (_cache.Standings.Count >= _cache.ActiveMatch.PlayerInMatches.Count)
            {
                _cache.Standings = _cache.Standings.Recalc();

                foreach (var std in _cache.Standings)
                    _cache.CurrentRound.Standings.Add(std);
                
                _cache.AdvanceRound();

                _hub?.OnMatchUpdate(_cache.ActiveMatch);

                _cache.Standings.Clear();
            }
        }

        public bool DeleteStanding(Func<Standing, bool> shallDelete)
        {
            bool removed = false;

            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return removed;
            
            Round round = _cache.CurrentRound;

            int standingsBeforeDeleteCount = round.Standings.Count;

            foreach (var standing in round.Standings)
            {
                if (shallDelete(standing))
                {
                    round.Standings.Remove(standing);
                    removed = true;
                }
            }

            if (removed)
            {
                int newStandingsCount = round.Standings.Count;


                if (standingsBeforeDeleteCount != newStandingsCount && //If count changed
                    standingsBeforeDeleteCount > 0 && // and there were any standings before
                    newStandingsCount > 0) //and removed some and not all
                {
                    //Reopen current round
                    _cache.Standings.AddRange(round.Standings);
                    round.Standings.Clear();
                }
            }

            return removed;
        }
    }
}
