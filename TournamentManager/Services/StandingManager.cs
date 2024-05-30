using Microsoft.AspNet.SignalR;
using System.Globalization;
using System.Text.RegularExpressions;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class StandingManager : IStandingManager
    {
        private readonly ITournamentCache _cache;
        private readonly IMatchUpdate _hub;
        private readonly IGenericRepository<Standing> _standingRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly IGenericRepository<Player> _playerRepo;

        public StandingManager(IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IGenericRepository<Standing> standingRepo,
            ITournamentCache cache,
            IMatchUpdate matchUpdate)
        {
            _songRepo = songRepo;
            _playerRepo = playerRepo;
            _cache = cache;
            _standingRepo = standingRepo;
            _hub = matchUpdate;
        }

        public void AddStanding(Score score)
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
                Percentage = double.Parse(score.FormattedScore, CultureInfo.InvariantCulture),
                PlayerId = player.Id,
                SongId = song.Id,
                Song = song,
                Player = player,
                IsFailed = score.IsFailed
            };

            Standing duplicate = _standingRepo
               .GetAll()
               .Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.PlayerId)
               .FirstOrDefault();

            if (duplicate != null)
                return;

            AddStanding(standing);
        }

        public void AddStanding(Standing standing)
        {
            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return;

            if (standing.SongId == 0|| standing.PlayerId == 0)
                return;

            var playerInActiveMatch = _cache.ActiveMatch.PlayerInMatches.Where(pim => pim.PlayerId == standing.PlayerId).FirstOrDefault();
            var songInActiveMatch = _cache.ActiveMatch.SongInMatches.Where(pim => pim.SongId == standing.SongId).FirstOrDefault();

            if (playerInActiveMatch == null || songInActiveMatch == null)
                return;

            standing.RoundId = _cache.CurrentRound.Id;

            if (_cache.CurrentRound.Standings.Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.SongId).Count() > 0)
                return;
            
            _standingRepo.Add(standing);
            _cache.CurrentRound.Standings.Add(standing);

            if (_cache.CurrentRound.IsComplete())
            {
                _cache.CurrentRound.Standings.Recalc();
                
                foreach (var recalcStanding in _cache.CurrentRound.Standings)
                    _standingRepo.Update(recalcStanding);
                _cache.AdvanceRound();
                
                _hub?.OnMatchUpdate(new MatchUpdateDTO() { MatchId = _cache.ActiveMatch.Id, PhaseId = _cache.ActiveMatch.PhaseId, DivisionId = _cache.ActiveMatch.Phase.DivisionId });
            }
        }
        public bool DeleteStanding(Func<Standing, bool> shallDelete)
        {
            bool removed = false;

            if (_cache.ActiveMatch == null || _cache.CurrentRound == null)
                return removed;

            Round round = _cache.CurrentRound;

            foreach (var standing in round.Standings)
            {
                if (shallDelete(standing))
                {
                    _standingRepo.DeleteById(standing.Id);
                    round.Standings.Remove(standing);
                    removed = true;
                }
            }

            return removed;
        }
    }
}
