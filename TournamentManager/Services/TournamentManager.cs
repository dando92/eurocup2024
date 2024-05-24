using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentManager : IStandingSubscriber
    {
        private IEnumerator<Round> _iterator;
        private Round _currentRound = null;
        private Match _activeMatch;

        private List<Standing> _localStandings;

        private readonly IGenericRepository<Round> _roundRepository;

        public TournamentManager(IGenericRepository<Round> roundRepository)
        {
            _roundRepository = roundRepository;
            _localStandings = new List<Standing>();
        }

        public void SetActiveMatch(Match match)
        {
            if (match == _activeMatch)
                return;

            if (_activeMatch != null && match == null)
            {
                _currentRound = null;
            }
            else if (_activeMatch == null && match != null)
            {
                AdvanceRound();
            }

            _activeMatch = match;
        }

        public void OnNewStanding(Standing standing)
        {
            if (_activeMatch == null || _currentRound == null)
                return;
            
            standing.RoundId = _currentRound.Id;

            if (standing.Song == null || standing.Player == null)
                return;

            _localStandings.Add(standing);

            if (_localStandings.Count >= _activeMatch.PlayerInMatches.Count)
            {
                _localStandings = _localStandings.Recalc();

                foreach (var std in _localStandings)
                    _currentRound.Standings.Add(std);

                _roundRepository.Update(_currentRound);
                AdvanceRound();
                _localStandings.Clear();
            }

            //Match ended since all the rounds have been played
            if (_currentRound == null)
                _activeMatch = null;
        }

        public Round AdvanceRound()
        {
            if (_activeMatch == null)
                return null;

            if (_iterator == null)
                _iterator = GetIterator();
            else
                _iterator.MoveNext();

            _currentRound = _iterator.Current;

            return _currentRound;
        }

        private IEnumerator<Round> GetIterator()
        {
            foreach (var round in _activeMatch.Rounds)
                yield return round;
        }
    }
}
