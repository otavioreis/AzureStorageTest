using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Util
{
    public static class SequentialGuid
    {
        private static readonly DateTime BaseDate = new DateTime(2015, 1, 1, 0, 0, 0);

        public static Guid NewGuid()
        {
            var randomPart = Guid.NewGuid().ToByteArray();

            return NewGuid(randomPart, BaseDate, DateTime.UtcNow);
        }

        public static Guid NewGuid(byte[] randomPart, DateTime baseDate, DateTime endDate)
        {
            var guidPayload = new byte[16];
            var timestamp = endDate.Ticks - BaseDate.Ticks;
            byte[] sequencePart = BitConverter.GetBytes(timestamp);

            //first 10 bytes of 16 bytes - [0] to [9]
            Buffer.BlockCopy(randomPart, 0, guidPayload, 0, 10);

            //last 6 bytes of 8 bytes - [2] to [7]
            Buffer.BlockCopy(sequencePart, 2, guidPayload, 10, 6);

            return new Guid(guidPayload);
        }
    }
}
