using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/*

--- Day 17: Two Steps Forward ---

You're trying to access a secure vault protected by a 4x4 grid of small rooms connected by doors. 
You start in the top-left room (marked S), and you can access the vault (marked V) once you reach the bottom-right room:

#########
#S| | | #
#-#-#-#-#
# | | | #
#-#-#-#-#
# | | | #
#-#-#-#-#
# | | |  
####### V

Fixed walls are marked with #, and doors are marked with - or |.

The doors in your current room are either open or closed (and locked) based on the hexadecimal MD5 hash of a passcode (your puzzle input) followed by a sequence of uppercase characters representing the path you have taken so far (U for up, D for down, L for left, and R for right).

Only the first four characters of the hash are used; they represent, respectively, the doors up, down, left, and right from your current position. 
Any b, c, d, e, or f means that the corresponding door is open; any other character (any number or a) means that the corresponding door is closed and locked.

To access the vault, all you need to do is reach the bottom-right room; reaching this room opens the vault and all doors in the maze.

For example, suppose the passcode is hijkl. 
Initially, you have taken no steps, and so your path is empty: you simply find the MD5 hash of hijkl alone. 
The first four characters of this hash are ced9, which indicate that up is open (c), down is open (e), left is open (d), and right is closed and locked (9). 
Because you start in the top-left corner, there are no "up" or "left" doors to be open, so your only choice is down.

Next, having gone only one step (down, or D), you find the hash of hijklD. 
This produces f2bc, which indicates that you can go back up, left (but that's a wall), or right. 
Going right means hashing hijklDR to get 5745 - all doors closed and locked. 
However, going up instead is worthwhile: even though it returns you to the room you started in, your path would then be DU, opening a different set of doors.

After going DU (and then hashing hijklDU to get 528e), only the right door is open; after going DUR, all doors lock. 
(Fortunately, your actual passcode is not hijkl).

Passcodes actually used by Easter Bunny Vault Security do allow access to the vault if you know the right path. 

For example:

If your passcode were ihgpwlah, the shortest path would be DDRRRD.
With kglvqrro, the shortest path would be DDUDRLRRUDRD.
With ulqzkmiv, the shortest would be DRURDRUDDLLDLUURRDULRLDUUDDDRR.
Given your vault's passcode, what is the shortest path (the actual path, not just the length) to reach the vault?

Your puzzle input is mmsxrhfx.

Your puzzle answer was RLDUDRDDRR.

--- Part Two ---

You're curious how robust this security solution really is, and so you decide to find longer and longer paths which still provide access to the vault. You remember that paths always end the first time they reach the bottom-right room (that is, they can never pass through it, only end in it).

For example:

If your passcode were ihgpwlah, the longest path would take 370 steps.
With kglvqrro, the longest path would be 492 steps long.
With ulqzkmiv, the longest path would be 830 steps long.
What is the length of the longest path that reaches the vault?

*/

namespace Day17
{
    class Program
    {
        struct Node
        {
            public int x;
            public int y;
            public string path;
            public int parentNodeIndex;
        };

        static readonly int MAX_NUM_NODES = 100000;
        static Node[] sNodes;
        static List<int>[] sLinks;
        static string sPasscode;
        static MD5 sMD5;
        static int sNextNodeIndex;
        static readonly Dictionary<string, string> sComputedMD5s = new Dictionary<string, string>(500000);

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Input should be one line long {lines.Length}");
            }
            var passcode = lines[0];
            if (part1)
            {
                var result1 = ShortestPath(passcode);
                Console.WriteLine($"Day17 : Result1 {result1}");
                var expected = "RLDUDRDDRR";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = LongestSteps(passcode);
                Console.WriteLine($"Day17 : Result2 {result2}");
                var expected = 590;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static string GetHashFromPath(string path)
        {
            if (sComputedMD5s.TryGetValue(path, out string hash))
            {
                return hash;
            }
            var totalKey = $"{sPasscode}{path}";
            byte[] inputBytes = Encoding.ASCII.GetBytes(totalKey);
            byte[] asciiBytes = new byte[32];
            byte[] hashBytes = sMD5.ComputeHash(inputBytes);
            for (var j = 0; j < hashBytes.Length; ++j)
            {
                var sourceByte = hashBytes[j];
                byte c1 = (byte)((sourceByte >> 4) & 0xF);
                byte c2 = (byte)((sourceByte >> 0) & 0xF);
                if (c1 > 9)
                {
                    c1 += (byte)'a' - 10;
                }
                else
                {
                    c1 += (byte)'0';
                }
                if (c2 > 9)
                {
                    c2 += (byte)'a' - 10;
                }
                else
                {
                    c2 += (byte)'0';
                }
                asciiBytes[j * 2 + 0] = c1;
                asciiBytes[j * 2 + 1] = c2;
            }
            string hashString = "";
            for (var i = 0; i < asciiBytes.Length; ++i)
            {
                hashString += (char)asciiBytes[i];
            }
            sComputedMD5s[path] = hashString;

            return hashString;
        }

        static bool IsDoorOpen(string hash, int door)
        {
            var c = hash[door];
            return (c >= 'b') && (c <= 'f');
        }

        static int AddNode(int x, int y, string path, int parentNodeIndex)
        {
            if ((x < 0) || (x > 3))
            {
                return -1;
            }
            if ((y < 0) || (y > 3))
            {
                return -1;
            }
            if (sNextNodeIndex == MAX_NUM_NODES)
            {
                throw new InvalidProgramException($"Ran out of nodes {MAX_NUM_NODES}");
            }
            var nodeIndex = sNextNodeIndex;
            ref var node = ref sNodes[nodeIndex];
            node.x = x;
            node.y = y;
            node.path = path;
            node.parentNodeIndex = parentNodeIndex;
            ++sNextNodeIndex;

            return nodeIndex;
        }

        static void AddLink(int x, int y, string path, int parentNodeIndex)
        {
            var linkIndex = AddNode(x, y, path, parentNodeIndex);
            if (linkIndex >= 0)
            {
                sLinks[parentNodeIndex].Add(linkIndex);
            }
        }

        static void AddLinksFromNode(int nodeIndex)
        {
            if (sLinks[nodeIndex] != null)
            {
                return;
            }

            var node = sNodes[nodeIndex];
            var path = node.path;
            var x = node.x;
            var y = node.y;
            var hash = GetHashFromPath(path);

            //Only the first four characters of the hash are used; they represent, respectively, the doors up, down, left, and right from your current position. 
            //Any b, c, d, e, or f means that the corresponding door is open; any other character (any number or a) means that the corresponding door is closed and locked.
            var upOpen = IsDoorOpen(hash, 0);
            var downOpen = IsDoorOpen(hash, 1);
            var leftOpen = IsDoorOpen(hash, 2);
            var rightOpen = IsDoorOpen(hash, 3);
            sLinks[nodeIndex] = new List<int>();
            if (upOpen)
            {
                var upPath = path + "U";
                AddLink(x + 0, y - 1, upPath, nodeIndex);
            }
            if (downOpen)
            {
                var downPath = path + "D";
                AddLink(x + 0, y + 1, downPath, nodeIndex);
            }
            if (leftOpen)
            {
                var leftPath = path + "L";
                AddLink(x - 1, y + 0, leftPath, nodeIndex);
            }
            if (rightOpen)
            {
                var rightPath = path + "R";
                AddLink(x + 1, y + 0, rightPath, nodeIndex);
            }
        }

        public static string ShortestPath(string passcode)
        {
            sMD5 = MD5.Create();
            sComputedMD5s.Clear();
            sPasscode = passcode;
            sNextNodeIndex = 0;
            sNodes = new Node[MAX_NUM_NODES];
            sLinks = new List<int>[MAX_NUM_NODES];

            var nodeIndex = AddNode(0, 0, "", -1);
            AddLinksFromNode(nodeIndex);
            return FindShortestRoute(nodeIndex);
        }

        static string FindShortestRoute(int startNodeIndex)
        {
            Queue<int> nodesToVisit = new Queue<int>();
            nodesToVisit.Enqueue(startNodeIndex);
            List<int> visited = new List<int>(sNodes.Length * 100);
            Dictionary<int, int> parents = new Dictionary<int, int>(sNodes.Length * 100);
            var minNumSteps = int.MaxValue;
            var shortestRoute = "";
            while (nodesToVisit.Count > 0)
            {
                var nodeIndex = nodesToVisit.Dequeue();
                var x = sNodes[nodeIndex].x;
                var y = sNodes[nodeIndex].y;
                var foundEnd = (x == 3) && (y == 3);
                //Console.WriteLine($"Node:{nodeIndex} {x},{y}");
                if (foundEnd)
                {
                    int numSteps = 0;
                    int currentNode = nodeIndex;
                    bool foundParent = true;
                    while (foundParent)
                    {
                        //(x, y) = GetXYFromNodeIndex(currentNodeIndex);
                        //Console.WriteLine($"Node:{currentNodeIndex} {x},{y}");
                        numSteps++;
                        foundParent = parents.TryGetValue(currentNode, out var parentNode);
                        if (parentNode == startNodeIndex)
                        {
                            break;
                        }
                        currentNode = parentNode;
                    }
                    //Console.WriteLine($"Solved numSteps:{numSteps}");
                    if (numSteps < minNumSteps)
                    {
                        shortestRoute = sNodes[nodeIndex].path;
                        minNumSteps = numSteps;
                    }
                }

                if (!foundEnd)
                {
                    if (sLinks[nodeIndex] == null)
                    {
                        AddLinksFromNode(nodeIndex);
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
                }
            };

            if (minNumSteps < int.MaxValue)
            {
                return shortestRoute;
            }
            return null;
        }

        public static int LongestSteps(string passcode)
        {
            sMD5 = MD5.Create();
            sComputedMD5s.Clear();
            sPasscode = passcode;
            sNextNodeIndex = 0;
            sNodes = new Node[MAX_NUM_NODES];
            sLinks = new List<int>[MAX_NUM_NODES];

            var nodeIndex = AddNode(0, 0, "", -1);
            AddLinksFromNode(nodeIndex);
            return FindLongestPath(nodeIndex);
        }

        static int FindLongestPath(int startNodeIndex)
        {
            Queue<int> nodesToVisit = new Queue<int>();
            nodesToVisit.Enqueue(startNodeIndex);
            List<int> visited = new List<int>(sNodes.Length * 100);
            Dictionary<int, int> parents = new Dictionary<int, int>(sNodes.Length * 100);
            var maxNumSteps = int.MinValue;
            var longestRoute = "";
            while (nodesToVisit.Count > 0)
            {
                var nodeIndex = nodesToVisit.Dequeue();
                var x = sNodes[nodeIndex].x;
                var y = sNodes[nodeIndex].y;
                var foundEnd = (x == 3) && (y == 3);
                //Console.WriteLine($"Node:{nodeIndex} {x},{y}");
                if (foundEnd)
                {
                    int numSteps = 0;
                    int currentNode = nodeIndex;
                    bool foundParent = true;
                    while (foundParent)
                    {
                        //(x, y) = GetXYFromNodeIndex(currentNodeIndex);
                        //Console.WriteLine($"Node:{currentNodeIndex} {x},{y}");
                        numSteps++;
                        foundParent = parents.TryGetValue(currentNode, out var parentNode);
                        if (parentNode == startNodeIndex)
                        {
                            break;
                        }
                        currentNode = parentNode;
                    }
                    //Console.WriteLine($"Solved numSteps:{numSteps}");
                    if (numSteps > maxNumSteps)
                    {
                        longestRoute = sNodes[nodeIndex].path;
                        maxNumSteps = numSteps;
                    }
                }

                if (!foundEnd)
                {
                    if (sLinks[nodeIndex] == null)
                    {
                        AddLinksFromNode(nodeIndex);
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
                }
            };

            if (maxNumSteps > int.MinValue)
            {
                return maxNumSteps;
            }
            return -1;
        }

        public static void Run()
        {
            Console.WriteLine("Day17 : Start");
            _ = new Program("Day17/input.txt", true);
            _ = new Program("Day17/input.txt", false);
            Console.WriteLine("Day17 : End");
        }
    }
}
