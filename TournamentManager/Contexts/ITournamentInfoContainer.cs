using TournamentManager.DbModels;

namespace TournamentManager.Contexts
{
    public interface ITournamentInfoContainer
    {
        Song GetSongById(int id);
        Player GetPlayerById(int id);


        Song GetSongByName(string name);
        Player GetPlayerByName(string name);

    }
}
