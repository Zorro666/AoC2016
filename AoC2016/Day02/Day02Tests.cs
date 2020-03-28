using System.Collections.Generic;
using NUnit.Framework;

namespace Day02
{
    [TestFixture]
    public class Tests
    {
        static string[] testCodes = new string[] {
"ULL",
"RRDDD",
"LURDL",
"UUUUD"
        };

        [Test]
        [TestCase("ULL", 1)]
        [TestCase("RRDDD", 9)]
        [TestCase("LURDL", 4)]
        [TestCase("UUUUD", 5)]
        public void KeyCode(string code, int expectedKey)
        {
            Assert.That(Program.KeyCode(new string[] { code }), Is.EqualTo(expectedKey));
        }

        public static IEnumerable<TestCaseData> TestCodeCases = new[]
        {
            new TestCaseData(testCodes, 1985).SetName("TestCode 1985")
        };

        [Test]
        [TestCaseSource("TestCodeCases")]
        public void KeyCode(string[] codes, int expectedCode)
        {
            Assert.That(Program.KeyCode(codes), Is.EqualTo(expectedCode));
        }

        public static IEnumerable<TestCaseData> TestCode5x5Cases = new[]
        {
            new TestCaseData(testCodes, "5DB3").SetName("TestCode5x5 5DB3")
        };

        [Test]
        [TestCaseSource("TestCode5x5Cases")]
        public void KeyCode5x5(string[] codes, string expectedCode)
        {
            Assert.That(Program.KeyCode5x5(codes), Is.EqualTo(expectedCode));
        }
    }
}
