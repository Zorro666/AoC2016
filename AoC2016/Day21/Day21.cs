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
        struct Operation
        {
            public enum Command { SWAP, ROTATE, MOVE, REVERSE };
            public Command command;
            public int indexA;
            public int indexB;
            public char letterA;
            public char letterB;
            public int rotateCount;
        }

        static Operation[] sOperations;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            var start = "abcdefgh".ToCharArray();
            var scramble = new char[start.Length];
            Scramble(start, ref scramble);

            if (part1)
            {
                var result1 = new string(scramble);
                Console.WriteLine($"Day21 : Result1 {result1}");
                var expected = "bfheacgd";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var scramble2 = "fbgdceah".ToCharArray();
                var unscramble2 = new char[scramble2.Length];
                UnScramble(in scramble2, ref unscramble2);
                var checkScramble2 = new char[scramble2.Length];
                Scramble(unscramble2, ref checkScramble2);
                if (new string(checkScramble2) != new string(scramble2))
                {
                    throw new InvalidProgramException($"Part2 is broken {new string(checkScramble2)} != {new string(scramble2)}");
                }
                var result2 = new string(unscramble2);
                Console.WriteLine($"Day21 : Result2 {result2}");
                var expected = "gcehdbfa";
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void UnScramble(in char[] scrambled, ref char[] unscrambled)
        {
            var wordLength = scrambled.Length;
            var testInput = new char[wordLength];
            var testResult = new char[wordLength];
            var counts = new int[wordLength];
            for (var i = 0; i < wordLength; ++i)
            {
                testInput[i] = (char)('a' + i);
            }
            bool doMore = true;

            do
            {
                Scramble(testInput, ref testResult);
                bool same = true;
                for (var i = 0; i < wordLength; ++i)
                {
                    if (testResult[i] != scrambled[i])
                    {
                        same = false;
                        break;
                    }
                }
                if (same == true)
                {
                    unscrambled = testInput;
                    return;
                }
                bool validCombination = false;
                // Next combination
                while (!validCombination)
                {
                    int carry = 1;
                    for (var i = 0; i < wordLength; ++i)
                    {
                        int oldValue = testInput[i] - 'a';
                        int newValue = oldValue + carry;
                        if (newValue >= wordLength)
                        {
                            carry = 1;
                            newValue = 0;
                        }
                        else
                        {
                            carry = 0;
                        }
                        testInput[i] = (char)('a' + newValue);
                        if (carry == 0)
                        {
                            break;
                        }
                    }
                    for (var i = 0; i < wordLength; ++i)
                    {
                        counts[i] = 0;
                    }

                    for (var i = 0; i < wordLength; ++i)
                    {
                        int value = testInput[i] - 'a';
                        counts[value] = 1;
                    }
                    validCombination = true;
                    for (var i = 0; i < wordLength; ++i)
                    {
                        if (counts[i] != 1)
                        {
                            validCombination = false;
                            break;
                        }
                    }
                }
            }
            while (doMore);
            throw new InvalidProgramException($"UnScramble failed to find an answer");
        }

        public static void Parse(string[] lines)
        {
            sOperations = new Operation[lines.Length];
            for (var o = 0; o < sOperations.Length; o++)
            {
                sOperations[o] = ParseOperation(lines[o]);
            }
        }

        static void CheckBuffer(char[] buffer)
        {
            var bufferLength = buffer.Length;
            var counts = new int[bufferLength];
            for (var i = 0; i < bufferLength; ++i)
            {
                var c = buffer[i];
                var cIndex = c - 'a';
                if ((cIndex < 0) || (cIndex > bufferLength))
                {
                    throw new InvalidProgramException($"CheckBuffer failed [{i}] '{cIndex}' range {0} -> {bufferLength}");
                }
                counts[cIndex] = 1;
            }
            for (var i = 0; i < bufferLength; ++i)
            {
                if (counts[i] != 1)
                {
                    throw new InvalidProgramException($"CheckBuffer failed [{i}] Count {counts[i]}");
                }
            }
        }

        public static void Scramble(in char[] start, ref char[] output)
        {
            var numChars = start.Length;
            char[] buffer1 = new char[start.Length];
            char[] buffer2 = new char[start.Length];
            for (var i = 0; i < numChars; ++i)
            {
                var c = start[i];
                buffer1[i] = c;
                buffer2[i] = c;
            }
            var buffers = new char[2][] { buffer1, buffer2 };
            var inIndex = 0;
            var outIndex = 1;
            foreach (var operation in sOperations)
            {
                var temp = inIndex;
                inIndex = outIndex;
                outIndex = temp;
                ProcessOperation(operation, buffers[inIndex], ref buffers[outIndex]);
                CheckBuffer(buffer1);
                CheckBuffer(buffer2);
            }
            for (var i = 0; i < numChars; ++i)
            {
                var outputBuffer = buffers[outIndex];
                var c = outputBuffer[i];
                output[i] = c;
            }
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

        static int FindIndexOf(in char[] input, in char letter)
        {
            for (var i = 0; i < input.Length; ++i)
            {
                if (input[i] == letter)
                {
                    return i;
                }
            }
            return -1;
        }

        static void ProcessOperation(Operation operation, in char[] input, ref char[] output)
        {
            for (var i = 0; i < input.Length; ++i)
            {
                output[i] = input[i];
            }
            var cmd = operation.command;
            if (cmd == Operation.Command.SWAP)
            {
                var indexA = operation.indexA;
                var indexB = operation.indexB;
                if ((indexA < 0) && (indexB < 0))
                {
                    indexA = FindIndexOf(input, operation.letterA);
                    indexB = FindIndexOf(input, operation.letterB);
                }
                if ((indexA < 0) || (indexA >= input.Length))
                {
                    throw new InvalidProgramException($"Invalid indexA {indexA} range:0-{input.Length}");
                }
                if ((indexB < 0) || (indexB >= input.Length))
                {
                    throw new InvalidProgramException($"Invalid indexB {indexB} range:0-{input.Length}");
                }
                var charA = input[indexA];
                var charB = input[indexB];
                output[indexB] = charA;
                output[indexA] = charB;
            }
            else if (cmd == Operation.Command.ROTATE)
            {
                var count = operation.rotateCount;
                var letter = operation.letterA;
                if ((count == -1) && (letter != '0'))
                {
                    var index = FindIndexOf(input, letter);
                    if ((index < 0) || (index >= input.Length))
                    {
                        throw new InvalidProgramException($"Invalid index {index} range:0-{input.Length}");
                    }
                    count = 1 + index;
                    if (index >= 4)
                    {
                        ++count;
                    }
                }
                for (var i = 0; i < input.Length; ++i)
                {
                    var inputChar = input[i];
                    var outputIndex = i + count;
                    while (outputIndex < 0)
                    {
                        outputIndex += input.Length;
                    }
                    outputIndex %= input.Length;
                    output[outputIndex] = inputChar;
                }
            }
            else if (cmd == Operation.Command.REVERSE)
            {
                var indexA = operation.indexA;
                var indexB = operation.indexB;
                for (var i = indexA; i <= indexB; ++i)
                {
                    var inputChar = input[i];
                    var outputIndex = indexB - (i - indexA);
                    output[outputIndex] = inputChar;
                }
            }
            else if (cmd == Operation.Command.MOVE)
            {
                var indexA = operation.indexA;
                var indexB = operation.indexB;
                var movedChar = input[indexA];
                // Remove from source
                for (var i = indexA; i < input.Length - 1; ++i)
                {
                    var inputChar = input[i + 1];
                    var outputIndex = i;
                    output[outputIndex] = inputChar;
                }
                // Insert at destination
                for (var i = input.Length - 2; i >= indexB; --i)
                {
                    var inputChar = output[i];
                    var outputIndex = i + 1;
                    output[outputIndex] = inputChar;
                }
                output[indexB] = movedChar;
            }
            else
            {
                throw new InvalidProgramException($"Unknown command {cmd} operation:{operation}");
            }
        }

        static Operation ParseOperation(string operation)
        {
            var tokens = operation.Split(' ');
            var cmd = tokens[0];

            Operation op;
            op.indexA = -1;
            op.indexB = -1;
            op.rotateCount = -1;
            op.letterA = '0';
            op.letterB = '0';

            if (cmd == "swap")
            {
                op.command = Operation.Command.SWAP;
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
                if (option == "position")
                {
                    op.indexA = ParseIndex(tokens[2]);
                    op.indexB = ParseIndex(tokens[5]);
                }
                else
                {
                    op.letterA = ParseLetter(tokens[2]);
                    op.letterB = ParseLetter(tokens[5]);
                }
            }
            else if (cmd == "rotate")
            {
                op.command = Operation.Command.ROTATE;
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
                    op.rotateCount = count;
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
                    op.letterA = ParseLetter(tokens[6]);
                }
            }
            else if (cmd == "reverse")
            {
                op.command = Operation.Command.REVERSE;
                //"reverse positions X through Y" means that the span of letters at indexes X through Y (including the letters at X and Y) should be reversed in order.
                if (tokens[1] != "positions")
                {
                    throw new InvalidProgramException($"Expected `positions` got {tokens[1]} operation:{operation}");
                }
                if (tokens[3] != "through")
                {
                    throw new InvalidProgramException($"Expected `through` got {tokens[3]} operation:{operation}");
                }
                op.indexA = ParseIndex(tokens[2]);
                op.indexB = ParseIndex(tokens[4]);
            }
            else if (cmd == "move")
            {
                op.command = Operation.Command.MOVE;
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
                op.indexA = ParseIndex(tokens[2]);
                op.indexB = ParseIndex(tokens[5]);
            }
            else
            {
                throw new InvalidProgramException($"Unknown command {cmd} operation:{operation}");
            }
            return op;
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
