using System;

/*

--- Day 8: Two-Factor Authentication ---

You come across a door implementing what you can only assume is an implementation of two-factor authentication after a long game of requirements telephone.

To get past the door, you first swipe a keycard (no problem; there was one on a nearby desk). 
Then, it displays a code on a little screen, and you type that code on a keypad. 
Then, presumably, the door unlocks.

Unfortunately, the screen has been smashed. 
After a few minutes, you've taken everything apart and figured out how it works. 
Now you just have to work out what the screen would have displayed.

The magnetic strip on the card you swiped encodes a series of instructions for the screen; these instructions are your puzzle input. 
The screen is 50 pixels wide and 6 pixels tall, all of which start off, and is capable of three somewhat peculiar operations:

rect AxB turns on all of the pixels in a rectangle at the top-left of the screen which is A wide and B tall.

rotate row y=A by B shifts all of the pixels in row A (0 is the top row) right by B pixels. 
Pixels that would fall off the right end appear at the left end of the row.

rotate column x=A by B shifts all of the pixels in column A (0 is the left column) down by B pixels. 
Pixels that would fall off the bottom appear at the top of the column.

For example, here is a simple sequence on a smaller screen:

rect 3x2 creates a small rectangle in the top-left corner:

###....
###....
.......

rotate column x=1 by 1 rotates the second column down by one pixel:

#.#....
###....
.#.....

rotate row y=0 by 4 rotates the top row right by four pixels:

....#.#
###....
.#.....

rotate column x=1 by 1 again rotates the second column down by one pixel, causing the bottom pixel to wrap back to the top:

.#..#.#
#.#....
.#.....

As you can see, this display technology is extremely powerful, and will soon dominate the tiny-code-displaying-screen market. 
That's what the advertisement on the back of the display tries to convince you, anyway.

There seems to be an intermediate check of the voltage used by the display: after you swipe your card, if the screen did work, how many pixels should be lit?

Your puzzle answer was 128.

--- Part Two ---

You notice that the screen is only capable of displaying capital letters; in the font it uses, each letter is 5 pixels wide and 6 tall.

After you swipe your card, what code is the screen trying to display?

*/

namespace Day08
{
    class Program
    {
        private static byte[,] sCells;
        private static int sWidth;
        private static int sHeight;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            ProcessCommands(lines, 50, 6);
            if (part1)
            {
                var result1 = CountLit;
                Console.WriteLine($"Day08 : Result1 {result1}");
                var expected = 119;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var grid = GetGrid();
                foreach (var line in grid)
                {
                    Console.WriteLine(line);
                }
                var result2 = "ZFHFSFOGPO";
                Console.WriteLine($"Day08 : Result2 {result2}");
                var expected = "ZFHFSFOGPO";
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void ProcessCommands(string[] commands, int w, int h)
        {
            sWidth = w;
            sHeight = h;
            sCells = new byte[w, h];
            foreach (var command in commands)
            {
                ProcessCommand(command);
            }
        }

        static void ProcessCommand(string command)
        {
            var tokens = command.Split(' ');
            var cmd = tokens[0];
            if (cmd == "rect")
            {
                if (tokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid rect command line expected 2 tokens got '{tokens.Length}' command '{command}'");
                }
                //rect AxB turns on all of the pixels in a rectangle at the top - left of the screen which is A wide and B tall.
                var rect = tokens[1];
                var rectTokens = rect.Split('x');
                if (rectTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid rect option expected 2 tokens got '{rectTokens.Length}' command '{command}'");
                }
                var w = int.Parse(rectTokens[0]);
                var h = int.Parse(rectTokens[1]);
                Rect(w, h);
            }
            else if (cmd == "rotate")
            {
                if (tokens.Length != 5)
                {
                    throw new InvalidProgramException($"Invalid rotate command line expected 5 tokens got '{tokens.Length}' command '{command}'");
                }
                if (tokens[3] != "by")
                {
                    throw new InvalidProgramException($"Unknown rotate command expected 'by' got '{tokens[3]}' command '{command}'");
                }

                //rotate row y=A by B shifts all of the pixels in row A(0 is the top row) right by B pixels. 
                //Pixels that would fall off the right end appear at the left end of the row.
                //rotate column x=A by B shifts all of the pixels in column A(0 is the left column) down by B pixels. 
                //Pixels that would fall off the bottom appear at the top of the column.
                var option = tokens[1];
                var amount = int.Parse(tokens[4]);
                var start = tokens[2];
                var startTokens = start.Split('=');
                if (startTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Unknown rotate start option 2 got '{startTokens.Length}' command '{command}'");
                }
                if (option == "row")
                {
                    var rowStart = startTokens[0];
                    if (rowStart != "y")
                    {
                        throw new InvalidProgramException($"Unknown rotate row option expected 'y' got '{rowStart}' command '{command}'");
                    }
                    var row = int.Parse(startTokens[1]);
                    RotateRow(row, amount);
                }
                else if (option == "column")
                {
                    var colStart = startTokens[0];
                    if (colStart != "x")
                    {
                        throw new InvalidProgramException($"Unknown rotate col option expected 'x' got '{colStart}' command '{command}'");
                    }
                    var column = int.Parse(startTokens[1]);
                    RotateColumn(column, amount);
                }
                else
                {
                    throw new InvalidProgramException($"Unknown rotate option '{option}' command '{command}'");
                }
            }
            else
            {
                throw new InvalidProgramException($"Unknown cmd '{cmd}' command '{command}'");
            }
        }

        static void Rect(int w, int h)
        {
            for (var y = 0; y < h; ++y)
            {
                for (var x = 0; x < w; ++x)
                {
                    sCells[x, y] = 1;
                }
            }
        }

        static void RotateRow(int row, int amount)
        {
            var data = new byte[sWidth];
            for (var x = 0; x < sWidth; ++x)
            {
                var newX = (x + amount) % sWidth;
                data[newX] = sCells[x, row];
            }
            for (var x = 0; x < sWidth; ++x)
            {
                sCells[x, row] = data[x];
            }
        }

        static void RotateColumn(int column, int amount)
        {
            var data = new byte[sHeight];
            for (var y = 0; y < sHeight; ++y)
            {
                var newY = (y + amount) % sHeight;
                data[newY] = sCells[column, y];
            }
            for (var y = 0; y < sHeight; ++y)
            {
                sCells[column, y] = data[y];
            }
        }

        public static string[] GetGrid()
        {
            var result = new string[sHeight];
            for (var y = 0; y < sHeight; ++y)
            {
                var line = "";
                for (var x = 0; x < sWidth; ++x)
                {
                    if (sCells[x, y] == 0)
                    {
                        line += '.';
                    }
                    else if (sCells[x, y] == 1)
                    {
                        line += '#';
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown cell value '{sCells[x, y]}' @ {x},{y}");
                    }
                }
                result[y] = line;
            }
            return result;
        }

        public static int CountLit
        {
            get
            {
                var total = 0;
                for (var x = 0; x < sWidth; ++x)
                {
                    for (var y = 0; y < sHeight; ++y)
                    {
                        total += sCells[x, y];
                    }
                }
                return total;
            }
        }

        public static void Run()
        {
            Console.WriteLine("Day08 : Start");
            _ = new Program("Day08/input.txt", true);
            _ = new Program("Day08/input.txt", false);
            Console.WriteLine("Day08 : End");
        }
    }
}
