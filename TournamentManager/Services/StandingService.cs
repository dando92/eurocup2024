using System.Net.WebSockets;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text;

namespace TournamentManager.Services
{
    public class StandingService
    {
        private RequestDelegate next;

        private readonly Scheduler _scheduler;
        private IServiceScopeFactory _serviceScopeFactory;
        public StandingService(RequestDelegate _next, IServiceScopeFactory serviceScopeFactory, Scheduler scheduler)
        {
            this.next = _next;
            _serviceScopeFactory = serviceScopeFactory;
            _scheduler = scheduler;
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
                            logUpdate.LogError($"Error parsing score from itg, drop connection - {ex.ToString()} " );
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
