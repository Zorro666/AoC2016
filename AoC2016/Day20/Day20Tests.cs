using NUnit.Framework;

namespace Day20
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] { "5-8", "0-2", "4-7" }, 3U)]
        public void MinUnused(string[] lines, uint expected)
        {
            Program.Parse(lines);
            Assert.That(Program.FindMin(), Is.EqualTo(expected));
        }
    }
}
