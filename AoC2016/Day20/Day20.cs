using System;

/*

--- Day 20: Firewall Rules ---

You'd like to set up a small hidden computer here so you can use it to get back into the network later. 
However, the corporate firewall only allows communication with certain external IP addresses.

You've retrieved the list of blocked IPs from the firewall, but the list seems to be messy and poorly maintained, and it's not clear which IPs are allowed. 
Also, rather than being written in dot-decimal notation, they are written as plain 32-bit integers, which can have any value from 0 through 4294967295, inclusive.

For example, suppose only the values 0 through 9 were valid, and that you retrieved the following blacklist:

5-8
0-2
4-7
The blacklist specifies ranges of IPs (inclusive of both the start and end value) that are not allowed. 
Then, the only IPs that this firewall allows are 3 and 9, since those are the only numbers not in any range.

Given the list of blocked IPs you retrieved from the firewall (your puzzle input), what is the lowest-valued IP that is not blocked?

Your puzzle answer was 17348574.

--- Part Two ---

How many IPs are allowed by the blacklist?

*/

namespace Day20
{
    class Program
    {
        static uint[] sStarts;
        static uint[] sEnds;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = FindMin();
                Console.WriteLine($"Day20 : Result1 {result1}");
                var expected = 4793564;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = CountAllowed();
                Console.WriteLine($"Day20 : Result2 {result2}");
                var expected = 146;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sStarts = new uint[lines.Length];
            sEnds = new uint[lines.Length];
            var i = 0;
            foreach (var line in lines)
            {
                var tokens = line.Split('-');
                uint start = uint.Parse(tokens[0]);
                uint end = uint.Parse(tokens[1]);
                sStarts[i] = start;
                sEnds[i] = end;
                //Console.WriteLine($"Range[{i}] {sStarts[i]} -> {sEnds[i]}");
                ++i;
            }

            for (i = 0; i < sStarts.Length - 1; ++i)
            {
                for (var j = i + 1; j < sStarts.Length; ++j)
                {
                    var startI = sStarts[i];
                    var startJ = sStarts[j];
                    if (startJ < startI)
                    {
                        sStarts[i] = startJ;
                        sStarts[j] = startI;
                        var endI = sEnds[i];
                        var endJ = sEnds[j];
                        sEnds[i] = endJ;
                        sEnds[j] = endI;
                    }
                }
            }
            for (i = 0; i < sStarts.Length; ++i)
            {
                //Console.WriteLine($"Range[{i}] {sStarts[i]} -> {sEnds[i]}");
            }
        }

        public static uint FindMin()
        {
            uint minUnused = 0;
            for (var i = 0; i < sStarts.Length; ++i)
            {
                var start = sStarts[i];
                var end = sEnds[i];
                if ((minUnused >= start) && (minUnused <= end))
                {
                    minUnused = end + 1;
                }
            }
            return minUnused;
        }

        static bool CanMergeRange(int i)
        {
            var startI = sStarts[i];
            var endI = sEnds[i];
            if ((sStarts[i] == 0) && (sEnds[i] == 0))
            {
                return false;
            }
            if (endI != uint.MaxValue)
            {
                ++endI;
            }
            for (var j = i + 1; j < sStarts.Length; ++j)
            {
                var startJ = sStarts[j];
                var endJ = sEnds[j];
                if (startJ > endI)
                {
                    return false;
                }
                if ((sStarts[j] == 0) && (sEnds[j] == 0))
                {
                    return false;
                }
                if ((startJ >= startI) && (startJ <= endI))
                {
                    if (endJ > sEnds[i])
                    {
                        sEnds[i] = endJ;
                    }
                    for (var k = j; k < sStarts.Length - 1; ++k)
                    {
                        sStarts[k] = sStarts[k + 1];
                        sEnds[k] = sEnds[k + 1];
                    }
                    sStarts[^1] = 0;
                    sEnds[^1] = 0;
                    return true;
                }
            }
            return false;
        }

        static void MergeRanges()
        {
            for (var i = 0; i < sStarts.Length; ++i)
            {
                while (CanMergeRange(i)) ;
            }
        }

        public static uint CountAllowed()
        {
            MergeRanges();
            var count = 0U;
            var previousEnd = sEnds[0];
            for (var i = 1; i < sStarts.Length; ++i)
            {
                if ((sStarts[i] != 0) || (sEnds[i] != 0))
                {
                    var delta = sStarts[i] - (previousEnd + 1);
                    count += delta;
                    previousEnd = sEnds[i];
                    //Console.WriteLine($"Range[{i}] {sStarts[i]} -> {sEnds[i]} count {count} delta {delta}");
                }
            }
            if (previousEnd < uint.MaxValue)
            {
                count += uint.MaxValue - (previousEnd + 1);
            }
            //Console.WriteLine($"count {count}");
            return count;
        }

        public static void Run()
        {
            Console.WriteLine("Day20 : Start");
            _ = new Program("Day20/input.txt", true);
            _ = new Program("Day20/input.txt", false);
            Console.WriteLine("Day20 : End");
        }
    }
}
