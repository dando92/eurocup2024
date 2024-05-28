using Moq;
using TournamentManager.Contexts;
using TournamentManager.Controllers;
using TournamentManager.DbModels;
using TournamentManager.Services;

namespace TournamentManager.Tests
{
    [TestClass]
    public class TournamentControllerVerification
    {
        TournamentCache _cache;
        Mock<IRawStandingSubscriber> _rawStandingSub;
        Mock<IGenericRepository<Division>> _divisionRepo;
        Mock<IGenericRepository<DbModels.Match>> _matchRepo;
        Mock<IGenericRepository<Phase>> _phaseRepo;
        Mock<IGenericRepository<Song>> _songRepo;
        TournamentController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _rawStandingSub = new Mock<IRawStandingSubscriber>();
            _divisionRepo = new Mock<IGenericRepository<Division>>();
            _matchRepo = new Mock<IGenericRepository<DbModels.Match>>();
            _phaseRepo = new Mock<IGenericRepository<Phase>>();
            _songRepo = new Mock<IGenericRepository<Song>>();
            _cache = new TournamentCache();

            _divisionRepo
                .Setup(s => s.GetAll(true))
                .Returns(TournamentControllerDatabase.Division.AsQueryable());

            _divisionRepo
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns((int id) => TournamentControllerDatabase.Division.Find(p => p.Id == id));

            _songRepo
                .Setup(s => s.GetAll(true))
                .Returns(TournamentControllerDatabase.Songs.AsQueryable());

            _songRepo
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns((int id) => TournamentControllerDatabase.Songs.Find(p => p.Id == id));

            _matchRepo
                .Setup(s => s.GetAll(true))
                .Returns(TournamentControllerDatabase.Matches.AsQueryable());

            _matchRepo
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns((int id) => TournamentControllerDatabase.Matches.Find(p => p.Id == id));

            _phaseRepo
                .Setup(s => s.GetAll(true))
                .Returns(TournamentControllerDatabase.Phases.AsQueryable());

            _phaseRepo
                .Setup(s => s.GetById(It.IsAny<int>()))
                .Returns((int id) => TournamentControllerDatabase.Phases.Find(p => p.Id == id));

            _controller = new TournamentController(_cache, _divisionRepo.Object, _rawStandingSub.Object, _matchRepo.Object, _phaseRepo.Object, _songRepo.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void CreateMatchWithRoll()
        {
            _controller.AddMatch(new Requests.PostAddMatch()
            {
                DivisionId = 0,
                Group = "g1",
                Levels = "8,9",
                MatchName = "First",
                PhaseId = 0,
                PlayerIds = [0, 1]
            });
            //_controller.DeleteStanding();
            //_controller.DeleteStandingForPlayer();
            //_controller.EditMatchSong();
            //_controller.AddSongToMatch();
            //_controller.SetActiveMatch();
        }

        [TestMethod]
        public void CreateMatchWithSongIds()
        {
            _controller.AddMatch(new Requests.PostAddMatch()
            {
                DivisionId = 0,
                Group = "g1",
                Levels = null,
                SongIds = [0, 2],
                MatchName = "First",
                PhaseId = 0,
                PlayerIds = [0, 1]
            });
            //_controller.DeleteStanding();
            //_controller.DeleteStandingForPlayer();
            //_controller.EditMatchSong();
            //_controller.AddSongToMatch();
            //_controller.SetActiveMatch();
        }

        [TestMethod]
        public void AddSongToMatch()
        {
            CreateMatchWithSongIds();


            _controller.AddSongToMatch(new Requests.PostAddSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g1",
                SongId = 1,
                Level = null
            });

            _controller.AddSongToMatch(new Requests.PostAddSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g2",
                SongId = null,
                Level = "9"
            });
            _controller.EditMatchSong(new Requests.PostEditSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g2",
                SongId = null,
                Level = "10",
                EditSongId = 0
            });

            _controller.EditMatchSong(new Requests.PostEditSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g2",
                SongId = 0,
                Level = null,
                EditSongId = 2
            });
        }

        [TestMethod]
        public void EditMatchSong()
        {
            CreateMatchWithSongIds();

            _controller.EditMatchSong(new Requests.PostEditSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g2",
                SongId = null,
                Level = "10",
                EditSongId = 0
            });

            _controller.EditMatchSong(new Requests.PostEditSongToMatch()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0,
                Group = "g2",
                SongId = 0,
                Level = null,
                EditSongId = 2
            });
        }


        [TestMethod]
        public void AddStanding()
        {
            CreateMatchWithSongIds();

            _controller.UpdateScore(new Requests.PostStandingRequest()
            {
                Song = "",
                Player = "p1",
                Percentage = 100
            });

            _controller.UpdateScore(new Requests.PostStandingRequest()
            {
                Song = "",
                Player = "p1",
                Percentage = 90
            });
        }
        //_controller.DeleteStanding();
        //_controller.DeleteStandingForPlayer();
        //_controller.SetActiveMatch();

        [TestMethod]
        public void SetActiveMatch_WorksCorrectly()
        {
            CreateMatchWithSongIds();

            _controller.SetActiveMatch(new Requests.PostActiveMatchRequest()
            {
                DivisionId = 0,
                PhaseId = 0,
                MatchId = 0
            });
        }
    }
}