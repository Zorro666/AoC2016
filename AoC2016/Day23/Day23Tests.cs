using NUnit.Framework;

namespace Day23
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"cpy 2 a",
"tgl a",
"tgl a",
"tgl a",
"cpy 1 a",
"dec a",
"dec a" }, 3, TestName = "TestProgram With tgl A = 3")]
        [TestCase(new string[] {
"cpy 41 a",
"inc a",
"inc a",
"dec a",
"jnz a 2",
"dec a" }, 42, TestName = "TestProgram No tgl A = 42")]
        public void TestCode(string[] code, int expectedA)
        {
            Program.Parse(code);
            Program.RunProgram();
            Assert.That(Program.A, Is.EqualTo(expectedA));
        }
    }
}
