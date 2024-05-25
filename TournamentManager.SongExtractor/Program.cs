using System.Text.Json;
using TournamentManager.DbModels;

namespace TournamentManager.SongExtractor
{
    public class Program
    {
        public static Song GetSongFromSSC(string pack, string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
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

            return new Song()
            {
                Title = title,
                Group = Path.GetFileName(pack),
                Difficulty = int.Parse(difficulty)
            };
        }

        public static Song GetSongFromSM(string pack, string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            string title = "";
            string difficulty = "";

            for(int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("#TITLE:"))
                {
                    title = lines[i].Split(":")[1];

                    title = title.Remove(title.Length - 1);
                }

                if (lines[i].Contains("Challenge:") || lines[i].Contains("Hard:") || lines[i].Contains("Medium:") || lines[i].Contains("Easy:"))
                {
                    difficulty = lines[i + 1].Split(":")[0].Trim();
                    break;
                }
            }

            return new Song()
            {
                Title = title,
                Group = Path.GetFileName(pack),
                Difficulty = int.Parse(difficulty)
            };
        }

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

                    string ssc = files.Where(f => Path.GetExtension(f) == ".ssc").FirstOrDefault();

                    if (ssc != null)
                        canzoncine.Add(GetSongFromSSC(pack, ssc));
                    else 
                    {
                        string sm = files.Where(f => Path.GetExtension(f) == ".sm").FirstOrDefault();

                        if (sm != null)
                            canzoncine.Add(GetSongFromSM(pack, sm));
                    }
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