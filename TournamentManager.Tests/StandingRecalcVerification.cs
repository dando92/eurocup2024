using TournamentManager.DbModels;

namespace TournamentManager.Tests
{

    [TestClass]
    public class StandingRecalcVerification 
    {
        [TestMethod]
        [DataRow(10, 11, 12, 13)]
        [DataRow(10, 13, 12, 11)]
        [DataRow(10, 13, 11, 12)]
        [DataRow(12, 11, 13, 10)]
        public void NoTiesOrderedCorrectly(int p1, int p2, int p3, int p4)
        {
            List<Standing> standings = new List<Standing>()
            {
                new Standing() { Percentage = p1, },
                new Standing() { Percentage = p2, },
                new Standing() { Percentage = p3, },
                new Standing() { Percentage = p4, }
            };

            standings = standings.Recalc();

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
            List<Standing> standings = new List<Standing>()
            {
                new Standing() { Percentage = p1, },
                new Standing() { Percentage = p2, },
                new Standing() { Percentage = p3, },
                new Standing() { Percentage = p4, }
            };

            standings = standings.Recalc();

            Assert.AreEqual(standings[0].Percentage, 12);
            Assert.AreEqual(standings[1].Percentage, 12);
            Assert.AreEqual(standings[2].Percentage, 11);
            Assert.AreEqual(standings[3].Percentage, 11);
            Assert.AreEqual(standings[0].Score, 4);
            Assert.AreEqual(standings[1].Score, 4);
            Assert.AreEqual(standings[2].Score, 3);
            Assert.AreEqual(standings[3].Score, 3);
        }

        [TestMethod]
        [DataRow(11, 12)]
        [DataRow(12, 11)]
        public void TwoPlayer(int p1, int p2)
        {
            List<Standing> standings = new List<Standing>()
            {
                new Standing() { Percentage = p1, },
                new Standing() { Percentage = p2, }
            };

            standings = standings.Recalc();

            Assert.AreEqual(standings[0].Percentage, 12);
            Assert.AreEqual(standings[1].Percentage, 11);
            Assert.AreEqual(standings[0].Score, 2);
            Assert.AreEqual(standings[1].Score, 1);
        }

        [TestMethod]
        [DataRow(12, 12)]
        public void TwoPlayerTie(int p1, int p2)
        {
            List<Standing> standings = new List<Standing>()
            {
                new Standing() { Percentage = p1, },
                new Standing() { Percentage = p2, }
            };

            standings = standings.Recalc();

            Assert.AreEqual(standings[0].Percentage, 12);
            Assert.AreEqual(standings[1].Percentage, 12);
            Assert.AreEqual(standings[0].Score, 2);
            Assert.AreEqual(standings[1].Score, 2);
        }

    }
}