using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;

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
    }
}
