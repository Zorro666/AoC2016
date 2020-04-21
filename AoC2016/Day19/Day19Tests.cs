using NUnit.Framework;

namespace Day19
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(5, false, 3)]
        [TestCase(5, true, 2)]
        public void ElfWithPresents(int elfCount, bool steal, int expectedElf)
        {
            Assert.That(Program.ElfWithPresents(elfCount, steal), Is.EqualTo(expectedElf));
        }
    }
}
