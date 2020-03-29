using NUnit.Framework;

namespace Day07
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("abba[mnop]qrst", true)]
        [TestCase("abcd[bddb]xyyx", false)]
        [TestCase("aaaa[qwer]tyui", false)]
        [TestCase("ioxxoj[asdfgh]zxcvbn", true)]
        public void Day07(string ip, bool expected)
        {
            Assert.That(Program.SupportsTCP(ip), Is.EqualTo(expected));
        }
    }
}
