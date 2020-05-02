using NUnit.Framework;

namespace Day22
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
@"root@ebhq-gridcenter# df -h",
@"Filesystem              Size  Used  Avail  Use%",
@"/dev/grid/node-x0-y0     92T   68T    24T   73%",
@"/dev/grid/node-x0-y1     87T   73T    14T   83%",
@"/dev/grid/node-x0-y2     89T   64T    25T   71%",
@"/dev/grid/node-x0-y3     91T   64T    27T   70%",
@"/dev/grid/node-x0-y4     86T   73T    13T   84%",
@"/dev/grid/node-x0-y5     90T   71T    19T   78%",
@"/dev/grid/node-x0-y6     88T   66T    22T   75%",
@"/dev/grid/node-x1-y0     88T    6T    22T   75%"
        }, 7, TestName = "ViablePairs 7")]
        public void ViablePairs(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.CountViablePairs(), Is.EqualTo(expected));
        }

        [TestCase(new string[] {
@"root@ebhq-gridcenter# df -h",
@"Filesystem              Size  Used  Avail  Use%",
@"/dev/grid/node-x0-y0   10T    8T     2T   80%",
@"/dev/grid/node-x0-y1   11T    6T     5T   54%",
@"/dev/grid/node-x0-y2   32T   28T     4T   87%",
@"/dev/grid/node-x1-y0    9T    7T     2T   77%",
@"/dev/grid/node-x1-y1    8T    0T     8T    0%",
@"/dev/grid/node-x1-y2   11T    7T     4T   63%",
@"/dev/grid/node-x2-y0   10T    6T     4T   60%",
@"/dev/grid/node-x2-y1    9T    8T     1T   88%",
@"/dev/grid/node-x2-y2    9T    6T     3T   66%"
        }, 7, TestName = "ShortestSteps 7")]
        public void ShortestSteps(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.ShortestSteps(), Is.EqualTo(expected));
        }
    }
}
