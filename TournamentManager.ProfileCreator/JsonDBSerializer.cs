public class JsonDBSerializer : IPlayerSerializer
{
    public string OutputFolder { get; set; }

    public JsonDBSerializer(string outputFolder)
    {
        OutputFolder = outputFolder;
    }

    public void Serialize(List<Player> players)
    {
        string fileName = Path.Combine(OutputFolder, "Players.json");

        File.WriteAllText(fileName, players.Serialize());
    }
}
