using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentCache : ITournamentCache
    {
        private IEnumerator<Round> _iterator;
        private readonly IMatchUpdate _hub;
        private Round _currentRound = null;
        private Match _activeMatch;
        
        public Round CurrentRound { get => _currentRound; }
        public Match ActiveMatch { get => _activeMatch; }

        public TournamentCache(IMatchUpdate hub)
        {
            _hub = hub;
        }

        public void SetActiveMatch(Match match)
        {
            if (_activeMatch != null)
            {
                _iterator = null;
                _currentRound = null;
            }

            _activeMatch = match;
            
            AdvanceRound();

            _hub.OnMatchUpdate(new MatchUpdateDTO() { MatchId = _activeMatch.Id, PhaseId = _activeMatch.PhaseId, DivisionId = _activeMatch.Phase.DivisionId })
        }

        public Round AdvanceRound(bool forceRestart = false)
        {
            if (_activeMatch == null)
                return null; 

            if (_iterator == null || forceRestart)
                _iterator = GetIterator();

            bool notLastElement =_iterator.MoveNext();
            
            if (notLastElement)
            {
                _currentRound = _iterator.Current;
            }
            else
            {
                _iterator = GetIterator();
                _currentRound = null;
            }
                

            return _currentRound;
        }

        private IEnumerator<Round> GetIterator()
        {
            foreach (var round in _activeMatch.Rounds)
            {
                //If round is already populated.
                if (round.IsComplete())
                    continue;
             
                yield return round;
            }
        }
    }
}
