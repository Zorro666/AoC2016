using NUnit.Framework;

namespace Day04
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("aaaaa-bbb-z-y-x-123[abxyz]", 123)]
        [TestCase("a-b-c-d-e-f-g-h-987[abcde]", 987)]
        [TestCase("not-a-real-room-404[oarel]", 404)]
        [TestCase("totally-real-room-200[decoy]", 0)]
        public void RoomSectorID(string room, int expected)
        {
            Assert.That(Program.RoomSectorID(room), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("aaaaa-bbb-z-y-x-123[abxyz]", "aaaaa-bbb-z-y-x-")]
        [TestCase("a-b-c-d-e-f-g-h-987[abcde]", "a-b-c-d-e-f-g-h-")]
        [TestCase("not-a-real-room-404[oarel]", "not-a-real-room-")]
        [TestCase("totally-real-room-200[decoy]", "totally-real-room-")]
        public void RoomName(string room, string expected)
        {
            Assert.That(Program.RoomName(room), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("qzmt-zixmtkozy-ivhz-343[zimth]", "very encrypted name")]
        public void DecryptRoomName(string room, string expected)
        {
            Assert.That(Program.DecryptRoomName(room), Is.EqualTo(expected));
        }
    }
}
