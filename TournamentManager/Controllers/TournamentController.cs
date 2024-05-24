using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly IGenericRepository<Division> _divisionRepo;
        private readonly IGenericRepository<Song> _songRepo;
        private readonly TorunamentCache _cache;
        private IRawStandingSubscriber _subscriber;

        public TournamentController(TorunamentCache cache,
            IGenericRepository<Division> divisionRepo,
            IRawStandingSubscriber subscriber)
        {
            _subscriber = subscriber;
            _divisionRepo = divisionRepo;
            _cache = cache;
        }

        [HttpPost("addMatch")]
        public IActionResult AddMatch(PostAddMatch request)
        {
            var division = _divisionRepo
                .GetById(request.DivisionId);
            
            if (division == null)
                return NotFound();

            var phase = division
                .Phases
                .Where(p => p.Id == request.PhaseId).FirstOrDefault();

            if (phase == null)
                return NotFound();

            int[] levels = request.Levels.Split(",").Select(s => int.Parse(s)).ToArray();
            
            var match = new Match()
            {
                Name = request.MatchName,
                Phase = phase,
                PhaseId = phase.Id,
                PlayerInMatches = new List<PlayerInMatch>(request.PlayerIds.Length),
                SongInMatches = new List<SongInMatch>(levels.Length),
                Rounds = new List<Round>(levels.Length),
            };

            foreach (int player in request.PlayerIds)
                match.PlayerInMatches.Add(new PlayerInMatch() { PlayerId = player, MatchId = match.Id, Match = match });

            List<int> availableSongs = _songRepo.GetAvailableSong(phase, request.Group);

            foreach (var level in levels)
            {
                var round = new Round()
                {
                    Match = match,
                    MatchId = match.Id,
                    Standings = new List<Standing>()
                };

                int randomSong = availableSongs.RandomElement();
                
                availableSongs.Remove(randomSong);

                match.SongInMatches.Add(new SongInMatch() { SongId = randomSong, MatchId = match.Id });

                match.Rounds.Add(round);
            }

            phase.Matches.Add(match);

            _divisionRepo.Update(division);

            return Ok();
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

    }
}
