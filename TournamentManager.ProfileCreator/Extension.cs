using System.Text.Json;

public static class Extension
{
    public static string Serialize(this List<Player> players)
    {
        return JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
    }


    public static string SerializeEditable(this Player player)
    {
        string end = StepmaniaProfileFolderSerializer.Editable;

        return end
            .Replace(Player.TeamNameTemplate, player.Team)
            .Replace(Player.PlayerNameTemplate, player.Name);
    }

    public static string SerializeUserPrefs(this Player player)
    {
        string end = StepmaniaProfileFolderSerializer.SimplyLovePrefs;

        foreach (var mod in player.Mods)
            end = end.Replace(mod.Key, mod.Value);

        return end;
    }
}
