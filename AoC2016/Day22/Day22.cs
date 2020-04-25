using System;

/*

--- Day 22: Grid Computing ---

You gain access to a massive storage cluster arranged in a grid; each storage node is only connected to the four nodes directly adjacent to it (three if the node is on an edge, two if it's in a corner).

You can directly access data only on node /dev/grid/node-x0-y0, but you can perform some limited actions on the other nodes:

You can get the disk usage of all nodes (via df). 
The result of doing this is in your puzzle input.
You can instruct a node to move (not copy) all of its data to an adjacent node (if the destination node has enough space to receive the data). 
The sending node is left empty after this operation.
Nodes are named by their position: the node named node-x10-y10 is adjacent to nodes node-x9-y10, node-x11-y10, node-x10-y9, and node-x10-y11.

Before you begin, you need to understand the arrangement of data on these nodes. 
Even though you can only move data between directly connected nodes, you're going to need to rearrange a lot of the data to get access to the data you need. 
Therefore, you need to work out how you might be able to shift data around.

To do this, you'd like to count the number of viable pairs of nodes. 
A viable pair is any two nodes (A,B), regardless of whether they are directly connected, such that:

Node A is not empty (its Used is not zero).
Nodes A and B are not the same node.
The data on node A (its Used) would fit on node B (its Avail).
How many viable pairs of nodes are there?

root@ebhq-gridcenter# df -h
Filesystem              Size  Used  Avail  Use%
/dev/grid/node-x0-y0     92T   68T    24T   73%
/dev/grid/node-x0-y1     87T   73T    14T   83%
/dev/grid/node-x0-y2     89T   64T    25T   71%
/dev/grid/node-x0-y3     91T   64T    27T   70%
/dev/grid/node-x0-y4     86T   73T    13T   84%
/dev/grid/node-x0-y5     90T   71T    19T   78%
/dev/grid/node-x0-y6     88T   66T    22T   75%

*/

namespace Day22
{
    class Program
    {
        struct Node
        {
            public int X;
            public int Y;
            public int Size;
            public int Used;
            public int Avail;
        }

        static Node[] sNodes;
        static int sNodeCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = CountViablePairs();
                Console.WriteLine($"Day22 : Result1 {result1}");
                var expected = 888;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day22 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 2)
            {
                throw new InvalidProgramException($"Not enough input data need at least 3 lines {lines.Length}");
            }
            string expected;

            expected = @"root@ebhq-gridcenter# df -h";
            if (lines[0].Trim() != expected)
            {
                throw new InvalidProgramException($"Invalid first line '{lines[0]}' Expected:'{expected}'");
            }

            expected = @"Filesystem              Size  Used  Avail  Use%";
            if (lines[1].Trim() != expected)
            {
                throw new InvalidProgramException($"Invalid second line '{lines[1]}' Expected:'{expected}'");
            }

            sNodes = new Node[lines.Length - 2];
            sNodeCount = 0;
            for (var i = 2; i < lines.Length; ++i)
            {
                //"/dev/grid/node-x0-y0     92T   68T    24T   73%"
                var tokens = lines[i].Trim().Split(' ');
                var nodeName = tokens[0];

                var nextToken = 1;
                for (var j = 1; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var size = int.Parse(tokens[nextToken].TrimEnd('T'));
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var used = int.Parse(tokens[nextToken].TrimEnd('T'));
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var avail = int.Parse(tokens[nextToken].TrimEnd('T'));
                ++nextToken;
                for (var j = nextToken; j < tokens.Length; ++j)
                {
                    if (tokens[j].Trim().Length != 0)
                    {
                        nextToken = j;
                        break;
                    }
                }
                var availPercentage = int.Parse(tokens[nextToken].TrimEnd('%'));
                var nameTokens = nodeName.Split('-');
                var x = int.Parse(nameTokens[1].TrimStart('x'));
                var y = int.Parse(nameTokens[2].TrimStart('y'));
                //Console.WriteLine($"X:{x} Y:{y} {size}T {used}T {avail}T {availPercentage}%");
                ref Node node = ref sNodes[sNodeCount];
                node.X = x;
                node.Y = y;
                node.Size = size;
                node.Used = used;
                node.Avail = avail;
                ++sNodeCount;
            }
        }

        public static int CountViablePairs()
        {
            var viableCount = 0;
            for (var i = 0; i < sNodeCount; ++i)
            {
                ref Node nodeA = ref sNodes[i];
                var usedA = nodeA.Used;
                if (usedA == 0)
                {
                    continue;
                }
                for (var j = 0; j < sNodeCount; ++j)
                {
                    if (j == i)
                    {
                        continue;
                    }
                    ref Node nodeB = ref sNodes[j];
                    var availB = nodeB.Avail;
                    if (usedA < availB)
                    {
                        ++viableCount;
                    }
                }
            }
            return viableCount;
        }


        public static void Run()
        {
            Console.WriteLine("Day22 : Start");
            _ = new Program("Day22/input.txt", true);
            _ = new Program("Day22/input.txt", false);
            Console.WriteLine("Day22 : End");
        }
    }
}

