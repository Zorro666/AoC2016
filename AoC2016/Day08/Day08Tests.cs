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
            new TestCaseData("rotate column x=1 by 1", rotateColumnx1by1).SetName("rotate column x=1 by 1"),
            new TestCaseData("rotate row y=0 by 4", rotateRowy0by4).SetName("rotate row y=0 by 4")
        };

        [Test]
        [TestCaseSource("TestCommandTests")]
        public void CommandTests(string command, string[] expectedResult)
        {
            Program.ProcessCommand(command);
            Assert.That(Program.GetGrid(), Is.EqualTo(expectedResult));
        }
    }
}
