using System;

/*

--- Day 23: Safe Cracking ---

This is one of the top floors of the nicest tower in EBHQ. The Easter Bunny's private office is here, complete with a safe hidden behind a painting, and who wouldn't hide a star in a safe behind a painting?

The safe has a digital screen and keypad for code entry. 
A sticky note attached to the safe has a password hint on it: "eggs". 
The painting is of a large rabbit coloring some eggs. You see 7.

When you go to type the code, though, nothing appears on the display; instead, the keypad comes apart in your hands, apparently having been smashed. 
Behind it is some kind of socket - one that matches a connector in your prototype computer! 
You pull apart the smashed keypad and extract the logic circuit, plug it into your computer, and plug your computer into the safe.

Now, you just need to figure out what output the keypad would have sent to the safe. 
You extract the assembunny code from the logic chip (your puzzle input).
The code looks like it uses almost the same architecture and instruction set that the monorail computer used! 
You should be able to use the same assembunny interpreter for this as you did there, but with one new instruction:

tgl x toggles the instruction x away (pointing at instructions like jnz does: positive means forward; negative means backward):

For one-argument instructions, inc becomes dec, and all other one-argument instructions become inc.
For two-argument instructions, jnz becomes cpy, and all other two-instructions become jnz.
The arguments of a toggled instruction are not affected.
If an attempt is made to toggle an instruction outside the program, nothing happens.
If toggling produces an invalid instruction (like cpy 1 2) and an attempt is later made to execute that instruction, skip it instead.
If tgl toggles itself (for example, if a is 0, tgl a would target itself and become inc a), the resulting instruction is not executed until the next time it is reached.
For example, given this program:

cpy 2 a
tgl a
tgl a
tgl a
cpy 1 a
dec a
dec a
cpy 2 a initializes register a to 2.
The first tgl a toggles an instruction a (2) away from it, which changes the third tgl a into inc a.
The second tgl a also modifies an instruction 2 away from it, which changes the cpy 1 a into jnz 1 a.
The fourth line, which is now inc a, increments a to 3.
Finally, the fifth line, which is now jnz 1 a, jumps a (3) instructions ahead, skipping the dec a instructions.
In this example, the final value in register a is 3.

The rest of the electronics seem to place the keypad entry (the number of eggs, 7) in register a, run the code, and then send the value left in register a to the safe.

What value should be sent to the safe?

Your puzzle answer was 12624.

--- Part Two ---

The safe doesn't open, but it does make several angry noises to express its frustration.

You're quite sure your logic is working correctly, so the only other thing is... you check the painting again. 
As it turns out, colored eggs are still eggs. Now you count 12.

As you run the program with this new input, the prototype computer begins to overheat. 
You wonder what's taking so long, and whether the lack of any instruction more powerful than "add one" has anything to do with it. 
Don't bunnies usually multiply?

Anyway, what value should actually be sent to the safe?

*/

namespace Day23
{
    class Program
    {
        struct Command
        {
            public enum Instruction
            {
                CPY, INC, DEC, JNZ, TGL
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
        static readonly long[] sRegisters = new long[4];
        static readonly ulong MAX_CYCLE_COUNT = 8L * 1024 * 1024 * 1024;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                sRegisters[(int)Command.Register.A] = 7;
                RunProgram();
                var result1 = A;
                Console.WriteLine($"Day23 : Result1 {result1}");
                var expected = 12624;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                sRegisters[(int)Command.Register.A] = 12;
                RunProgram();
                var result2 = A;
                Console.WriteLine($"Day23 : Result2 {result2}");
                // 7 = 12624
                // 8 = 47904
                // 9 = 370464
                // 10 = 3636384
                // 11 = 39924384
                // 12 = 479009184
                var expected = 479009184;
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
                    command.reg1 = Command.Register.INVALID;
                    command.value1 = -1;
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
                    command.reg1 = Command.Register.INVALID;
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
                    var destinationRegister = ParseRegister(tokens[2]);
                    if (destinationRegister == Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid jnz instruction '{line}' invalid offset register {tokens[2]}");
                    }
                    if (destinationRegister == Command.Register.IMMEDIATE)
                    {
                        if (!ParseImmediate(tokens[2], out long offset))
                        {
                            throw new InvalidProgramException($"Invalid jnz instruction '{line}' invalid offset immediate {tokens[2]}");
                        }
                        command.value2 = offset;
                    }
                    command.reg2 = destinationRegister;
                }
                else if (instruction == "tgl")
                {
                    command.instruction = Command.Instruction.TGL;
                    if (tokens.Length != 2)
                    {
                        throw new InvalidProgramException($"Invalid tgl instruction '{line}' expected 2 tokens got {tokens.Length}");
                    }
                    var offsetRegister = ParseRegister(tokens[1]);
                    if ((offsetRegister == Command.Register.INVALID) || (offsetRegister == Command.Register.IMMEDIATE))
                    {
                        throw new InvalidProgramException($"Invalid tgl instruction '{line}' invalid offsetRegister register {tokens[1]}");
                    }
                    command.reg1 = Command.Register.INVALID;
                    command.value1 = -1;
                    command.reg2 = offsetRegister;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown instruction '{instruction}' {line}");
                }
            }
        }

        public static void RunProgram()
        {
            ulong cycles = 0;
            long pc = 0;
            long previousPC = -1;
            while ((pc >= 0) && (pc < sProgram.Length))
            {
                ++cycles;
                if (cycles > MAX_CYCLE_COUNT)
                {
                    throw new InvalidProgramException($"Ran out of cycles to run for {cycles}");

                }
                if ((cycles % (1L * 1024 * 1024 * 1024)) == 0)
                {
                    Console.WriteLine($"Cycles {cycles}");
                }
                if (pc == previousPC)
                {
                    throw new InvalidProgramException($"Invalid instruction processing PC did not change PC:{pc}");
                }
                previousPC = pc;
                var command = sProgram[pc];
                //cpy x y copies x (either an integer or the value of a register) into register y.
                //inc x increases the value of register x by one.
                //dec x decreases the value of register x by one.
                //jnz x y jumps to an instruction y away (positive & negative)
                if (command.reg2 == Command.Register.INVALID)
                {
                    throw new InvalidProgramException($"Invalid register2 instruction {command.instruction} PC:{pc}");
                }
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
                    if (command.reg2 == Command.Register.IMMEDIATE)
                    {
                        throw new InvalidProgramException($"Invalid CPY instruction target must be a register PC:{pc}");
                    }
                    sRegisters[(int)command.reg2] = value;
                    ++pc;
                }
                else if (command.instruction == Command.Instruction.INC)
                {
                    if (command.reg1 != Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid register1 instruction {command.instruction} PC:{pc}");
                    }
                    if (command.reg2 == Command.Register.IMMEDIATE)
                    {
                        throw new InvalidProgramException($"Invalid INC instruction target must be a register PC:{pc}");
                    }
                    sRegisters[(int)command.reg2] += 1;
                    ++pc;
                }
                else if (command.instruction == Command.Instruction.DEC)
                {
                    if (command.reg1 != Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid register1 instruction {command.instruction} PC:{pc}");
                    }
                    if (command.reg2 == Command.Register.IMMEDIATE)
                    {
                        throw new InvalidProgramException($"Invalid DEC instruction target must be a register PC:{pc}");
                    }
                    sRegisters[(int)command.reg2] -= 1;
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
                    long offset = 1;
                    if (value != 0)
                    {
                        if (command.reg2 == Command.Register.IMMEDIATE)
                        {
                            offset = command.value2;
                        }
                        else
                        {
                            offset = sRegisters[(int)command.reg2];
                        }
                    }
                    pc += (long)offset;
                }
                else if (command.instruction == Command.Instruction.TGL)
                {
                    if (command.reg1 != Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid register1 instruction {command.instruction} PC:{pc}");
                    }
                    if (command.reg2 == Command.Register.IMMEDIATE)
                    {
                        throw new InvalidProgramException($"Invalid TGL instruction target must be a register PC:{pc}");
                    }
                    long offset = sRegisters[(int)command.reg2];
                    long targetPC = pc + offset;
                    // If an attempt is made to toggle an instruction outside the program, nothing happens.
                    if ((targetPC >= 0) && (targetPC < sProgram.Length))
                    {
                        var sourceInstruction = sProgram[(long)targetPC].instruction;
                        var toggledInstruction = Command.Instruction.TGL;

                        // For one-argument instructions, inc becomes dec, and all other one-argument instructions become inc.
                        // For two-argument instructions, jnz becomes cpy, and all other two-instructions become jnz.
                        switch (sourceInstruction)
                        {
                            case Command.Instruction.CPY:
                                toggledInstruction = Command.Instruction.JNZ;
                                break;
                            case Command.Instruction.DEC:
                                toggledInstruction = Command.Instruction.INC;
                                break;
                            case Command.Instruction.INC:
                                toggledInstruction = Command.Instruction.DEC;
                                break;
                            case Command.Instruction.JNZ:
                                toggledInstruction = Command.Instruction.CPY;
                                break;
                            case Command.Instruction.TGL:
                                toggledInstruction = Command.Instruction.INC;
                                break;
                        }

                        if (toggledInstruction == Command.Instruction.TGL)
                        {
                            throw new InvalidProgramException($"Invalid toggledInstruction '{toggledInstruction}' must not be 'tgl'");
                        }
                        sProgram[(long)targetPC].instruction = toggledInstruction;
                    }
                    ++pc;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown instruction '{command.instruction}' PC:{pc}");
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

        public static long A { get { return (long)sRegisters[(long)Command.Register.A]; } }
        public static long B { get { return (long)sRegisters[(long)Command.Register.B]; } }
        public static long C { get { return (long)sRegisters[(long)Command.Register.C]; } }
        public static long D { get { return (long)sRegisters[(long)Command.Register.D]; } }

        public static void Run()
        {
            Console.WriteLine("Day23 : Start");
            _ = new Program("Day23/input.txt", true);
            _ = new Program("Day23/input.txt", false);
            Console.WriteLine("Day23 : End");
        }
    }
}
