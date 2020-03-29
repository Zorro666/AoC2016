using NUnit.Framework;

namespace Day03
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("  5  10  25", 0)]
        public void ValidTriangles(string triangle, int expectedValidCount)
        {
            Program.Parse(new string[] { triangle });
            Assert.That(Program.CountValidTriangles(), Is.EqualTo(expectedValidCount));
        }
    }
}
