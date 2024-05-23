using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class TournamentManager : IStandingSubscriber
    {
        private readonly ITournamentInfoContainer _infoProvider;

        private List<Standing> _localStandings;

        private readonly IGenericRepository<Round> _roundRepository;

        public Round Round => _infoProvider.GetCurrentRound();
        public Match Match => _infoProvider.GetActiveMatch();


        public TournamentManager(ITournamentInfoContainer infoProvider, IGenericRepository<Round> roundRepository)
        {
            _infoProvider = infoProvider;
            _roundRepository = roundRepository;
            _localStandings = new List<Standing>();
        }

        public void OnNewStanding(Standing standing)
        {
            if (Match == null || Round == null)
                return;

            Song song = _infoProvider.GetSongById(standing.SongId);
            Player player = _infoProvider.GetPlayerById(standing.PlayerId);

            if (song == null || player == null)
                return;

            _localStandings.Add(standing);

            if (_localStandings.Count >= Match.PlayerInMatches.Count)
            {
                _localStandings = _localStandings.Recalc();

                foreach (var std in _localStandings)
                    Round.Standings.Add(std);

                _roundRepository.Update(Round);
                Match.AdvanceRound();
                _localStandings.Clear();
            }

            //Match ended since all the rounds have been played
            if (Round == null)
                _infoProvider.SetActiveMatch(null);
        }
    }
}
