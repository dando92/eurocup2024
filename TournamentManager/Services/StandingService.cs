using Fleck;
using System.Text.Json;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public class StandingService : BackgroundService
    {
        private IRawStandingSubscriber _subscriber;

        WebSocketServer _server;

        public string IpAddress { get; }
        public ushort Port { get; }

        public StandingService(string ipAddress, ushort port,
            IRawStandingSubscriber subscriber)
        {
            IpAddress = ipAddress;
            Port = port;
            _subscriber = subscriber;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _server = new WebSocketServer($"ws://{IpAddress}:{Port}/");

            _server.Start(conn =>
            {
                conn.OnOpen = () =>
                {
                    //TODO: Necessary?
                };
                conn.OnMessage = message =>
                {
                    _subscriber.OnNewStanding(JsonSerializer.Deserialize<PostStandingRequest>(message));
                };
                conn.OnClose = () =>
                {
                    //TODO: Necessary?
                };
            });

            return Task.CompletedTask;
        }
    }
}
