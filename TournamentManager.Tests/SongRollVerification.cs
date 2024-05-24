using Moq;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Tests
{
    public static class SongRollExtension
    {
        public static Phase AddMatch(this Phase p, int matchId, int[] songsInMatch)
        {
            var match = new DbModels.Match()
            {
                Id = matchId,
                SongInMatches = new List<SongInMatch>()

            };

            foreach (int song in songsInMatch)
                match.SongInMatches.Add(new SongInMatch() { MatchId = matchId, SongId = song });

            p.Matches.Add(match);

            return p;
        }
    }

    [TestClass]
    public class SongRollVerification
    {
        [TestMethod]
        public void AvailableAndBannedSongs_WorksCorrectly()
        {
            Mock<IGenericRepository<Song>> _mock = new Mock<IGenericRepository<Song>>();
            Phase phase = new Phase()
            {
                Id = 0,
                Name = "First",
                Matches = new List<DbModels.Match>()
            };

            phase
                .AddMatch(0, [0])
                .AddMatch(1, [1, 2]);

            _mock
                .Setup(s => s.GetAll(true))
                .Returns(TestUtils.SongsInMatch);

            var banned = phase.GetBannedSongs();

            Assert.IsTrue(banned.Count == 3);
            Assert.IsTrue(banned.Contains(0));
            Assert.IsTrue(banned.Contains(1));
            Assert.IsTrue(banned.Contains(2));

            var availble = _mock.Object.GetAvailableSong(phase, null);

            Assert.IsTrue(availble.Count == 2);
            Assert.IsTrue(availble.Contains(3));
            Assert.IsTrue(availble.Contains(4));
        }

        [TestMethod]
        public void GroupsFiltered_Correctly()
        {
            IEnumerable<string> packs = TestUtils.SongsInMatch.ListGroups();

            Assert.AreEqual("g1", packs.ElementAt(0));
            Assert.AreEqual("g2", packs.ElementAt(1));
        }

        [TestMethod]
        public void AvailableAndBannedSongsByGroup_WorksCorrectly()
        {
            Mock<IGenericRepository<Song>> _mock = new Mock<IGenericRepository<Song>>();
            Phase phase = new Phase()
            {
                Id = 0,
                Name = "First",
                Matches = new List<DbModels.Match>()
            };

            phase
                .AddMatch(0, [0])
                .AddMatch(1, [2]);

            _mock
                .Setup(s => s.GetAll(true))
                .Returns(TestUtils.SongsInMatch);

            var banned = phase.GetBannedSongs();

            Assert.IsTrue(banned.Count == 2);
            Assert.IsTrue(banned.Contains(0));
            Assert.IsTrue(banned.Contains(2));

            var availble = _mock.Object.GetAvailableSong(phase, "g2");

            Assert.IsTrue(availble.Count == 2);
            Assert.IsTrue(availble.Contains(3));
            Assert.IsTrue(availble.Contains(4));
        }
    }
}