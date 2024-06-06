using Microsoft.AspNet.SignalR;
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
    public class PhasesController(Scheduler scheduler, IGenericRepository<Phase> repo) : ControllerBase
    {
        private readonly Scheduler _scheduler = scheduler;
        private readonly IGenericRepository<Phase> _repo = repo;

        [HttpGet]
        public IActionResult ListPhase()
        {
            var phases = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList());
            }).WaitResult<List<Phase>>();

            return Ok(phases);
        }

        [HttpGet("{id}")]
        public IActionResult GetPhase(int id)
        {
            var phase = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetById(id));
            }).WaitResult<Phase>();

            if (phase == null)
                return NotFound();

            return Ok(phase);
        }

        [HttpGet("{id}/matches")]
        public IActionResult GetPhaseMatches(int id)
        {
            var matches = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().Include(p => p.Matches).FirstOrDefault(p => p.Id == id)?.Matches.ToList());
            }).WaitResult<List<Match>>();

            return Ok(matches);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddPhase([FromBody] PostPhaseRequest request)
        {
            var phase = new Phase
            {
                DivisionId = request.DivisionId,
                Name = request.Name
            };

            _scheduler.Schedule((token) =>
            {
                _repo.Add(phase);
                _repo.Save();
            }).Wait();

            return Ok(phase);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult UpdatePhase(int id, [FromBody] PostPhaseRequest request)
        {
            var phase = _scheduler.Schedule((token) =>
            {
                var phase = _repo.GetById(id);

                if (phase == null)
                    return;

                phase.Name = request.Name;
                token.SetResult(phase);
            }).WaitResult<Phase>();

            if (phase == null)
                return NotFound();

            return Ok(phase);
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult DeletePhase(int id)
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
