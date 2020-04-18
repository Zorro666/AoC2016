using System;

/*
--- Day 15: Timing is Everything ---

The halls open into an interior plaza containing a large kinetic sculpture. 
The sculpture is in a sealed enclosure and seems to involve a set of identical spherical capsules that are carried to the top and allowed to bounce through the maze of spinning pieces.

Part of the sculpture is even interactive! When a button is pressed, a capsule is dropped and tries to fall through slots in a set of rotating discs to finally go through a little hole at the bottom and come out of the sculpture. 
If any of the slots aren't aligned with the capsule as it passes, the capsule bounces off the disc and soars away. 
You feel compelled to get one of those capsules.

The discs pause their motion each second and come in different sizes; they seem to each have a fixed number of positions at which they stop. 
You decide to call the position with the slot 0, and count up for each position it reaches next.

Furthermore, the discs are spaced out so that after you push the button, one second elapses before the first disc is reached, and one second elapses as the capsule passes from one disc to the one below it. 
So, if you push the button at time=100, then the capsule reaches the top disc at time=101, the second disc at time=102, the third disc at time=103, and so on.

The button will only drop a capsule at an integer time - no fractional seconds allowed.

For example, at time=0, suppose you see the following arrangement:

Disc #1 has 5 positions; at time=0, it is at position 4.
Disc #2 has 2 positions; at time=0, it is at position 1.
If you press the button exactly at time=0, the capsule would start to fall; it would reach the first disc at time=1. 
Since the first disc was at position 4 at time=0, by time=1 it has ticked one position forward. 
As a five-position disc, the next position is 0, and the capsule falls through the slot.

Then, at time=2, the capsule reaches the second disc. 
The second disc has ticked forward two positions at this point: it started at position 1, then continued to position 0, and finally ended up at position 1 again. 
Because there's only a slot at position 0, the capsule bounces away.

If, however, you wait until time=5 to push the button, then when the capsule reaches each disc, the first disc will have ticked forward 5+1 = 6 times (to position 0), and the second disc will have ticked forward 5+2 = 7 times (also to position 0). 
In this case, the capsule would fall through the discs and come out of the machine.

However, your situation has more than two discs; you've noted their positions in your puzzle input. 
What is the first time you can press the button to get a capsule?

Your puzzle answer was 400589.

--- Part Two ---

After getting the first capsule (it contained a star! what great fortune!), the machine detects your success and begins to rearrange itself.

When it's done, the discs are back in their original configuration as if it were time=0 again, but a new disc with 11 positions and starting at position 0 has appeared exactly one second below the previously-bottom disc.

With this new disc, and counting again starting from time=0 with the configuration in your puzzle input, what is the first time you can press the button to get another capsule?

*/

namespace Day15
{
    class Program
    {
        readonly static int MAX_NUM_DISCS = 16;
        static int[] sIDs = new int[MAX_NUM_DISCS];
        static int[] sNumPositions = new int[MAX_NUM_DISCS];
        static int[] sStartTimes = new int[MAX_NUM_DISCS];
        static int[] sStartPositions = new int[MAX_NUM_DISCS];

        static int[] sPositions = new int[MAX_NUM_DISCS];
        static int sTime;
        static int sDiscsCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = FindEscapeTime();
                Console.WriteLine($"Day15 : Result1 {result1}");
                var expected = 400589;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                sIDs[sDiscsCount] = sIDs[sDiscsCount - 1] + 1;
                sNumPositions[sDiscsCount] = 11;
                sStartTimes[sDiscsCount] = 0;
                sStartPositions[sDiscsCount] = 0;
                sPositions[sDiscsCount] = sStartPositions[sDiscsCount];
                ++sDiscsCount;
                var result2 = FindEscapeTime();
                Console.WriteLine($"Day15 : Result2 {result2}");
                var expected = 3045959;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sDiscsCount = 0;
            //"Disc #1 has 5 positions; at time=0, it is at position 4.",
            //"Disc #2 has 2 positions; at time=0, it is at position 1."
            foreach (var line in lines)
            {
                var tokens = line.Split(' ');
                if (tokens.Length != 12)
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 12 tokens got {tokens.Length}");
                }
                if (tokens[0] != "Disc")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'Disc' as token[0] got {tokens[0]}");
                }
                if (tokens[2] != "has")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'has' as token[2] got {tokens[2]}");
                }
                if (tokens[4] != "positions;")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'positions;' as token[4] got {tokens[4]}");
                }
                if (tokens[5] != "at")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'at' as token[5] got {tokens[5]}");
                }
                if (tokens[7] != "it")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'it' as token[7] got {tokens[7]}");
                }
                if (tokens[8] != "is")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'is' as token[8] got {tokens[8]}");
                }
                if (tokens[9] != "at")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'at' as token[9] got {tokens[9]}");
                }
                if (tokens[10] != "position")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'position' as token[10] got {tokens[10]}");
                }

                var idToken = tokens[1];
                if (idToken[0] != '#')
                {
                    throw new InvalidProgramException($"Invalid line {line} expected '#' as first character in the id token {idToken}");
                }

                var numPositionsToken = tokens[3];

                var startTimeToken = tokens[6];
                var startTimeTokens = startTimeToken.Split('=');
                if (startTimeTokens[0] != "time")
                {
                    throw new InvalidProgramException($"Invalid line {line} expected 'time as first token in the start time tokens {startTimeToken}");
                }
                if (!startTimeTokens[1].EndsWith(','))
                {
                    throw new InvalidProgramException($"Invalid line {line} expected ',' as end character in the start time tokens {startTimeToken}");
                }

                var startPositionToken = tokens[11];
                if (!startPositionToken.EndsWith('.'))
                {
                    throw new InvalidProgramException($"Invalid line {line} expected '.' as end character in the start position token {startPositionToken}");
                }

                var id = int.Parse(idToken.TrimStart('#'));
                var numPositions = int.Parse(numPositionsToken);
                var startTime = int.Parse(startTimeTokens[1].TrimEnd(','));
                var startPosition = int.Parse(startPositionToken.TrimEnd('.'));

                //Console.WriteLine($"ID:{id} Positions:{numPositions} Time:{startTime} Position:{startPosition}");
                sIDs[sDiscsCount] = id;
                sNumPositions[sDiscsCount] = numPositions;
                sStartTimes[sDiscsCount] = startTime;
                sStartPositions[sDiscsCount] = startPosition;
                ++sDiscsCount;
            }
            sTime = 0;
            for (var i = 0; i < sDiscsCount; ++i)
            {
                sPositions[i] = sStartPositions[i];
            }
        }

        static int FindEscapeTime()
        {
            for (var t = 0; t < 10000000; ++t)
            {
                if (CapsuleEscaped(t))
                {
                    return t;
                }
            }
            return -1;
        }

        public static bool CapsuleEscaped(int time)
        {
            sTime = time;
            for (var i = 0; i < sDiscsCount; ++i)
            {
                var diskPosition = (sStartPositions[i] + time + sIDs[i]) % sNumPositions[i];
                if (diskPosition != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Run()
        {
            Console.WriteLine("Day15 : Start");
            _ = new Program("Day15/input.txt", true);
            _ = new Program("Day15/input.txt", false);
            Console.WriteLine("Day15 : End");
        }
    }
}
