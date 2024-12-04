public class CsvPlayerRetriever : IPlayerDeserializer
{
    public char Separator { get; set; }
    public string Path { get; set; }

    public CsvPlayerRetriever(char separator, string path)
    {
        Separator = separator;
        Path = path;
    }


    public List<Player> GetPlayers()
    {
        List<Player> list = new List<Player>();
        string[] lines = File.ReadAllLines(Path);

        foreach (string line in lines)
        {
            var fields = line.Split(Separator);

            list.Add(new Player()
            {
                Name = fields[0],
                Team = fields[1],
                Mods = new List<KeyValuePair<string, string>>()
                {
                    new(Player.SpeedModTemplate, fields[2].Replace("M", "")),
                    new(Player.SpeedModTypeTemplate, "M"),
                    new(Player.ProspettivaTemplate, fields[3]),
                    new(Player.MiniTemplate, fields[4] == "0" ? "" : fields[4] + "%"),
                    new(Player.NoteskinTemplate, fields[5]),
                }
            });
        }

        return list;
    }
}