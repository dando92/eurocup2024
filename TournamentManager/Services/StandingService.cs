using System.Net.WebSockets;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using TournamentManager.DbModels;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace TournamentManager.Services
{
    public class RawStanding 
    {
        [JsonPropertyName("scores")]
        public Score[] Scores { get; set; }

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
    public class StandingServiceConfiguration
    {
        public string Address { get; set; }
        public ushort Port { get; set; }
    }

    public class StandingService
    {
        private RequestDelegate next;

        private IServiceScopeFactory _serviceScopeFactory;
        public StandingService(RequestDelegate _next, IServiceScopeFactory serviceScopeFactory)
        {
            this.next = _next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await RunAsync(socket);
        }

        private async Task RunAsync(WebSocket socket)
        {
            try
            {
                await Receive(socket);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private async Task Receive(WebSocket client)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IRawStandingSubscriber scopedProcessingService =
                    scope.ServiceProvider.GetRequiredService<IRawStandingSubscriber>();

                var buffer = new byte[1024*8];

                while (!source.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(buffer, source.Token);

                    if (res.MessageType == WebSocketMessageType.Text)
                    {
                        try
                        {
                            var mes = Encoding.UTF8.GetString(buffer, 0, res.Count);

                            RawStanding score = Deserialize<RawStanding>(mes);
                            scopedProcessingService.OnNewStanding(score);
                        }
                        catch
                        {

                        }
                    }

                }
            }
        }

        static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
