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
        public void SupportsTCP(string ip, bool expected)
        {
            Assert.That(Program.SupportsTCP(ip), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("aba[bab]xyz", true)]
        [TestCase("xyx[xyx]xyx", false)]
        [TestCase("aaa[kek]eke", true)]
        [TestCase("zazbz[bzb]cdb", true)]
        [TestCase("azb[bzb]cdb[aza]zbz", true)]
        public void SupportsSSL(string ip, bool expected)
        {
            Assert.That(Program.SupportsSSL(ip), Is.EqualTo(expected));
        }
    }
}
