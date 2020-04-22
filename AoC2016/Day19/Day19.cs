using System;
using System.Collections;
using System.Collections.Generic;

/*

--- Day 19: An Elephant Named Joseph ---

The Elves contact you over a highly secure emergency channel. 
Back at the North Pole, the Elves are busy misunderstanding White Elephant parties.

Each Elf brings a present. 
They all sit in a circle, numbered starting with position 1. 
Then, starting with the first Elf, they take turns stealing all the presents from the Elf to their left. 
An Elf with no presents is removed from the circle and does not take turns.

For example, with five Elves (numbered 1 to 5):

  1
5   2
 4 3
Elf 1 takes Elf 2's present.
Elf 2 has no presents and is skipped.
Elf 3 takes Elf 4's present.
Elf 4 has no presents and is also skipped.
Elf 5 takes Elf 1's two presents.
Neither Elf 1 nor Elf 2 have any presents, so both are skipped.
Elf 3 takes Elf 5's three presents.
So, with five Elves, the Elf that sits starting in position 3 gets all the presents.

With the number of Elves given in your puzzle input, which Elf gets all the presents?

Your puzzle answer was 1815603.

--- Part Two ---

Realizing the folly of their present-exchange rules, the Elves agree to instead steal presents from the Elf directly across the circle. 
If two Elves are across the circle, the one on the left (from the perspective of the stealer) is stolen from. 
The other rules remain unchanged: Elves with no presents are removed from the circle entirely, and the other elves move in slightly to keep the circle evenly spaced.

For example, with five Elves (again numbered 1 to 5):

The Elves sit in a circle; Elf 1 goes first:
  1
5   2
 4 3
Elves 3 and 4 are across the circle; Elf 3's present is stolen, being the one to the left. Elf 3 leaves the circle, and the rest of the Elves move in:
  1           1
5   2  -->  5   2
 4 -          4
Elf 2 steals from the Elf directly across the circle, Elf 5:
  1         1 
-   2  -->     2
  4         4 
Next is Elf 4 who, choosing between Elves 1 and 2, steals from Elf 1:
 -          2  
    2  -->
 4          4
Finally, Elf 2 steals from Elf 4:
 2
    -->  2  
 -
So, with five Elves, the Elf that sits starting in position 2 gets all the presents.

With the number of Elves given in your puzzle input, which Elf now gets all the presents?

*/

namespace Day19
{
    class Program
    {
        readonly static int MAX_NUM_ELVES = 128 * 1024 * 1024;
        readonly static BitArray sNonZeroElvesBitArray = new BitArray(MAX_NUM_ELVES, true);

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Expected single line input {lines.Length}");
            }
            var elfCount = int.Parse(lines[0]);

            if (part1)
            {
                var result1 = ElfWithPresents(elfCount, false);
                Console.WriteLine($"Day19 : Result1 {result1}");
                var expected = 1815603;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = ElfWithPresents(elfCount, true);
                Console.WriteLine($"Day19 : Result2 {result2}");
                var expected = 1410630;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static (int index, int bitPos) GetIndexBitPos(int e)
        {
            int index = e / (sizeof(ulong) * 8);
            int bitPos = e - index * sizeof(ulong) * 8;
            return (index, bitPos);
        }

        static int FindNonEmptySlow(int start, int skipCount, int count)
        {
            var e = start;
            int numOnesFound = 0;
            while (numOnesFound != skipCount)
            {
                do
                {
                    e = (e + 1) % count;
                } while (sNonZeroElvesBitArray[e] == false);
                ++numOnesFound;
            }
            if (e == start)
            {
                throw new InvalidProgramException($"Slow: No non-empty {start} : {skipCount}");
            }
            return e;
        }

        public static int ElfWithPresents(int elfCount, bool steal)
        {
            var nonZeroElvesCount = elfCount;
            var presentsPerElf = new int[elfCount];
            List<int> sNonZeroElves = new List<int>(elfCount);

            for (var e = 0; e < elfCount; ++e)
            {
                presentsPerElf[e] = 1;
                sNonZeroElves.Add(e);
                sNonZeroElvesBitArray[e] = true;
            }

            int elfWithAllPresents = -1;
            while (elfWithAllPresents == -1)
            {
                Console.WriteLine($"nonZeroElvesCount:{nonZeroElvesCount}");
                for (var e = 0; e < elfCount; ++e)
                {
                    if (sNonZeroElvesBitArray[e] == false)
                    {
                        continue;
                    }
                    if (e % 50000 == 0)
                    {
                        Console.WriteLine($"elf:{e}");
                    }
                    int elvesToSkip = 1;
                    if (steal)
                    {
                        elvesToSkip = nonZeroElvesCount / 2;
                    }
                    int index = sNonZeroElves.IndexOf(e);
                    int nextElfIndex = index + elvesToSkip;
                    nextElfIndex %= sNonZeroElves.Count;
                    int nextElf = sNonZeroElves[nextElfIndex];

                    if (nextElf == e)
                    {
                        elfWithAllPresents = e;
                        if (nonZeroElvesCount != 1)
                        {
                            throw new InvalidProgramException($"Invalid nonzeroElvesCount {nonZeroElvesCount}");
                        }
                    }
                    else
                    {
                        var presentsToAdd = presentsPerElf[nextElf];
                        if (presentsToAdd == 0)
                        {
                            throw new InvalidProgramException($"Invalid presentsToAdd {presentsToAdd}");
                        }
                        presentsPerElf[e] += presentsToAdd;
                        presentsPerElf[nextElf] = 0;
                        sNonZeroElvesBitArray[nextElf] = false;
                        sNonZeroElves.RemoveAt(nextElfIndex);
                        --nonZeroElvesCount;
                        if (nonZeroElvesCount == 1)
                        {
                            elfWithAllPresents = e;
                        }
                    }
                }
            };
            // Elf position starts at 1 not 0
            return elfWithAllPresents + 1;
        }

        public static int ElfWithPresentsSlow(int elfCount, bool steal)
        {
            var nonZeroElvesCount = elfCount;
            var presentsPerElf = new int[elfCount];

            for (var e = 0; e < elfCount; ++e)
            {
                presentsPerElf[e] = 1;
                sNonZeroElvesBitArray[e] = true;
            }

            int elfWithAllPresents = -1;
            while (elfWithAllPresents == -1)
            {
                Console.WriteLine($"nonZeroElvesCount:{nonZeroElvesCount}");
                for (var e = 0; e < elfCount; ++e)
                {
                    if (sNonZeroElvesBitArray[e] == false)
                    {
                        continue;
                    }
                    if (e % 5000 == 0)
                    {
                        Console.WriteLine($"elf:{e}");
                    }
                    int elvesToSkip = 1;
                    if (steal)
                    {
                        elvesToSkip = nonZeroElvesCount / 2;
                    }
                    int nextElf = FindNonEmptySlow(e, elvesToSkip, elfCount);

                    if (nextElf == e)
                    {
                        elfWithAllPresents = e;
                        if (nonZeroElvesCount != 1)
                        {
                            throw new InvalidProgramException($"Invalid nonzeroElvesCount {nonZeroElvesCount}");
                        }
                    }
                    else
                    {
                        var presentsToAdd = presentsPerElf[nextElf];
                        if (presentsToAdd == 0)
                        {
                            throw new InvalidProgramException($"Invalid presentsToAdd {presentsToAdd}");
                        }
                        presentsPerElf[e] += presentsToAdd;
                        presentsPerElf[nextElf] = 0;
                        sNonZeroElvesBitArray[nextElf] = false;
                        --nonZeroElvesCount;
                        if (nonZeroElvesCount == 1)
                        {
                            elfWithAllPresents = e;
                        }
                    }
                }
            };
            // Elf position starts at 1 not 0
            return elfWithAllPresents + 1;
        }

        public static void Run()
        {
            Console.WriteLine("Day19 : Start");
            _ = new Program("Day19/input.txt", true);
            _ = new Program("Day19/input.txt", false);
            Console.WriteLine("Day19 : End");
        }
    }
}
