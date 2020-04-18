using NUnit.Framework;

namespace Day13
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("10", 11)]
        public void MinSteps(string input, int expected)
        {
            Program.Parse(new string[] { input });
            Assert.That(Program.MinSteps(7, 4), Is.EqualTo(expected));
        }
    }
}
