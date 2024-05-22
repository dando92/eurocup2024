using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(TournamentDbContext context) : ControllerBase
    {
        private readonly TournamentDbContext _context = context;

        [HttpGet]
        public IActionResult ListDivision()
        {
            return Ok(_context.Divisions);
        }

        [HttpPost]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = new Division
            {
                Name = request.Name
            };

            _context.Divisions.Add(division);
            _context.SaveChanges();

            return Ok(division);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = request.Name;

            _context.SaveChanges();

            return Ok(division);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateDivisionPartial(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = request.Name;

            _context.SaveChanges();

            return Ok(division);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDivision(int id)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            _context.Divisions.Remove(division);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            var phases = _context.Phases.Where(p => p.DivisionId == id);

            return Ok(phases);
        }

        [HttpGet("{id}/phases/{phaseId}/matches")]
        public IActionResult ListMatches(int id, int phaseId)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            var phase = _context.Phases.Find(phaseId);

            if (phase == null)
            {
                return NotFound();
            }

            var matches = _context.Matches.Where(m => m.PhaseId == phaseId);

            return Ok(matches);
        }

        [HttpGet("{id}/phases/{phaseId}/matches/{matchId}/rounds")]
        public IActionResult ListRounds(int id, int phaseId, int matchId)
        {
            var division = _context.Divisions.Find(id);

            if (division == null)
            {
                return NotFound();
            }

            var phase = _context.Phases.Find(phaseId);

            if (phase == null)
            {
                return NotFound();
            }

            var match = _context.Matches.Find(matchId);

            if (match == null)
            {
                return NotFound();
            }

            var rounds = _context.Rounds.Where(r => r.MatchId == matchId);

            return Ok(rounds);
        }
    }
}
