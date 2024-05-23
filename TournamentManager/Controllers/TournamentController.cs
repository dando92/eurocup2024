using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        public const string ActiveMatchKey = "ActiveCache";
        public const string CurrentRoundKey = "CurrentRound";

        private readonly IGenericRepository<Standing> _standingsRepo;
        private readonly IGenericRepository<Division> _divisionRepo;
        private readonly ITournamentInfoContainer _infoContainer;

        private List<IStandingSubscriber> _subscribers;

        public TournamentController(ITournamentInfoContainer infoContainer, IGenericRepository<Division> divisionRepo, IGenericRepository<Standing> standingsRepo, List<IStandingSubscriber> subscribers)
        {
            _standingsRepo = standingsRepo;
            _divisionRepo = divisionRepo;
            _infoContainer = infoContainer;
            _subscribers = subscribers;
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

            _infoContainer.SetActiveMatch(activeMatch);

            return Ok();
        }


        [HttpPost("updateScore")]
        public IActionResult UpdateScore(PostStandingRequest request)
        {
            //TODO: Register score on standings anyway. Just don't update torunament info.
            //if (Match == null || Round == null)
            //    return NotFound();

            Song song = _infoContainer.GetSongByName(request.Player);
            Player player = _infoContainer.GetPlayerByName(request.Song);

            //Player or song not registered, do nothing
            if (song == null || player == null)
                return NotFound();

            Standing standing = new Standing()
            {
                Percentage = request.Percentage,
                RoundId = _infoContainer.GetCurrentRound().Id,
                PlayerId = player.Id,
                SongId = song.Id
            };
            
            NotifyNewStanding(standing);

            _standingsRepo.Add(standing);

            return Ok();
        }

        private void NotifyNewStanding(Standing standing)
        {
            if (_subscribers == null)
                return;

            foreach (var subscriber in _subscribers)
                subscriber.OnNewStanding(standing);
        }
    }
}
