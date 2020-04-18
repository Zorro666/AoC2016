using NUnit.Framework;

namespace Day14
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("abc", 22768)]
        public void Day14(string salt, int expectedIndex)
        {
            Program.Parse(new string[] { salt });
            Assert.That(Program.FindIndex(64), Is.EqualTo(expectedIndex));
        }
    }
}
