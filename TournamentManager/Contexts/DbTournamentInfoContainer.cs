using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public class DbTournamentInfoContainer : ITournamentInfoContainer
    {
        private readonly IGenericRepository<Song> _songRepository;
        private readonly IGenericRepository<Player> _playerRepository;

        public DbTournamentInfoContainer(IGenericRepository<Song> songRepository, IGenericRepository<Player> playerRepository)
        {
            _songRepository = songRepository;
            _playerRepository = playerRepository;
        }

        public Song GetSongById(int id)
        {

            return _songRepository.GetAll().Where(song => song.Id == id).FirstOrDefault();
        }

        public Player GetPlayerById(int id)
        {
            return _playerRepository.GetAll().Where(player => player.Id == id).FirstOrDefault();
        }

        public Song GetSongByName(string name)
        {
            return _songRepository.GetAll().Where(player => player.GetTitleWithFolder() == name).FirstOrDefault();
        }

        public Player GetPlayerByName(string name)
        {
            return _playerRepository.GetAll().Where(player => player.Name == name).FirstOrDefault();
        }
    }
}
