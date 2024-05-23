using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult AddMatch([FromBody] PostMatchRequest request)
        {
            var match = new Match
            {
                Name = request.Name,
                PhaseId = request.PhaseId,
            };

            _repo.Add(match);

            return Ok(match);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMatch(int id, [FromBody] PostMatchRequest request)
        {
            var match = _repo.GetById(id);

            if (match == null)
            {
                return NotFound();
            }

            match.Name = request.Name;

            _repo.Update(match);

            return Ok(match);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMatch(int id)
        {
            _repo.DeleteById(id);

            return Ok();
        }
    }
}
