using NUnit.Framework;

namespace Day18
{
    [TestFixture]
    public class Tests
    {

        [Test]
        [TestCase("..^^.", 1, ".^^^^", TestName = "Small Row 1")]
        [TestCase("..^^.", 2, "^^..^", TestName = "Small Row 2")]
        [TestCase(".^^.^.^^^^", 1, "^^^...^..^", TestName = "Large Row 1")]
        [TestCase(".^^.^.^^^^", 2, "^.^^.^.^^.", TestName = "Large Row 2")]
        [TestCase(".^^.^.^^^^", 3, "..^^...^^^", TestName = "Large Row 3")]
        [TestCase(".^^.^.^^^^", 4, ".^^^^.^^.^", TestName = "Large Row 4")]
        [TestCase(".^^.^.^^^^", 5, "^^..^.^^..", TestName = "Large Row 5")]
        [TestCase(".^^.^.^^^^", 6, "^^^^..^^^.", TestName = "Large Row 6")]
        [TestCase(".^^.^.^^^^", 7, "^..^^^^.^^", TestName = "Large Row 7")]
        [TestCase(".^^.^.^^^^", 8, ".^^^..^.^^", TestName = "Large Row 8")]
        [TestCase(".^^.^.^^^^", 9, "^^.^^^..^^", TestName = "Large Row 9")]
        public void ComputeRow(string start, int row, string expectedRow)
        {
            Assert.That(Program.ComputeRow(start, row), Is.EqualTo(expectedRow));
        }

        [Test]
        [TestCase("..^^.", 3, 6, TestName = "Small Count 6")]
        [TestCase(".^^.^.^^^^", 10, 38, TestName = "Large Count 38")]
        public void CountSafeTiles(string start, int rowCount, string expectedSafeTiles)
        {
            Assert.That(Program.CountSafeTiles(start, rowCount), Is.EqualTo(expectedSafeTiles));
        }
    }
}
