using NUnit.Framework;

namespace Day21
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("swap position 4 with position 0", "abcde", "ebcda")]
        [TestCase("swap letter d with letter b", "ebcda", "edcba")]
        [TestCase("reverse positions 0 through 4", "edcba", "abcde")]
        [TestCase("rotate left 1 step", "abcde", "bcdea")]
        [TestCase("move position 1 to position 4", "bcdea", "bdeac")]
        [TestCase("move position 3 to position 0", "bdeac", "abdec")]
        [TestCase("rotate based on position of letter b", "abdec", "ecabd")]
        [TestCase("rotate based on position of letter d", "ecabd", "decab")]
        public void ScrambleOperation(string operation, string input, string expectedResult)
        {
            Assert.That(Program.Operation(operation, input), Is.EqualTo(expectedResult));
        }
    }
}
