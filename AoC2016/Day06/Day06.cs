using System;

/*

--- Day 6: Signals and Noise ---

Something is jamming your communications with Santa. 
Fortunately, your signal is only partially jammed, and protocol in situations like this is to switch to a simple repetition code to get the message through.

In this model, the same message is sent repeatedly. 
You've recorded the repeating message signal (your puzzle input), but the data seems quite corrupted - almost too badly to recover. 
Almost.

All you need to do is figure out which character is most frequent for each position. 
For example, suppose you had recorded the following messages:

eedadn
drvtee
eandsr
raavrd
atevrs
tsrnev
sdttsa
rasrtv
nssdts
ntnada
svetve
tesnvt
vntsnd
vrdear
dvrsen
enarar

The most common character in the first column is e; in the second, a; in the third, s, and so on. 
Combining these characters returns the error-corrected message, easter.

Given the recording in your puzzle input, what is the error-corrected version of the message being sent?

Your puzzle answer was qrqlznrl.

--- Part Two ---

Of course, that would be the message - if you hadn't agreed to use a modified repetition code instead.

In this modified code, the sender instead transmits what looks like random data, but for each character, the character they actually want to send is slightly less likely than the others. 
Even after signal-jamming noise, you can look at the letter distributions in each column and choose the least common letter to reconstruct the original message.

In the above example, the least common character in the first column is a; in the second, d, and so on. Repeating this process for the remaining characters produces the original message, advent.

Given the recording in your puzzle input and this new decoding methodology, what is the original message that Santa is trying to send?

*/

namespace Day06
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (part1)
            {
                var result1 = RecoverCode(lines, true);
                Console.WriteLine($"Day06 : Result1 {result1}");
                var expected = "qrqlznrl";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = RecoverCode(lines, false);
                Console.WriteLine($"Day06 : Result2 {result2}");
                var expected = "kgzdfaon";
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static string RecoverCode(string[] lines, bool most)
        {
            var numDigits = lines[0].Length;
            var letterCounts = new int[numDigits, 26];

            foreach (var line in lines)
            {
                var width = line.Length;
                if (width != numDigits)
                {
                    throw new InvalidProgramException($"Unequal width codes Expected {numDigits} Got {width}");
                }
                for (var d = 0; d < numDigits; ++d)
                {
                    var c = line[d];
                    var index = c - 'a';
                    ++letterCounts[d, index];
                }
            }

            var bestLetterIndex = new int[numDigits];
            for (var d = 0; d < numDigits; ++d)
            {
                int target;
                if (most)
                {
                    target = int.MinValue;
                }
                else
                {
                    target = int.MaxValue;
                }
                for (var index = 0; index < 26; ++index)
                {
                    if (letterCounts[d, index] == 0)
                    {
                        continue;
                    }
                    if (most)
                    {
                        if (letterCounts[d, index] > target)
                        {
                            bestLetterIndex[d] = index;
                            target = letterCounts[d, index];
                        }
                    }
                    else
                    {
                        if (letterCounts[d, index] < target)
                        {
                            bestLetterIndex[d] = index;
                            target = letterCounts[d, index];
                        }
                    }
                }
            }
            var password = new char[numDigits];
            for (var d = 0; d < numDigits; ++d)
            {
                password[d] = (char)('a' + bestLetterIndex[d]);
            }

            return new string(password);
        }

        public static void Run()
        {
            Console.WriteLine("Day06 : Start");
            _ = new Program("Day06/input.txt", true);
            _ = new Program("Day06/input.txt", false);
            Console.WriteLine("Day06 : End");
        }
    }
}
