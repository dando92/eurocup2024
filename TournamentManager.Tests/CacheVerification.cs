//using TournamentManager.Services;

//namespace TournamentManager.Tests
//{

//    [TestClass]
//    public class CacheVerification
//    {
//        TournamentCache _cache;

//        [TestInitialize]
//        public void Initialize()
//        {
//            _cache = new TournamentCache();
//        }

//        [TestMethod]
//        public void ActiveMatch_ManagedCorrectly()
//        {
//            _cache.SetActiveMatch(TestUtils.Match);
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);
//            _cache.SetActiveMatch(null);
//            Assert.IsNull(_cache.ActiveMatch);
//            Assert.AreEqual(_cache.CurrentRound, null);
//        }

//        [TestMethod]
//        public void Iterator_ManagedCorrectly()
//        {
//            _cache.SetActiveMatch(TestUtils.Match);
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[0], _cache.CurrentRound);

//            _cache.AdvanceRound();
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[1], _cache.CurrentRound);

//            _cache.AdvanceRound();
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[2], _cache.CurrentRound);

//            _cache.AdvanceRound();
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.AreEqual(TestUtils.Match.Rounds.ToArray()[3], _cache.CurrentRound);

//            _cache.AdvanceRound();
//            Assert.AreEqual(TestUtils.Match, _cache.ActiveMatch);
//            Assert.IsNull(_cache.CurrentRound);
//        }
//    }
//}