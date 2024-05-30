using System.Net.WebSockets;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text;

namespace TournamentManager.Services
{
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
                IStandingManager scopedProcessingService =
                    scope.ServiceProvider.GetRequiredService<IStandingManager>();

                var buffer = new byte[1024*8];

                while (!source.IsCancellationRequested)
                {
                    var res = await client.ReceiveAsync(buffer, source.Token);

                    if (res.MessageType == WebSocketMessageType.Text)
                    {
                        try
                        {
                            var mes = Encoding.UTF8.GetString(buffer, 0, res.Count);

                            Score score = Deserialize<Score>(mes);
                            scopedProcessingService.AddStanding(score);
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
