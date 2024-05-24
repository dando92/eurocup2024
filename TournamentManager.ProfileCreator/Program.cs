using System.ComponentModel.DataAnnotations;
using System.Text.Json;

internal class Program
{
    public const string Editable = "[Editable]\r\nBirthYear=0\r\nCharacterID=default\r\nDisplayName=<Template>\r\nIgnoreStepCountCalories=0\r\nIsMale=1\r\nLastUsedHighScoreName=A\r\nVoomax=0.000000\r\nWeightPounds=0\r\n\r\n";
    public class Player
    {
        public string Name {get;set;} 
    }
    private static void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }

    public static List<Player> GetPlayers(string[] names)
    {
        List<Player> players = new List<Player>();

        for (int i = 0; i < names.Length; i++)
            players.Add(new Player() { Name = names[i] });

        return players;
    }

    public static void DeploySmPLayers(string outputFolder, string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            string folderName = i.ToString("D8");
            Directory.CreateDirectory(Path.Combine(outputFolder, folderName));

            Directory.CreateDirectory(Path.Combine(outputFolder, folderName, "EditCourses"));
            Directory.CreateDirectory(Path.Combine(outputFolder, folderName, "Edits"));
            Directory.CreateDirectory(Path.Combine(outputFolder, folderName, "Rivals"));
            Directory.CreateDirectory(Path.Combine(outputFolder, folderName, "Screenshots"));
            File.WriteAllText(Path.Combine(outputFolder, folderName, "Editable.ini"), Editable.Replace("<Template>", names[i]));
            File.Copy("Stats.xml", Path.Combine(outputFolder, folderName, "Stats.xml"));
            File.Copy("Type.ini", Path.Combine(outputFolder, folderName, "Type.ini"));
        }
    }

    private static void Main(string[] args)
    {
        if (args[0] == "--file" || args[0] == "-f")
        {
            string fileName = args[1];
            string[] p = File.ReadAllLines(fileName);
            string outputFolder = "Output";

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            List<Player> players = GetPlayers(p);
            DeploySmPLayers(outputFolder, p);

            File.WriteAllText(Path.Combine(outputFolder, "Players.json"), JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}