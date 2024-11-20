using System.Globalization;
using System.Linq;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        private readonly IMatchManager _matchManager;
        private readonly IScoreCalculator _calculator;

        public StandingManager(IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IGenericRepository<Standing> standingRepo,
            ITournamentCache cache,
            IMatchUpdate matchUpdate,
            ILogUpdate logHub,
            IMatchManager matchManager,
            IScoreCalculator calculator)
        {
            _songRepo = songRepo;
            _playerRepo = playerRepo;
            _cache = cache;
            _standingRepo = standingRepo;
            _hub = matchUpdate;
            _logHub = logHub;
            _matchManager = matchManager;
            _calculator = calculator;
        }

        public bool AddStanding(Score score)
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
                _logHub.LogError($"No song or player found for standing {score.Song} - {score.PlayerName}");
                return false;
            }
            
            Standing standing = new Standing()
            {
                Percentage = double.Parse(score.FormattedScore, CultureInfo.InvariantCulture),
                PlayerId = player.Id,
                SongId = song.Id,
                IsFailed = score.IsFailed
            };

            return AddStanding(standing);
        }

        public bool AddStanding(Standing standing)
        {
            Match activeMatch = GetActiveMatch();

            if (activeMatch == null)
            {
                _logHub.LogError($"No active match during standing push");
                return false;
            }
                
            if (standing.SongId == 0 || standing.PlayerId == 0)
            {
                _logHub.LogError("No song or player found for standing");
                return false;
            }
                
            Round round = GetRoundBySongId(standing.SongId);

            if (round == null)
            {
                _logHub.LogError("Round not found for active match" );
                return false;
            }
                
            var playerInActiveMatch = activeMatch.PlayerInMatches.Where(pim => pim.PlayerId == standing.PlayerId).FirstOrDefault();
            var songInActiveMatch = activeMatch.SongInMatches.Where(pim => pim.SongId == standing.SongId).FirstOrDefault();

            if (playerInActiveMatch == null || songInActiveMatch == null)
                return false;

            standing.RoundId = round.Id;

            if (round.Standings.Where(s => s.PlayerId == standing.PlayerId && s.SongId == standing.SongId).Count() > 0)
            {
                _logHub.LogError("Skip standing, duplicate");
                return false;
            }

            try
            {
                _logHub.LogMessage($"Add standing: {standing.PlayerId} Song: {standing.SongId}");
                // _standingRepo.Add(standing);
                _logHub.LogMessage($"Add to round: {standing.PlayerId} Song: {standing.SongId}");
                round.Standings.Add(standing);

                if ((round.Standings.Count >= activeMatch.PlayerInMatches.Count) && (!activeMatch.IsManualMatch))
                {
                    if (!activeMatch.IsManualMatch)
                    {
                        _logHub.LogMessage("Standing recalc...");
                        _calculator.Recalc(round.Standings);
                    }
                }

                _standingRepo.Save();

                _hub?.Update(activeMatch);
            }
            catch(Exception ex)
            {
                _logHub.LogError(ex.ToString());
                return false;
            }

            return true;
        }

        public bool EditStanding(int playerdId, int songId, double percentage, int score)
        {
            bool edited = false;

            if (GetActiveMatch() == null)
            {
                _logHub.LogError($"No active match during standing edit");
                return edited;
            }
                
            Round round = GetRoundBySongId(songId);

            if (round == null)
            {
                _logHub.LogError("Round not found for active match");
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

                        //_standingRepo.Update(standing);

                        edited = true;
                    }
                }

                if(edited)
                    _standingRepo.Save();
            }
            catch (Exception ex)
            {
                _logHub.LogError(ex.ToString());
                return false;
            }

            return edited;
        }

        public bool DeleteStanding(int playerdId, int songId)
        {
            bool removed = false;
            Match activeMatch = GetActiveMatch();
            if (activeMatch == null)
            {
                _logHub.LogError($"No active match during standing edit");
                return removed;
            }

            Round round = GetRoundBySongId(songId);

            if (round == null)
            {
                _logHub.LogError("Round not found for active match");
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
                    else
                    {
                        if(!activeMatch.IsManualMatch)
                            standing.Score = 0;
                    }
                }
                _standingRepo.Save();

                _hub?.Update(activeMatch);
            }
            catch (Exception ex)
            {
                _logHub.LogError(ex.Message);
                return false;
            }

            return removed;
        }


        private Round GetRoundBySongId(int id)
        {
            Match activeMatch = GetActiveMatch();
            
            if (activeMatch == null)
                return null;

            return activeMatch.Rounds.Where(r => r.SongId == id).FirstOrDefault();
        }

        private Match GetActiveMatch()
        {
            return _matchManager.GetMatchFromId(_cache.ActiveMatch)?.First();
        }
    }
}
