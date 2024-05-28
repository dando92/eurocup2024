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

        public void OnNewStanding(RawStanding request)
        {
            foreach (Score score in request.Scores)
            {
                Song song = _songRepo
                    .GetAll()
                    .Where(s => s.Title == Path.GetFileName(score.Song) && s.Group == Path.GetDirectoryName(score.Song))
                    .FirstOrDefault();

                Player player = _playerRepo
                    .GetAll()
                    .Where(s => s.Name == score.PlayerName)
                    .FirstOrDefault();

                //Player or song not registered, do nothing
                if (song == null || player == null)
                    return;

                Standing standing = new Standing()
                {
                    Percentage = double.Parse(score.FormattedScore),
                    PlayerId = player.Id,
                    SongId = song.Id,
                    Song = song,
                    Player = player,
                    IsFailed = score.IsFailed
                };

                NotifyNewStanding(standing);

                if(standing.RoundId == null)
                    _standingRepo.Add(standing);
            }
        }

        private void NotifyNewStanding(Standing standing)
        {
            if (_subscribers == null)
                return;

            _subscribers.OnNewStanding(standing);
        }
    }
}
