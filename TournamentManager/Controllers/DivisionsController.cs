using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController(Scheduler scheduler, IGenericRepository<Division> divisionRepo, IGenericRepository<Phase> phaseRepo, IGenericRepository<Match> matchRepo, IGenericRepository<Round> roundRepo) : ControllerBase
    {
        private readonly Scheduler _scheduler = scheduler;
        private readonly IGenericRepository<Division> _repo = divisionRepo;

        [HttpGet]
        public IActionResult ListDivision()
        {
            var divisions = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList());
            }).WaitResult<List<Division>>();

            return Ok(divisions);
        }

        [HttpGet("{id}")]
        public IActionResult GetDivision(int id)
        {
            var division = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetById(id));
            }).WaitResult<Division>();

            if (division == null)
                return NotFound();

            return Ok();
        }

        [HttpGet("{id}/phases")]
        public IActionResult ListPhases(int id)
        {
            var phases = _scheduler.Schedule((token) =>
            {
                var res = _repo.GetAll().Include(d => d.Phases).FirstOrDefault(d => d.Id == id).Phases.ToList();
                token.SetResult(res);
            }).WaitResult<List<Phase>>();

            return Ok(phases);
        }

        [HttpPost]
        public IActionResult AddDivision([FromBody] PostDivisionRequest request)
        {
            var division = _scheduler.Schedule((executionToken) =>
            {
                var division = new Division
                {
                    Name = request.Name
                };

                _repo.Add(division);
                _repo.Save();
                executionToken.SetResult(division);
            }).WaitResult<Division>();

            return Ok(division);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDivision(int id, [FromBody] PostDivisionRequest request)
        {
            var division = _scheduler.Schedule((token) =>
            {
                var division = _repo.GetById(id);

                if (division == null)
                    return;

                division.Name = request.Name;

                _repo.Save();
                token.SetResult(division);
            }).WaitResult<Division>();

            if (division == null)
                return NotFound();

            return Ok(division);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteDivision(int id)
        {
            _scheduler.Schedule((token) =>
            {
                _repo.DeleteById(id);

                _repo.Save();
            }).Wait();

            return Ok();
        }
    }
}
