using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager
{
    public static class Extension
    {
        public static List<Song> GetAvailableSong(this IGenericRepository<Song> songRepository, Phase phase)
        {
            List<Song> bannedSongs = phase.Matches.Select(m => m.SongInMatches.Select(sim => sim.Song)).First().ToList();
            return songRepository.GetAll().Where(s => !bannedSongs.Contains(s)).ToList();
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
