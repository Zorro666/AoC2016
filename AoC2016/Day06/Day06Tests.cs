using NUnit.Framework;

namespace Day06
{
    [TestFixture]
    public class Tests
    {
        static string[] testInputOne = new string[] {
"eedadn",
"drvtee",
"eandsr",
"raavrd",
"atevrs",
"tsrnev",
"sdttsa",
"rasrtv",
"nssdts",
"ntnada",
"svetve",
"tesnvt",
"vntsnd",
"vrdear",
"dvrsen",
"enarar"
            };

        public static TestCaseData[] TestInputCases = new TestCaseData[]
        {
            new TestCaseData(testInputOne, true, "easter").SetName("TestInput Most easter"),
            new TestCaseData(testInputOne, false, "advent").SetName("TestInput Least easter")
        };

        [Test]
        [TestCaseSource("TestInputCases")]
        public void RecoverCode(string[] lines, bool most, string expected)
        {
            Assert.That(Program.RecoverCode(lines, most), Is.EqualTo(expected));
        }
    }
}
