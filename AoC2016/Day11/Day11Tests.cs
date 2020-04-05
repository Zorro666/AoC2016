using NUnit.Framework;

namespace Day11
{
    [TestFixture]
    public class Tests
    {
        public static string[] testSetup = new string[] {
"The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.",
"The second floor contains a hydrogen generator.",
"The third floor contains a lithium generator.",
"The fourth floor contains nothing relevant."
        };

        public static TestCaseData[] TestSetupCases = new TestCaseData[] {
            new TestCaseData(testSetup, 11).SetName("TestSetup = 11")
        };

        [Test]
        [TestCaseSource("TestSetupCases")]
        public void Day11(string[] lines, int expectedMinMoves)
        {
            Program.Parse(lines);
            Assert.That(Program.MinimumMoves, Is.EqualTo(expectedMinMoves));
        }
    }
}
