using TournamentManager.DbModels;

namespace TournamentManager
{
    public interface ITournamentInfoContainer
    {
        Match GetActiveMatch();

        void SetActiveMatch(Match match);

        Round GetCurrentRound();

        Song GetSongById(int id);
        Player GetPlayerById(int id);


        Song GetSongByName(string name);
        Player GetPlayerByName(string name);

        void AddStanding(Standing standing);
    }
}
