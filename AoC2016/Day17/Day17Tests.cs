using NUnit.Framework;

namespace Day17
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("ihgpwlah", "DDRRRD")]
        [TestCase("kglvqrro", "DDUDRLRRUDRD")]
        [TestCase("ulqzkmiv", "DRURDRUDDLLDLUURRDULRLDUUDDDRR")]
        public void ShortestPath(string passcode, string expectedPath)
        {
            Assert.That(Program.ShortestPath(passcode), Is.EqualTo(expectedPath));
        }

        [Test]
        [TestCase("ihgpwlah", 370)]
        [TestCase("kglvqrro", 492)]
        [TestCase("ulqzkmiv", 830)]
        public void LongestSteps(string passcode, int expectedSteps)
        {
            Assert.That(Program.LongestSteps(passcode), Is.EqualTo(expectedSteps));
        }
    }
}
