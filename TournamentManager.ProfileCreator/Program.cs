internal class Program
{

    private static void Main(string[] args)
    {
        if (args[0] == "--file" || args[0] == "-f")
        {
            string fileName = args[1];

            string folder = Path.Combine(Path.GetDirectoryName(fileName), "Output");
            
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            IPlayerDeserializer ret = new CsvPlayerRetriever(';', fileName);
            IPlayerSerializer ret1 = new JsonDBSerializer(folder);
            IPlayerSerializer ret2 = new StepmaniaProfileFolderSerializer(folder);

            var players = ret.GetPlayers();

            ret1.Serialize(players);
            ret2.Serialize(players);
        }
    }
}