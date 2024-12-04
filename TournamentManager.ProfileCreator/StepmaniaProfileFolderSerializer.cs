public class StepmaniaProfileFolderSerializer : IPlayerSerializer
{
    public const string Editable = 
        $"[Editable]\r\n" +
        $"BirthYear=0\r\n" +
        $"CharacterID=default\r\n" +
        $"DisplayName={Player.PlayerNameTemplate}\r\n" +
        $"TeamName={Player.TeamNameTemplate}\r\n" +
        $"IgnoreStepCountCalories=0\r\n" +
        $"IsMale=1\r\n" +
        $"LastUsedHighScoreName=\r\n" +
        $"Voomax=0.000000\r\n" +
        $"WeightPounds=0\r\n";

    public const string SimplyLovePrefs = 
        $"[Simply Love]\r\n" +
        $"ActionOnMissedTarget=Nothing\r\n" +
        $"BackgroundBrightness=0%\r\n" +
        $"BackgroundFilter=Off\r\n" +
        $"ColumnCues=false\r\n" +
        $"ColumnFlashOnMiss=false\r\n" +
        $"ComboFont=Wendy\r\n" +
        $"DataVisualizations=None\r\n" +
        $"DisplayScorebox=true\r\n" +
        $"ErrorBar=None\r\n" +
        $"ErrorBarMultiTick=false\r\n" +
        $"ErrorBarTrim=Off\r\n" +
        $"ErrorBarUp=false\r\n" +
        $"HideCombo=false\r\n" +
        $"HideComboExplosions=false\r\n" +
        $"HideDanger=false\r\n" +
        $"HideEarlyDecentWayOffFlash=true\r\n" +
        $"HideEarlyDecentWayOffJudgments=true\r\n" +
        $"HideLifebar=false\r\n" +
        $"HideLights=true\r\n" +
        $"HideLookahead=false\r\n" +
        $"HideScore=false\r\n" +
        $"HideSongBG=false\r\n" +
        $"HideTargets=false\r\n" +
        $"HoldJudgment=Love 1x2 (doubleres).png\r\n" +
        $"JudgmentGraphic=Love 2x7 (doubleres).png\r\n" +
        $"JudgmentTilt=false\r\n" +
        $"LaneCover=0%\r\n" +
        $"LifeMeterType=Standard\r\n" +
        $"MeasureCounter=None\r\n" +
        $"MeasureCounterLeft=true\r\n" +
        $"MeasureCounterUp=false\r\n" +
        $"MeasureLines=Off\r\n" +
        $"Mini={Player.MiniTemplate}\r\n" +
        $"NPSGraphAtTop=false\r\n" +
        $"NoteFieldOffsetX=0\r\n" +
        $"NoteFieldOffsetY=0\r\n" +
        $"NoteSkin={Player.NoteskinTemplate}\r\n" +
        $"Pacemaker=false\r\n" +
        $"PlayerOptionsString={Player.SpeedModTypeTemplate}{Player.SpeedModTemplate}, {Player.MiniTemplate} Mini, {Player.ProspettivaTemplate}, {Player.NoteskinTemplate}\r\n" +
        $"ShowEXScore=false\r\n" +
        $"ShowFaPlusPane=true\r\n" +
        $"ShowFaPlusWindow=false\r\n" +
        $"SpeedMod={Player.SpeedModTemplate}\r\n" +
        $"SpeedModType={Player.SpeedModTypeTemplate}\r\n" +
        $"SubtractiveScoring=false\r\n" +
        $"TargetScore=11\r\n" +
        $"VisualDelay=0ms\r\n";

    public string OutputFolder { get; set; }

    public StepmaniaProfileFolderSerializer(string outputFolder)
    {
        OutputFolder = outputFolder;
    }

    public void Serialize(List<Player> players)
    {
        foreach (Player p in players)
        {
            string folderName = Path.Combine(OutputFolder, p.Name);
            Directory.CreateDirectory(folderName);

            DeploySmPLayer(folderName, p);
        }
        string fileName = Path.Combine(OutputFolder, "Players.json");

        File.WriteAllText(fileName, players.Serialize());
    }

    public static void DeploySmPLayer(string outputFolder, Player player)
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
