using System;

/*

--- Day 4: Security Through Obscurity ---

Finally, you come across an information kiosk with a list of rooms. 
Of course, the list is encrypted and full of decoy data, but the instructions to decode the list are barely hidden nearby. 
Better remove the decoy data first.

Each room consists of an encrypted name (lowercase letters separated by dashes) followed by a dash, a sector ID, and a checksum in square brackets.

A room is real (not a decoy) if the checksum is the five most common letters in the encrypted name, in order, with ties broken by alphabetization. 
For example:

aaaaa-bbb-z-y-x-123[abxyz] is a real room because the most common letters are a (5), b (3), and then a tie between x, y, and z, which are listed alphabetically.
a-b-c-d-e-f-g-h-987[abcde] is a real room because although the letters are all tied (1 of each), the first five are listed alphabetically.
not-a-real-room-404[oarel] is a real room.
totally-real-room-200[decoy] is not.

Of the real rooms from the list above, the sum of their sector IDs is 1514.

What is the sum of the sector IDs of the real rooms?

Your puzzle answer was 245102.

--- Part Two ---

With all the decoy data out of the way, it's time to decrypt this list and get moving.

The room names are encrypted by a state-of-the-art shift cipher, which is nearly unbreakable without the right software. 
However, the information kiosk designers at Easter Bunny HQ were not expecting to deal with a master cryptographer like yourself.

To decrypt a room name, rotate each letter forward through the alphabet a number of times equal to the room's sector ID. 
A becomes B, B becomes C, Z becomes A, and so on. 
Dashes become spaces.

For example, the real name for qzmt-zixmtkozy-ivhz-343 is very encrypted name.

What is the sector ID of the room where North Pole objects are stored?

*/

namespace Day04
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (part1)
            {
                long result1 = CountValidRooms(lines);
                Console.WriteLine($"Day04 : Result1 {result1}");
                long expected = 185371;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = FindDecryptedRoom(lines, "northpole object storage");
                Console.WriteLine($"Day04 : Result2 {result2}");
                long expected = 984;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static int CountValidRooms(string[] rooms)
        {
            int sum = 0;
            foreach (var room in rooms)
            {
                sum += RoomSectorID(room);
            }
            return sum;
        }

        static int FindDecryptedRoom(string[] rooms, string roomToFind)
        {
            foreach (var room in rooms)
            {
                var roomName = DecryptRoomName(room);
                if (roomName == roomToFind)
                {
                    Console.WriteLine($"{roomName}");
                    return RoomSectorID(room);
                }
            }
            return -1;
        }

        static void DecodeRoomString(string room, out int sectorID, out string roomName)
        {
            //aaaaa-bbb-z-y-x-123[abxyz] is a real room because the most common letters are a (5), b (3), and then a tie between x, y, and z, which are listed alphabetically.
            //a-b-c-d-e-f-g-h-987[abcde] is a real room because although the letters are all tied (1 of each), the first five are listed alphabetically.
            //not-a-real-room-404[oarel] is a real room.
            //totally-real-room-200[decoy] is not.
            var tokens = room.Trim().Split('[');
            var roomNameString = tokens[0];
            var letterCounts = new int[26];
            var sectorIDstring = "";
            bool parseSectorID = false;
            roomName = "";
            foreach (var c in roomNameString)
            {
                if (c != '-')
                {
                    if (parseSectorID == false)
                    {
                        if ((c >= 'a') && (c <= 'z'))
                        {
                            int letter = c - 'a';
                            ++letterCounts[letter];
                        }
                        else if ((c >= '0') && (c <= '9'))
                        {
                            parseSectorID = true;
                        }
                        else
                        {
                            throw new InvalidProgramException($"Bad character parsing roomName '{room}'");
                        }
                    }
                    if (parseSectorID)
                    {
                        if ((c >= '0') && (c <= '9'))
                        {
                            sectorIDstring += c;
                        }
                        else
                        {
                            throw new InvalidProgramException($"Bad character in sectorID '{c}' '{room}'");
                        }
                    }
                }
                if (parseSectorID == false)
                {
                    roomName += c;
                }
            }

            var checksum = tokens[1].Trim().Split(']')[0].Trim();
            if (checksum.Length != 5)
            {
                throw new InvalidProgramException($"Invalid checksum '{checksum}' Length != 5 {checksum.Length} '{room}'");
            }
            var topFiveLetters = new int[5];
            var topFiveCounts = new int[5];
            for (var l = 0; l < 26; ++l)
            {
                for (var i = 0; i < 5; ++i)
                {
                    var count = topFiveCounts[i];
                    var lc = letterCounts[l];
                    if (lc > count)
                    {
                        for (var j = 4; j > i; --j)
                        {
                            topFiveCounts[j] = topFiveCounts[j - 1];
                            topFiveLetters[j] = topFiveLetters[j - 1];
                        }
                        topFiveCounts[i] = lc;
                        topFiveLetters[i] = l;
                        break;
                    }
                }
            }

            sectorID = int.Parse(sectorIDstring);
            for (var i = 0; i < 5; ++i)
            {
                var expectedLetter = topFiveLetters[i] + 'a';
                var actualLetter = checksum[i];
                if (expectedLetter != actualLetter)
                {
                    sectorID = 0;
                    break;
                }
            }
        }

        public static int RoomSectorID(string room)
        {
            DecodeRoomString(room, out int sectorID, out string _);
            return sectorID;
        }

        public static string RoomName(string room)
        {
            DecodeRoomString(room, out int _, out string roomName);
            return roomName;
        }

        public static string DecryptRoomName(string room)
        {
            DecodeRoomString(room, out int sectorID, out string roomName);
            var decrypted = "";
            foreach (var c in roomName)
            {
                char dc;
                if (c == '-')
                {
                    dc = ' ';
                }
                else
                {
                    int index = c - 'a';
                    index += sectorID;
                    index %= 26;
                    dc = (char)(index + 'a');
                }
                decrypted += dc;
            }
            decrypted = decrypted.Trim();
            return decrypted;
        }

        public static void Run()
        {
            Console.WriteLine("Day04 : Start");
            _ = new Program("Day04/input.txt", true);
            _ = new Program("Day04/input.txt", false);
            Console.WriteLine("Day04 : End");
        }
    }
}
