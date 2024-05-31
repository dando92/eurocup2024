using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager
{
    public static class Extension
    {
        public static IEnumerable<string> ListGroups(this List<Song> songs)
        { 
            return songs.Select(s => s.Group).Distinct();
        }

        public static List<int> GetBannedSongs(this Division division)
        {
            List<int> bannedSongs = new List<int>();

            foreach (var phase in division.Phases)
            {
                var songInMathces = phase.Matches.Select(m => m.SongInMatches);

                foreach (IEnumerable<SongInMatch> sim in songInMathces)
                    bannedSongs.AddRange(sim.Select(m => m.SongId));
            }

            return bannedSongs;
        }

        public static List<int> GetAvailableSong(this IGenericRepository<Song> songRepository, Division division, int level, string group)
        {
            List<int> availableSongs = new List<int>();
            List<int> allSongs = songRepository.GetAll().Where(s => (group == null || (group!=null && s.Group == group)) && s.Difficulty == level).Select(s => s.Id).ToList();
            List<int> bannedSongs = division.GetBannedSongs();

            foreach (int s in allSongs)
            {
                if (!bannedSongs.Contains(s))
                    availableSongs.Add(s);
            }
            
            return availableSongs;
        }

        public static int RollSong(this IGenericRepository<Song> songRepository, Division division, string group, int level)
        {
            return songRepository.GetAvailableSong(division, level, group).RandomElement();
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing<T>(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static List<Standing> Recalc(this ICollection<Standing> standings)
        {
            int failed = standings.Count(s => s.IsFailed);
            int maxPoints = standings.Count - failed;
            var orderedStandings = standings.Where(s => !s.IsFailed).OrderByDescending(s => s.Percentage).ToList();

            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Score = maxPoints;

                if (i + 1 < orderedStandings.Count)
                {
                    if (orderedStandings[i].Percentage > orderedStandings[i + 1].Percentage)
                        maxPoints--;
                }
            }

            return orderedStandings;
        }
    }
}
