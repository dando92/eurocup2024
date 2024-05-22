using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        public const string ActiveMatchKey = "ActiveCache";
        public const string CurrentRoundKey = "CurrentRound";

        private readonly TournamentDbContext _context;
        private readonly ICache _cache;

        private Match _activeMatch = null;
        private Round _currRound = null;

        public TournamentController(TournamentDbContext context, ICache cache)
        {
            _context = context;
            _cache = cache;

            _activeMatch = _cache.Get<Match>(ActiveMatchKey);
            _currRound = _cache.Get<Round>(CurrentRoundKey);
        }

        [HttpGet]
        public IActionResult ListDivision()
        {
            return Ok(_context.Divisions);
        }

        [HttpPost]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeDivision = _context.Divisions.Where(d => d.Id == request.DivisionId).FirstOrDefault();

            if (activeDivision == null)
                return NotFound();

            var activePhase = activeDivision.Phases.Where(p => p.Id == request.PhaseId).FirstOrDefault();

            if (activePhase == null)
                return NotFound();

            var activeMatch = activePhase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (activeMatch == null)
                return NotFound();

            _cache.Add(ActiveMatchKey, activeMatch);
            _cache.Add(CurrentRoundKey, activeMatch.GetNextRound().Current);

            return Ok();
        }


        private Song GetSongByName(string name)
        {
            foreach (var sim in _activeMatch.SongInMatches)
            {
                if (sim.Song.Title == name)
                    return sim.Song;
            }

            return null;
        }

        private Player GetPlayerByName(string name)
        {
            foreach(var pim in _activeMatch.PlayerInMatches)
            {
                if (pim.Player.Name == name)
                    return pim.Player;
            }

            return null;
        }

        [HttpPost("updateScore")]
        public IActionResult UpdateScore(PostStandingRequest request)
        {
            //TODO: Registriamo comunque lo score?
            if (_activeMatch == null || _currRound == null)
                return NotFound();

            Song song = GetSongByName(request.Player);
            Player player = GetPlayerByName(request.Song);

            if (song == null || player == null)
                return NotFound();

            Standing standing = new Standing()
            {
                Percentage = request.Percentage,
                Player = player,
                Song = song
            };

            _context.Standings.Add(standing);
            _currRound.Standings.Add(standing);

            if (_currRound.Standings.Count >= _activeMatch.PlayerInMatches.Count)
            {
                _currRound.Standings = _currRound.Standings.Recalc();
                _currRound = _activeMatch.GetNextRound().Current;
            }    

            //Match ended since all the rounds have been played
            if(_currRound == null)
                _activeMatch = null;
            
            _context.SaveChanges();

            return Ok();
        }
    }
}
