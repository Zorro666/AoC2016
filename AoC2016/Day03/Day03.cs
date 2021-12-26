using System;

/*

--- Day 3: Squares With Three Sides ---

Now that you can think clearly, you move deeper into the labyrinth of hallways and office furniture that makes up this part of Easter Bunny HQ. 
This must be a graphic design department; the walls are covered in specifications for triangles.

Or are they?

The design document gives the side lengths of each triangle it describes, but... 5 10 25? 
Some of these aren't triangles. 
You can't help but mark the impossible ones.

In a valid triangle, the sum of any two sides must be larger than the remaining side. 
For example, the "triangle" given above is impossible, because 5 + 10 is not larger than 25.

In your puzzle input, how many of the listed triangles are possible?

Your puzzle answer was 993.

--- Part Two ---

Now that you've helpfully marked up their design documents, it occurs to you that triangles are specified in groups of three vertically. 
Each set of three numbers in a column specifies a triangle. 
Rows are unrelated.

For example, given the following specification, numbers with the same hundreds digit would be part of the same triangle:

101 301 501
102 302 502
103 303 503
201 401 601
202 402 602
203 403 603

In your puzzle input, and instead reading by columns, how many of the listed triangles are possible?

*/

namespace Day03
{
    class Program
    {
        struct Triangle
        {
            public int a;
            public int b;
            public int c;
        };

        static Triangle[] sTris;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            if (part1)
            {
                long result1 = CountValidTriangles();
                Console.WriteLine($"Day03 : Result1 {result1}");
                long expected = 917;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = CountValidTrianglesColumns();
                Console.WriteLine($"Day03 : Result2 {result2}");
                long expected = 1649;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sTris = new Triangle[lines.Length];
            int triIndex = 0;
            var sides = new int[3];
            foreach (var line in lines)
            {
                var buffer = line.Trim();
                var token = "";
                var index = 0;
                foreach (var c in buffer)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        token += c;
                    }
                    else
                    {
                        if (token.Length > 0)
                        {
                            sides[index] = int.Parse(token);
                            ++index;
                            token = "";
                        }
                    }
                }
                if (token.Length > 0)
                {
                    sides[index] = int.Parse(token);
                }
                sTris[triIndex].a = sides[0];
                sTris[triIndex].b = sides[1];
                sTris[triIndex].c = sides[2];
                ++triIndex;
            }
        }

        public static int CountValidTriangles()
        {
            int count = 0;
            foreach (var tri in sTris)
            {
                if (ValidTriangle(tri))
                {
                    ++count;
                }
            }
            return count;
        }

        static bool ValidTriangle(Triangle tri)
        {
            var sides = new int[3];
            sides[0] = tri.a;
            sides[1] = tri.b;
            sides[2] = tri.c;
            int maxSide = 0;
            if (sides[1] > sides[maxSide])
            {
                maxSide = 1;
            }
            if (sides[2] > sides[maxSide])
            {
                maxSide = 2;
            }
            bool valid = false;
            if (maxSide == 0)
            {
                if (sides[1] + sides[2] > sides[0])
                {
                    valid = true;
                }
            }
            else if (maxSide == 1)
            {
                if (sides[0] + sides[2] > sides[1])
                {
                    valid = true;
                }
            }
            else if (maxSide == 2)
            {
                if (sides[0] + sides[1] > sides[2])
                {
                    valid = true;
                }
            }
            return valid;
        }

        public static int CountValidTrianglesColumns()
        {
            int count = 0;
            for (var i = 0; i < sTris.Length; i += 3)
            {
                Triangle tri0;
                Triangle tri1;
                Triangle tri2;
                tri0.a = sTris[i + 0].a;
                tri1.a = sTris[i + 0].b;
                tri2.a = sTris[i + 0].c;
                tri0.b = sTris[i + 1].a;
                tri1.b = sTris[i + 1].b;
                tri2.b = sTris[i + 1].c;
                tri0.c = sTris[i + 2].a;
                tri1.c = sTris[i + 2].b;
                tri2.c = sTris[i + 2].c;

                if (ValidTriangle(tri0))
                {
                    ++count;
                }
                if (ValidTriangle(tri1))
                {
                    ++count;
                }
                if (ValidTriangle(tri2))
                {
                    ++count;
                }
            }
            return count;
        }

        public static void Run()
        {
            Console.WriteLine("Day03 : Start");
            _ = new Program("Day03/input.txt", true);
            _ = new Program("Day03/input.txt", false);
            Console.WriteLine("Day03 : End");
        }
    }
}
