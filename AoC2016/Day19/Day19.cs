using System;
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
        readonly static byte[] sNonZeroElves = new byte[MAX_NUM_ELVES];
        readonly static int[] sSpanStarts = new int[MAX_NUM_ELVES];
        readonly static int[] sSpanLengths = new int[MAX_NUM_ELVES];
        static int sSpanCount = 0;

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
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static void InitSpans(int count)
        {
            sSpanCount = 0;
            sSpanStarts[sSpanCount] = 0;
            sSpanLengths[sSpanCount] = count;
            sSpanCount = 1;
        }

        static void SplitSpan(int index)
        {
            //CheckSpans();
            var span = FindStartingSpan(index);
            var spanStart = sSpanStarts[span];
            var spanLength = sSpanLengths[span];

            var leftSpan = span;
            var rightSpan = span + 1;

            var leftSpanStart = spanStart;
            var leftSpanLength = index - leftSpanStart;

            var rightSpanStart = index + 1;
            var rightSpanLength = spanStart + spanLength - rightSpanStart;

            sSpanStarts[leftSpan] = leftSpanStart;
            sSpanLengths[leftSpan] = leftSpanLength;

            if (rightSpanLength > 0)
            {
                InsertSpan(rightSpan);
                sSpanStarts[rightSpan] = rightSpanStart;
                sSpanLengths[rightSpan] = rightSpanLength;
            }

            //Console.WriteLine($"RemovedIndex {index} {leftSpanLength} {rightSpanLength}");
            if (leftSpanLength == 0)
            {
                RemoveEmptySpans();
            }
            //CheckSpans();
        }

        static void InsertSpan(int index)
        {
            for (var i = sSpanCount; i > index; --i)
            {
                sSpanStarts[i] = sSpanStarts[i - 1];
                sSpanLengths[i] = sSpanLengths[i - 1];
            }
            ++sSpanCount;
        }

        static void RemoveSpan(int index)
        {
            for (var i = index; i < sSpanCount - 1; ++i)
            {
                sSpanStarts[i] = sSpanStarts[i + 1];
                sSpanLengths[i] = sSpanLengths[i + 1];
            }
            --sSpanCount;
        }

        static void RemoveEmptySpans()
        {
            for (var i = 0; i < sSpanCount; ++i)
            {
                if (sSpanLengths[i] == 0)
                {
                    //Console.WriteLine($"Removing Empty Span {i}");
                    RemoveSpan(i);
                    --i;
                }
            }
            //CheckSpans();
        }

        static int FindStartingSpan(int start)
        {
            var startingSpan = -1;
            for (var i = 0; i < sSpanCount; ++i)
            {
                var spanStart = sSpanStarts[i];
                var spanEnd = spanStart + sSpanLengths[i];
                if ((start >= spanStart) && (start < spanEnd))
                {
                    startingSpan = i;
                    break;
                }
            }
            if (startingSpan == -1)
            {
                throw new InvalidProgramException($"Failed to find starting span for {start}");
            }
            return startingSpan;
        }

        static void CheckSpans()
        {
            var start = -1;
            var end = -1;
            for (var i = 0; i < sSpanCount; ++i)
            {
                var spanStart = sSpanStarts[i];
                var spanLen = sSpanLengths[i];
                var spanEnd = spanStart + spanLen;
                if (spanStart <= start)
                {
                    throw new InvalidProgramException($"Bad span start {spanStart} <= {start} span[{i}]");
                }
                if (spanStart <= end)
                {
                    throw new InvalidProgramException($"Bad span start {spanStart} <= {end} span[{i}]");
                }
                if (spanEnd <= spanStart)
                {
                    throw new InvalidProgramException($"Bad span end {spanEnd} <= {spanStart} span[{i}]");
                }
                start = spanStart;
                end = spanEnd;
                //Console.WriteLine($"Span[{i}] {spanStart} -> {spanStart + spanLen - 1} : {spanLen}");
            }
        }

        static int FindNonEmpty(int start, int skipCount, int count)
        {
            var indexSpans = FindNonEmptySpans(start, skipCount, count);
            /*
            var indexSlow = FindNonEmptySlow(start, skipCount, count);
            if (indexSpans != indexSlow)
            {
                throw new InvalidProgramException($"{start} : {skipCount} FindNonEmpty spans != slow {indexSpans} != {indexSlow}");
            }
            */
            return indexSpans;
        }

        static int FindNonEmptySpans(int start, int skipCount, int count)
        {
            //CheckSpans();
            var span = FindStartingSpan(start);
            var remainder = skipCount;
            var spanStart = sSpanStarts[span];
            var spanLength = sSpanLengths[span];
            var spanEnd = spanStart + spanLength;
            var startOffset = start - spanStart;
            if (start + skipCount > spanEnd)
            {
                ++span;
                span %= sSpanCount;
                remainder -= spanEnd - start;
                startOffset = 0;
            }

            while (remainder >= 0)
            {
                spanStart = sSpanStarts[span];
                spanLength = sSpanLengths[span];
                if (spanLength > remainder)
                {
                    var index = spanStart + startOffset + remainder;
                    index %= count;
                    return index;
                }
                startOffset = 0;
                remainder -= spanLength;
                ++span;
                span %= sSpanCount;
            }
            throw new InvalidProgramException($"Spans: No non-empty {start} : {skipCount}");
        }

        static int FindNonEmptySlow(int start, int skipCount, int count)
        {
            var index = start;
            for (var i = 0; i < skipCount; ++i)
            {
                do
                {
                    index = (index + 1) % count;
                }
                while (sNonZeroElves[index] == 0);
            }
            if (index == start)
            {
                throw new InvalidProgramException($"Slow: No non-empty {start} : {skipCount}");
            }
            return index;
        }

        public static int ElfWithPresents(int elfCount, bool steal)
        {
            InitSpans(elfCount);
            var nonZeroElvesCount = elfCount;
            var presentsPerElf = new int[elfCount];

            for (var e = 0; e < elfCount; ++e)
            {
                presentsPerElf[e] = 1;
                sNonZeroElves[e] = 1;
            }

            int elfWithAllPresents = -1;
            while (elfWithAllPresents == -1)
            {
                Console.WriteLine($"nonZeroElvesCount:{nonZeroElvesCount}");
                for (var e = 0; e < elfCount; ++e)
                {
                    if (sNonZeroElves[e] == 0)
                    {
                        continue;
                    }
                    if (e % 5000 == 0)
                    {
                        Console.WriteLine($"elf:{e} {sSpanCount}");
                    }
                    int elvesToSkip = 1;
                    if (steal)
                    {
                        elvesToSkip = nonZeroElvesCount / 2;
                    }
                    //int nextElf = FindNonEmpty(e, elvesToSkip, elfCount);
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
                        --nonZeroElvesCount;
                        sNonZeroElves[nextElf] = 0;
                        //SplitSpan(nextElf);
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
