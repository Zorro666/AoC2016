using System;
using System.Security.Cryptography;
using System.Text;

/*

--- Day 5: How About a Nice Game of Chess? ---

You are faced with a security door designed by Easter Bunny engineers that seem to have acquired most of their security knowledge by watching hacking movies.

The eight-character password for the door is generated one character at a time by finding the MD5 hash of some Door ID (your puzzle input) and an increasing integer index (starting with 0).

A hash indicates the next character in the password if its hexadecimal representation starts with five zeroes. 
If it does, the sixth character in the hash is the next character of the password.

For example, if the Door ID is abc:

The first index which produces a hash that starts with five zeroes is 3231929, which we find by hashing abc3231929; 
the sixth character of the hash, and thus the first character of the password, is 1.
5017308 produces the next interesting hash, which starts with 000008f82..., so the second character of the password is 8.
The third time a hash starts with five zeroes is for abc5278568, discovering the character f.
In this example, after continuing this search a total of eight times, the password is 18f47a30.

Given the actual Door ID, what is the password?

Your puzzle input is reyedfim.

Your puzzle answer was f97c354d.

--- Part Two ---

As the door slides open, you are presented with a second door that uses a slightly more inspired security mechanism. Clearly unimpressed by the last version (in what movie is the password decrypted in order?!), the Easter Bunny engineers have worked out a better solution.

Instead of simply filling in the password from left to right, the hash now also indicates the position within the password to fill. 
You still look for hashes that begin with five zeroes; however, now, the sixth character represents the position (0-7), and the seventh character is the character to put in that position.

A hash result of 000001f means that f is the second character in the password. 
Use only the first result for each position, and ignore invalid positions.

For example, if the Door ID is abc:

The first interesting hash is from abc3231929, which produces 0000015...; so, 5 goes in position 1: _5______.
In the previous method, 5017308 produced an interesting hash; however, it is ignored, because it specifies an invalid position (8).
The second interesting hash is at index 5357525, which produces 000004e...; so, e goes in position 4: _5__e___.
You almost choke on your popcorn as the final character falls into place, producing the password 05ace8e3.

Given the actual Door ID and this new method, what is the password? 
Be extra proud of your solution if it uses a cinematic "decrypting" animation.

*/

namespace Day05
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (part1)
            {
                var result1 = Password(lines[0].Trim());
                Console.WriteLine($"Day05 : Result1 {result1}");
                var expected = "f97c354d";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = PasswordComplex(lines[0].Trim());
                Console.WriteLine($"Day05 : Result2 {result2}");
                var expected = "863dde27";
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static char FindFiveZeroHash(string secretKey, ref long result)
        {
            using (var md5 = MD5.Create())
            {
                while (result < 10 * 1000 * 1000)
                {
                    var totalKey = $"{secretKey}{result}";
                    byte[] inputBytes = Encoding.ASCII.GetBytes(totalKey);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // First five nibbles must be 0
                    if ((hashBytes[0] == 0) && (hashBytes[1] == 0) && ((hashBytes[2] & 0xF0) == 0))
                    {
                        var c = hashBytes[2] & 0xF;
                        if (c < 10)
                        {
                            return (char)('0' + c);
                        }
                        else
                        {
                            return (char)('a' + (c - 10));
                        }
                    }
                    ++result;
                }
            }
            throw new InvalidProgramException($"No match found {result}");
        }

        static (int, char) FindFiveZeroHashComplex(string secretKey, ref long result)
        {
            using (var md5 = MD5.Create())
            {
                while (result < 50 * 1000 * 1000)
                {
                    var totalKey = $"{secretKey}{result}";
                    byte[] inputBytes = Encoding.ASCII.GetBytes(totalKey);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // First five nibbles must be 0
                    if ((hashBytes[0] == 0) && (hashBytes[1] == 0) && ((hashBytes[2] & 0xF0) == 0))
                    {
                        // index = 6th character
                        // value = 7th character
                        var i = hashBytes[2] & 0xF;
                        var c = (hashBytes[3] & 0xF0) >> 4;
                        if (c < 10)
                        {
                            return (i, (char)('0' + c));
                        }
                        else
                        {
                            return (i, (char)('a' + (c - 10)));
                        }
                    }
                    ++result;
                }
            }
            throw new InvalidProgramException($"No match found {result}");
        }

        public static string Password(string doorID)
        {
            var password = new char[8];
            for (var i = 0; i < 8; ++i)
            {
                password[i] = '*';
            }

            long start = 0;
            for (var i = 0; i < 8; ++i)
            {
                var c = FindFiveZeroHash(doorID, ref start);
                password[i] = c;
                Console.WriteLine($"{new string(password)}");
                ++start;
            }
            return new string(password);
        }

        public static string PasswordComplex(string doorID)
        {
            var password = new char[8];
            for (var i = 0; i < 8; ++i)
            {
                password[i] = '*';
            }

            long start = 0;
            int numToFind = 8;
            while (numToFind > 0)
            {
                (var index, var c) = FindFiveZeroHashComplex(doorID, ref start);
                if ((index >= 0) && (index < 8))
                {
                    if (password[index] == '*')
                    {
                        --numToFind;
                        password[index] = c;
                        Console.WriteLine($"{new string(password)}");
                    }
                }
                ++start;
            };
            return new string(password);
        }

        public static void Run()
        {
            Console.WriteLine("Day05 : Start");
            _ = new Program("Day05/input.txt", true);
            _ = new Program("Day05/input.txt", false);
            Console.WriteLine("Day05 : End");
        }
    }
}
