using Microsoft.EntityFrameworkCore;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class SongRoller : ISongRoller
    {
        private readonly IGenericRepository<Song> _songRepo;
        private readonly IGenericRepository<Division> _divisionRepo;

        public SongRoller(IGenericRepository<Song> songRepo, IGenericRepository<Division> divisionRepo)
        {
            _songRepo = songRepo;
            _divisionRepo = divisionRepo;
        }

        public List<int> RollSongs(int divisionId, string group, string levels)
        {
            List<int> songs = new List<int>();

            var intLevels = levels.Split(",").Select(s => int.Parse(s)).ToArray();

            foreach (var level in intLevels)
            {
                int songId = RollSong(divisionId, group, level);

                if (songId != 0)
                    songs.Add(songId);
            }

            return songs;
        }

        public int RollSong(int divisionId, string group, int level)
        {
            Division division = GetDivision(divisionId);

            if (division == null)
                return 0;

            return _songRepo.RollSong(division, group, level);
        }

        private Division GetDivision(int id)
        {
            return _divisionRepo.GetAll()
                .Include(m => m.Phases)
                    .ThenInclude(m => m.Matches)
                        .ThenInclude(m => m.SongInMatches)
                .Where(m => m.Id == id).FirstOrDefault();
        }
    }
}
