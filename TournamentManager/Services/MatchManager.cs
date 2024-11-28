using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public interface IMatchManager
    {
        Match AddMatch(string matchName, string notes, string subtitle, int[] playerIds, int phaseId, bool isManualMatch);
        void AddRandomSongsToMatch(int matchId, int divisionId, string group, string levels);
        void AddRandomSongsToMatch(Match match, int divisionId, string group, string levels);
        void AddSongsToMatch(Match match, int[] songIds);
        void AddSongsToMatch(int matchId, int[] songIds);
        void RemoveSongFromMatch(int matchId, int songId);
        void RemoveSongFromMatch(Match match, int songId);
        IQueryable<Match> GetMatchFromId(int matchId);
        IQueryable<Match> GetMatchesFromPhaseId(int phaseId);
        void SetActiveMatch(Match match);
    }

    public class MatchManager : IMatchManager
    {
        private readonly ISongRoller _roller;
        private readonly IGenericRepository<Match> _matchRepo;
        private readonly IGenericRepository<Round> _roundRepo;
        private readonly ITournamentCache _cache;
        private readonly IMatchUpdate _hub;

        public MatchManager(ISongRoller roller, 
            IGenericRepository<Match> matchRepo, 
            IGenericRepository<Round> roundRepo, 
            ITournamentCache cache, 
            IMatchUpdate hub)
        {
            _roller = roller;
            _matchRepo = matchRepo;
            _roundRepo = roundRepo;
            _cache = cache;
            _hub = hub;
        }

        public void SetActiveMatch(Match match)
        {
            _cache.SetActiveMatch(match);
            _hub?.Update(match);
        }

        public Match AddMatch(string matchName, string notes, string subtitle, int[] playerIds, int phaseId, bool isManualMatch)
        {
            var newMatch = CreateMatch(matchName, notes, subtitle, playerIds, isManualMatch);

            newMatch.PhaseId = phaseId;

            _matchRepo.Add(newMatch);

            _matchRepo.Save();

            return newMatch;
        }

        public void RemoveSongFromMatch(Match match, int songId)
        {
            var sim = match.SongInMatches.Where(sim => sim.SongId == songId).FirstOrDefault();

            if (sim == null)
                return;

            match.SongInMatches.Remove(sim);

            var round = _roundRepo.GetAll().Where(round => round.MatchId == match.Id && round.Standings.Count == 0).FirstOrDefault();

            _roundRepo.DeleteById(round.Id);

            _roundRepo.Save();
            _hub?.Update(match);
        }

        public void RemoveSongFromMatch(int matchId, int songId)
        {
            var match = GetMatchFromId(matchId).FirstOrDefault();

            if (match == null)
                return;

            RemoveSongFromMatch(match, songId);
        }

        public void AddSongsToMatch(int matchId, int[] songIds)
        {
            var match = GetMatchFromId(matchId).FirstOrDefault();

            if (match == null)
                return;

            AddSongsToMatch(match, songIds);
        }

        public void AddSongsToMatch(Match match, int[] songIds)
        {
            foreach (var songId in songIds)
                AddSongToMatch(match, songId);

            _matchRepo.Save();
            _hub?.Update(match);
        }

        public void AddRandomSongsToMatch(Match match, int divisionId, string group, string levels)
        {
            int[] songIds = _roller.RollSongs(divisionId, group, levels).ToArray();

            AddSongsToMatch(match, songIds);
        }

        public void AddRandomSongsToMatch(int matchId, int divisionId, string group, string levels)
        {
            var match = GetMatchFromId(matchId).FirstOrDefault();

            if (match == null)
                return;

            AddRandomSongsToMatch(match, divisionId, group, levels);
        }

        protected void AddSongToMatch(Match match, int songId)
        {
            _roundRepo.Add(CreateRound(match, songId));
        }

        private Match CreateMatch(string matchName, string notes, string subTitle, int[] players, bool isManualMatch)
        {
            var match = new Match()
            {
                Name = matchName,
                Subtitle = subTitle,
                Notes = notes,
                IsManualMatch = isManualMatch,
                PlayerInMatches = new List<PlayerInMatch>(players.Length),
                SongInMatches = new List<SongInMatch>(),
                Rounds = new List<Round>(),
            };

            foreach (var player in players)
                match.PlayerInMatches.Add(new PlayerInMatch() { PlayerId = player, MatchId = match.Id, Match = match });

            return match;
        }

        private Round CreateRound(Match match, int songId)
        {
            var round = new Round()
            {
                Match = match,
                MatchId = match.Id,
                SongId = songId,
                Standings = new List<Standing>()
            };

            match.SongInMatches.Add(new SongInMatch() { SongId = songId, MatchId = match.Id });

            match.Rounds.Add(round);

            return round;
        }

        public IQueryable<Match> GetMatchesFromPhaseId(int phaseId)
        {
            var matches = _matchRepo.GetAll()
                .Include(m => m.Rounds)
                    .ThenInclude(m => m.Standings)
                .Include(m => m.PlayerInMatches)
                    .ThenInclude(p => p.Player)
                .Include(m => m.SongInMatches)
                    .ThenInclude(s => s.Song)
                .Where(m => m.PhaseId == phaseId);

            return matches;
        }

        public IQueryable<Match> GetMatchFromId(int matchId)
        {
            if (matchId == 0)
                return null;

            return _matchRepo
                .GetAll()
                .Include(m => m.Phase)
                .Include(m => m.Rounds)
                    .ThenInclude(m => m.Standings)
                        .ThenInclude(p => p.Player)
                            .ThenInclude(p => p.Team)
                .Include(m => m.PlayerInMatches)
                    .ThenInclude(p => p.Player)
                        .ThenInclude(p => p.Team)
                .Include(m => m.SongInMatches)
                    .ThenInclude(s => s.Song)
                .Where(m => m.Id == matchId);
        }
    }
}
