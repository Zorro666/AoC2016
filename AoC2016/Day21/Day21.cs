using System;

/*

--- Day 21: Scrambled Letters and Hash ---

The computer system you're breaking into uses a weird scrambling function to store its passwords. 
It shouldn't be much trouble to create your own scrambled password so you can add it to the system; you just have to implement the scrambler.

The scrambling function is a series of operations (the exact list is provided in your puzzle input). 
Starting with the password to be scrambled, apply each operation in succession to the string. 
The individual operations behave as follows:

swap position X with position Y means that the letters at indexes X and Y (counting from 0) should be swapped.
swap letter X with letter Y means that the letters X and Y should be swapped (regardless of where they appear in the string).
rotate left/right X steps means that the whole string should be rotated; for example, one right rotation would turn abcd into dabc.
rotate based on position of letter X means that the whole string should be rotated to the right based on the index of letter X (counting from 0) as determined before this instruction does any rotations. 
Once the index is determined, rotate the string to the right one time, plus a number of times equal to that index, plus one additional time if the index was at least 4.
reverse positions X through Y means that the span of letters at indexes X through Y (including the letters at X and Y) should be reversed in order.
move position X to position Y means that the letter which is at index X should be removed from the string, then inserted such that it ends up at index Y.
For example, suppose you start with abcde and perform the following operations:

swap position 4 with position 0 swaps the first and last letters, producing the input for the next step, ebcda.
swap letter d with letter b swaps the positions of d and b: edcba.
reverse positions 0 through 4 causes the entire string to be reversed, producing abcde.
rotate left 1 step shifts all letters left one position, causing the first letter to wrap to the end of the string: bcdea.
move position 1 to position 4 removes the letter at position 1 (c), then inserts it at position 4 (the end of the string): bdeac.
move position 3 to position 0 removes the letter at position 3 (a), then inserts it at position 0 (the front of the string): abdec.
rotate based on position of letter b finds the index of letter b (1), then rotates the string right once plus a number of times equal to that index (2): ecabd.
rotate based on position of letter d finds the index of letter d (4), then rotates the string right once, plus a number of times equal to that index, plus an additional time because the index was at least 4, for a total of 6 right rotations: decab.
After these steps, the resulting scrambled password is decab.

Now, you just need to generate a new scrambled password and you can access the system. 
Given the list of scrambling operations in your puzzle input, what is the result of scrambling abcdefgh?

Your puzzle answer was dgfaehcb.

--- Part Two ---

You scrambled the password correctly, but you discover that you can't actually modify the password file on the system. You'll need to un-scramble one of the existing passwords by reversing the scrambling process.

What is the un-scrambled version of the scrambled password fbgdceah?
*/

namespace Day21
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);

            if (part1)
            {
                var result1 = Process("abcdefgh", lines);
                Console.WriteLine($"Day21 : Result1 {result1}");
                var expected = "dgfaehcb";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day21 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static string Process(string start, string[] lines)
        {
            var input = start;
            string output = null;
            foreach (var line in lines)
            {
                output = Operation(line, input);
                input = output;
            }
            return output;
        }

        static char ParseLetter(string letterText)
        {
            var letter = letterText[0];
            if ((letter < 'a') || (letter > 'h'))
            {
                throw new InvalidProgramException($"Invalid letter `{letter}' not in range a-h");
            }
            return letter;
        }

        static int ParseIndex(string indexText)
        {
            var index = int.Parse(indexText);
            if ((index < 0) || (index > 7))
            {
                throw new InvalidProgramException($"Invalid index `{indexText}' {index} not in range 0-4");
            }
            return index;
        }

        public static string Operation(string operation, string input)
        {
            var tokens = operation.Split(' ');
            var cmd = tokens[0];
            var inputChars = input.ToCharArray();
            var outputChars = input.ToCharArray();
            if (cmd == "swap")
            {
                //"swap position X with position Y" means that the letters at indexes X and Y (counting from 0) should be swapped.
                //"swap letter X with letter Y "means that the letters X and Y should be swapped (regardless of where they appear in the string).
                var option = tokens[1];
                if ((option != "position") && (option != "letter"))
                {
                    throw new InvalidProgramException($"Expected `position` or 'letter' got {tokens[1]} operation:{operation}");
                }
                if (tokens[3] != "with")
                {
                    throw new InvalidProgramException($"Expected `with` got {tokens[3]} operation:{operation}");
                }
                if (tokens[4] != option)
                {
                    throw new InvalidProgramException($"Expected `{option}` got {tokens[4]} operation:{operation}");
                }
                int indexA;
                int indexB;
                if (option == "position")
                {
                    indexA = ParseIndex(tokens[2]);
                    indexB = ParseIndex(tokens[5]);
                }
                else
                {
                    var letterA = ParseLetter(tokens[2]);
                    var letterB = ParseLetter(tokens[5]);
                    indexA = input.IndexOf(letterA);
                    indexB = input.IndexOf(letterB);
                }
                if ((indexA < 0) || (indexA >= input.Length))
                {
                    throw new InvalidProgramException($"Invalid index {indexA} {indexB} range:0-{input.Length} operation:{operation}");
                }
                var charA = inputChars[indexA];
                var charB = inputChars[indexB];
                outputChars[indexB] = charA;
                outputChars[indexA] = charB;
            }
            else if (cmd == "rotate")
            {
                //"rotate left/right X steps "means that the whole string should be rotated; for example, one right rotation would turn abcd into dabc.
                //"rotate based on position of letter X "means that the whole string should be rotated to the right based on the index of letter X (counting from 0) as determined before this instruction does any rotations. 
                //Once the index is determined, rotate the string to the right one time, plus a number of times equal to that index, plus one additional time if the index was at least 4.
                var option = tokens[1];
                if ((option != "left") && (option != "right") && (option != "based"))
                {
                    throw new InvalidProgramException($"Expected `left` or 'right' or 'based' got {tokens[1]} operation:{operation}");
                }
                int count = -1;
                if ((option == "left") || (option == "right"))
                {
                    if ((tokens[3] != "steps") && (tokens[3] != "step"))
                    {
                        throw new InvalidProgramException($"Expected `steps` or `step` got {tokens[3]} operation:{operation}");
                    }
                    count = int.Parse(tokens[2]);
                    if (option == "left")
                    {
                        count = -count;
                    }
                }
                else
                {
                    if (tokens[2] != "on")
                    {
                        throw new InvalidProgramException($"Expected `on` got {tokens[2]} operation:{operation}");
                    }
                    if (tokens[3] != "position")
                    {
                        throw new InvalidProgramException($"Expected `position` got {tokens[3]} operation:{operation}");
                    }
                    if (tokens[4] != "of")
                    {
                        throw new InvalidProgramException($"Expected `of` got {tokens[4]} operation:{operation}");
                    }
                    if (tokens[5] != "letter")
                    {
                        throw new InvalidProgramException($"Expected `letter` got {tokens[5]} operation:{operation}");
                    }
                    var letter = ParseLetter(tokens[6]);
                    var index = input.IndexOf(letter);
                    count = 1 + index;
                    if (index >= 4)
                    {
                        ++count;
                    }
                }
                for (var i = 0; i < input.Length; ++i)
                {
                    var inputChar = inputChars[i];
                    var outputIndex = i + count;
                    while (outputIndex < 0)
                    {
                        outputIndex += input.Length;
                    }
                    outputIndex %= input.Length;
                    outputChars[outputIndex] = inputChar;
                }
            }
            else if (cmd == "reverse")
            {
                //"reverse positions X through Y" means that the span of letters at indexes X through Y (including the letters at X and Y) should be reversed in order.
                if (tokens[1] != "positions")
                {
                    throw new InvalidProgramException($"Expected `positions` got {tokens[1]} operation:{operation}");
                }
                if (tokens[3] != "through")
                {
                    throw new InvalidProgramException($"Expected `through` got {tokens[3]} operation:{operation}");
                }
                var indexA = ParseIndex(tokens[2]);
                var indexB = ParseIndex(tokens[4]);
                for (var i = indexA; i <= indexB; ++i)
                {
                    var inputChar = inputChars[i];
                    var outputIndex = indexB - (i - indexA);
                    outputChars[outputIndex] = inputChar;
                }
            }
            else if (cmd == "move")
            {
                //"move position X to position Y" means that the letter which is at index X should be removed from the string, then inserted such that it ends up at index Y.
                if (tokens[1] != "position")
                {
                    throw new InvalidProgramException($"Expected `position` got {tokens[1]} operation:{operation}");
                }
                if (tokens[3] != "to")
                {
                    throw new InvalidProgramException($"Expected `to` got {tokens[3]} operation:{operation}");
                }
                if (tokens[4] != "position")
                {
                    throw new InvalidProgramException($"Expected `position` got {tokens[4]} operation:{operation}");
                }
                var indexA = ParseIndex(tokens[2]);
                var indexB = ParseIndex(tokens[5]);
                var movedChar = inputChars[indexA];
                // Remove from source
                for (var i = indexA; i < inputChars.Length - 1; ++i)
                {
                    var inputChar = inputChars[i + 1];
                    var outputIndex = i;
                    outputChars[outputIndex] = inputChar;
                }
                // Insert at destination
                for (var i = inputChars.Length - 2; i >= indexB; --i)
                {
                    var inputChar = outputChars[i];
                    var outputIndex = i + 1;
                    outputChars[outputIndex] = inputChar;
                }
                outputChars[indexB] = movedChar;
            }
            else
            {
                throw new InvalidProgramException($"Unknown command {cmd} operation:{operation}");
            }
            return new string(outputChars);
        }

        public static void Run()
        {
            Console.WriteLine("Day21 : Start");
            _ = new Program("Day21/input.txt", true);
            _ = new Program("Day21/input.txt", false);
            Console.WriteLine("Day21 : End");
        }
    }
}
