using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public class RawStandingSubscriber : IRawStandingSubscriber
    {
        private IStandingSubscriber _subscribers;

        private readonly IGenericRepository<Standing> _standingRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly IGenericRepository<Player> _playerRepo;

        public RawStandingSubscriber(IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IGenericRepository<Standing> standingRepo,
            IStandingSubscriber subscribers)
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
                SongId = song.Id,
                Song = song,
                Player = player
            };

            NotifyNewStanding(standing);

            _standingRepo.Add(standing);
        }

        private void NotifyNewStanding(Standing standing)
        {
            if (_subscribers == null)
                return;

            _subscribers.OnNewStanding(standing);
        }
    }
}
