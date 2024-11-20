namespace TournamentManager.DbModels;

public class Team
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Score{ get; set; }

    public ICollection<Player> Players { get; set; }
}