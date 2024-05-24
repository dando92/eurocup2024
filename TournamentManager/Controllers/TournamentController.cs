using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly IGenericRepository<Division> _divisionRepo;
        private readonly Services.TournamentManager _manager;
        private IRawStandingSubscriber _subscriber;

        public TournamentController(Services.TournamentManager manager,
            IGenericRepository<Division> divisionRepo,
            IGenericRepository<Standing> standingsRepo,
            IGenericRepository<Song> songRepo,
            IGenericRepository<Player> playerRepo,
            IRawStandingSubscriber subscriber)
        {
            _subscriber = subscriber;
            _divisionRepo = divisionRepo;
            _manager = manager;
        }

        [HttpPost]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeDivision = _divisionRepo.GetById(request.DivisionId);

            if (activeDivision == null)
                return NotFound();

            var activePhase = activeDivision.Phases.Where(p => p.Id == request.PhaseId).FirstOrDefault();

            if (activePhase == null)
                return NotFound();

            var activeMatch = activePhase.Matches.Where(m => m.Id == request.MatchId).FirstOrDefault();

            if (activeMatch == null)
                return NotFound();

            _manager.SetActiveMatch(activeMatch);

            return Ok();
        }

        [HttpPost("updateScore")]
        public IActionResult UpdateScore(PostStandingRequest request)
        {
            _subscriber.OnNewStanding(request);
            return Ok();
        }

    }
}
