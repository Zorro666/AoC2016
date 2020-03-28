using NUnit.Framework;

namespace Day01
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("R2, L3", 5)]
        [TestCase("R2, R2, R2", 2)]
        [TestCase("R5, L5, R5, R3", 12)]
        public void DistanceTravelled(string moves, int expectedDistance)
        {
            Program.ParseLines(new string[] { moves });
            Assert.That(Program.Distance(), Is.EqualTo(expectedDistance));
        }

        [Test]
        [TestCase("R8, R4, R4, R8", 4)]
        public void HQDistance(string moves, int expectedHQdistance)
        {
            Program.ParseLines(new string[] { moves });
            Assert.That(Program.HQDistance(), Is.EqualTo(expectedHQdistance));
        }
    }
}
