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

    public class TournamentCache : ITournamentCache
    {
        private IEnumerator<Round> _iterator;
        private Round _currentRound = null;
        private Match _activeMatch;
        
        public Round CurrentRound { get => _currentRound; }
        public Match ActiveMatch { get => _activeMatch; }
        public List<Standing> Standings { get; set; }

        public TournamentCache()
        {
            Standings = new List<Standing>();
        }

        public void SetActiveMatch(Match match)
        {
            if (match == _activeMatch)
                return;

            if (_activeMatch != null && match == null)
            {
                _iterator = null;
                _currentRound = null;
                _activeMatch = match;
            }
            else if (_activeMatch == null && match != null)
            {
                _activeMatch = match;
                AdvanceRound();
            }
        }

        public Round AdvanceRound()
        {
            if (_activeMatch == null)
                return null; 

            if (_iterator == null)
                _iterator = GetIterator();
            
            bool notLastElement =_iterator.MoveNext();

            if (notLastElement)
                _currentRound = _iterator.Current;
            else
                _currentRound = null;

            return _currentRound;
        }

        private IEnumerator<Round> GetIterator()
        {
            foreach (var round in _activeMatch.Rounds)
                yield return round;
        }
    }
}
