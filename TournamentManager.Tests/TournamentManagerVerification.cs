using Moq;
using TournamentManager.Contexts;
using TournamentManager.DbModels;
using TournamentManager.Services;

namespace TournamentManager.Tests
{

    [TestClass]
    public class TournamentManagerVerification
    {
        Services.TournamentManager _tournamentManager;
        TorunamentCache _cache;
        Mock<IGenericRepository<Round>> _mock;

        [TestInitialize]
        public void Initialize()
        {
            _mock = new Mock<IGenericRepository<Round>>();
            _cache = new TorunamentCache();
            _tournamentManager = new Services.TournamentManager(_cache, _mock.Object, null);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (Round round in TestUtils.Match.Rounds.ToArray())
                round.Standings.Clear();
        }

        [TestMethod]
        public void AdvanceRoundTillMatchEnd()
        {
            _cache.SetActiveMatch(TestUtils.Match);

            for (int i = 0; i < TestUtils.Match.Rounds.Count; i++)
            {
                Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[i], _cache.CurrentRound);

                for (int j = 0; j < TestUtils.PlayersInMatch.Count; j++)
                {
                    _tournamentManager.OnNewStanding(new Standing()
                    {
                        Id = j,
                        Percentage = 100,
                        Player = TestUtils.PlayersInMatch[j],
                        Song = TestUtils.SongsInMatch[j]
                    });
                }
            }

            Assert.IsNull(_cache.CurrentRound);
        }

        [TestMethod]
        public void CachedStandings_AreCorrect()
        {
            _cache.SetActiveMatch(TestUtils.Match);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
            int index = 0;

            _tournamentManager.OnNewStanding(new Standing()
            {
                Id = index,
                Percentage = 80,
                Player = TestUtils.PlayersInMatch[index],
                Song = TestUtils.SongsInMatch[index]
            });

            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
            Assert.AreEqual(++index, _cache.Standings.Count);

            _tournamentManager.OnNewStanding(new Standing()
            {
                Id = index,
                Percentage = 60,
                Player = TestUtils.PlayersInMatch[index],
                Song = TestUtils.SongsInMatch[index]
            });

            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
            Assert.AreEqual(++index, _cache.Standings.Count);

            _tournamentManager.OnNewStanding(new Standing()
            {
                Id = index,
                Percentage = 100,
                Player = TestUtils.PlayersInMatch[index],
                Song = TestUtils.SongsInMatch[index]
            });

            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
            Assert.AreEqual(++index, _cache.Standings.Count);

            _tournamentManager.OnNewStanding(new Standing()
            {
                Id = index,
                Percentage = 40,
                Player = TestUtils.PlayersInMatch[index],
                Song = TestUtils.SongsInMatch[index]
            });

            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[1], _cache.CurrentRound);
            Assert.AreEqual(0, _cache.Standings.Count);
            Assert.IsNotNull(_cache.ActiveMatch);
        }

        [TestMethod]
        public void UpdateStandingsRepository_WorksCorrectly()
        {
            Round updatedRound = null;
            _mock.Setup(c => c.Update(It.IsAny<Round>()))
                .Callback<Round>((round) => { updatedRound = round; });

            CachedStandings_AreCorrect();

            _mock.Verify(c => c.Update(It.IsAny<Round>()), Times.Once());

            Assert.AreEqual(4, updatedRound.Standings.ToArray()[0].Score);
            Assert.AreEqual(100, updatedRound.Standings.ToArray()[0].Percentage);

            Assert.AreEqual(3, updatedRound.Standings.ToArray()[1].Score);
            Assert.AreEqual(80, updatedRound.Standings.ToArray()[1].Percentage);

            Assert.AreEqual(2, updatedRound.Standings.ToArray()[2].Score);
            Assert.AreEqual(60, updatedRound.Standings.ToArray()[2].Percentage);

            Assert.AreEqual(1, updatedRound.Standings.ToArray()[3].Score);
            Assert.AreEqual(40, updatedRound.Standings.ToArray()[3].Percentage);
        }
    }
}