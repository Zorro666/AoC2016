using NUnit.Framework;

namespace Day15
{
    [TestFixture]
    public class Tests
    {
        public static string[] testInput = new string[] {
"Disc #1 has 5 positions; at time=0, it is at position 4.",
"Disc #2 has 2 positions; at time=0, it is at position 1."
    };

        public static TestCaseData[] CapsuleTestCases = new TestCaseData[] {
            new TestCaseData(testInput, 0, false).SetName("CapsuleEscaped 0 false"),
            new TestCaseData(testInput, 1, false).SetName("CapsuleEscaped 1 false"),
            new TestCaseData(testInput, 2, false).SetName("CapsuleEscaped 2 false"),
            new TestCaseData(testInput, 3, false).SetName("CapsuleEscaped 3 false"),
            new TestCaseData(testInput, 4, false).SetName("CapsuleEscaped 4 false"),
            new TestCaseData(testInput, 5, true).SetName("CapsuleEscaped 5 true"),
            new TestCaseData(testInput, 6, false).SetName("CapsuleEscaped 6 false"),
        };

        [Test]
        [TestCaseSource("CapsuleTestCases")]
        public void CapsuleEscaped(string[] input, int time, bool expected)
        {
            Program.Parse(input);
            Assert.That(Program.CapsuleEscaped(time), Is.EqualTo(expected));
        }
    }
}
