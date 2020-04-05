using System;

/*

--- Day 10: Balance Bots ---

You come upon a factory in which many robots are zooming around handing small microchips to each other.

Upon closer examination, you notice that each bot only proceeds when it has two microchips, and once it does, it gives each one to a different bot or puts it in a marked "output" bin. 
Sometimes, bots take microchips from "input" bins, too.

Inspecting one of the microchips, it seems like they each contain a single number; the bots must use some logic to decide what to do with each chip. 
You access the local control computer and download the bots' instructions (your puzzle input).

Some of the instructions specify that a specific-valued microchip should be given to a specific bot; the rest of the instructions indicate what a given bot should do with its lower-value or higher-value chip.

For example, consider the following instructions:

value 5 goes to bot 2
bot 2 gives low to bot 1 and high to bot 0
value 3 goes to bot 1
bot 1 gives low to output 1 and high to bot 0
bot 0 gives low to output 2 and high to output 0
value 2 goes to bot 2

Initially, bot 1 starts with a value-3 chip, and bot 2 starts with a value-2 chip and a value-5 chip.
Because bot 2 has two microchips, it gives its lower one (2) to bot 1 and its higher one (5) to bot 0.
Then, bot 1 has two microchips; it puts the value-2 chip in output 1 and gives the value-3 chip to bot 0.
Finally, bot 0 has two microchips; it puts the 3 in output 2 and the 5 in output 0.
In the end, output bin 0 contains a value-5 microchip, output bin 1 contains a value-2 microchip, and output bin 2 contains a value-3 microchip. 
In this configuration, bot number 2 is responsible for comparing value-5 microchips with value-2 microchips.

Based on your instructions, what is the number of the bot that is responsible for comparing value-61 microchips with value-17 microchips?

Your puzzle answer was 157.

--- Part Two ---

What do you get if you multiply together the values of one chip in each of outputs 0, 1, and 2?

*/

namespace Day10
{
    class Program
    {
        static readonly int MAX_OUTPUTS = 100000;
        static readonly int MAX_VALUES = 100000;
        static readonly int MAX_BOTS = 100000;

        static (int low, int high)[] sValues = new (int low, int high)[MAX_VALUES];
        static int[] sOutputs = new int[MAX_OUTPUTS];
        static bool[] sBotsSet = new bool[MAX_BOTS];
        static (int low, bool lowBot, int high, bool highBot)[] sBots = new (int, bool, int, bool)[MAX_BOTS];
        static (int low, int high)[] sBotsState = new (int low, int high)[MAX_BOTS];
        static bool[] sBotsActive = new bool[MAX_BOTS];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = ComparingBot(61, 17);
                Console.WriteLine($"Day10 : Result1 {result1}");
                var expected = 157;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = OutputValue(0) * OutputValue(1) * OutputValue(2);
                Console.WriteLine($"Day10 : Result2 {result2}");
                long expected = 1085;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] instructions)
        {
            for (var b = 0; b < MAX_BOTS; ++b)
            {
                sBots[b].low = int.MaxValue;
                sBots[b].lowBot = false;
                sBots[b].high = int.MinValue;
                sBots[b].highBot = false;
                sBotsSet[b] = false;
            }

            for (var v = 0; v < MAX_VALUES; ++v)
            {
                sValues[v].low = int.MaxValue;
                sValues[v].high = int.MinValue;
            }

            for (var o = 0; o < MAX_OUTPUTS; ++o)
            {
                sOutputs[o] = int.MinValue;
            }

            foreach (var instruction in instructions)
            {
                ParseInstruction(instruction);
            }
            RunInstructions();
        }

        static void ParseInstruction(string instruction)
        {
            /*
            value 5 goes to bot 2
            bot 2 gives low to bot 1 and high to bot 0
            bot 1 gives low to output 1 and high to bot 0
            bot 0 gives low to output 2 and high to output 0
            */
            var tokens = instruction.Split(' ');
            var cmd = tokens[0];
            if (cmd == "value")
            {
                ParseValueInstruction(tokens, instruction);
            }
            else if (cmd == "bot")
            {
                ParseBotInstruction(tokens, instruction);
            }
            else
            {
                throw new InvalidProgramException($"Unknown cmd '{cmd}' instruction '{instruction}'");
            }
        }

        static void ParseValueInstruction(string[] tokens, string instruction)
        {
            // value 5 goes to bot 2
            if (tokens.Length != 6)
            {
                throw new InvalidProgramException($"Invalid value instruction '{instruction}' expected 6 tokens got {tokens.Length}");
            }
            if (tokens[2] != "goes")
            {
                throw new InvalidProgramException($"Invalid value instruction '{instruction}' expected 'goes' got {tokens[2]}");
            }
            if (tokens[3] != "to")
            {
                throw new InvalidProgramException($"Invalid value instruction '{instruction}' expected 'to' got {tokens[3]}");
            }
            if (tokens[4] != "bot")
            {
                throw new InvalidProgramException($"Invalid value instruction '{instruction}' expected 'bot' got {tokens[4]}");
            }
            var value = int.Parse(tokens[1]);
            var bot = int.Parse(tokens[5]);
            sValues[bot].low = Math.Min(sValues[bot].low, value);
            sValues[bot].high = Math.Max(sValues[bot].high, value);
        }

        static void ParseBotInstruction(string[] tokens, string instruction)
        {
            // bot 0 gives low to output 2 and high to output 0
            if (tokens.Length != 12)
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 12 tokens got {tokens.Length}");
            }
            if (tokens[2] != "gives")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'gives' got {tokens[2]}");
            }
            if (tokens[3] != "low")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'low' got {tokens[3]}");
            }
            if (tokens[4] != "to")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'to' got {tokens[3]}");
            }
            if (tokens[7] != "and")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'and' got {tokens[7]}");
            }
            if (tokens[8] != "high")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'high' got {tokens[8]}");
            }
            if (tokens[9] != "to")
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' expected 'to' got {tokens[9]}");
            }
            var bot = int.Parse(tokens[1]);
            if (sBotsSet[bot] == true)
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' bot {bot} already set");
            }
            // bot 0 gives low to output 2 and high to output 0
            var lowTarget = tokens[5];
            var lowValue = int.Parse(tokens[6]);
            bool lowTargetBot;
            if (lowTarget == "output")
            {
                lowTargetBot = false;
            }
            else if (lowTarget == "bot")
            {
                lowTargetBot = true;
            }
            else
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' unknown low target {lowTarget}");
            }
            sBots[bot].low = lowValue;
            sBots[bot].lowBot = lowTargetBot;


            var highTarget = tokens[10];
            var highValue = int.Parse(tokens[11]);
            bool highTargetBot;
            if (highTarget == "output")
            {
                highTargetBot = false;
            }
            else if (highTarget == "bot")
            {
                highTargetBot = true;
            }
            else
            {
                throw new InvalidProgramException($"Invalid bot instruction '{instruction}' unknown low target {lowTarget}");
            }
            sBots[bot].high = highValue;
            sBots[bot].highBot = highTargetBot;

            sBotsSet[bot] = true;
        }

        static void RunInstructions()
        {
            for (var i = 0; i < MAX_BOTS; ++i)
            {
                if (sBotsSet[i])
                {
                    sBotsState[i].low = sValues[i].low;
                    sBotsState[i].high = sValues[i].high;
                    if ((sBotsState[i].low != int.MaxValue) && (sBotsState[i].high != int.MinValue))
                    {
                        if (sBotsState[i].low != sBotsState[i].high)
                        {
                            sBotsActive[i] = true;
                        }
                    }
                }
            }
            int activeBotsCount;
            do
            {
                activeBotsCount = 0;
                for (var i = 0; i < MAX_BOTS; ++i)
                {
                    if (sBotsSet[i] && sBotsActive[i])
                    {
                        var lowValue = sBotsState[i].low;
                        var lowTarget = sBots[i].low;
                        if (sBots[i].lowBot)
                        {
                            sBotsState[lowTarget].low = Math.Min(sBotsState[lowTarget].low, lowValue);
                            sBotsState[lowTarget].high = Math.Max(sBotsState[lowTarget].high, lowValue);
                            if ((sBotsState[lowTarget].low != int.MaxValue) && (sBotsState[lowTarget].high != int.MinValue))
                            {
                                if (sBotsState[lowTarget].low != sBotsState[lowTarget].high)
                                {
                                    sBotsActive[lowTarget] = true;
                                }
                            }
                        }
                        else
                        {
                            sOutputs[lowTarget] = lowValue;
                        }
                        var highValue = sBotsState[i].high;
                        var highTarget = sBots[i].high;
                        if (sBots[i].highBot)
                        {
                            sBotsState[highTarget].low = Math.Min(sBotsState[highTarget].low, highValue);
                            sBotsState[highTarget].high = Math.Max(sBotsState[highTarget].high, highValue);
                            if ((sBotsState[highTarget].low != int.MaxValue) && (sBotsState[highTarget].high != int.MinValue))
                            {
                                if (sBotsState[highTarget].low != sBotsState[highTarget].high)
                                {
                                    sBotsActive[highTarget] = true;
                                }
                            }
                        }
                        else
                        {
                            sOutputs[highTarget] = highValue;
                        }
                        sBotsActive[i] = false;
                        ++activeBotsCount;
                    }
                }
            } while (activeBotsCount > 0);
        }

        public static int OutputValue(int bin)
        {
            return sOutputs[bin];
        }

        public static int ComparingBot(int a, int b)
        {
            for (var i = 0; i < MAX_BOTS; ++i)
            {
                var (low, high) = sBotsState[i];
                if (((low == a) && (high == b)) ||
                    ((low == b) && (high == a)))
                {
                    return i;
                }
            }
            throw new InvalidProgramException($"Bot comparing {a} and {b} not found");
        }

        public static void Run()
        {
            Console.WriteLine("Day10 : Start");
            _ = new Program("Day10/input.txt", true);
            _ = new Program("Day10/input.txt", false);
            Console.WriteLine("Day10 : End");
        }
    }
}
