using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController(TournamentDbContext context) : ControllerBase
    {
        private readonly TournamentDbContext _context = context;

        [HttpGet]
        public IActionResult ListAllSongs()
        {
            return Ok(_context.Songs);
        }

        [HttpPost]
        public IActionResult AddSong([FromBody] PostSongRequest request)
        {
            var song = new Song
            {
                Title = request.Title,
                Difficulty = request.Difficulty ?? 0,
                Group = request.Group
            };

            _context.Songs.Add(song);
            _context.SaveChanges();

            return Ok(song);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSong(int id, [FromBody] PostSongRequest request)
        {
            var song = _context.Songs.Find(id);

            if (song == null)
            {
                return NotFound();
            }

            song.Title = request.Title;
            song.Difficulty = request.Difficulty;
            song.Group = request.Group;

            _context.SaveChanges();

            return Ok(song);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateSongPartial(int id, [FromBody] PostSongRequest request)
        {
            var song = _context.Songs.Find(id);

            if (song == null)
            {
                return NotFound();
            }

            if (request.Title != null)
            {
                song.Title = request.Title;
            }

            if (request.Difficulty != null)
            {
                song.Difficulty = request.Difficulty;
            }

            if (request.Group != null)
            {
                song.Group = request.Group;
            }

            _context.SaveChanges();

            return Ok(song);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
        {
            var song = _context.Songs.Find(id);

            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            _context.SaveChanges();

            return Ok();
        }
    }
}
