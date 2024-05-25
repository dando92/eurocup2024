using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(IGenericRepository<Division> divisionRepo, IGenericRepository<Phase> phaseRepo, IGenericRepository<Match> matchRepo, IGenericRepository<Round> roundRepo) : ControllerBase
    {
        private readonly IGenericRepository<Division> _divisionRepo = divisionRepo;
        private readonly IGenericRepository<Phase> _phaseRepo = phaseRepo;
        private readonly IGenericRepository<Match> _matchRepo = matchRepo;
        private readonly IGenericRepository<Round> _roundRepo = roundRepo;

        [HttpGet]
        public IActionResult ListDivision()
        {
            return Ok(_divisionRepo.GetAll());
        }

        [HttpPost]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = new Division
            {
                Name = request.Name
            };

            _divisionRepo.Add(division);

            return Ok(division);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _divisionRepo.GetById(id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = request.Name;

            _divisionRepo.Update(division);

            return Ok(division);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDivision(int id)
        {
            _divisionRepo.DeleteById(id);

            return Ok();
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            return Ok(_phaseRepo.GetAll().Where(p => p.DivisionId == id));
        }

        [HttpGet("{id}/phases/{phaseId}/matches")]
        public IActionResult ListMatches(int id, int phaseId)
        {
            if(_divisionRepo.GetById(id) == null)
            {
                return NotFound();
            }

            return Ok(_matchRepo.GetAll().Where(m => m.PhaseId  == phaseId));
        }

        [HttpGet("{id}/phases/{phaseId}/matches/{matchId}/rounds")]
        public IActionResult ListRounds(int id, int phaseId, int matchId)
        {
            if(_divisionRepo.GetById(id) == null)
            {
                return NotFound();
            }

            if(_phaseRepo.GetById(phaseId) == null)
            {
                return NotFound();
            }

            if(_matchRepo.GetById(matchId) == null)
            {
                return NotFound();
            }

            return Ok(_roundRepo.GetAll().Where(r => r.MatchId == matchId));
        }
    }
}
