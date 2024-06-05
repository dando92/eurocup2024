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
    public class MatchesController(Scheduler scheduler, IGenericRepository<Match> repo) : ControllerBase
    {
        private readonly Scheduler _scheduler = scheduler;
        private readonly IGenericRepository<Match> _repo = repo;

        [HttpGet]
        public IActionResult ListMatches()
        {
            var matches = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList());
            }).WaitResult<List<Match>>();
            
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public IActionResult GetMatch(int id)
        {
            var match = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetById(id));
            }).WaitResult<Match>();

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpGet("{id}/rounds")]
        public IActionResult ListRounds(int id)
        {
            var rounds = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll()
                .Include(m => m.Rounds)
                    .ThenInclude(r => r.Standings)
                .FirstOrDefault(m => m.Id == id)?.Rounds);
            }).WaitResult<List<Round>>();

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

            _scheduler.Schedule((token) =>
            {
                _repo.Add(match);
                _repo.Save();
            }).Wait();

            return Ok(match);
        }

        [HttpPut]
        public IActionResult UpdateMatch([FromBody] PostMatchRequest request)
        {
            var match = _scheduler.Schedule((token) =>
            {
                var match = _repo.GetAll().Where(m => m.Id == request.MatchId).FirstOrDefault();

                if (request.Name != null)
                    match.Name = request.Name;

                if (request.Subtitle != null)
                    match.Subtitle = request.Subtitle;

                if (request.Notes != null)
                    match.Notes = request.Notes;

                _repo.Save();
                token.SetResult(match);

            }).WaitResult<Match>();
            
            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMatch(int id)
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
