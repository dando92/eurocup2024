using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Tests
{

    [TestClass]
    public class UnitTest1 : IGenericRepository<Round>
    {
        MoqTorunamentContainer ads = new MoqTorunamentContainer();
        Services.TournamentManager _manager;

        public void Add(Round entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Round?> GetAll(bool tracked = true)
        {
            throw new NotImplementedException();
        }

        public Round GetById(int id)
        {
            throw new NotImplementedException();
        }

        [TestInitialize]
        public void Initialize()
        {
            _manager = new Services.TournamentManager(ads, this);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [DataRow(10, 11, 12, 13)]
        [DataRow(10, 13, 12, 11)]
        [DataRow(10, 13, 11, 12)]
        [DataRow(12, 11, 13, 10)]
        public void NoTiesOrderedCorrectly(int p1, int p2, int p3, int p4)
        {
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p1,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p2,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p3,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p4,
            });

            Assert.IsNotNull(_invokedRound);

            var standings = _invokedRound.Standings.ToList();

            Assert.AreEqual(standings[0].Percentage, 13);
            Assert.AreEqual(standings[1].Percentage, 12);
            Assert.AreEqual(standings[2].Percentage, 11);
            Assert.AreEqual(standings[3].Percentage, 10);
            Assert.AreEqual(standings[0].Score, 4);
            Assert.AreEqual(standings[1].Score, 3);
            Assert.AreEqual(standings[2].Score, 2);
            Assert.AreEqual(standings[3].Score, 1);
        }

        [TestMethod]
        [DataRow(11, 11, 12, 12)]
        [DataRow(12, 11, 12, 11)]
        public void TwoTie(int p1, int p2, int p3, int p4)
        {
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p1,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p2,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p3,
            });
            _manager.OnNewStanding(new Standing()
            {
                Percentage = p4,
            });

            Assert.IsNotNull(_invokedRound);

            var standings = _invokedRound.Standings.ToList();

            Assert.AreEqual(standings[0].Percentage, 12);
            Assert.AreEqual(standings[1].Percentage, 12);
            Assert.AreEqual(standings[2].Percentage, 11);
            Assert.AreEqual(standings[3].Percentage, 11);
            Assert.AreEqual(standings[0].Score, 4);
            Assert.AreEqual(standings[1].Score, 4);
            Assert.AreEqual(standings[2].Score, 3);
            Assert.AreEqual(standings[3].Score, 3);
        }

        Round _invokedRound = null;
        public void Update(Round entity)
        {
            _invokedRound = entity;
        }
    }
}