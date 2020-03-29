using NUnit.Framework;

namespace Day05
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("abc", "18f47a30")]
        public void Password(string doorID, string expected)
        {
            Assert.That(Program.Password(doorID), Is.EqualTo(expected));
        }
        [Test]
        [TestCase("abc", "05ace8e3")]
        public void PasswordComplex(string doorID, string expected)
        {
            Assert.That(Program.PasswordComplex(doorID), Is.EqualTo(expected));
        }
    }
}
