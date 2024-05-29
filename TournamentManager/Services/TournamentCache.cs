﻿using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public interface ITournamentCache
    {
        Round CurrentRound { get; }
        Match ActiveMatch { get; }
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

        public TournamentCache()
        {
        }

        public void SetActiveMatch(Match match)
        {
            if (match == _activeMatch)
                return;
            
            if (_activeMatch != null)
            {
                _iterator = null;
                _currentRound = null;
            }

            _activeMatch = match;
            AdvanceRound();
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
            {
                //If round is already populated.
                if (round.Standings.Count == _activeMatch.PlayerInMatches.Count)
                    continue;
             
                yield return round;
            }
        }
    }
}
