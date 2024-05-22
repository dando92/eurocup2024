using TournamentManager.DbModels;

namespace TournamentManager
{
    public static class Extension
    {
        public static IEnumerator<Round> GetNextRound(this Match match)
        {
            foreach (var round in match.Rounds)
                yield return round;
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
