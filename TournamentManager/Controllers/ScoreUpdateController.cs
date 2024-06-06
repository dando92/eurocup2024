using System.Net.WebSockets;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    public class ScoreUpdateController : ControllerBase
    {
        public string _currentSong = "";
        public Dictionary<string, Score> _scoreByPlayer = new Dictionary<string, Score>();

        private readonly Scheduler _scheduler;
        private IServiceScopeFactory _serviceScopeFactory;

        public ScoreUpdateController(IServiceScopeFactory serviceScopeFactory, Scheduler scheduler)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _scheduler = scheduler;
        }

        //[Route("/ws")]
        [HttpGet("/scoreChange")]
        public async Task Get()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return;
            }
            var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
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
            var buffer = new byte[1024];

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IScoreUpdate scoreUpdate = scope.ServiceProvider.GetRequiredService<IScoreUpdate>();
                ILogUpdate logUpdate = scope.ServiceProvider.GetRequiredService<ILogUpdate>();

                while (!source.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(buffer, source.Token);

                    if (res.MessageType == WebSocketMessageType.Text)
                    {
                        try
                        {
                            var mes = Encoding.UTF8.GetString(buffer, 0, res.Count);
                            Score score = Deserialize<Score>(mes);

                            if (_currentSong != score.Song)
                            {
                                _scoreByPlayer.Clear();
                                _currentSong = score.Song;
                            }

                            if(!_scoreByPlayer.ContainsKey(score.PlayerName))
                                _scoreByPlayer.Add(score.PlayerName, score);
                            else
                                _scoreByPlayer[score.PlayerName] = score;

                            scoreUpdate.OnScoreUpdate(new ScoreUpdateDTO() { Json = mes });
                        }
                        catch (Exception ex)
                        {
                            //logUpdate.LogError($"Not a valid score");
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
