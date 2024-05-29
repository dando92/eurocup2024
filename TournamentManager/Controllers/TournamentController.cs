using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.DTOs;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly IGenericRepository<Division> _divisionRepo;
        private readonly IGenericRepository<Phase> _phaseRepo;
        private readonly IGenericRepository<Match> _matchRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly TournamentCache _cache;
        private Services.TournamentManager _subscriber;

        public TournamentController(TournamentCache cache,
            IGenericRepository<Division> divisionRepo,
            Services.TournamentManager subscriber,
            IGenericRepository<Match> matchRepo,
            IGenericRepository<Phase> phaseRepo,
            IGenericRepository<Song> songRepo
            )
        {
            _subscriber = subscriber;
            _divisionRepo = divisionRepo;
            _cache = cache;
            _matchRepo = matchRepo;
            _phaseRepo = phaseRepo;
            _songRepo = songRepo;
        }

        [HttpGet("deleteStanding/{songId}")]
        public IActionResult DeleteStanding(int songId)
        {
            if (_cache.ActiveMatch == null)
                return NotFound();

            if (songId == 0)
                return NotFound();

            return DeleteStanding((standing) => standing.SongId == songId);
        }

        [HttpGet("deleteStandingForPlayer")]
        public IActionResult DeleteStandingForPlayer(PostDeleteStandingByPlayer request)
        {
            if (_cache.ActiveMatch == null)
                return NotFound();

            if (request.SongId == 0 || request.PlayerId == 0)
                return NotFound();

            return DeleteStanding((standing) => (standing.SongId == request.SongId) && (standing.PlayerId == request.PlayerId));
        }

        [HttpGet("expandPhase/{id}")]
        public IActionResult GetPhaseExpanded(int id)
        {
            var matches = GetMatchesFromPhaseId(id);

            var matchesDto = matches.Select(match => new MatchDto
            {
                Id = match.Id,
                Name = match.Name,
                Players = match.PlayerInMatches.Select(p => p.Player).ToList(),
                Songs = match.SongInMatches.Select(s => s.Song).ToList(),
                Rounds = match.Rounds.ToList()
            });

            return Ok(matchesDto);
        }

        [HttpPost("editMatchSong")]
        public IActionResult EditMatchSong(PostEditSongToMatch request)
        {
            var division = _divisionRepo.GetAll()
                .Include(m => m.Phases)
                    .ThenInclude(m => m.Matches)
                        .ThenInclude(m => m.SongInMatches)
                .Where(m => m.Id == request.DivisionId).FirstOrDefault();

            if (division == null)
                return NotFound();

            var match = GetMatchFromId(request.MatchId).FirstOrDefault();

            if (match == null)
                return NotFound();

            var sim = match.SongInMatches.Where(sim => sim.SongId == request.EditSongId).FirstOrDefault();

            if (sim == null)
                return NotFound();

            if (request.SongId != null)
                sim.SongId = (int)request.SongId;
            else if (request.Level != null)
            {
                var level = int.Parse(request.Level);
                sim.SongId = _songRepo.RollSong(division, request.Group, level);
            }

            _matchRepo.Update(match);

            return Ok(GetPhaseExpanded(request.PhaseId));
        }

        [HttpPost("addSongToMatch")]
        public IActionResult AddSongToMatch(PostAddSongToMatch request)
        {
            var division = _divisionRepo.GetAll()
                .Include(m => m.Phases)
                    .ThenInclude(m => m.Matches)
                        .ThenInclude(m => m.SongInMatches)
                .Where(m => m.Id == request.DivisionId).FirstOrDefault();

            if (division == null)
                return NotFound();

            var match = GetMatchFromId(request.MatchId).FirstOrDefault();

            if (match == null)
                return NotFound();

            if (request.SongId != null)
                AddRound(match, (int)request.SongId);
            else if (request.Level != null)
            {
                var level = int.Parse(request.Level);
                AddRound(match, _songRepo.RollSong(division, request.Group, level));
            }
            
            _matchRepo.Save();

            return Ok(GetPhaseExpanded(request.PhaseId));
        }

        [HttpPost("addMatch")]
        public IActionResult AddMatch(PostAddMatch request)
        {
            var division = _divisionRepo.GetAll()
                .Include(m => m.Phases)
                    .ThenInclude(m => m.Matches)
                        .ThenInclude(m => m.SongInMatches)
                .Where(m => m.Id == request.DivisionId).FirstOrDefault();

            if (division == null)
                return NotFound();

            var songs = new List<int>();

            if (request.SongIds != null)
                songs = request.SongIds;
            else if (request.Levels != null)
            {
                var levels = request.Levels.Split(",").Select(s => int.Parse(s)).ToArray();

                foreach (var level in levels)
                    songs.Add(_songRepo.RollSong(division, request.Group, level));
            }

            var newMatch = CreateMatch(request.MatchName, request.PlayerIds, songs);

            newMatch.PhaseId = request.PhaseId;
            
            _matchRepo.Add(newMatch);

            return Ok(GetPhaseExpanded(request.PhaseId));
        }

        [HttpPost("setActiveMatch")]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeMatch = GetMatchFromId(request.MatchId).FirstOrDefault();

            if (activeMatch == null)
                return NotFound();

            _cache.SetActiveMatch(activeMatch);

            return Ok();
        }

        [HttpGet("activeMatch")]
        public IActionResult GetActiveMatch()
        {
            var activeMatch = _cache.ActiveMatch;

            if (activeMatch == null)
                return NotFound();

            return Ok(activeMatch);
        }

        [HttpPost("addStanding")]
        public IActionResult AddStanding(Standing request)
        {
            _subscriber.OnNewStanding(request);
            
            _matchRepo.Save();

            return Ok();
        }
            
        private Match CreateMatch(string matchName, int[] players, List<int> songs)
        {
            
            var match = new Match()
            {
                Name = matchName,
                PlayerInMatches = new List<PlayerInMatch>(players.Length),
                SongInMatches = new List<SongInMatch>(),
                Rounds = new List<Round>(),
            };

            foreach (var player in players)
                match.PlayerInMatches.Add(new PlayerInMatch() { PlayerId = player, MatchId = match.Id, Match = match });

            if (songs != null)
            {
                foreach (var song in songs)
                    AddRound(match, song);
            }
            
            return match;
        }

        private void AddRound(Match match, int songId)
        {
            var round = new Round()
            {
                Match = match,
                MatchId = match.Id,
                Standings = new List<Standing>()
            };

            match.SongInMatches.Add(new SongInMatch() { SongId = songId, MatchId = match.Id });

            match.Rounds.Add(round);
        }

        private IActionResult DeleteStanding(Func<Standing, bool> shallDelete)
        {
            bool removed = _subscriber.DeleteStanding(shallDelete);

            if (!removed)
                return NotFound();

            _matchRepo.Save();

            return Ok(GetPhaseExpanded(_cache.ActiveMatch.PhaseId));
        }

        private IQueryable<Match> GetMatchesFromPhaseId(int phaseId)
        {
            var matches = _matchRepo.GetAll()
                .Include(m => m.Rounds)
                    .ThenInclude(m => m.Standings)
                .Include(m => m.PlayerInMatches)
                    .ThenInclude(p => p.Player)
                .Include(m => m.SongInMatches)
                    .ThenInclude(s => s.Song)
                .Where(m => m.PhaseId == phaseId);

            return matches;
        }

        private IQueryable<Match> GetMatchFromId(int matchId)
        {
            return _matchRepo
                .GetAll()
                .Include(m => m.Rounds)
                    .ThenInclude(m => m.Standings)
                .Include(m => m.PlayerInMatches)
                    .ThenInclude(p => p.Player)
                .Include(m => m.SongInMatches)
                    .ThenInclude(s => s.Song)
                .Where(m => m.Id == matchId);
        }
    }
}
