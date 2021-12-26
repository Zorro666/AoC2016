using System;

/*

--- Day 1: No Time for a Taxicab ---

Santa's sleigh uses a very high-precision clock to guide its movements, and the clock's oscillator is regulated by stars. Unfortunately, the stars have been stolen... by the Easter Bunny. To save Christmas, Santa needs you to retrieve all fifty stars by December 25th.

Collect stars by solving puzzles. Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first. Each puzzle grants one star. Good luck!

You're airdropped near Easter Bunny Headquarters in a city somewhere. "Near", unfortunately, is as close as you can get - the instructions on the Easter Bunny Recruiting Document the Elves intercepted start here, and nobody had time to work them out further.

The Document indicates that you should start at the given coordinates (where you just landed) and face North. 
Then, follow the provided sequence: either turn left (L) or right (R) 90 degrees, then walk forward the given number of blocks, ending at a new intersection.

There's no time to follow such ridiculous instructions on foot, though, so you take a moment and work out the destination. Given that you can only walk on the street grid of the city, how far is the shortest path to the destination?

For example:

Following R2, L3 leaves you 2 blocks East and 3 blocks North, or 5 blocks away.
R2, R2, R2 leaves you 2 blocks due South of your starting position, which is 2 blocks away.
R5, L5, R5, R3 leaves you 12 blocks away.
How many blocks away is Easter Bunny HQ?

252

--- Part Two ---

Then, you notice the instructions continue on the back of the Recruiting Document. Easter Bunny HQ is actually at the first location you visit twice.

For example, if your instructions are R8, R4, R4, R8, the first location you visit twice is 4 blocks away, due East.

How many blocks away is the first location you visit twice?

*/

namespace Day01
{
    class Program
    {
        static readonly long MAX_MAP_SIZE = 500;
        static long sX;
        static long sY;
        static long sDX;
        static long sDY;
        static bool[] sVisited;
        static long sHQdistance;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            ParseLines(lines);

            if (part1)
            {
                long result1 = Distance();
                Console.WriteLine($"Day01 : Result1 {result1}");
                long expected = 253;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = sHQdistance;
                Console.WriteLine($"Day01 : Result2 {result2}");
                long expected = 126;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void ParseLines(string[] lines)
        {
            sVisited = new bool[MAX_MAP_SIZE * MAX_MAP_SIZE * 4];
            sX = 0;
            sY = 0;
            sDX = 0;
            sDY = 1;
            sHQdistance = -1;
            foreach (var line in lines)
            {
                ParseMoves(line);
            }
        }

        static void ParseMoves(string line)
        {
            var moves = line.Split(',');
            foreach (var m in moves)
            {
                var move = m.Trim();
                var operation = move[0];
                if (operation == 'L')
                {
                    // North 0,1 => West -1,0 => South 0,-1 => East 1,0 => North 0,1
                    var oldDX = sDX;
                    var oldDY = sDY;
                    sDX = -oldDY;
                    sDY = oldDX;
                }
                else if (operation == 'R')
                {
                    // North 0,1 => East 1,0 => South 0,-1 => West -1,0 => North 0,1
                    var oldDX = sDX;
                    var oldDY = sDY;
                    sDX = oldDY;
                    sDY = -oldDX;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown operation '{operation}'");
                }
                var distance = int.Parse(move.Substring(1));
                for (var i = 0; i < distance; ++i)
                {
                    sX += sDX;
                    sY += sDY;
                    if (Math.Abs(sX) > MAX_MAP_SIZE)
                    {
                        throw new InvalidProgramException($"sX is too large {sX} max {MAX_MAP_SIZE}");
                    }
                    if (Math.Abs(sY) > MAX_MAP_SIZE)
                    {
                        throw new InvalidProgramException($"sY is too large {sY} max {MAX_MAP_SIZE}");
                    }
                    var index = (sX + MAX_MAP_SIZE) * (MAX_MAP_SIZE * 2) + (sY + MAX_MAP_SIZE);
                    if (sVisited[index])
                    {
                        if (sHQdistance < 0)
                        {
                            sHQdistance = Distance();
                        }
                    }
                    sVisited[index] = true;
                }
            }
        }

        public static long Distance()
        {
            return Math.Abs(sX) + Math.Abs(sY);
        }

        public static long HQDistance()
        {
            return sHQdistance;
        }

        public static void Run()
        {
            Console.WriteLine("Day01 : Start");
            _ = new Program("Day01/input.txt", true);
            _ = new Program("Day01/input.txt", false);
            Console.WriteLine("Day01 : End");
        }
    }
}
