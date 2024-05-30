using System.Text.Json.Serialization;

namespace TournamentManager.Services
{
    public class HoldNote
    {
        [JsonPropertyName("none")]
        public long None { get; set; }

        [JsonPropertyName("letGo")]
        public long LetGo { get; set; }

        [JsonPropertyName("held")]
        public long Held { get; set; }

        [JsonPropertyName("missed")]
        public long Missed { get; set; }
    }

    public class TapNote
    {
        [JsonPropertyName("none")]
        public long None { get; set; }

        [JsonPropertyName("hitMine")]
        public long HitMine { get; set; }

        [JsonPropertyName("avoidMine")]
        public long AvoidMine { get; set; }

        [JsonPropertyName("checkpointMiss")]
        public long CheckpointMiss { get; set; }

        [JsonPropertyName("miss")]
        public long Miss { get; set; }

        [JsonPropertyName("W5")]
        public long W5 { get; set; }

        [JsonPropertyName("W4")]
        public long W4 { get; set; }

        [JsonPropertyName("W3")]
        public long W3 { get; set; }

        [JsonPropertyName("W2")]
        public long W2 { get; set; }

        [JsonPropertyName("W1")]
        public long W1 { get; set; }

        [JsonPropertyName("W0")]
        public long W0 { get; set; }

        [JsonPropertyName("checkpointHit")]
        public long CheckpointHit { get; set; }
    }
    public class Score
    {
        [JsonPropertyName("song")]
        public string Song { get; set; }

        [JsonPropertyName("playerNumber")]
        public long PlayerNumber { get; set; }

        [JsonPropertyName("playerName")]
        public string PlayerName { get; set; }

        [JsonPropertyName("actualDancePoints")]
        public long ActualDancePoints { get; set; }

        [JsonPropertyName("currentPossibleDancePoints")]
        public long CurrentPossibleDancePoints { get; set; }

        [JsonPropertyName("possibleDancePoints")]
        public long PossibleDancePoints { get; set; }

        [JsonPropertyName("formattedScore")]
        public string FormattedScore { get; set; }

        [JsonPropertyName("life")]
        public double Life { get; set; }

        [JsonPropertyName("isFailed")]
        public bool IsFailed { get; set; }

        [JsonPropertyName("tapNote")]
        public TapNote TapNote { get; set; }

        [JsonPropertyName("holdNote")]
        public HoldNote HoldNote { get; set; }

        [JsonPropertyName("totalHoldsCount")]
        public long TotalHoldsCount { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
