using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public class RawStandingSubscriber : IRawStandingSubscriber
    {
        private List<IStandingSubscriber> _subscribers;

        private readonly IGenericRepository<Standing> _standingRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly IGenericRepository<Player> _playerRepo;

        public RawStandingSubscriber(IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IGenericRepository<Standing> standingRepo,
            List<IStandingSubscriber> subscribers)
        {
            _songRepo = songRepo;
            _playerRepo = playerRepo;
            _subscribers = subscribers;
            _standingRepo = standingRepo;
        }

        public void OnNewStanding(PostStandingRequest request)
        {
            Song song = _songRepo
                .GetAll()
                .Where(s => s.Title == request.Song)
                .FirstOrDefault();

            Player player = _playerRepo
                .GetAll()
                .Where(s => s.Name == request.Player)
                .FirstOrDefault();

            //Player or song not registered, do nothing
            if (song == null || player == null)
                return;

            Standing standing = new Standing()
            {
                Percentage = request.Percentage,
                PlayerId = player.Id,
                SongId = song.Id
            };

            NotifyNewStanding(standing);

            _standingRepo.Add(standing);

        }

        private void NotifyNewStanding(Standing standing)
        {
            if (_subscribers == null)
                return;

            foreach (var subscriber in _subscribers)
                subscriber.OnNewStanding(standing);
        }
    }

    public class TournamentManager : IStandingSubscriber
    {
        private IEnumerator<Round> _iterator;
        private Round _currentRound = null;
        private Match _activeMatch;

        private readonly ITournamentInfoContainer _infoProvider;

        private List<Standing> _localStandings;

        private readonly IGenericRepository<Round> _roundRepository;

        public TournamentManager(ITournamentInfoContainer infoProvider, IGenericRepository<Round> roundRepository)
        {
            _infoProvider = infoProvider;
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

            Song song = _infoProvider.GetSongById(standing.SongId);
            Player player = _infoProvider.GetPlayerById(standing.PlayerId);

            if (song == null || player == null)
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
