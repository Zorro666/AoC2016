using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

/*

--- Day 14: One-Time Pad ---

In order to communicate securely with Santa while you're on this mission, you've been using a one-time pad that you generate using a pre-agreed algorithm. 
Unfortunately, you've run out of keys in your one-time pad, and so you need to generate some more.

To generate keys, you first get a stream of random data by taking the MD5 of a pre-arranged salt (your puzzle input) and an increasing integer index (starting with 0, and represented in decimal); the resulting MD5 hash should be represented as a string of lowercase hexadecimal digits.

However, not all of these MD5 hashes are keys, and you need 64 new keys for your one-time pad. 
A hash is a key only if:

It contains three of the same character in a row, like 777. 
Only consider the first such triplet in a hash.
One of the next 1000 hashes in the stream contains that same character five times in a row, like 77777.
Considering future hashes for five-of-a-kind sequences does not cause those hashes to be skipped; instead, regardless of whether the current hash is a key, always resume testing for keys starting with the very next hash.

For example, if the pre-arranged salt is abc:

The first index which produces a triple is 18, because the MD5 hash of abc18 contains ...cc38887a5.... 
However, index 18 does not count as a key for your one-time pad, because none of the next thousand hashes (index 19 through index 1018) contain 88888.
The next index which produces a triple is 39; the hash of abc39 contains eee. 
It is also the first key: one of the next thousand hashes (the one at index 816) contains eeeee.
None of the next six triples are keys, but the one after that, at index 92, is: it contains 999 and index 200 contains 99999.
Eventually, index 22728 meets all of the criteria to generate the 64th key.
So, using our example salt of abc, index 22728 produces the 64th key.

Given the actual salt in your puzzle input, what index produces your 64th one-time pad key?

Your puzzle input is zpqevtbw.


Your puzzle answer was 16106.

--- Part Two ---

Of course, in order to make this process even more secure, you've also implemented key stretching.

Key stretching forces attackers to spend more time generating hashes. 
Unfortunately, it forces everyone else to spend more time, too.

To implement key stretching, whenever you generate a hash, before you use it, you first find the MD5 hash of that hash, then the MD5 hash of that hash, and so on, a total of 2016 additional hashings. 
Always use lowercase hexadecimal representations of hashes.

For example, to find the stretched hash for index 0 and salt abc:

Find the MD5 hash of abc0: 577571be4de9dcce85a041ba0410f29f.
Then, find the MD5 hash of that hash: eec80a0c92dc8a0777c619d9bb51e910.
Then, find the MD5 hash of that hash: 16062ce768787384c81fe17a7a60c7e3.
...repeat many times...
Then, find the MD5 hash of that hash: a107ff634856bb300138cac6568c0f24.
So, the stretched hash for index 0 in this situation is a107ff.... 
In the end, you find the original hash (one use of MD5), then find the hash-of-the-previous-hash 2016 times, for a total of 2017 uses of MD5.

The rest of the process remains the same, but now the keys are entirely different. 
Again for salt abc:

The first triple (222, at index 5) has no matching 22222 in the next thousand hashes.
The second triple (eee, at index 10) hash a matching eeeee at index 89, and so it is the first key.
Eventually, index 22551 produces the 64th key (triple fff with matching fffff at index 22859.
Given the actual salt in your puzzle input and using 2016 extra MD5 calls of key stretching, what index now produces your 64th one-time pad key?

*/

namespace Day14
{
    class Program
    {
        static string sSalt;
        static MD5 sMD5;
        private static readonly Dictionary<int, string> sComputedMD5s = new Dictionary<int, string>(500000);

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = FindIndex(64, 0);
                Console.WriteLine($"Day14 : Result1 {result1}");
                var expected = 15035;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = FindIndex(64, 2016);
                Console.WriteLine($"Day14 : Result2 {result2}");
                var expected = 19968;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Input must be a single line {lines.Length}");
            }
            sSalt = lines[0].Trim();
            sMD5 = MD5.Create();
            sComputedMD5s.Clear();
        }

        static string GetHashASCIIBytes(int index, int stretchingCount)
        {
            if (sComputedMD5s.TryGetValue(index, out string hash))
            {
                return hash;
            }
            var totalKey = $"{sSalt}{index}";
            byte[] inputBytes = Encoding.ASCII.GetBytes(totalKey);
            byte[] asciiBytes = new byte[32];
            for (var i = 0; i <= stretchingCount; ++i)
            {
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
                inputBytes = asciiBytes;
            }
            string hashString = "";
            for (var i = 0; i < asciiBytes.Length; ++i)
            {
                hashString += (char)asciiBytes[i];
            }
            sComputedMD5s[index] = hashString;
            return hashString;
        }

        static (int, char) FindThreeCharHash(int startIndex, int stretchingCount)
        {
            for (var index = startIndex; index < startIndex + 1000; ++index)
            {
                var asciiBytes = GetHashASCIIBytes(index, stretchingCount);
                for (var i = 0; i < asciiBytes.Length - 2; ++i)
                {
                    var c = asciiBytes[i];
                    if ((c == asciiBytes[i + 1]) && (c == asciiBytes[i + 2]))
                    {
                        return (index, c);
                    }
                }
            }
            throw new InvalidProgramException($"Could not find 3 character hash starting from {startIndex}");
        }

        static bool FindFiveCharHash(int startIndex, char c, int stretchingCount)
        {
            for (var index = startIndex; index <= startIndex + 1000; ++index)
            {
                var asciiBytes = GetHashASCIIBytes(index, stretchingCount);
                for (var i = 0; i < asciiBytes.Length - 4; ++i)
                {
                    bool foundMatch = true;
                    for (var j = 0; j < 5; ++j)
                    {
                        if (asciiBytes[i + j] != c)
                        {
                            foundMatch = false;
                            break;
                        }
                    }
                    if (foundMatch)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static int FindIndex(int targetCount, int stretchingCount)
        {
            var foundCount = 0;
            var index = 0;
            while (index < 300000)
            {
                char c;
                int threeIndex;
                (threeIndex, c) = FindThreeCharHash(index, stretchingCount);
                //Console.WriteLine($"Found Three Char Start {threeIndex} {c:x}");
                if (FindFiveCharHash(threeIndex + 1, c, stretchingCount))
                {
                    //Console.WriteLine($"{foundCount} Found valid key {threeIndex}");
                    ++foundCount;
                    if (foundCount == targetCount)
                    {
                        return threeIndex;
                    }
                }
                index = threeIndex + 1;
            }
            return -1;
        }

        public static void Run()
        {
            Console.WriteLine("Day14 : Start");
            _ = new Program("Day14/input.txt", true);
            _ = new Program("Day14/input.txt", false);
            Console.WriteLine("Day14 : End");
        }
    }
}
