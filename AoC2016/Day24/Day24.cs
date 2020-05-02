using System;
using System.Collections.Generic;

/*

--- Day 24: Air Duct Spelunking ---

You've finally met your match; the doors that provide access to the roof are locked tight, and all of the controls and related electronics are inaccessible.
You simply can't reach them.

The robot that cleans the air ducts, however, can.

It's not a very fast little robot, but you reconfigure it to be able to interface with some of the exposed wires that have been routed through the HVAC system.
If you can direct it to each of those locations, you should be able to bypass the security controls.

You extract the duct layout for this area from some blueprints you acquired and create a map with the relevant locations marked (your puzzle input).
0 is your current location, from which the cleaning robot embarks; the other numbers are (in no particular order) the locations the robot needs to visit at least once each.
Walls are marked as #, and open passages are marked as ..
Numbers behave like open passages.

For example, suppose you have a map like the following:

###########
#0.1.....2#
#.#######.#
#4.......3#
###########
To reach all of the points of interest as quickly as possible, you would have the robot take the following path:

0 to 4 (2 steps)
4 to 1 (4 steps; it can't move diagonally)
1 to 2 (6 steps)
2 to 3 (2 steps)
Since the robot isn't very fast, you need to find it the shortest route.
This path is the fewest steps (in the above example, a total of 14) required to start at 0 and then visit every other location at least once.

Given your actual map, and starting from location 0, what is the fewest number of steps required to visit every non-0 number marked on the map at least once?

Your puzzle answer was 518.

--- Part Two ---

Of course, if you leave the cleaning robot somewhere weird, someone is bound to notice.

What is the fewest number of steps required to start at 0, visit every non-0 number marked on the map at least once, and then return to 0?

*/

namespace Day24
{
    class Program
    {
        struct Node
        {
            public int x;
            public int y;
            public int type;
        };

        static readonly int MAX_NUM_LOCATIONS = 10;
        static readonly int MAX_MAP_SIZE = 256;
        static readonly int[,] sMap = new int[MAX_MAP_SIZE, MAX_MAP_SIZE];
        static (int w, int h) sMapSize;
        static Node[] sNodes;
        static List<int>[] sLinks;
        static readonly int[] sLocations = new int[MAX_NUM_LOCATIONS];
        static readonly int[,] sDistances = new int[MAX_NUM_LOCATIONS, MAX_NUM_LOCATIONS];
        static int sLocationsCount = 0;
        static int sMinSteps;
        static bool[] sVisited = new bool[MAX_NUM_LOCATIONS];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = ShortestSteps(false);
                Console.WriteLine($"Day24 : Result1 {result1}");
                var expected = 518;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = ShortestSteps(true);
                Console.WriteLine($"Day24 : Result2 {result2}");
                var expected = 716;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 1)
            {
                throw new InvalidProgramException($"Invalid map input not enough lines {lines.Length}");
            }

            var width = lines[0].Trim().Length;
            if (width > MAX_MAP_SIZE)
            {
                throw new InvalidProgramException($"Invalid map width {width} > {MAX_MAP_SIZE}");
            }
            var height = 0;
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (trimmed.Length != width)
                {
                    throw new InvalidProgramException($"Invalid map input line width is different {trimmed.Length} != {width}");
                }
                var y = height;
                for (var x = 0; x < width; ++x)
                {
                    var c = trimmed[x];
                    int type = -666;
                    if (c == '#')
                    {
                        type = 0;
                    }
                    else if (c == '.')
                    {
                        type = 1;
                    }
                    else if ((c >= '0') || (c <= '9'))
                    {
                        type = 10 + (c - '0');
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown cell {c} at {x},{y}");
                    }
                    sMap[x, y] = type;
                }
                ++height;
                if (height > MAX_MAP_SIZE)
                {
                    throw new InvalidProgramException($"Invalid map height {height} > {MAX_MAP_SIZE}");
                }
            }

            var maxNodeCount = height * width;
            sMapSize.w = width;
            sMapSize.h = height;
            sLocationsCount = 0;
            sNodes = new Node[maxNodeCount];
            sLinks = new List<int>[maxNodeCount];

            for (var l = 0; l < MAX_NUM_LOCATIONS; ++l)
            {
                sLocations[l] = -1;
                for (var j = 0; j < MAX_NUM_LOCATIONS; ++j)
                {
                    sDistances[l, j] = -1;
                }
            }

            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    var nodeIndex = GetNodeIndex(x, y);
                    ref var node = ref sNodes[nodeIndex];
                    var type = sMap[x, y];
                    node.x = x;
                    node.y = y;
                    node.type = type;
                    if (type >= 10)
                    {
                        var location = type - 10;
                        sLocations[location] = nodeIndex;
                        sLocationsCount = Math.Max(sLocationsCount, location + 1);
                    }
                    var links = new List<int>(4);
                    int nX;
                    int nY;

                    nX = x + 1;
                    nY = y + 0;
                    if ((nX >= 0) && (nX < width) && (nY >= 0) && (nY < height) && (sMap[nX, nY] > 0))
                    {
                        links.Add(GetNodeIndex(nX, nY));
                    }
                    nX = x - 1;
                    nY = y + 0;
                    if ((nX >= 0) && (nX < width) && (nY >= 0) && (nY < height) && (sMap[nX, nY] > 0))
                    {
                        links.Add(GetNodeIndex(nX, nY));
                    }
                    nX = x + 0;
                    nY = y - 1;
                    if ((nX >= 0) && (nX < width) && (nY >= 0) && (nY < height) && (sMap[nX, nY] > 0))
                    {
                        links.Add(GetNodeIndex(nX, nY));
                    }
                    nX = x + 0;
                    nY = y + 1;
                    if ((nX >= 0) && (nX < width) && (nY >= 0) && (nY < height) && (sMap[nX, nY] > 0))
                    {
                        links.Add(GetNodeIndex(nX, nY));
                    }
                    sLinks[nodeIndex] = links;
                }
            }
            for (var l = 0; l < sLocationsCount; ++l)
            {
                if (sLocations[l] == -1)
                {
                    throw new InvalidProgramException($"Failed to find location {l} out of {sLocationsCount}");
                }
            }
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

        public static int ShortestSteps(bool returnHome)
        {
            for (var l = 0; l < sLocationsCount - 1; ++l)
            {
                var startLocation = sLocations[l];
                for (var j = l + 1; j < sLocationsCount; ++j)
                {
                    var endLocation = sLocations[j];
                    var distance = ShortestPath(startLocation, endLocation);
                    sDistances[l, j] = distance;
                    sDistances[j, l] = distance;
                }
                sVisited[l] = false;
            }

            var numSteps = 0;
            sMinSteps = int.MaxValue;
            sVisited[0] = true;
            ShortestPathImpl(0, numSteps, returnHome);
            return sMinSteps;
        }

        static void ShortestPathImpl(int start, int numSteps, bool returnHome)
        {
            for (var l = 1; l < sLocationsCount; ++l)
            {
                if (sVisited[l])
                {
                    continue;
                }
                var next = l;
                sVisited[next] = true;
                var distance = numSteps + sDistances[start, next];
                if (distance < sMinSteps)
                {
                    bool foundEnd = true;
                    for (var j = 0; j < sLocationsCount; ++j)
                    {
                        if (!sVisited[j])
                        {
                            foundEnd = false;
                            break;
                        }
                    }
                    if (foundEnd)
                    {
                        if (returnHome)
                        {
                            distance += sDistances[next, 0];
                        }
                        //Console.WriteLine($"Found goal {distance} Min:{sMinSteps}");
                        sMinSteps = Math.Min(sMinSteps, distance);
                        sVisited[next] = false;
                        return;
                    }
                    ShortestPathImpl(next, distance, returnHome);
                }
                sVisited[next] = false;
            }
        }

        static int ShortestPath(int startIndex, int endIndex)
        {
            Queue<int> nodesToVisit = new Queue<int>();
            nodesToVisit.Enqueue(startIndex);
            List<int> visited = new List<int>(sNodes.Length * 100);
            Dictionary<int, int> parents = new Dictionary<int, int>(sNodes.Length * 100);
            int minNumSteps = int.MaxValue;
            while (nodesToVisit.Count > 0)
            {
                var nodeIndex = nodesToVisit.Dequeue();
                if (nodeIndex == endIndex)
                {
                    int numSteps = 0;
                    int currentNode = endIndex;
                    bool foundParent = true;
                    while (foundParent)
                    {
                        numSteps++;
                        foundParent = parents.TryGetValue(currentNode, out var parentNode);
                        if (parentNode == startIndex)
                        {
                            break;
                        }
                        currentNode = parentNode;
                    }
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
            Console.WriteLine("Day24 : Start");
            _ = new Program("Day24/input.txt", true);
            _ = new Program("Day24/input.txt", false);
            Console.WriteLine("Day24 : End");
        }
    }
}
