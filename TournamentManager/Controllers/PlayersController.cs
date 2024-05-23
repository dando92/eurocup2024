using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController(IGenericRepository<Player> repo) : ControllerBase
    {
        private readonly IGenericRepository<Player> _repo = repo;

        [HttpGet]
        public IActionResult ListAllPlayers()
        {
            return Ok(_repo.GetAll());
        }

        [HttpPost]
        public IActionResult AddPlayer([FromBody] PostPlayerRequest request)
        {
            var player = new Player
            {
                Name = request.Name
            };
            _repo.Add(player);


            return Ok(player);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePlayer(int id, [FromBody] PostPlayerRequest request)
        {
            var player = _repo.GetById(id);

            if (player == null)
            {
                return NotFound();
            }

            player.Name = request.Name;

            _repo.Update(player);

            return Ok(player);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePlayerPartial(int id, [FromBody] PostPlayerRequest request)
        {
            var player = _repo.GetById(id);

            if (player == null)
            {
                return NotFound();
            }

            player.Name = request.Name;

            _repo.Update(player);

            return Ok(player);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(int id)
        {
            _repo.DeleteById(id);

            return Ok();
        }
    }
}
