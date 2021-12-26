using System;

/*

--- Day 25: Clock Signal ---

You open the door and find yourself on the roof.
The city sprawls away from you for miles and miles.

There's not much time now - it's already Christmas, but you're nowhere near the North Pole, much too far to deliver these stars to the sleigh in time.

However, maybe the huge antenna up here can offer a solution.
After all, the sleigh doesn't need the stars, exactly; it needs the timing data they provide, and you happen to have a massive signal generator right here.

You connect the stars you have to your prototype computer, connect that to the antenna, and begin the transmission.

Nothing happens.

You call the service number printed on the side of the antenna and quickly explain the situation.
"I'm not sure what kind of equipment you have connected over there," he says, "but you need a clock signal." You try to explain that this is a signal for a clock.

"No, no, a clock signal - timing information so the antenna computer knows how to read the data you're sending it.
An endless, alternating pattern of 0, 1, 0, 1, 0, 1, 0, 1, 0, 1...." He trails off.

You ask if the antenna can handle a clock signal at the frequency you would need to use for the data from the stars.
"There's no way it can! The only antenna we've installed capable of that is on top of a top-secret Easter Bunny installation, and you're definitely not-" You hang up the phone.

You've extracted the antenna's clock signal generation assembunny code (your puzzle input); it looks mostly compatible with code you worked on just recently.

This antenna code, being a signal generator, uses one extra instruction:

out x transmits x (either an integer or the value of a register) as the next value for the clock signal.
The code takes a value (via register a) that describes the signal to generate, but you're not sure how it's used.
You'll have to find the input to produce the right signal through experimentation.

What is the lowest positive integer that can be used to initialize register a and cause the code to output a clock signal of 0, 1, 0, 1... repeating forever?

*/

namespace Day25
{
    class Program
    {
        struct Command
        {
            public enum Instruction
            {
                CPY, INC, DEC, JNZ, TGL, OUT
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
        static Command[] sSavedProgram;
        static readonly long[] sRegisters = new long[4];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = FindInputSignal();
                Console.WriteLine($"Day25 : Result1 {result1}");
                var expected = 180;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                throw new InvalidProgramException($"Part2 is not supported");
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
                // out x transmits x (either an integer or the value of a register) as the next value for the clock signal.
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
                else if (instruction == "out")
                {
                    command.instruction = Command.Instruction.OUT;
                    if (tokens.Length != 2)
                    {
                        throw new InvalidProgramException($"Invalid out instruction '{line}' expected 2 tokens got {tokens.Length}");
                    }
                    var outRegister = ParseRegister(tokens[1]);
                    if (outRegister == Command.Register.INVALID)
                    {
                        throw new InvalidProgramException($"Invalid out instruction '{line}' invalid out register {tokens[1]}");
                    }
                    if (outRegister == Command.Register.IMMEDIATE)
                    {
                        if (!ParseImmediate(tokens[1], out long value))
                        {
                            throw new InvalidProgramException($"Invalid out instruction '{line}' invalid out immediate {tokens[1]}");
                        }
                        command.value2 = value;
                    }
                    command.reg1 = Command.Register.INVALID;
                    command.value1 = -1;
                    command.reg2 = outRegister;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown instruction '{instruction}' {line}");
                }
            }
            sSavedProgram = new Command[sProgram.Length];
        }

        public static bool RunProgram(ulong maxCycles)
        {
            ulong cycles = 0;
            long pc = 0;
            long previousPC = -1;
            long expectedOutput = 0;
            while ((pc >= 0) && (pc < sProgram.Length))
            {
                ++cycles;
                if (cycles >= maxCycles)
                {
                    return true;
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
                            case Command.Instruction.OUT:
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
                else if (command.instruction == Command.Instruction.OUT)
                {
                    long output;
                    if (command.reg2 == Command.Register.IMMEDIATE)
                    {
                        output = command.value2;
                    }
                    else
                    {
                        output = sRegisters[(int)command.reg2];
                    }
                    if (output != expectedOutput)
                    {
                        return false;
                    }
                    expectedOutput ^= 1;
                    ++pc;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown instruction '{command.instruction}' PC:{pc}");
                }
            }
            return true;
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

        static bool TestInputSignal(long a, ulong maxCycles)
        {
            for (var i = 0; i < sProgram.Length; ++i)
            {
                sSavedProgram[i] = sProgram[i];
            }
            for (var i = 0; i < 4; ++i)
            {
                sRegisters[i] = 0;
            }

            sRegisters[0] = a;
            bool result = RunProgram(maxCycles);

            for (var i = 0; i < sProgram.Length; ++i)
            {
                sProgram[i] = sSavedProgram[i];
            }
            return result;
        }

        static long FindInputSignal()
        {
            var maxA = 1024 * 1024;
            ulong maxCycles = 128 * 1024 * 1024;

            for (var a = 0; a < maxA; ++a)
            {
                if ((a % 1000) == 0)
                {
                    Console.WriteLine($"a {a} maxCycles:{maxCycles}");
                }
                if (TestInputSignal(a, maxCycles))
                {
                    return a;
                }
            }
            throw new InvalidProgramException($"Failed to find a matching input signal max:{maxA}");
        }

        public static void Run()
        {
            Console.WriteLine("Day25 : Start");
            _ = new Program("Day25/input.txt", true);
            Console.WriteLine("Day25 : End");
        }
    }
}
