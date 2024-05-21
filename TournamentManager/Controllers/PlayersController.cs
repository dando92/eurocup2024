using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly TournamentDbContext _context;

        public PlayersController(TournamentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ListAllPlayers()
        {
            return Ok(_context.Players);
        }

        [HttpPost]
        public IActionResult AddPlayer([FromBody] PostPlayerRequest request)
        {
            var player = new Player
            {
                Name = request.Name
            };

            _context.Players.Add(player);
            _context.SaveChanges();

            return Ok(player);
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(int id)
        {
            var player = _context.Players.Find(id);

            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            _context.SaveChanges();

            return Ok();
        }
    }
}
