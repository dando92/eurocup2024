using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhasesController(TournamentDbContext context) : ControllerBase
    {
        private readonly TournamentDbContext _context = context;

        [HttpGet]
        public IActionResult ListPhase()
        {
            return Ok(_context.Phases);
        }

        [HttpPost]
        public IActionResult AddPhase([FromBody] PostPhaseRequest request)
        {
            var phase = new Phase
            {
                DivisionId = request.DivisionId,
                Name = request.Name
            };

            _context.Phases.Add(phase);
            _context.SaveChanges();

            return Ok(phase);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePhase(int id, [FromBody] PostPhaseRequest request)
        {
            var phase = _context.Phases.Find(id);

            if (phase == null)
            {
                return NotFound();
            }

            phase.Name = request.Name;

            _context.SaveChanges();

            return Ok(phase);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePhasePartial(int id, [FromBody] PostPhaseRequest request)
        {
            var phase = _context.Phases.Find(id);

            if (phase == null)
            {
                return NotFound();
            }

            phase.Name = request.Name;

            _context.SaveChanges();

            return Ok(phase);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePhase(int id)
        {
            var phase = _context.Phases.Find(id);

            if (phase == null)
            {
                return NotFound();
            }

            _context.Phases.Remove(phase);
            _context.SaveChanges();

            return Ok();
        }
    }
}
