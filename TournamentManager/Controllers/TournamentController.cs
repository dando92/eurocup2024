using Microsoft.AspNetCore.Mvc;
using TournamentManager.DbModels;
using TournamentManager.DTOs;
using TournamentManager.Requests;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    [Route("api/[controller]")]
    [Route("ws")]
    [ApiController]
    public class TournamentController : ControllerBase
    {        
        private readonly ITournamentCache _cache;
        private IStandingManager _standingManager;
        private readonly IMatchManager _matchManager;

        public TournamentController(ITournamentCache cache,
            IStandingManager standingManager,
            IMatchManager matchManager)
        {
            _standingManager = standingManager;
            _cache = cache;
            _matchManager = matchManager;
        }

        [HttpGet("matches/{id}")]
        public IActionResult GetMatches(int id)
        {
            var match = GetMatchDtoFromId(id);

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpPost("editStanding")]
        public IActionResult EditStanding(PostEditStanding request)
        {
            bool edited = _standingManager.EditStanding(request.PlayerId, request.SongId, request.Percentage, request.Score);

            if (edited)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
            else
                return NotFound();
        }

        [HttpDelete("deleteStanding/{playerId}/{songId}")]
        public IActionResult DeleteStandingForPlayer(int playerId, int songId)
        {
            bool removed = _standingManager.DeleteStanding(playerId, songId);

            if (removed)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
            else
                return NotFound();
        }

        [HttpGet("expandPhase/{id}")]
        public IActionResult GetPhaseExpanded(int id)
        {
            return Ok(GetMatchesDtoFromPhaseId(id));
        }

        [HttpPost("editMatchSong")]
        public IActionResult EditMatchSong(PostEditSongToMatch request)
        {
            _matchManager.RemoveSongFromMatch(request.MatchId, request.EditSongId);
            
            return AddSongToMatch(request);
        }

        [HttpPost("addSongToMatch")]
        public IActionResult AddSongToMatch(PostAddSongToMatch request)
        {
            if (request.SongId != 0)
                _matchManager.AddSongsToMatch(request.MatchId, [request.SongId]);
            else if (request.Level != null)
                _matchManager.AddRandomSongsToMatch(request.MatchId, request.DivisionId, request.Group, request.Level);

            return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
        }

        [HttpPost("addMatch")]
        public IActionResult AddMatch(PostAddMatch request)
        {
            Match match = _matchManager.AddMatch(request.MatchName, request.Notes, request.Subtitle, request.PlayerIds, request.PhaseId, request.IsManualMatch);

            if (request.SongIds != null)
                _matchManager.AddSongsToMatch(match, request.SongIds.ToArray());
            else if (request.Levels != null)
            {
                _matchManager.AddRandomSongsToMatch(match, request.DivisionId, request.Group, request.Levels);
            }

            return Ok(GetMatchDtoFromId(match.Id));
        }

        [HttpPost("setActiveMatch")]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeMatch = _matchManager.GetMatchFromId(request.MatchId).FirstOrDefault();

            if (activeMatch == null)
                return NotFound();

            _matchManager.SetActiveMatch(activeMatch);
            
            return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
        }

        [HttpGet("activeMatch")]
        public IActionResult GetActiveMatch()
        {
            var activeMatch = _cache.ActiveMatch;

            if (activeMatch == null)
                return NotFound();

            return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
        }

        [HttpPost("addStanding")]
        public IActionResult AddStanding(Standing request)
        {
            bool added = _standingManager.AddStanding(request);

            if (added)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch.Id));
            else
                return NotFound();
        }

        private MatchDto GetMatchDtoFromId(int matchId)
        {
            var match = _matchManager.GetMatchFromId(matchId).FirstOrDefault();

            return GetMatchDto(match);
        }

        private MatchDto GetMatchDto(Match match)
        {
            var matchDto = new MatchDto
            {
                Id = match.Id,
                PhaseId = match.PhaseId,
                Name = match.Name,
                Subtitle = match.Subtitle,
                Notes = match.Notes,
                Players = match.PlayerInMatches.Select(p => p.Player).ToList(),
                Songs = match.SongInMatches.Select(s => s.Song).ToList(),
                Rounds = match.Rounds.ToList()
            };

            return matchDto;
        }

        private List<MatchDto> GetMatchesDtoFromPhaseId(int phaseId)
        {
            var matches = _matchManager.GetMatchesFromPhaseId(phaseId);

            var matchesDto = matches.Select(match => new MatchDto
            {
                Id = match.Id,
                PhaseId = match.PhaseId,
                Name = match.Name,
                Subtitle = match.Subtitle,
                Notes = match.Notes,
                Players = match.PlayerInMatches.Select(p => p.Player).ToList(),
                Songs = match.SongInMatches.Select(s => s.Song).ToList(),
                Rounds = match.Rounds.ToList()
            }).ToList();

            return matchesDto;
        }

    }
}
