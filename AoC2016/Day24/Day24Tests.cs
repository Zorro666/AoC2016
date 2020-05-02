using NUnit.Framework;

namespace Day24
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"###########",
"#0.1.....2#",
"#.#######.#",
"#4.......3#",
"###########" }, 14, TestName = "ShortestSteps 14")]
        public void ShortestSteps(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.ShortestSteps, Is.EqualTo(expected));
        }
    }
}
