using NUnit.Framework;

namespace Day10
{
    [TestFixture]
    public class Tests
    {
        public static string[] testInstructions = new string[] {
"value 5 goes to bot 2",
"bot 2 gives low to bot 1 and high to bot 0",
"value 3 goes to bot 1",
"bot 1 gives low to output 1 and high to bot 0",
"bot 0 gives low to output 2 and high to output 0",
"value 2 goes to bot 2"
        };

        public static TestCaseData[] TestOutputValueCases = new TestCaseData[] {
            new TestCaseData(testInstructions, 0, 5).SetName("Test OutputValue 0 = 5"),
            new TestCaseData(testInstructions, 1, 2).SetName("Test OutputValue 1 = 2"),
            new TestCaseData(testInstructions, 2, 3).SetName("Test OutputValue 2 = 3")
        };

        [Test]
        [TestCaseSource("TestOutputValueCases")]
        public void Day10(string[] instructions, int outputBin, int expectedValue)
        {
            Program.Parse(instructions);
            Assert.That(Program.OutputValue(outputBin), Is.EqualTo(expectedValue));
        }

        public static TestCaseData[] TestBotComparingCases = new TestCaseData[] {
            new TestCaseData(testInstructions, 5, 2, 2).SetName("Test CompariingBot (5,2) = 2"),
        };
        [Test]
        [TestCaseSource("TestBotComparingCases")]
        public void Day10(string[] instructions, int a, int b, int expectedBot)
        {
            Program.Parse(instructions);
            Assert.That(Program.ComparingBot(a, b), Is.EqualTo(expectedBot));
        }
    }
}
