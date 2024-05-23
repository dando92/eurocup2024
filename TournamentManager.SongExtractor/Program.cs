using System.Text.Json;
using TournamentManager.DbModels;

namespace TournamentManager.SongExtractor
{
    public class Program
    {
        public static List<Song> GetSong(string songsPath)
        {
            List<Song> canzoncine = new List<Song>();

            string[] packs = Directory.EnumerateDirectories(songsPath).ToArray();

            foreach (string pack in packs)
            {
                string[] songs = Directory.EnumerateDirectories(pack).ToArray();

                foreach (string song in songs)
                {
                    string[] files = Directory.GetFiles(song);

                    string file = files.Where(f => Path.GetExtension(f) == ".ssc").First();
                    string[] lines = File.ReadAllLines(file);
                    string title = "";
                    string difficulty = "";

                    foreach (string line in lines)
                    {
                        if (line.Contains("#TITLE:"))
                        {
                            title = line.Split(":")[1];

                            title = title.Remove(title.Length - 1);
                        }

                        if (line.Contains("#METER:"))
                        {
                            difficulty = line.Split(":")[1];
                            difficulty = difficulty.Remove(difficulty.Length - 1);
                            break;
                        }
                    }

                    canzoncine.Add(new Song()
                    {
                        Title = title,
                        Group = Path.GetFileName(pack),
                        Difficulty = int.Parse(difficulty)
                    });

                }
            }

            return canzoncine;
        }

        private static void Main(string[] args)
        {
            if (args[0] == "--path" || args[0] == "-p")
            {
                var songs = GetSong(args[1]);
                File.WriteAllText("Songs.json", JsonSerializer.Serialize(songs, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine("Songs.json created!");
            }
            else
                Console.WriteLine("No --path or -p defined");

            Console.ReadLine();
        }
    }
}