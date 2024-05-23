using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager
{
    public class DbTournamentInfoContainer : ITournamentInfoContainer
    {
        private Match _activeMatch = null;
        private Round _currentRound = null;

        private readonly IGenericRepository<Song> _songRepository;
        private readonly IGenericRepository<Player> _playerRepository;
        private readonly IGenericRepository<Standing> _standingRepository;
        public DbTournamentInfoContainer(IGenericRepository<Song> songRepository, IGenericRepository<Player> playerRepository, IGenericRepository<Standing> standingRepository)
        {
            _songRepository = songRepository;
            _playerRepository = playerRepository;
            _standingRepository = standingRepository;
        }

        public Match GetActiveMatch()
        {
            return _activeMatch;
        }

        public Round GetCurrentRound()
        {
            return _currentRound;
        }

        public Song GetSongById(int id)
        {
            
            return _songRepository.GetAll().Where(song => song.Id == id).FirstOrDefault();
        }

        public Player GetPlayerById(int id)
        {
            return _playerRepository.GetAll().Where(player => player.Id == id).FirstOrDefault();
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
                _currentRound = match.AdvanceRound().Current;
            }

            _activeMatch = match;
        }

        public Song GetSongByName(string name)
        {
            return _songRepository.GetAll().Where(player => player.GetTitleWithFolder() == name).FirstOrDefault();
        }

        public Player GetPlayerByName(string name)
        {
            return _playerRepository.GetAll().Where(player => player.Name == name).FirstOrDefault();
        }

        public void AddStanding(Standing standing)
        {
            _standingRepository.Add(standing);
        }
    }
}
