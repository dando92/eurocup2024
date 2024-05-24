using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public interface ITournamentCache
    {
        Round CurrentRound { get; }
        Match ActiveMatch { get; }
        List<Standing> Standings { get; set; }
        Round AdvanceRound();
        void SetActiveMatch(Match match);
    }

    public class TorunamentCache : ITournamentCache
    {
        private IEnumerator<Round> _iterator;
        private Round _currentRound = null;
        private Match _activeMatch;
        
        public Round CurrentRound { get => _currentRound; }
        public Match ActiveMatch { get => _activeMatch; }
        public List<Standing> Standings { get; set; }

        public TorunamentCache()
        {
            Standings = new List<Standing>();
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
