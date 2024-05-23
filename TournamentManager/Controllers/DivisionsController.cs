using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(IGenericRepository<Division> repo) : ControllerBase
    {
        private readonly IGenericRepository<Division> repo = repo;

        [HttpGet]
        public IActionResult ListDivision()
        {
            return Ok(repo.GetAll());
        }

        [HttpPost]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = new Division
            {
                Name = request.Name
            };

            repo.Add(division);

            return Ok(division);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = repo.GetById(id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = request.Name;

            repo.Update(division);

            return Ok(division);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDivision(int id)
        {
            repo.DeleteById(id);

            return Ok();
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            return Ok(repo.GetById(id).Phases);
        }

        [HttpGet("{id}/phases/{phaseId}/matches")]
        public IActionResult ListMatches(int id, int phaseId)
        {
            return Ok(repo.GetById(id)
                .Phases.Where(p => p.Id == phaseId).First()
                .Matches);
        }

        [HttpGet("{id}/phases/{phaseId}/matches/{matchId}/rounds")]
        public IActionResult ListRounds(int id, int phaseId, int matchId)
        {
            return Ok(repo.GetById(id)
                .Phases.Where(p => p.Id == phaseId).First()
                .Matches.Where(m => m.Id == matchId).First()
                .Rounds);
        }
    }
}
