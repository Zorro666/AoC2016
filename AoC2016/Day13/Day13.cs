using System;
using System.Collections.Generic;

/*

--- Day 13: A Maze of Twisty Little Cubicles ---

You arrive at the first floor of this new building to discover a much less welcoming environment than the shiny atrium of the last one. Instead, you are in a maze of twisty little cubicles, all alike.

Every location in this area is addressed by a pair of non-negative integers (x,y). Each such coordinate is either a wall or an open space. You can't move diagonally. The cube maze starts at 0,0 and seems to extend infinitely toward positive x and y; negative values are invalid, as they represent a location outside the building. You are in a small waiting area at 1,1.

While it seems chaotic, a nearby morale-boosting poster explains, the layout is actually quite logical. You can determine whether a given x,y coordinate will be a wall or an open space using a simple system:

Find x*x + 3*x + 2*x*y + y + y*y.
Add the office designer's favorite number (your puzzle input).
Find the binary representation of that sum; count the number of bits that are 1.
If the number of bits that are 1 is even, it's an open space.
If the number of bits that are 1 is odd, it's a wall.
For example, if the office designer's favorite number were 10, drawing walls as # and open spaces as ., the corner of the building containing 0,0 would look like this:

  0123456789
0 .#.####.##
1 ..#..#...#
2 #....##...
3 ###.#.###.
4 .##..#..#.
5 ..##....#.
6 #...##.###
Now, suppose you wanted to reach 7,4. The shortest route you could take is marked as O:

  0123456789
0 .#.####.##
1 .O#..#...#
2 #OOO.##...
3 ###O#.###.
4 .##OO#OO#.
5 ..##OOO.#.
6 #...##.###
Thus, reaching 7,4 would take a minimum of 11 steps (starting from your current location, 1,1).

What is the fewest number of steps required for you to reach 31,39?

Your puzzle input is 1350.

Your puzzle answer was 92.

--- Part Two ---

How many locations (distinct x,y coordinates, including your starting location) can you reach in at most 50 steps?

*/

namespace Day13
{
    class Program
    {
        struct Node
        {
            public int x;
            public int y;
            public int type;
        };

        static readonly int MAX_MAP_SIZE = 1024;
        static readonly int[,] sMap = new int[MAX_MAP_SIZE, MAX_MAP_SIZE];
        static (int w, int h) sMapSize;
        static Node[] sNodes;
        static List<int>[] sLinks;

        static int sFavouriteNumber;
        static readonly int sStartX = 1;
        static readonly int sStartY = 1;
        static int sTargetX;
        static int sTargetY;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = MinSteps(31, 39);
                Console.WriteLine($"Day13 : Result1 {result1}");
                var expected = 92;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = FindLocations(50);
                Console.WriteLine($"Day13 : Result2 {result2}");
                var expected = 124;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length > 1)
            {
                throw new InvalidProgramException($"Invalid input only one line is supported {lines.Length}");
            }
            sFavouriteNumber = int.Parse(lines[0]);
            GenerateMap();
        }

        static bool IsWall(int x, int y)
        {
            if (x < 0)
            {
                return true;
            }
            if (y < 0)
            {
                return true;
            }
            var sum = x * x + 3 * x + 2 * x * y + y + y * y;
            sum += sFavouriteNumber;

            var countOneBits = 0;
            while (sum != 0)
            {
                ++countOneBits;
                sum &= (sum - 1);
            };
            return (countOneBits & 1) == 1;
        }

        static void GenerateMap()
        {
            for (int y = 0; y < MAX_MAP_SIZE; ++y)
            {
                for (int x = 0; x < MAX_MAP_SIZE; ++x)
                {
                    sMap[x, y] = -1;
                }
            }

            sMapSize.w = 64;
            sMapSize.h = 64;
            sNodes = new Node[sMapSize.w * sMapSize.h];
            sLinks = new List<int>[sMapSize.w * sMapSize.h];
            for (int y = 0; y < sMapSize.h; ++y)
            {
                for (int x = 0; x < sMapSize.w; ++x)
                {
                    sMap[x, y] = IsWall(x, y) ? 1 : 0;
                    var nodeIndex = GetNodeIndex(x, y);
                    sNodes[nodeIndex].x = x;
                    sNodes[nodeIndex].y = y;
                    sNodes[nodeIndex].type = sMap[x, y];
                }
            }

            for (int y = 0; y < sMapSize.h; ++y)
            {
                for (int x = 0; x < sMapSize.w; ++x)
                {
                    var nodeIndex = x + y * sMapSize.w;
                    int cell = sMap[x, y];

                    sLinks[nodeIndex] = new List<int>();
                    sLinks[nodeIndex].Clear();
                    if (cell == -1)
                    {
                        continue;
                    }
                    if (cell == 1)
                    {
                        continue;
                    }
                    if ((x - 1 >= 0) && (sMap[x - 1, y] != 1))
                    {
                        var targetNodeIndex = x - 1 + y * sMapSize.w;
                        sLinks[nodeIndex].Add(targetNodeIndex);
                    }
                    if ((x + 1 < sMapSize.w) && (sMap[x + 1, y] != 1))
                    {
                        var targetNodeIndex = x + 1 + y * sMapSize.w;
                        sLinks[nodeIndex].Add(targetNodeIndex);
                    }
                    if ((y - 1 >= 0) && (sMap[x, y - 1] != 1))
                    {
                        var targetNodeIndex = x + (y - 1) * sMapSize.w;
                        sLinks[nodeIndex].Add(targetNodeIndex);
                    }
                    if ((y + 1 < sMapSize.h) && (sMap[x, y + 1] != 1))
                    {
                        var targetNodeIndex = x + (y + 1) * sMapSize.w;
                        sLinks[nodeIndex].Add(targetNodeIndex);
                    }
                }
            }
        }

        public static void OutputMap(bool detailed)
        {
            for (int y = 0; y < sMapSize.h; ++y)
            {
                string line = "";
                for (int x = 0; x < sMapSize.w; ++x)
                {
                    int cell = sMap[x, y];
                    if ((x == sStartX) && (y == sStartY))
                    {
                        line += 'S';
                    }
                    else if ((x == sTargetX) && (y == sTargetY))
                    {
                        line += 'E';
                    }
                    else if (cell == -1)
                    {
                        line += ' ';
                    }
                    else if (cell == 0)
                    {
                        line += '.';
                    }
                    else if (cell == 1)
                    {
                        line += '#';
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown map[{x},{y}] {cell}");
                    }
                }
                Console.WriteLine($"{line}");
            }

            if (detailed)
            {
                for (int nodeIndex = 0; nodeIndex < sNodes.Length; ++nodeIndex)
                {
                    var node = sNodes[nodeIndex];
                    Console.WriteLine($"Node:{node.x},{node.y} {node.type}");
                    foreach (var linkTargetIndex in sLinks[nodeIndex])
                    {
                        (int linkX, int linkY) = GetXYFromNodeIndex(linkTargetIndex);
                        Console.WriteLine($"Link:{linkTargetIndex} {linkX},{linkY}");
                    }
                }
            }
            Console.WriteLine($"Map Dimensions:{sMapSize.w} x {sMapSize.h}");
        }

        static int GetNodeIndex(int x, int y)
        {
            if ((x < 0) || (x >= sMapSize.w))
            {
                throw new ArgumentOutOfRangeException("x", $"Invalid value {x} out of range 0-{sMapSize.w}");
            }
            if ((y < 0) || (y >= sMapSize.h))
            {
                throw new ArgumentOutOfRangeException("y", $"Invalid value {y} out of range 0-{sMapSize.h}");
            }
            return x + y * sMapSize.w;
        }

        static (int, int) GetXYFromNodeIndex(int nodeIndex)
        {
            int x = nodeIndex % sMapSize.w;
            int y = nodeIndex / sMapSize.w;
            return (x, y);
        }

        public static int MinSteps(int targetX, int targetY)
        {
            sTargetX = targetX;
            sTargetY = targetY;
            //OutputMap(false);
            var startIndex = GetNodeIndex(sStartX, sStartY);
            var endIndex = GetNodeIndex(sTargetX, sTargetY);
            return ShortestPath(startIndex, endIndex);
        }

        static int FindLocations(int maxNumSteps)
        {
            var startIndex = GetNodeIndex(sStartX, sStartY);
            var count = 0;
            for (int y = 0; y < sStartY + maxNumSteps + 1; ++y)
            {
                for (int x = 0; x < sStartX + maxNumSteps + 1; ++x)
                {
                    int cell = sMap[x, y];
                    if (cell == 0)
                    {
                        var endIndex = GetNodeIndex(x, y);
                        var stepCount = ShortestPath(startIndex, endIndex);
                        if ((stepCount >= 0) && (stepCount <= maxNumSteps))
                        {
                            //Console.WriteLine($"{x},{y} {stepCount}");
                            ++count;
                        }
                    }
                }
            }
            return count;
        }

        static int ShortestPath(int startIndex, int endIndex)
        {
            //(int fromX, int fromY) = GetXYFromNodeIndex(startIndex);
            //Console.WriteLine($"ShortestPath Start {startIndex} {fromX},{fromY}");
            Queue<int> nodesToVisit = new Queue<int>();
            nodesToVisit.Enqueue(startIndex);
            List<int> visited = new List<int>(sNodes.Length * 100);
            Dictionary<int, int> parents = new Dictionary<int, int>(sNodes.Length * 100);
            int minNumSteps = int.MaxValue;
            while (nodesToVisit.Count > 0)
            {
                var nodeIndex = nodesToVisit.Dequeue();
                //(int x, int y) = GetXYFromNodeIndex(nodeIndex);
                //Console.WriteLine($"Node:{nodeIndex} {x},{y}");
                if (nodeIndex == endIndex)
                {
                    int numSteps = 0;
                    int currentNode = endIndex;
                    bool foundParent = true;
                    while (foundParent)
                    {
                        //(x, y) = GetXYFromNodeIndex(currentNodeIndex);
                        //Console.WriteLine($"Node:{currentNodeIndex} {x},{y}");
                        numSteps++;
                        foundParent = parents.TryGetValue(currentNode, out var parentNode);
                        if (parentNode == startIndex)
                        {
                            break;
                        }
                        currentNode = parentNode;
                    }
                    //Console.WriteLine($"Solved numSteps:{numSteps}");
                    if (numSteps < minNumSteps)
                    {
                        minNumSteps = numSteps;
                    }
                }

                foreach (var link in sLinks[nodeIndex])
                {
                    if (!visited.Contains(link))
                    {
                        visited.Add(link);
                        nodesToVisit.Enqueue(link);
                        parents[link] = nodeIndex;
                    }
                }
            };

            if (minNumSteps < int.MaxValue)
            {
                return minNumSteps;
            }
            return -1;
        }

        public static void Run()
        {
            Console.WriteLine("Day13 : Start");
            _ = new Program("Day13/input.txt", true);
            _ = new Program("Day13/input.txt", false);
            Console.WriteLine("Day13 : End");
        }
    }
}
