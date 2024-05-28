using System.Net.WebSockets;
using TournamentManager.Requests;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System;

namespace TournamentManager.Services
{
    public class StandingServiceConfiguration
    {
        public string Address { get; }
        public ushort Port { get; }
    }
    public class StandingService : BackgroundService
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private ClientWebSocket _socket;

        public string Address { get; }
        public ushort Port { get; }
        public StandingService(IServiceScopeFactory serviceScopeFactory)
        {
            Address = "localhost";
            Port = 8080;
            _serviceScopeFactory = serviceScopeFactory;
        }
        //public StandingService(StandingServiceConfiguration config, IServiceScopeFactory serviceScopeFactory)
        //{
        //    Address = config.Address;
        //    Port = config.Port;
        //    _serviceScopeFactory = serviceScopeFactory;
        //}

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _socket = new ClientWebSocket();
            
            Receive(stoppingToken);

            return Task.CompletedTask;
        }

        private async void Receive(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _socket.ConnectAsync(new Uri($"ws://{Address}:{Port}/"), stoppingToken);

                    var buffer = new byte[1024];

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var res = await _socket.ReceiveAsync(buffer, stoppingToken);
                        string json = System.Text.Encoding.ASCII.GetString(buffer);

                        using (IServiceScope scope = _serviceScopeFactory.CreateScope())
                        {
                            IRawStandingSubscriber scopedProcessingService =
                                scope.ServiceProvider.GetRequiredService<IRawStandingSubscriber>();

                            scopedProcessingService.OnNewStanding(JsonSerializer.Deserialize<PostStandingRequest>(json));
                        }
                    }
                }
                catch
                {
                    await Task.Delay(5000);
                }
            }
        }
    }
}
