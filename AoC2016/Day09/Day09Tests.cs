using NUnit.Framework;

namespace Day09
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("ADVENT", "ADVENT")]
        [TestCase("A(1x5)BC", "ABBBBBC")]
        [TestCase("(3x3)XYZ", "XYZXYZXYZ")]
        [TestCase("A(2x2)BCD(2x2)EFG", "ABCBCDEFEFG")]
        [TestCase("(6x1)(1x3)A", "(1x3)A")]
        [TestCase("X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY")]
        public void Decompressed(string compressed, string expectedString)
        {
            Assert.That(Program.Decompress(compressed), Is.EqualTo(expectedString));
        }

        [Test]
        [TestCase("ADVENT", 6)]
        [TestCase("A(1x5)BC", 7)]
        [TestCase("(3x3)XYZ", 9)]
        [TestCase("A(2x2)BCD(2x2)EFG", 11)]
        [TestCase("(6x1)(1x3)A", 6)]
        [TestCase("X(8x2)(3x3)ABCY", 18)]
        public void DecompressedLength(string compressed, int expectedLength)
        {
            _ = Program.Decompress(compressed);
            Assert.That(Program.DecompressedLength, Is.EqualTo(expectedLength));
        }
    }
}
