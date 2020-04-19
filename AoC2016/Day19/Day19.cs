using System;

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

*/

namespace Day19
{
    class Program
    {
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
                var result1 = ElfWithPresents(elfCount);
                Console.WriteLine($"Day19 : Result1 {result1}");
                var expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day19 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static int ElfWithPresents(int elfCount)
        {
            return -1;
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
