using System.Net.WebSockets;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TournamentManager.Services;

namespace TournamentManager.Controllers
{
    public class StandingController : ControllerBase
    {
        private readonly Scheduler _scheduler;
        private IServiceScopeFactory _serviceScopeFactory;
        public StandingController(IServiceScopeFactory serviceScopeFactory, Scheduler scheduler)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _scheduler = scheduler;
        }

        //[Route("/ws")]
        [HttpGet("/ws")]
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

            using (IServiceScope logScope = _serviceScopeFactory.CreateScope())
            {
                ILogUpdate logUpdate = logScope.ServiceProvider.GetRequiredService<ILogUpdate>();

                while (!source.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(buffer, source.Token);

                    if (res.MessageType == WebSocketMessageType.Text)
                    {
                        try
                        {
                            var mes = Encoding.UTF8.GetString(buffer, 0, res.Count);
                            Score score = Deserialize<Score>(mes);

                            using (IServiceScope standinManagerScope = _serviceScopeFactory.CreateScope())
                            {
                                IStandingManager scopedProcessingService = standinManagerScope.ServiceProvider.GetRequiredService<IStandingManager>();

                                _scheduler.Schedule((token) =>
                                {
                                    token.SetResult(scopedProcessingService.AddStanding(score));
                                }).WaitResult<bool>();
                            }
                        }
                        catch (Exception ex)
                        {
                            logUpdate.LogError($"Error parsing score from itg, drop connection - {ex.ToString()} ");
                            source.Cancel();
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
