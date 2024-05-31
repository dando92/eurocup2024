using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class StandingManager : IStandingManager
    {
        private readonly ITournamentCache _cache;
        private readonly IMatchUpdate _hub;
        private readonly ILogUpdate _logHub;
        private readonly IGenericRepository<Standing> _standingRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly IGenericRepository<Player> _playerRepo;

        public StandingManager(IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IGenericRepository<Standing> standingRepo,
            ITournamentCache cache,
            IMatchUpdate matchUpdate,
            ILogUpdate logHub)
        {
            _songRepo = songRepo;
            _playerRepo = playerRepo;
            _cache = cache;
            _standingRepo = standingRepo;
            _hub = matchUpdate;
            _logHub = logHub;
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
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "No song or player found for standing" });
                return;
            }
                
            Standing standing = new Standing()
            {
                Percentage = double.Parse(score.FormattedScore, CultureInfo.InvariantCulture),
                PlayerId = player.Id,
                SongId = song.Id,
                IsFailed = score.IsFailed
            };

            Standing duplicate = _standingRepo
               .GetAll()
               .Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.PlayerId)
               .FirstOrDefault();

            if (duplicate != null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = $"duplicate found for {song.Title} - {player.Name}" });
                return;
            }
                
            AddStanding(standing);
        }

        public void AddStanding(Standing standing)
        {
            if (_cache.ActiveMatch == null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = $"No active match during standing push" });
                return;
            }
                
            if (standing.SongId == 0 || standing.PlayerId == 0)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "No song or player found for standing" });
                return;
            }
                
            Round round = _cache.GetRoundBySongId(standing.SongId);

            if (round == null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "Round not found for active match" });
                return;
            }
                
            var playerInActiveMatch = _cache.ActiveMatch.PlayerInMatches.Where(pim => pim.PlayerId == standing.PlayerId).FirstOrDefault();
            var songInActiveMatch = _cache.ActiveMatch.SongInMatches.Where(pim => pim.SongId == standing.SongId).FirstOrDefault();

            if (playerInActiveMatch == null || songInActiveMatch == null)
                return;

            standing.RoundId = round.Id;

            if (round.Standings.Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.SongId).Count() > 0)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "Skip standing, duplicate" });
                return;
            }
            try
            {
                _standingRepo.Add(standing);
                round.Standings.Add(standing);

                if (round.IsComplete())
                {
                    if (!_cache.ActiveMatch.IsManualMatch)
                    {
                        round.Standings.Recalc();

                        foreach (var recalcStanding in round.Standings)
                            _standingRepo.Update(recalcStanding);
                    }

                    _hub?.OnMatchUpdate(new MatchUpdateDTO() { MatchId = _cache.ActiveMatch.Id, PhaseId = _cache.ActiveMatch.PhaseId, DivisionId = _cache.ActiveMatch.Phase.DivisionId });
                }
            }
            catch(Exception ex)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Exception = ex.Message });
            }
        }

        public bool EditStanding(int playerdId, int songId, double percentage, int score)
        {
            bool edited = false;

            if (_cache.ActiveMatch == null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = $"No active match during standing edit" });
                return edited;
            }
                
            Round round = _cache.GetRoundBySongId(songId);

            if (round == null)
            { 
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "Round not found for active match" });
                return edited;
            }

            try
            {
                foreach (var standing in round.Standings)
                {
                    if (standing.PlayerId == playerdId && standing.SongId == songId)
                    {
                        standing.Percentage = percentage;
                        standing.Score = score;

                        _standingRepo.Update(standing);

                        edited = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Exception = ex.Message });
            }

            return edited;
        }

        public bool DeleteStanding(int playerdId, int songId)
        {
            bool removed = false;

            if (_cache.ActiveMatch == null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = $"No active match during standing edit" });
                return removed;
            }

            Round round = _cache.GetRoundBySongId(songId);

            if (round == null)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Message = "Round not found for active match" });
                return removed;
            }

            try
            {
                foreach (var standing in round.Standings)
                {
                    if (standing.PlayerId == playerdId && standing.SongId == songId)
                    {
                        _standingRepo.DeleteById(standing.Id);
                        round.Standings.Remove(standing);
                        removed = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logHub.OnLogUpdate(new LogUpdateDTO() { Exception = ex.Message });
            }

            return removed;
        }
    }
}
