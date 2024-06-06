using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController(Scheduler scheduler, IGenericRepository<Player> repo) : ControllerBase
    {
        Scheduler _scheduler = scheduler;
        private readonly IGenericRepository<Player> _repo = repo;

        [HttpGet]
        public IActionResult ListAllPlayers()
        {
            var players = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList());
            }).WaitResult<List<Player>>();
            
            return Ok(players);
        }

        [HttpPost("addBatchPlayer")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddBatchPlayer([FromBody] PostBatchPlayerRequest request)
        {
            List<Player> players = new List<Player>();

            foreach (PostPlayerRequest p in request.Players)
                players.Add(new Player
                {
                    Name = p.Name
                });

            _scheduler.Schedule((token) =>
            {
                _repo.AddRange(players);
                _repo.Save();
            }).Wait();

            return Ok(players);
        }

        [HttpPost]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddPlayer([FromBody] PostPlayerRequest request)
        {
            var player = new Player
            {
                Name = request.Name
            };

            _scheduler.Schedule((token) =>
            {
                _repo.Add(player);
                _repo.Save();
            }).Wait();

            return Ok(player);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult UpdatePlayer(int id, [FromBody] PostPlayerRequest request)
        {
            var player = _scheduler.Schedule((token) =>
            {
                var player = _repo.GetById(id);

                if (player == null)
                    return;

                player.Name = request.Name;

                _repo.Save();
                token.SetResult(player);
            }).WaitResult<Player>();

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult DeletePlayer(int id)
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
