using System.Text.Json.Serialization;

public class Player
{
    public const string PlayerNameTemplate = "%%NAME%%";
    public const string TeamNameTemplate = "%%TEAM%%";

    public const string NoteskinTemplate = "%%NOTESKIN%%";
    public const string ProspettivaTemplate = "%%PROSPETTIVA%%";
    public const string MiniTemplate = "%%Mini%%";

    public const string SpeedModTemplate = "%%SPEED_MOD%%";
    public const string SpeedModTypeTemplate = "%%SPEED_MOD_TYPE%%";

    public string Name { get; set; }
    public string Team { get; set; }

    [JsonIgnore]
    public List<KeyValuePair<string, string>> Mods { get; set; }
}