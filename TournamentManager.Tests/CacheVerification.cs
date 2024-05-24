using TournamentManager.Services;

namespace TournamentManager.Tests
{
    [TestClass]
    public class CacheVerification
    {
        TorunamentCache _cache;

        [TestInitialize]
        public void Initialize()
        {
            _cache = new TorunamentCache();
        }

        [TestMethod]
        public void ActiveMatchManagedCorrectly()
        {
            _cache.SetActiveMatch(TestUtils.Match);
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
            _cache.SetActiveMatch(null);
            Assert.IsNull(_cache.ActiveMatch);
            Assert.AreEqual(_cache.CurrentRound, null);
        }

        [TestMethod]
        public void IteratorManagedCorrectly()
        {
            _cache.SetActiveMatch(TestUtils.Match);
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);

            _cache.AdvanceRound();
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[1], _cache.CurrentRound);

            _cache.AdvanceRound();
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[2], _cache.CurrentRound);

            _cache.AdvanceRound();
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[3], _cache.CurrentRound);

            _cache.AdvanceRound();
            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
            Assert.IsNull(_cache.CurrentRound);
        }
    }
}