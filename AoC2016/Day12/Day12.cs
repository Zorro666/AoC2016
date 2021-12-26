using System;

/*

--- Day 12: Leonardo's Monorail ---

You finally reach the top floor of this building: a garden with a slanted glass ceiling. 
Looks like there are no more stars to be had.

While sitting on a nearby bench amidst some tiger lilies, you manage to decrypt some of the files you extracted from the servers downstairs.

According to these documents, Easter Bunny HQ isn't just this building - it's a collection of buildings in the nearby area. 
They're all connected by a local monorail, and there's another building not far from here! Unfortunately, being night, the monorail is currently not operating.

You remotely connect to the monorail control systems and discover that the boot sequence expects a password. 
The password-checking logic (your puzzle input) is easy to extract, but the code it uses is strange: it's assembunny code designed for the new computer you just assembled. 
You'll have to execute the code and get the password.

The assembunny code you've extracted operates on four registers (a, b, c, and d) that start at 0 and can hold any integer. 
However, it seems to make use of only a few instructions:

cpy x y copies x (either an integer or the value of a register) into register y.
inc x increases the value of register x by one.
dec x decreases the value of register x by one.
jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.
The jnz instruction moves relative to itself: an offset of -1 would continue at the previous instruction, while an offset of 2 would skip over the next instruction.

For example:

cpy 41 a
inc a
inc a
dec a
jnz a 2
dec a
The above code would set register a to 41, increase its value by 2, decrease its value by 1, and then skip the last dec a (because a is not zero, so the jnz a 2 skips it), leaving register a at 42. 
When you move past the last instruction, the program halts.

After executing the assembunny code in your puzzle input, what value is left in register a?

Your puzzle answer was 318077.

--- Part Two ---

As you head down the fire escape to the monorail, you notice it didn't start; register c needs to be initialized to the position of the ignition key.

If you instead initialize register c to be 1, what value is now left in register a?

*/

namespace Day12
{
    class Program
    {
        struct Command
        {
            public enum Instruction
            {
                CPY, INC, DEC, JNZ
            };
            public enum Register
            {
                A = 0, B = 1, C = 2, D = 3, INVALID, IMMEDIATE
            };
            public Instruction instruction;
            public Register reg1;
            public Register reg2;
            public long value1;
            public long value2;
        };

        static Command[] sProgram;
        static long[] sRegisters = new long[4];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                RunProgram();
                var result1 = A;
                Console.WriteLine($"Day12 : Result1 {result1}");
                var expected = 318020;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                sRegisters[(long)Command.Register.C] = 1;
                RunProgram();
                var result2 = A;
                Console.WriteLine($"Day12 : Result2 {result2}");
                var expected = 9227674;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sProgram = new Command[lines.Length];
            for (var i = 0; i < lines.Length; ++i)
            {
                //cpy x y copies x (either an integer or the value of a register) into register y.
                //inc x increases the value of register x by one.
                //dec x decreases the value of register x by one.
                //jnz x y jumps to an instruction y away (positive & negative)
                var line = lines[i];
                var tokens = line.Split(' ');
                var instruction = tokens[0];
                ref var command = ref sProgram[i];
                if (instruction == "cpy")
                {
                    command.instruction = Command.Instruction.CPY;
                    if (tokens.Length != 3)
                    {
                        throw new InvalidProgramException($"Invalid cpy instruction '{line}' expected 3 tokens got {tokens.Length}");
                    }
                    var sourceRegister = ParseRegister(tokens[1]);
                    if (sourceRegister == Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid cpy instruction '{line}' invalid source register {tokens[1]}");
                    }
                    if (sourceRegister == Command.Register.IMMEDIATE)
                    {
                        if (!ParseImmediate(tokens[1], out long value))
                        {
                            throw new InvalidProgramException($"Invalid cpy instruction '{line}' invalid source immediate {tokens[1]}");
                        }
                        command.value1 = value;
                    }
                    command.reg1 = sourceRegister;
                    var destinationRegister = ParseRegister(tokens[2]);
                    if ((destinationRegister == Command.Register.INVALID) || (destinationRegister == Command.Register.IMMEDIATE))
                    {
                        throw new InvalidProgramException($"Invalid cpy instruction '{line}' invalid destination register {tokens[2]}");
                    }
                    command.reg2 = destinationRegister;
                }
                else if (instruction == "inc")
                {
                    command.instruction = Command.Instruction.INC;
                    if (tokens.Length != 2)
                    {
                        throw new InvalidProgramException($"Invalid inc instruction '{line}' expected 2 tokens got {tokens.Length}");
                    }
                    var destinationRegister = ParseRegister(tokens[1]);
                    if ((destinationRegister == Command.Register.INVALID) || (destinationRegister == Command.Register.IMMEDIATE))
                    {
                        throw new InvalidProgramException($"Invalid inc instruction '{line}' invalid destination register {tokens[1]}");
                    }
                    command.reg1 = Command.Register.IMMEDIATE;
                    command.value1 = +1;
                    command.reg2 = destinationRegister;
                }
                else if (instruction == "dec")
                {
                    command.instruction = Command.Instruction.DEC;
                    if (tokens.Length != 2)
                    {
                        throw new InvalidProgramException($"Invalid dec instruction '{line}' expected 2 tokens got {tokens.Length}");
                    }
                    var destinationRegister = ParseRegister(tokens[1]);
                    if ((destinationRegister == Command.Register.INVALID) || (destinationRegister == Command.Register.IMMEDIATE))
                    {
                        throw new InvalidProgramException($"Invalid dec instruction '{line}' invalid destination register {tokens[1]}");
                    }
                    command.reg1 = Command.Register.IMMEDIATE;
                    command.value1 = -1;
                    command.reg2 = destinationRegister;
                }
                else if (instruction == "jnz")
                {
                    command.instruction = Command.Instruction.JNZ;
                    if (tokens.Length != 3)
                    {
                        throw new InvalidProgramException($"Invalid jnz instruction '{line}' expected 3 tokens got {tokens.Length}");
                    }
                    var sourceRegister = ParseRegister(tokens[1]);
                    if (sourceRegister == Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid jnz instruction '{line}' invalid source register {tokens[1]}");
                    }
                    if (sourceRegister == Command.Register.IMMEDIATE)
                    {
                        if (!ParseImmediate(tokens[1], out long value))
                        {
                            throw new InvalidProgramException($"Invalid jnz instruction '{line}' invalid source immediate {tokens[1]}");
                        }
                        command.value1 = value;
                    }
                    command.reg1 = sourceRegister;
                    if (!ParseImmediate(tokens[2], out long offset))
                    {
                        throw new InvalidProgramException($"Invalid jnz instruction '{line}' invalid offset {tokens[2]}");
                    }
                    command.reg2 = Command.Register.IMMEDIATE;
                    command.value2 = offset;
                }
            }
        }

        public static void RunProgram()
        {
            long pc = 0;
            while ((pc >= 0) && (pc < sProgram.Length))
            {
                var command = sProgram[pc];
                //cpy x y copies x (either an integer or the value of a register) into register y.
                //inc x increases the value of register x by one.
                //dec x decreases the value of register x by one.
                //jnz x y jumps to an instruction y away (positive & negative)
                if (command.instruction == Command.Instruction.CPY)
                {
                    long value;
                    if (command.reg1 == Command.Register.IMMEDIATE)
                    {
                        value = command.value1;
                    }
                    else
                    {
                        value = sRegisters[(int)command.reg1];
                    }
                    sRegisters[(int)command.reg2] = value;
                    ++pc;
                }
                else if (command.instruction == Command.Instruction.INC)
                {
                    sRegisters[(int)command.reg2] += command.value1;
                    ++pc;
                }
                else if (command.instruction == Command.Instruction.DEC)
                {
                    sRegisters[(int)command.reg2] += command.value1;
                    ++pc;
                }
                else if (command.instruction == Command.Instruction.JNZ)
                {
                    long value;
                    if (command.reg1 == Command.Register.IMMEDIATE)
                    {
                        value = command.value1;
                    }
                    else
                    {
                        value = sRegisters[(int)command.reg1];
                    }
                    if (value != 0)
                    {
                        pc += command.value2;
                    }
                    else
                    {
                        ++pc;
                    }
                }
            }
        }

        static Command.Register ParseRegister(string token)
        {
            if (ParseImmediate(token, out long _))
            {
                return Command.Register.IMMEDIATE;
            }
            if (token.Length > 1)
            {
                return Command.Register.INVALID;
            }
            var c = token[0];
            if (c == 'a')
            {
                return Command.Register.A;
            }
            else if (c == 'b')
            {
                return Command.Register.B;
            }
            else if (c == 'c')
            {
                return Command.Register.C;
            }
            else if (c == 'd')
            {
                return Command.Register.D;
            }
            else
            {
                return Command.Register.INVALID;
            }
        }

        static bool ParseImmediate(string token, out long value)
        {
            return long.TryParse(token, out value);
        }

        public static long A { get { return sRegisters[(long)Command.Register.A]; } }
        public static long B { get { return sRegisters[(long)Command.Register.B]; } }
        public static long C { get { return sRegisters[(long)Command.Register.C]; } }
        public static long D { get { return sRegisters[(long)Command.Register.D]; } }

        public static void Run()
        {
            Console.WriteLine("Day12 : Start");
            _ = new Program("Day12/input.txt", true);
            _ = new Program("Day12/input.txt", false);
            Console.WriteLine("Day12 : End");
        }
    }
}
