using Microsoft.AspNetCore.Mvc;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController(Scheduler scheduler, IGenericRepository<Team> teamsRepo, IGenericRepository<Player> playersRepo) : ControllerBase
{
    [HttpGet]
    public IActionResult ListAllTeams()
    {
        var teams = scheduler.Schedule((token) =>
        {
            token.SetResult(teamsRepo.GetAll().ToList());
        }).WaitResult<List<Team>>();

        return Ok(teams);
    }
    
    [HttpPost]
    [TypeFilter(typeof(AuthorizationFilterAttribute))]
    public IActionResult AddTeam([FromBody] PostTeamRequest request)
    {
        var team = new Team
        {
            Name = request.Name
        };

        scheduler.Schedule((token) =>
        {
            teamsRepo.Add(team);
            teamsRepo.Save();
        }).Wait();

        return Ok(team);
    }
    
    [HttpDelete("{id}")]
    [TypeFilter(typeof(AuthorizationFilterAttribute))]
    public IActionResult DeleteTeam(int id)
    {
        scheduler.Schedule((token) =>
        {
            var team = teamsRepo.GetById(id);
            if (team == null)
            {
                token.SetResult(NotFound());
                return;
            }
            
            var teamPlayers = playersRepo.GetAll().Where(p => p.TeamId == team.Id);
            
            foreach (var player in teamPlayers)
            {
                player.TeamId = null;
            }
            
            playersRepo.Save();

            teamsRepo.DeleteById(team.Id);
            teamsRepo.Save();
            token.SetResult(Ok());
        }).Wait();

        return Ok();
    }
    
    [HttpPut("{id}")]
    [TypeFilter(typeof(AuthorizationFilterAttribute))]
    public IActionResult UpdateTeam(int id, [FromBody] PostTeamRequest request)
    {
        scheduler.Schedule((token) =>
        {
            var team = teamsRepo.GetById(id);
            if (team == null)
            {
                token.SetResult(NotFound());
                return;
            }

            team.Name = request.Name;
            teamsRepo.Save();
            token.SetResult(Ok());
        }).Wait();

        return Ok();
    }
}