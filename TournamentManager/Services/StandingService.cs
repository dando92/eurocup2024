using Fleck;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Requests;

namespace TournamentManager.Services
{
    public class StandingService : BackgroundService
    {
        private List<IStandingSubscriber> _subscribers;
        WebSocketServer _server;

        public string IpAddress { get; }
        public ushort Port { get; }
        private ITournamentInfoContainer _infoContainer;
        private IGenericRepository<Standing> _standingRepo;

        public StandingService(string ipAddress, ushort port, ITournamentInfoContainer infoContainer, IGenericRepository<Standing> standingRepo, List<IStandingSubscriber> subscribers)
        {
            IpAddress = ipAddress;
            Port = port;
            _infoContainer = infoContainer;
            _subscribers = subscribers;
            _standingRepo = standingRepo;
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
                    PostStandingRequest request = JsonSerializer.Deserialize<PostStandingRequest>(message);

                    Song song = _infoContainer.GetSongByName(request.Player);
                    Player player = _infoContainer.GetPlayerByName(request.Song);

                    //Player or song not registered, do nothing
                    if (song == null || player == null)
                        return;

                    Standing standing = new Standing()
                    {
                        Percentage = request.Percentage,
                        RoundId = _infoContainer.GetCurrentRound().Id,
                        PlayerId = player.Id,
                        SongId = song.Id
                    };

                    _standingRepo.Add(standing);
                    
                    NotifyNewStanding(standing);
                };
                conn.OnClose = () =>
                {
                    //TODO: Necessary?
                };
            });

            return Task.CompletedTask;
        }


        private void NotifyNewStanding(Standing standing)
        {
            if (_subscribers == null)
                return;

            foreach (var subscriber in _subscribers)
                subscriber.OnNewStanding(standing);
        }
    }
}
