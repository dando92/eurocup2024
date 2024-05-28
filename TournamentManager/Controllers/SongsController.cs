using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController(IGenericRepository<Song> repo) : ControllerBase
    {
        private readonly IGenericRepository<Song> _repo = repo;

        [HttpGet]
        public IActionResult ListAllSongs()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("groups")]
        public IActionResult ListAllGroups()
        {
            return Ok(_repo.GetAll().ToList().ListGroups());
        }

        [HttpPost("AddBatchSongs")]
        public IActionResult AddBatchSongs([FromBody] PostBatchSongRequest request)
        {
            var _players = new List<Song>();

            foreach (var song in request.Songs)
                _players.Add(new Song
                {
                    Title = song.Title,
                    Group = song.Group,
                    Difficulty = song.Difficulty,
                });

            _repo.AddRange(_players);

            return Ok(_players);
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

            _repo.Add(song);

            return Ok(song);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSong(int id, [FromBody] PostSongRequest request)
        {
            var song = _repo.GetById(id);

            if (song == null)
            {
                return NotFound();
            }

            song.Title = request.Title;
            song.Difficulty = request.Difficulty;
            song.Group = request.Group;

            _repo.Update(song);

            return Ok(song);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateSongPartial(int id, [FromBody] PostSongRequest request)
        {
            var song = _repo.GetById(id);

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

            _repo.Update(song);

            return Ok(song);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
        {
            _repo.DeleteById(id);

            return Ok();
        }
    }
}
