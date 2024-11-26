using Microsoft.AspNetCore.Mvc;
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
        [HttpGet]
        public IActionResult ListAllPlayers()
        {
            var players = scheduler.Schedule((token) =>
            {
                token.SetResult(repo.GetAll().ToList());
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
                    Name = p.Name,
                    Score = 0
                });

            scheduler.Schedule((token) =>
            {
                repo.AddRange(players);
                repo.Save();
            }).Wait();

            return Ok(players);
        }

        [HttpPost("{id}/assignToTeam/{teamId}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AssignPlayerToTeam(int id, int teamId)
        {
            var player = scheduler.Schedule((token) =>
            {
                var player = repo.GetById(id);
                if (player == null)
                    return;

                player.TeamId = teamId;
                repo.Save();

                token.SetResult(player);
            }).WaitResult<Player>();

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost("{id}/removeFromTeam")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult RemovePlayerFromTeam(int id)
        {
            var player = scheduler.Schedule((token) =>
            {
                var player = repo.GetById(id);
                if (player == null)
                    return;

                player.TeamId = null;
                repo.Save();
                token.SetResult(player);
            }).WaitResult<Player>();

            if (player == null)
                return NotFound();

            return Ok(player);
        }


        [HttpPost]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddPlayer([FromBody] PostPlayerRequest request)
        {
            var player = new Player
            {
                Name = request.Name,
                Score = 0
            };

            scheduler.Schedule((token) =>
            {
                repo.Add(player);
                repo.Save();
            }).Wait();

            return Ok(player);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult UpdatePlayer(int id, [FromBody] PostPlayerRequest request)
        {
            var player = scheduler.Schedule((token) =>
            {
                var player = repo.GetById(id);

                if (player == null)
                    return;

                player.Name = request.Name;

                repo.Save();
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
            scheduler.Schedule((token) =>
            {
                repo.DeleteById(id);
                repo.Save();
            }).Wait();

            return Ok();
        }
    }
}
