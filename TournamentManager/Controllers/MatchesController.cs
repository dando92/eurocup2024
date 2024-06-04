using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController(IGenericRepository<Match> repo) : ControllerBase
    {
        private readonly IGenericRepository<Match> _repo = repo;

        [HttpGet]
        public IActionResult ListMatches()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetMatch(int id)
        {
            var match = _repo.GetById(id);

            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }

        [HttpGet("{id}/rounds")]
        public IActionResult ListRounds(int id)
        {
            var rounds = _repo.GetAll()
                .Include(m => m.Rounds)
                    .ThenInclude(r => r.Standings)
                .FirstOrDefault(m => m.Id == id)?.Rounds;

            return Ok(rounds);
        }

        [HttpPost]
        public IActionResult AddMatch([FromBody] PostMatchRequest request)
        {
            var match = new Match
            {
                Name = request.Name,
                PhaseId = request.MatchId,
            };

            _repo.Add(match);
            
            _repo.Save();

            return Ok(match);
        }

        [HttpPut]
        public IActionResult UpdateMatch([FromBody] PostMatchRequest request)
        {
            var match = _repo.GetAll().Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (match == null)
            {
                return NotFound();
            }

            if (request.Name != null)
                match.Name = request.Name;

            if (request.Subtitle != null)
                match.Subtitle = request.Subtitle;

            if (request.Notes != null)
                match.Notes = request.Notes;

            _repo.Save();

            return Ok(match);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMatch(int id)
        {
            _repo.DeleteById(id);
            _repo.Save();
            return Ok();
        }
    }
}
