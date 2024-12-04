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

    public static void DeploySmFolder(this Player player, string outputFolder)
    {
        Directory.CreateDirectory(Path.Combine(outputFolder, "EditCourses"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "Edits"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "Rivals"));
        Directory.CreateDirectory(Path.Combine(outputFolder, "Screenshots"));

        File.WriteAllText(Path.Combine(outputFolder, "Editable.ini"), player.SerializeEditable());
        File.WriteAllText(Path.Combine(outputFolder, "Simply Love UserPrefs.ini"), player.SerializeUserPrefs());

        File.Copy("Stats.xml", Path.Combine(outputFolder, "Stats.xml"));
        File.Copy("Type.ini", Path.Combine(outputFolder, "Type.ini"));
    }
}
