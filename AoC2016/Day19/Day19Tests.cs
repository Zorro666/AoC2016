using NUnit.Framework;

namespace Day19
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(5, 3)]
        public void ElfWithPresents(int elfCount, int expectedElf)
        {
            Assert.That(Program.ElfWithPresents(elfCount), Is.EqualTo(expectedElf));
        }
    }
}
