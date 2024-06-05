using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController(Scheduler scheduler, IGenericRepository<Song> repo) : ControllerBase
    {
        private readonly Scheduler _scheduler = scheduler;
        private readonly IGenericRepository<Song> _repo = repo;

        [HttpGet]
        public IActionResult ListAllSongs()
        {
            var songs = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList());
            }).WaitResult<List<Song>>();

            return Ok(songs);
        }

        [HttpGet("groups")]
        public IActionResult ListAllGroups()
        {
            var groups = _scheduler.Schedule((token) =>
            {
                token.SetResult(_repo.GetAll().ToList().ListGroups().ToList());
            }).WaitResult<List<string>>();

            return Ok(groups);
        }

        [HttpPost("AddBatchSongs")]
        public IActionResult AddBatchSongs([FromBody] PostBatchSongRequest request)
        {
            var songs = new List<Song>();

            foreach (var song in request.Songs)
                songs.Add(new Song
                {
                    Title = song.Title,
                    Group = song.Group,
                    Difficulty = song.Difficulty,
                });

            _scheduler.Schedule((token) =>
            {
                _repo.AddRange(songs);
                _repo.Save();
            }).Wait();

            return Ok(songs);
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

            _scheduler.Schedule((token) =>
            {
                _repo.Add(song);
                _repo.Save();
            }).Wait();

            return Ok(song);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSong(int id, [FromBody] PostSongRequest request)
        {
            var song = _scheduler.Schedule((token) =>
            {
                var song = _repo.GetById(id);

                if (song == null)
                    return;

                song.Title = request.Title;
                song.Difficulty = request.Difficulty;
                song.Group = request.Group;

                _repo.Save();
                token.SetResult(song);
            }).WaitResult<Song>();
            
            if (song == null)
                return NotFound();

            return Ok(song);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateSongPartial(int id, [FromBody] PostSongRequest request)
        {
            var song = _scheduler.Schedule((token) =>
            {
                var song = _repo.GetById(id);

                if (song == null)
                    return;

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

                _repo.Save();

                token.SetResult(song);
            }).WaitResult<Song>();

            if (song == null)
                return NotFound();

            return Ok(song);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteSong(int id)
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
