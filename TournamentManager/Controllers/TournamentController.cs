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
    public class TournamentController(TournamentDbContext context) : ControllerBase
    {
        private readonly TournamentDbContext _context = context;
        private Match _activeMatch = null;
        private Round _currRound = null;

        private IEnumerator<Round> GetNextRound()
        {
            foreach (var round in _activeMatch.Rounds)
                yield return round;
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

            _activeMatch = activePhase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (_activeMatch == null)
                return NotFound();

            _currRound = GetNextRound().Current;

            return Ok();
        }


        private Song GetSongByName(string name)
        {
            return null;
        }

        private Player GetPLayerByName(string name)
        {
            return null;
        }

        [HttpPost("updateScore")]
        public IActionResult UpdateScore(PostStandingRequest request)
        {
            Song song = GetSongByName(request.Player);
            Player player = GetPLayerByName(request.Song);

            Standing standing = new Standing()
            {
                Percentage = request.Percentage,
                Player = player,
                Song = song
            };

            _context.Standings.Add(standing);
            _context.SaveChanges();

            _currRound.StandingsInRounds.Add(new StandingInRound()
            {
                Round = _currRound,
                Standing = standing
            });

            if (_currRound.StandingsInRounds.Count >= _activeMatch.PlayersInMatches.Count)
            {
                //TODO:
                //Order standings and calc points

                _currRound = GetNextRound().Current;
            }    

            //Match ended since all the rounds have been played
            if(_currRound == null)
                _activeMatch = null;

            return Ok();
        }
    }
}
