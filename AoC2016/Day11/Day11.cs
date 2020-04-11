using System;

/*

--- Day 11: Radioisotope Thermoelectric Generators ---

You come upon a column of four floors that have been entirely sealed off from the rest of the building except for a small dedicated lobby. 
There are some radiation warnings and a big sign which reads "Radioisotope Testing Facility".

According to the project status board, this facility is currently being used to experiment with Radioisotope Thermoelectric Generators (RTGs, or simply "generators") that are designed to be paired with specially-constructed microchips. 
Basically, an RTG is a highly radioactive rock that generates electricity through heat.

The experimental RTGs have poor radiation containment, so they're dangerously radioactive. 
The chips are prototypes and don't have normal radiation shielding, but they do have the ability to generate an electromagnetic radiation shield when powered. 
Unfortunately, they can only be powered by their corresponding RTG. 
An RTG powering a microchip is still dangerous to other microchips.

In other words, if a chip is ever left in the same area as another RTG, and it's not connected to its own RTG, the chip will be fried. 
Therefore, it is assumed that you will follow procedure and keep chips connected to their corresponding RTG when they're in the same room, and away from other RTGs otherwise.

These microchips sound very interesting and useful to your current activities, and you'd like to try to retrieve them. 
The fourth floor of the facility has an assembling machine which can make a self-contained, shielded computer for you to take with you - that is, if you can bring it all of the RTGs and microchips.

Within the radiation-shielded part of the facility (in which it's safe to have these pre-assembly RTGs), there is an elevator that can move between the four floors. 
Its capacity rating means it can carry at most yourself and two RTGs or microchips in any combination. 
(They're rigged to some heavy diagnostic equipment - the assembling machine will detach it for you.) 
As a security measure, the elevator will only function if it contains at least one RTG or microchip. 
The elevator always stops on each floor to recharge, and this takes long enough that the items within it and the items on that floor can irradiate each other. 
(You can prevent this if a Microchip and its Generator end up on the same floor in this way, as they can be connected while the elevator is recharging.)

You make some notes of the locations of each component of interest (your puzzle input). 
Before you don a hazmat suit and start moving things around, you'd like to have an idea of what you need to do.

When you enter the containment area, you and the elevator will start on the first floor.

For example, suppose the isolated area has the following arrangement:

The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
The second floor contains a hydrogen generator.
The third floor contains a lithium generator.
The fourth floor contains nothing relevant.
As a diagram (F# for a Floor number, E for Elevator, H for Hydrogen, L for Lithium, M for Microchip, and G for Generator), the initial state looks like this:

F4 .  .  .  .  .  
F3 .  .  .  LG .  
F2 .  HG .  .  .  
F1 E  .  HM .  LM 
Then, to get everything up to the assembling machine on the fourth floor, the following steps could be taken:

Bring the Hydrogen-compatible Microchip to the second floor, which is safe because it can get power from the Hydrogen Generator:

F4 .  .  .  .  .  
F3 .  .  .  LG .  
F2 E  HG HM .  .  
F1 .  .  .  .  LM 
Bring both Hydrogen-related items to the third floor, which is safe because the Hydrogen-compatible microchip is getting power from its generator:

F4 .  .  .  .  .  
F3 E  HG HM LG .  
F2 .  .  .  .  .  
F1 .  .  .  .  LM 
Leave the Hydrogen Generator on floor three, but bring the Hydrogen-compatible Microchip back down with you so you can still use the elevator:

F4 .  .  .  .  .  
F3 .  HG .  LG .  
F2 E  .  HM .  .  
F1 .  .  .  .  LM 
At the first floor, grab the Lithium-compatible Microchip, which is safe because Microchips don't affect each other:

F4 .  .  .  .  .  
F3 .  HG .  LG .  
F2 .  .  .  .  .  
F1 E  .  HM .  LM 
Bring both Microchips up one floor, where there is nothing to fry them:

F4 .  .  .  .  .  
F3 .  HG .  LG .  
F2 E  .  HM .  LM 
F1 .  .  .  .  .  
Bring both Microchips up again to floor three, where they can be temporarily connected to their corresponding generators while the elevator recharges, preventing either of them from being fried:

F4 .  .  .  .  .  
F3 E  HG HM LG LM 
F2 .  .  .  .  .  
F1 .  .  .  .  .  
Bring both Microchips to the fourth floor:

F4 E  .  HM .  LM 
F3 .  HG .  LG .  
F2 .  .  .  .  .  
F1 .  .  .  .  .  
Leave the Lithium-compatible microchip on the fourth floor, but bring the Hydrogen-compatible one so you can still use the elevator; this is safe because although the Lithium Generator is on the destination floor, you can connect Hydrogen-compatible microchip to the Hydrogen Generator there:

F4 .  .  .  .  LM 
F3 E  HG HM LG .  
F2 .  .  .  .  .  
F1 .  .  .  .  .  
Bring both Generators up to the fourth floor, which is safe because you can connect the Lithium-compatible Microchip to the Lithium Generator upon arrival:

F4 E  HG .  LG LM 
F3 .  .  HM .  .  
F2 .  .  .  .  .  
F1 .  .  .  .  .  
Bring the Lithium Microchip with you to the third floor so you can use the elevator:

F4 .  HG .  LG .  
F3 E  .  HM .  LM 
F2 .  .  .  .  .  
F1 .  .  .  .  .  
Bring both Microchips to the fourth floor:

F4 E  HG HM LG LM 
F3 .  .  .  .  .  
F2 .  .  .  .  .  
F1 .  .  .  .  .  
In this arrangement, it takes 11 steps to collect all of the objects at the fourth floor for assembly. 
(Each elevator stop counts as one step, even if nothing is added to or removed from it.)

In your situation, what is the minimum number of steps required to bring all of the objects to the fourth floor?

*/

namespace Day11
{
    class Program
    {
        static readonly int MAX_ELEMENTS = 128;
        static string[] sKnownGenerators = new string[MAX_ELEMENTS];
        static string[] sKnownMicrochips = new string[MAX_ELEMENTS];
        static int sGeneratorCount;
        static int sMicrochipCount;
        static int sElevatorFloor;
        static int[] sGeneratorFloors = new int[MAX_ELEMENTS];
        static int[] sMicrochipFloors = new int[MAX_ELEMENTS];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                var result1 = MinimumMoves;
                Console.WriteLine($"Day11 : Result1 {result1}");
                var expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = -123;
                Console.WriteLine($"Day11 : Result2 {result2}");
                long expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sGeneratorCount = 0;
            sMicrochipCount = 0;
            MinimumMoves = int.MaxValue;
            /*
            "The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.",
            "The second floor contains a hydrogen generator.",
            "The third floor contains a lithium generator.",
            "The fourth floor contains nothing relevant."
            */
            foreach (var line in lines)
            {
                var tokens = line.Trim().Split(' ');
                if (tokens[0] != "The")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 'The' got '{tokens[0]}'");
                }
                if (tokens[2] != "floor")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 'floor' got '{tokens[2]}'");
                }
                if (tokens[3] != "contains")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 'contains' got '{tokens[3]}'");
                }

                var floorToken = tokens[1];
                int floor;
                if (floorToken == "first")
                {
                    floor = 1;
                }
                else if (floorToken == "second")
                {
                    floor = 2;
                }
                else if (floorToken == "third")
                {
                    floor = 3;
                }
                else if (floorToken == "fourth")
                {
                    floor = 4;
                }
                else
                {
                    throw new InvalidProgramException($"Invalid line '{line}' unknown floor '{floorToken}'");
                }
                Console.Write($"Floor {floor} ");
                if (tokens[4] == "nothing")
                {
                    if (tokens[5] != "relevant.")
                    {
                        throw new InvalidProgramException($"Invalid line '{line}' unknown token expecting `relevant.` '{tokens[5]}'");
                    }
                    else
                    {
                        Console.WriteLine($"is empty");
                        continue;
                    }
                }
                else if (tokens[4] == "a")
                {
                    for (int t = 4; t < tokens.Length;)
                    {
                        if (tokens[t] == "and")
                        {
                            ++t;
                            if (tokens[t] == "a")
                            {
                                ++t;
                            }
                            else
                            {
                                throw new InvalidProgramException($"Invalid line '{line}' unknown token expecting `and a` '{tokens[t - 1]} {tokens[t]}'");
                            }
                        }
                        else if (tokens[t] == "a")
                        {
                            ++t;
                        }
                        var elementString = tokens[t];
                        var typeString = tokens[t + 1];
                        var type = "";
                        var element = -1;
                        if (typeString.StartsWith("microchip"))
                        {
                            type = "M";
                            var toks = elementString.Split('-');
                            if (toks[1] != "compatible")
                            {
                                throw new InvalidProgramException($"Invalid line '{line}' microchip element name should end with '-compatible' '{elementString}");
                            }
                            elementString = toks[0];
                            for (var i = 0; i < sMicrochipCount; ++i)
                            {
                                if (sKnownMicrochips[i] == elementString)
                                {
                                    throw new InvalidProgramException($"Invalid line '{line}' microchip '{elementString}' already known");
                                }
                            }
                            if (element == -1)
                            {
                                element = sMicrochipCount;
                                sKnownMicrochips[sMicrochipCount] = elementString;
                                sMicrochipFloors[sMicrochipCount] = floor;
                                ++sMicrochipCount;
                            }
                        }
                        else if (typeString.StartsWith("generator"))
                        {
                            type = "G";
                            for (var i = 0; i < sGeneratorCount; ++i)
                            {
                                if (sKnownGenerators[i] == elementString)
                                {
                                    throw new InvalidProgramException($"Invalid line '{line}' generator '{elementString}' already known");
                                }
                            }
                            if (element == -1)
                            {
                                element = sGeneratorCount;
                                sKnownGenerators[sGeneratorCount] = elementString;
                                sGeneratorFloors[sGeneratorCount] = floor;
                                ++sGeneratorCount;
                            }
                        }
                        else
                        {
                            throw new InvalidProgramException($"Invalid line '{line}' unknown element '{elementString}' type `microchip' or 'generator'` '{typeString}'");
                        }
                        t += 2;
                        Console.Write($"Found '{element}' '{type}' ");
                    }
                    Console.WriteLine($"");
                }
                else
                {
                    throw new InvalidProgramException($"Invalid line '{line}' unknown token expecting `nothing' or 'a'` '{tokens[4]}'");
                }
            }
            for (var g = 0; g < sGeneratorCount; ++g)
            {
                Console.WriteLine($"Generator[{g}] '{sKnownGenerators[g]}'");
            }
            for (var m = 0; m < sMicrochipCount; ++m)
            {
                Console.WriteLine($"Microship[{m}] '{sKnownMicrochips[m]}'");
            }
            sElevatorFloor = 1;
            ValidateState();

            // Update the sGeneratorFloors generator Index to use the chip index
            var generatorIndexToChipIndex = new int[sGeneratorCount];
            for (var g = 0; g < sGeneratorCount; ++g)
            {
                generatorIndexToChipIndex[g] = -1;
                var generator = sKnownGenerators[g];
                for (var m = 0; m < sMicrochipCount; ++m)
                {
                    if (sKnownMicrochips[m] == generator)
                    {
                        generatorIndexToChipIndex[g] = m;
                        break;
                    }
                }
                if (generatorIndexToChipIndex[g] == -1)
                {
                    throw new InvalidProgramException($"Microchip for '{generator}' not found");
                }
            }
            var oldGeneratorFloors = new int[sGeneratorCount];
            for (var g = 0; g < sGeneratorCount; ++g)
            {
                oldGeneratorFloors[g] = sGeneratorFloors[g];
            }
            for (var g = 0; g < sGeneratorCount; ++g)
            {
                var chipIndex = generatorIndexToChipIndex[g];
                sGeneratorFloors[chipIndex] = oldGeneratorFloors[g];
            }

            var state = GenerateState();
            Console.WriteLine($"{state}");
            if (!IsValidState(state))
            {
                throw new InvalidProgramException($"Invalid State '{state}'");
            }
        }

        static string GenerateState()
        {
            var state = "";
            state += (char)(sElevatorFloor + '0');
            for (var c = 0; c < sMicrochipCount; ++c)
            {
                state += (char)(sGeneratorFloors[c] + '0');
                state += (char)(sMicrochipFloors[c] + '0');
            }
            return state;
        }

        static void ValidateState()
        {
            if (sGeneratorCount != sMicrochipCount)
            {
                throw new InvalidProgramException($"Number of generators != number of microchips {sGeneratorCount} != {sMicrochipCount}");
            }
            for (var g = 0; g < sGeneratorCount; ++g)
            {
                var generator = sKnownGenerators[g];
                var mIndex = -1;
                for (var m = 0; m < sMicrochipCount; ++m)
                {
                    if (sKnownMicrochips[m] == generator)
                    {
                        mIndex = m;
                        break;
                    }
                }
                if (mIndex == -1)
                {
                    throw new InvalidProgramException($"Microchip for '{generator}' not found");
                }
            }
            for (var m = 0; m < sMicrochipCount; ++m)
            {
                var microchip = sKnownMicrochips[m];
                var gIndex = -1;
                for (var g = 0; g < sGeneratorCount; ++g)
                {
                    if (sKnownGenerators[g] == microchip)
                    {
                        gIndex = g;
                        break;
                    }
                }
                if (gIndex == -1)
                {
                    throw new InvalidProgramException($"Generator for '{microchip}' not found");
                }
            }
        }

        //"State = "[1-4]<[1-4][1-4] x NumChips>
        // E + 1 or 2 things
        // G : on its own destroys things
        // Need G+C : to protect
        // E up 1 or down 1
        static void ComputeValidMoves(string startingState)
        {
        }

        // E can't be on its own
        // Chip must be with its Generator : if a different generator is on its own
        static bool IsValidState(string state)
        {
            var elevatorFloor = state[0] - '0';
            var numChips = (state.Length - 1) / 2;
            for (var f = 1; f <= 4; ++f)
            {
                var thisFloor = (char)('0' + f);
                var onThisFloor = 0;
                var exposedChips = 0;
                var exposedGenerators = 0;
                for (var c = 0; c < numChips; ++c)
                {
                    var i = 1 + c * 2;
                    var genertorOnFloor = 0;
                    var chipOnFloor = 0;
                    if (state[i] == thisFloor)
                    {
                        genertorOnFloor = 1;
                    }
                    if (state[i + 1] == thisFloor)
                    {
                        chipOnFloor = 1;
                    }
                    if (chipOnFloor == 1)
                    {
                        exposedChips += chipOnFloor - genertorOnFloor;
                    }
                    if (genertorOnFloor == 1)
                    {
                        exposedGenerators += genertorOnFloor - chipOnFloor;
                    }
                    onThisFloor += genertorOnFloor;
                    onThisFloor += chipOnFloor;
                }
                if (f == elevatorFloor)
                {
                    if (onThisFloor == 0)
                    {
                        return false;
                    }
                }
                if ((exposedChips > 0) && (exposedGenerators > 0))
                {
                    return false;
                }
            }
            return true;
        }

        public static int MinimumMoves { get; private set; }

        public static void Run()
        {
            Console.WriteLine("Day11 : Start");
            _ = new Program("Day11/input.txt", true);
            _ = new Program("Day11/input.txt", false);
            Console.WriteLine("Day11 : End");
        }
    }
}
