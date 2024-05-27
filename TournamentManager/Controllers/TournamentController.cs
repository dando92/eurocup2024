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
        private readonly TorunamentCache _cache;
        private IRawStandingSubscriber _subscriber;

        public TournamentController(TorunamentCache cache,
            IGenericRepository<Division> divisionRepo,
            IRawStandingSubscriber subscriber,
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

        [HttpPost("editMatchSong")]
        public IActionResult EditMatchSong(PostEditSongToMatch request)
        {
            var division = _divisionRepo.GetById(request.DivisionId);

            if (division == null)
                return NotFound();

            var phase = _phaseRepo.GetById(request.PhaseId);

            if (phase == null)
                return NotFound();

            Match match = phase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (match == null)
                return NotFound();
            var sim = match.SongInMatches.Where(sim => sim.SongId == request.EditSongId).FirstOrDefault();
            
            if (sim == null)
                return NotFound();

            if (request.SongId != null)
                sim.SongId = request.SongId;
            else if (request.Level != null)
            {
                int level = int.Parse(request.Level);
                sim.SongId = _songRepo.RollSong(phase, request.Group, level);
            }

            return Ok();
        }

        [HttpPost("addSongToMatch")]
        public IActionResult AddSongToMatch(PostAddSongToMatch request)
        {
            var division = _divisionRepo.GetById(request.DivisionId);

            if (division == null)
                return NotFound();

            var phase = _phaseRepo.GetById(request.PhaseId);

            if (phase == null)
                return NotFound();

            Match match = phase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (match == null)
                return NotFound();

            if (request.SongId != null)
                AddRound(match, request.SongId);
            else if (request.Level != null)
            {
                int level = int.Parse(request.Level);
                AddRound(match, _songRepo.RollSong(phase, request.Group, level));
            }

            return Ok();
        }

        [HttpPost("addMatch")]
        public IActionResult AddMatch(PostAddMatch request)
        {
            var division = _divisionRepo
                .GetById(request.DivisionId);
            
            if (division == null)
                return NotFound();

            var phase = _phaseRepo.GetById(request.PhaseId);

            phase.Matches = GetMatchesFromPhaseId(phase.Id).ToList();

            if (phase == null)
                return NotFound();
            
            List<int> songs = new List<int>();

            if (request.SongIds != null)
                songs = request.SongIds;
            else if (request.Levels != null)
            {
                int[] levels = request.Levels.Split(",").Select(s => int.Parse(s)).ToArray();

                foreach (var level in levels)
                    songs.Add(_songRepo.RollSong(phase, request.Group, level));
            }

            CreateMatch(phase, request.MatchName, request.PlayerIds, songs);

            _divisionRepo.Update(division);

            return Ok();
        }

        private void CreateMatch(Phase phase, string matchName, int[] players, List<int> songs)
        {
            var match = new Match()
            {
                Name = matchName,
                Phase = phase,
                PhaseId = phase.Id,
                PlayerInMatches = new List<PlayerInMatch>(players.Length),
                SongInMatches = new List<SongInMatch>(),
                Rounds = new List<Round>(),
            };
            
            phase.Matches.Add(match);

            foreach (int player in players)
                match.PlayerInMatches.Add(new PlayerInMatch() { PlayerId = player, MatchId = match.Id, Match = match });

            if (songs != null)
            {
                foreach (var song in songs)
                    AddRound(match, song);
            }
        }

        [HttpPost]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeDivision = _divisionRepo.GetById(request.DivisionId);

            if (activeDivision == null)
                return NotFound();

            var activePhase = activeDivision.Phases.Where(p => p.Id == request.PhaseId).FirstOrDefault();

            if (activePhase == null)
                return NotFound();

            var activeMatch = activePhase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (activeMatch == null)
                return NotFound();

            _cache.SetActiveMatch(activeMatch);

            return Ok();
        }

        [HttpPost("updateScore")]
        public IActionResult UpdateScore(PostStandingRequest request)
        {
            _subscriber.OnNewStanding(request);

            return Ok();
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

    }
}
