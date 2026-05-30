using System;
using System.Security.Cryptography;

namespace MtProto.WM6.Crypto
{
    public static class RandomHelper
    {
        public static byte[] Bytes(int count)
        {
            byte[] b = new byte[count];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
            return b;
        }

        public static long Int64()
        {
            byte[] b = Bytes(8);
            return BitConverter.ToInt64(b, 0);
        }
    }
}
