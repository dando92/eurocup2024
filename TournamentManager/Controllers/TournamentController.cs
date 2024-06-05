﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly Scheduler _scheduler;
        private readonly ITournamentCache _cache;
        private IStandingManager _standingManager;
        private readonly IMatchManager _matchManager;

        public TournamentController(ITournamentCache cache,
            IStandingManager standingManager,
            IMatchManager matchManager,
            Scheduler scheduler)
        {
            _standingManager = standingManager;
            _cache = cache;
            _matchManager = matchManager;
            _scheduler = scheduler;
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
            var edited = _scheduler.Schedule((token) => 
            {
                token.SetResult(_standingManager.EditStanding(request.PlayerId, request.SongId, request.Percentage, request.Score));
            }).WaitResult<bool>();
            
            if (edited)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
            else
                return NotFound();
        }

        [HttpDelete("deleteStanding/{playerId}/{songId}")]
        public IActionResult DeleteStandingForPlayer(int playerId, int songId)
        {
            var removed = _scheduler.Schedule((token) =>
            {
                token.SetResult(_standingManager.DeleteStanding(playerId, songId));
            }).WaitResult<bool>();

            if (removed)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
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
            _scheduler.Schedule((token) =>
            {
                _matchManager.RemoveSongFromMatch(request.MatchId, request.EditSongId);
            }).Wait();

            return AddSongToMatch(request);
        }

        [HttpPost("addSongToMatch")]
        public IActionResult AddSongToMatch(PostAddSongToMatch request)
        {
            _scheduler.Schedule((token) =>
            {
                if (request.SongId != 0)
                    _matchManager.AddSongsToMatch(request.MatchId, [request.SongId]);
                else if (request.Level != null)
                    _matchManager.AddRandomSongsToMatch(request.MatchId, request.DivisionId, request.Group, request.Level);
            }).Wait();

            return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
        }

        [HttpPost("addMatch")]
        public IActionResult AddMatch(PostAddMatch request)
        {
            var match = _scheduler.Schedule((token) =>
            {
                var match = _matchManager.AddMatch(request.MatchName, request.Notes, request.Subtitle, request.PlayerIds, request.PhaseId, request.IsManualMatch);
                
                if (request.SongIds != null)
                    _matchManager.AddSongsToMatch(match, request.SongIds.ToArray());
                else if (request.Levels != null)
                {
                    _matchManager.AddRandomSongsToMatch(match, request.DivisionId, request.Group, request.Levels);
                }

                token.SetResult(match);

            }).WaitResult<Match>();

            return Ok(GetMatchDtoFromId(match.Id));
        }

        [HttpPost("setActiveMatch")]
        public IActionResult SetActiveMatch([FromBody] PostActiveMatchRequest request)
        {
            var activeMatch = _scheduler.Schedule((token) =>
            {
                token.SetResult(_matchManager.GetMatchFromId(request.MatchId).FirstOrDefault());
            }).WaitResult<Match>();

            if (activeMatch == null)
                return NotFound();

            _matchManager.SetActiveMatch(activeMatch);

            return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
        }

        [HttpGet("activeMatch")]
        public IActionResult GetActiveMatch()
        {
            if (_cache.ActiveMatch == 0)
                return NotFound();

            return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
        }

        [HttpPost("addStanding")]
        public IActionResult AddStanding(Standing request)
        {
            var added = _scheduler.Schedule((token) =>
            {
                token.SetResult(_standingManager.AddStanding(request));
            }).WaitResult<bool>();

            if (added)
                return Ok(GetMatchDtoFromId(_cache.ActiveMatch));
            else
                return NotFound();
        }

        private MatchDto GetMatchDtoFromId(int matchId)
        {
            return _scheduler.Schedule((token) =>
            {
                var match = _matchManager.GetMatchFromId(matchId).FirstOrDefault();
                
                if (match == null)
                    return;

                token.SetResult(GetMatchDto(match));
            }).WaitResult<MatchDto>();
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
                IsManualMatch = match.IsManualMatch,
                Players = match.PlayerInMatches.Select(p => p.Player).ToList(),
                Songs = match.SongInMatches.Select(s => s.Song).ToList(),
                Rounds = match.Rounds.ToList()
            };

            return matchDto;
        }

        private List<MatchDto> GetMatchesDtoFromPhaseId(int phaseId)
        {
            return _scheduler.Schedule((token) =>
            {
                var matches = _matchManager.GetMatchesFromPhaseId(phaseId).ToList();

                var matchesDto = matches.Select(match => new MatchDto
                {
                    Id = match.Id,
                    PhaseId = match.PhaseId,
                    Name = match.Name,
                    Subtitle = match.Subtitle,
                    Notes = match.Notes,
                    IsManualMatch = match.IsManualMatch,
                    Players = match.PlayerInMatches.Select(p => p.Player).ToList(),
                    Songs = match.SongInMatches.Select(s => s.Song).ToList(),
                    Rounds = match.Rounds.ToList()
                }).ToList();
                
                token.SetResult(matchesDto);
            }).WaitResult<List<MatchDto>>();
        }

    }
}
