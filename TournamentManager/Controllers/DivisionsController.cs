using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(IGenericRepository<Division> divisionRepo, IGenericRepository<Phase> phaseRepo, IGenericRepository<Match> matchRepo, IGenericRepository<Round> roundRepo) : ControllerBase
    {
        private readonly IGenericRepository<Division> _repo = divisionRepo;
        private readonly IGenericRepository<Phase> _phaseRepo = phaseRepo;
        private readonly IGenericRepository<Match> _matchRepo = matchRepo;
        private readonly IGenericRepository<Round> _roundRepo = roundRepo;

        [HttpGet]
        public IActionResult ListDivision()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetDivision(int id)
        {
            var division = _repo.GetById(id);

            if (division == null)
            {
                return NotFound();
            }

            return Ok(division);
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            return Ok(_repo.GetAll().Include(d => d.Phases).FirstOrDefault(d=>d.Id == id).Phases);
        }

        [HttpPost]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = new Division
            {
                Name = request.Name
            };

            _repo.Add(division);
            _repo.Save();
            return Ok(division);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _repo.GetById(id);

            if (division == null)
            {
                return NotFound();
            }

            division.Name = request.Name;

            _repo.Save();

            return Ok(division);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDivision(int id)
        {
            _repo.DeleteById(id);
            
            _repo.Save();

            return Ok();
        }
    }
}
