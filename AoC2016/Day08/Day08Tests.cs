using NUnit.Framework;

namespace Day08
{
    [TestFixture]
    public class Tests
    {
        public static string[] rect3x2result = new string[]
        {
"###....",
"###....",
"......."
        };

        public static string[] rotateColumnx1by1 = new string[]
        {
"#.#....",
"###....",
".#....."
        };

        public static string[] rotateRowy0by4 = new string[]
        {
"....#.#",
"###....",
".#....."
        };

        public static TestCaseData[] TestCommandTests = new TestCaseData[]
        {
            new TestCaseData("rect 3x2", rect3x2result).SetName("rect 3x2"),
            new TestCaseData("rect 3x2;rotate column x=1 by 1", rotateColumnx1by1).SetName("rect3x2; rotate column x=1 by 1"),
            new TestCaseData("rect 3x2;rotate column x=1 by 1;rotate row y=0 by 4", rotateRowy0by4).SetName("rect 3x2; rotate column x=1 by 1;rotate row y=0 by 4")
        };

        [Test]
        [TestCaseSource("TestCommandTests")]
        public void CommandTests(string command, string[] expectedResult)
        {
            var lines = command.Split(';');
            Program.ProcessCommands(lines, 7, 3);
            Assert.That(Program.GetGrid(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("rect 3x2", 6, TestName = "rect 3x2 = 6")]
        [TestCase("rect 3x2;rotate column x=1 by 1", 6, TestName = "rect3x2; rotate column x=1 by 1 = 6")]
        [TestCase("rect 3x2;rotate column x=1 by 1;rotate row y=0 by 4", 6, TestName = "rect 3x2; rotate column x=1 by 1;rotate row y=0 by 4 = 6")]
        public void CountLitTests(string command, int expectedCount)
        {
            var lines = command.Split(';');
            Program.ProcessCommands(lines, 7, 3);
            Assert.That(Program.CountLit, Is.EqualTo(expectedCount));
        }
    }
}
