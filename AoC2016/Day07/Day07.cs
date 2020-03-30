using System;

/*

--- Day 7: Internet Protocol Version 7 ---

While snooping around the local network of EBHQ, you compile a list of IP addresses (they're IPv7, of course; IPv6 is much too limited). 
You'd like to figure out which IPs support TLS (transport-layer snooping).

An IP supports TLS if it has an Autonomous Bridge Bypass Annotation, or ABBA. An ABBA is any four-character sequence which consists of a pair of two different characters followed by the reverse of that pair, such as xyyx or abba. 
However, the IP also must not have an ABBA within any hypernet sequences, which are contained by square brackets.

For example:

abba[mnop]qrst supports TLS (abba outside square brackets).
abcd[bddb]xyyx does not support TLS (bddb is within square brackets, even though xyyx is outside square brackets).
aaaa[qwer]tyui does not support TLS (aaaa is invalid; the interior characters must be different).
ioxxoj[asdfgh]zxcvbn supports TLS (oxxo is outside square brackets, even though it's within a larger string).

How many IPs in your puzzle input support TLS?

Your puzzle answer was 115.

--- Part Two ---

You would also like to know which IPs support SSL (super-secret listening).

An IP supports SSL if it has an Area-Broadcast Accessor, or ABA, anywhere in the supernet sequences (outside any square bracketed sections), and a corresponding Byte Allocation Block, or BAB, anywhere in the hypernet sequences. 
An ABA is any three-character sequence which consists of the same character twice with a different character between them, such as xyx or aba. A corresponding BAB is the same characters but in reversed positions: yxy and bab, respectively.

For example:

aba[bab]xyz supports SSL (aba outside square brackets with corresponding bab within square brackets).
xyx[xyx]xyx does not support SSL (xyx, but no corresponding yxy).
aaa[kek]eke supports SSL (eke in supernet with corresponding kek in hypernet; the aaa sequence is not related, because the interior character must be different).
zazbz[bzb]cdb supports SSL (zaz has no corresponding aza, but zbz has a corresponding bzb, even though zaz and zbz overlap).

How many IPs in your puzzle input support SSL?

*/

namespace Day07
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (part1)
            {
                var result1 = CountTCPs(lines);
                Console.WriteLine($"Day07 : Result1 {result1}");
                var expected = 115;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = CountSSLs(lines);
                Console.WriteLine($"Day07 : Result2 {result2}");
                var expected = 231;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static int CountTCPs(string[] lines)
        {
            int count = 0;
            foreach (var line in lines)
            {
                if (SupportsTCP(line))
                {
                    ++count;
                }
            }
            return count;
        }

        static int CountSSLs(string[] lines)
        {
            int count = 0;
            foreach (var line in lines)
            {
                if (SupportsSSL(line))
                {
                    ++count;
                }
            }
            return count;
        }

        public static bool SupportsTCP(string ip)
        {
            //abba[mnop]qrst supports TLS (abba outside square brackets).
            //abcd[bddb]xyyx does not support TLS (bddb is within square brackets, even though xyyx is outside square brackets).
            //aaaa[qwer]tyui does not support TLS (aaaa is invalid; the interior characters must be different).
            //ioxxoj[asdfgh]zxcvbn supports TLS (oxxo is outside square brackets, even though it's within a larger string).
            bool foundABBA = false;
            bool insideSquareBrackets = false;
            bool invalid = false;
            for (var i = 0; i < ip.Length - 3; ++i)
            {
                if (ip[i] == '[')
                {
                    insideSquareBrackets = true;
                }
                if (insideSquareBrackets && (ip[i] == ']'))
                {
                    insideSquareBrackets = false;
                }
                if (ip[i + 0] == ip[i + 3])
                {
                    if (ip[i + 1] == ip[i + 2])
                    {
                        if (ip[i + 0] != ip[i + 1])
                        {
                            if (!insideSquareBrackets)
                            {
                                foundABBA = true;
                            }
                            else
                            {
                                invalid = true;
                            }
                        }
                    }
                }
            }
            return foundABBA && !invalid;
        }

        public static bool SupportsSSL(string ip)
        {
            //aba[bab]xyz supports SSL (aba outside square brackets with corresponding bab within square brackets).
            //xyx[xyx]xyx does not support SSL (xyx, but no corresponding yxy).
            //aaa[kek]eke supports SSL (eke in supernet with corresponding kek in hypernet; the aaa sequence is not related, because the interior character must be different).
            //zazbz[bzb]cdb supports SSL (zaz has no corresponding aza, but zbz has a corresponding bzb, even though zaz and zbz overlap).
            int foundBABCount = 0;
            var As = new char[ip.Length];
            var Bs = new char[ip.Length];
            bool insideSquareBrackets = false;
            for (var i = 0; i < ip.Length - 2; ++i)
            {
                if (ip[i] == '[')
                {
                    insideSquareBrackets = true;
                }
                if (insideSquareBrackets && (ip[i] == ']'))
                {
                    insideSquareBrackets = false;
                }
                if (insideSquareBrackets)
                {
                    if (ip[i + 0] == ip[i + 2])
                    {
                        if (ip[i + 0] != ip[i + 1])
                        {
                            Bs[foundBABCount] = ip[i + 0];
                            As[foundBABCount] = ip[i + 1];
                            ++foundBABCount;
                        }
                    }
                }
            }
            if (foundBABCount == 0)
            {
                return false;
            }

            insideSquareBrackets = false;
            for (var i = 0; i < ip.Length - 2; ++i)
            {
                if (ip[i] == '[')
                {
                    insideSquareBrackets = true;
                }
                if (insideSquareBrackets && (ip[i] == ']'))
                {
                    insideSquareBrackets = false;
                }
                if (!insideSquareBrackets)
                {
                    if (ip[i + 0] == ip[i + 2])
                    {
                        if (ip[i + 0] != ip[i + 1])
                        {
                            for (var bab = 0; bab < foundBABCount; ++bab)
                            {
                                var a = As[bab];
                                var b = Bs[bab];
                                if ((a == ip[i + 0]) && (b == ip[i + 1]))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void Run()
        {
            Console.WriteLine("Day07 : Start");
            _ = new Program("Day07/input.txt", true);
            _ = new Program("Day07/input.txt", false);
            Console.WriteLine("Day07 : End");
        }
    }
}
