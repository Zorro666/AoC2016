using NUnit.Framework;

namespace Day16
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("1", "100")]
        [TestCase("0", "001")]
        [TestCase("11111", "11111000000")]
        [TestCase("111100001010", "1111000010100101011110000")]
        public void DragonCurve(string input, string expected)
        {
            Assert.That(Program.DragonCurve(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("110010110100", "100")]
        public void Checksum(string input, string expected)
        {
            Assert.That(Program.Checksum(input), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("10000", 20, "01100")]
        public void FillDisk(string start, int diskSize, string expected)
        {
            Assert.That(Program.FillDisk(start, diskSize), Is.EqualTo(expected));
        }
    }
}
