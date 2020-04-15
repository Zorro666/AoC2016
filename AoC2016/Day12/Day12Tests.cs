using NUnit.Framework;

namespace Day12
{
    [TestFixture]
    public class Tests
    {
        public static string[] testProgram = new string[] {
"cpy 41 a",
"inc a",
"inc a",
"dec a",
"jnz a 2",
"dec a"
        };

        public static TestCaseData[] TestProgramCases = new TestCaseData[] {
            new TestCaseData(testProgram, 42).SetName("TestProgram a=42")
        };

        [Test]
        [TestCaseSource("TestProgramCases")]
        public void TestCode(string[] code, int expectedA)
        {
            Program.Parse(code);
            Program.RunProgram();
            Assert.That(Program.A, Is.EqualTo(expectedA));
        }
    }
}
