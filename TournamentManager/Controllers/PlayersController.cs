using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController(Scheduler scheduler, IGenericRepository<Player> playersRepo, IGenericRepository<Team> teamsRepo) : ControllerBase
    {
        [HttpGet]
        public IActionResult ListAllPlayers()
        {
            var players = scheduler.Schedule((token) =>
            {
                token.SetResult(playersRepo.GetAll().ToList());
            }).WaitResult<List<Player>>();

            return Ok(players);
        }

        [HttpPost("addBatchPlayer")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AddBatchPlayer([FromBody] PostBatchPlayerRequest request)
        {
            List<Player> players = new List<Player>();
            var teams = teamsRepo.GetAll().ToList();

            foreach (PostPlayerRequest p in request.Players)
            {
                Team team = null;

                if(!string.IsNullOrEmpty(p.Team))
                {
                    team = teams.FirstOrDefault(t => t.Name == p.Team);

                    if(team == null) 
                    {
                        team = new Team
                        {
                            Name = p.Team,
                            Score = 0
                        };

                        scheduler.Schedule((token) =>
                        {
                            teamsRepo.Add(team);
                            teamsRepo.Save();
                            teams.Add(team);
                        }).Wait();

                        teams.Add(team);
                    }
                }

                players.Add(new Player
                {
                    Name = p.Name,
                    Score = 0,
                    TeamId = team == null ? null : team?.Id
                });
            }
            
            scheduler.Schedule((token) =>
            {
                playersRepo.AddRange(players);
                playersRepo.Save();
            }).Wait();

            return Ok(players);
        }

        [HttpPost("{id}/assignToTeam/{teamId}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult AssignPlayerToTeam(int id, int teamId)
        {
            var player = scheduler.Schedule((token) =>
            {
                var player = playersRepo.GetById(id);
                if (player == null)
                    return;

                player.TeamId = teamId;
                playersRepo.Save();

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
                var player = playersRepo.GetById(id);
                if (player == null)
                    return;

                player.TeamId = null;
                playersRepo.Save();
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
                playersRepo.Add(player);
                playersRepo.Save();
            }).Wait();

            return Ok(player);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(AuthorizationFilterAttribute))]
        public IActionResult UpdatePlayer(int id, [FromBody] PostPlayerRequest request)
        {
            var player = scheduler.Schedule((token) =>
            {
                var player = playersRepo.GetById(id);

                if (player == null)
                    return;

                player.Name = request.Name;

                playersRepo.Save();
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
                playersRepo.DeleteById(id);
                playersRepo.Save();
            }).Wait();

            return Ok();
        }
    }
}
