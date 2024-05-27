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

        public static List<int> GetBannedSongs(this Phase phase)
        {
            List<int> bannedSongs = new List<int>();
            
            var songInMathces = phase.Matches.Select(m => m.SongInMatches);
            
            foreach(IEnumerable<SongInMatch> sim in songInMathces)
                bannedSongs.AddRange(sim.Select(m => m.SongId));

            return bannedSongs;
        }

        public static List<int> GetAvailableSong(this IGenericRepository<Song> songRepository, Phase phase, int level, string group)
        {
            List<int> availableSongs = new List<int>();
            List<int> allSongs = songRepository.GetAll().Where(s => (group == null || (group!=null && s.Group == group)) && s.Difficulty == level).Select(s => s.Id).ToList();
            List<int> bannedSongs = phase.GetBannedSongs();

            foreach (int s in allSongs)
            {
                if (!bannedSongs.Contains(s))
                    availableSongs.Add(s);
            }
            
            return availableSongs;
        }

        public static int RollSong(this IGenericRepository<Song> songRepository, Phase phase, string group, int level)
        {
            return songRepository.GetAvailableSong(phase, level, group).RandomElement();
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
            int maxPoints = standings.Count;
            var orderedStandings = standings.OrderByDescending(s => s.Percentage).ToList();

            for (int i = 0; i < standings.Count; i++)
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
