using System;

/*

--- Day 2: Bathroom Security ---

You arrive at Easter Bunny Headquarters under cover of darkness. However, you left in such a rush that you forgot to use the bathroom! Fancy office buildings like this one usually have keypad locks on their bathrooms, so you search the front desk for the code.

"In order to improve security," the document you find says, "bathroom codes will no longer be written down. Instead, please memorize and follow the procedure below to access the bathrooms."

The document goes on to explain that each button to be pressed can be found by starting on the previous button and moving to adjacent buttons on the keypad: U moves up, D moves down, L moves left, and R moves right. Each line of instructions corresponds to one button, starting at the previous button (or, for the first line, the "5" button); press whatever button you're on at the end of each line. If a move doesn't lead to a button, ignore it.

You can't hold it much longer, so you decide to figure out the code as you walk to the bathroom. 
You picture a keypad like this:

1 2 3
4 5 6
7 8 9
Suppose your instructions are:

ULL
RRDDD
LURDL
UUUUD
You start at "5" and move up (to "2"), left (to "1"), and left (you can't, and stay on "1"), so the first button is 1.
Starting from the previous button ("1"), you move right twice (to "3") and then down three times (stopping at "9" after two moves and ignoring the third), ending up with 9.
Continuing from "9", you move left, up, right, down, and left, ending with 8.
Finally, you move up four times (stopping at "2"), then down once, ending with 5.
So, in this example, the bathroom code is 1985.

Your puzzle input is the instructions from the document you found at the front desk. What is the bathroom code?

95549

--- Part Two ---

You finally arrive at the bathroom (it's a several minute walk from the lobby so visitors can behold the many fancy conference rooms and water coolers on this floor) and go to punch in the code. Much to your bladder's dismay, the keypad is not at all like you imagined it. Instead, you are confronted with the result of hundreds of man-hours of bathroom-keypad-design meetings:

    1
  2 3 4
5 6 7 8 9
  A B C
    D

You still start at "5" and stop when you're at an edge, but given the same instructions as above, the outcome is very different:

You start at "5" and don't move at all (up and left are both edges), ending at 5.
Continuing from "5", you move right twice and down three times (through "6", "7", "B", "D", "D"), ending at D.
Then, from "D", you move five more times (through "D", "B", "C", "C", "B"), ending at B.
Finally, after five more moves, you end at 3.
So, given the actual keypad layout, the code would be 5DB3.

Using the same instructions in your puzzle input, what is the correct bathroom code?
*/

namespace Day02
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (part1)
            {
                long result1 = KeyCode(lines);
                Console.WriteLine($"Day02 : Result1 {result1}");
                long expected = 95549;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = KeyCode5x5(lines);
                Console.WriteLine($"Day02 : Result2 {result2}");
                var expected = "D87AD";
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static int KeyCode(string[] codes)
        {
            var x = 0;
            var y = 0;
            var keyCode = 0;
            foreach (var code in codes)
            {
                foreach (var m in code)
                {
                    var dx = 0;
                    var dy = 0;
                    if (m == 'U')
                    {
                        dy = -1;
                    }
                    else if (m == 'D')
                    {
                        dy = +1;
                    }
                    else if (m == 'L')
                    {
                        dx = -1;
                    }
                    else if (m == 'R')
                    {
                        dx = +1;
                    }
                    x += dx;
                    y += dy;
                    x = Math.Clamp(x, -1, +1);
                    y = Math.Clamp(y, -1, +1);
                }
                var key = 1 + (x + 1) + (y + 1) * 3;
                keyCode *= 10;
                keyCode += key;
            }
            return keyCode;
        }

        public static string KeyCode5x5(string[] codes)
        {
            int x;
            int y;
            var charCode = new char[5, 5];
            for (y = 0; y < 5; ++y)
            {
                for (x = 0; x < 5; ++x)
                {
                    charCode[x, y] = '*';
                }
            }
            /*
                1
              2 3 4
            5 6 7 8 9
              A B C
                D
            */
            charCode[2, 0] = '1';
            charCode[1, 1] = '2';
            charCode[2, 1] = '3';
            charCode[3, 1] = '4';
            charCode[0, 2] = '5';
            charCode[1, 2] = '6';
            charCode[2, 2] = '7';
            charCode[3, 2] = '8';
            charCode[4, 2] = '9';
            charCode[1, 3] = 'A';
            charCode[2, 3] = 'B';
            charCode[3, 3] = 'C';
            charCode[2, 4] = 'D';
            x = 0;
            y = 2;
            string keyCode = "";
            foreach (var code in codes)
            {
                foreach (var m in code)
                {
                    var dx = 0;
                    var dy = 0;
                    if (m == 'U')
                    {
                        dy = -1;
                    }
                    else if (m == 'D')
                    {
                        dy = +1;
                    }
                    else if (m == 'L')
                    {
                        dx = -1;
                    }
                    else if (m == 'R')
                    {
                        dx = +1;
                    }
                    var newX = x + dx;
                    var newY = y + dy;
                    newX = Math.Clamp(newX, 0, 4);
                    newY = Math.Clamp(newY, 0, 4);
                    var c = charCode[newX, newY];
                    if (c != '*')
                    {
                        x = newX;
                        y = newY;
                    }
                }
                keyCode += charCode[x, y];
            }
            return keyCode;
        }

        public static void Run()
        {
            Console.WriteLine("Day02 : Start");
            _ = new Program("Day02/input.txt", true);
            _ = new Program("Day02/input.txt", false);
            Console.WriteLine("Day02 : End");
        }
    }
}
