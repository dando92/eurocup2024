using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController(TournamentDbContext context) : ControllerBase
    {
        private readonly TournamentDbContext _context = context;

        [HttpGet]
        public IActionResult ListMatches()
        {
            return Ok(_context.Matches);
        }

        [HttpPost]
        public IActionResult AddMatch([FromBody] PostMatchRequest request)
        {
            var match = new Match
            {
                Name = request.Name,
                PhaseId = request.PhaseId,
            };

            _context.Matches.Add(match);
            _context.SaveChanges();

            return Ok(match);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMatch(int id, [FromBody] PostMatchRequest request)
        {
            var match = _context.Matches.Find(id);

            if (match == null)
            {
                return NotFound();
            }

            match.Name = request.Name;

            _context.SaveChanges();

            return Ok(match);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMatch(int id)
        {
            var match = _context.Matches.Find(id);

            if (match == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(match);
            _context.SaveChanges();

            return Ok();
        }
    }
}
