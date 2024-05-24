namespace TournamentManager.Services
{
    public class StandingService : BackgroundService
    {
        private IRawStandingSubscriber _subscriber;

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
            //TODO: Open client towards NodeJs app
            //OnReceiveMessage ->
            //_subscriber.OnNewStanding(JsonSerializer.Deserialize<PostStandingRequest>(message));

            return Task.CompletedTask;
        }
    }
}
