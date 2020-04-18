using NUnit.Framework;

namespace Day14
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("abc", 1, 0, 39)]
        [TestCase("abc", 2, 0, 92)]
        [TestCase("abc", 64, 0, 22728)]
        [TestCase("abc", 1, 2016, 10)]
        [TestCase("abc", 64, 2016, 22551)]
        public void Day14(string salt, int keyCount, int stretchingCount, int expectedIndex)
        {
            Program.Parse(new string[] { salt });
            Assert.That(Program.FindIndex(keyCount, stretchingCount), Is.EqualTo(expectedIndex));
        }
    }
}
