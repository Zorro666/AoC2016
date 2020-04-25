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
        [TestCase("rotate right 1 step", "abcde", "eabcd")]
        [TestCase("move position 1 to position 4", "bcdea", "bdeac")]
        [TestCase("move position 3 to position 0", "bdeac", "abdec")]
        [TestCase("rotate based on position of letter b", "abdec", "ecabd")]
        [TestCase("rotate based on position of letter d", "ecabd", "decab")]
        public void Scramble(string operation, string input, string expectedResult)
        {
            Program.Parse(new string[] { operation });
            var scrambled = new char[input.Length];
            Program.Scramble(input.ToCharArray(), ref scrambled);
            Assert.That(new string(scrambled), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("swap position 4 with position 0", "abcde")]
        [TestCase("swap letter d with letter b", "ebcda")]
        [TestCase("reverse positions 0 through 4", "edcba")]
        [TestCase("rotate left 1 step", "abcde")]
        [TestCase("rotate right 1 step", "abcde")]
        [TestCase("move position 1 to position 4", "bcdea")]
        [TestCase("move position 3 to position 0", "bdeac")]
        [TestCase("rotate based on position of letter b", "abdec")]
        public void UnScrambleOperation(string operation, string input)
        {
            Program.Parse(new string[] { operation });
            var scrambled = input.ToCharArray();
            var unScrambled = new char[input.Length];
            Program.Scramble(input.ToCharArray(), ref scrambled);
            Program.UnScramble(scrambled, ref unScrambled);
            Assert.That(new string(unScrambled), Is.EqualTo(input));
        }
    }
}
