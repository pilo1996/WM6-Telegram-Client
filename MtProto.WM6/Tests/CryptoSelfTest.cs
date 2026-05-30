using System;
using MtProto.WM6.Crypto;

namespace MtProto.WM6.Tests
{
    public static class CryptoSelfTest
    {
        public static void Run()
        {
            BigInteger two = new BigInteger(new byte[] { 2 });
            BigInteger ten = new BigInteger(new byte[] { 10 });
            BigInteger mod = new BigInteger(new byte[] { 17 });
            byte[] got = BigInteger.ModPow(two, ten, mod).ToByteArrayBigEndian();
            if (got.Length != 1 || got[0] != 4) throw new Exception("BigInteger.ModPow self-test failed.");

            byte[] key = new byte[32];
            byte[] iv = new byte[32];
            byte[] plain = new byte[16];
            byte[] enc = AesIge.Encrypt(plain, key, iv);
            byte[] dec = AesIge.Decrypt(enc, key, iv);
            if (!HashHelper.AreEqual(plain, dec)) throw new Exception("AES-IGE self-test failed.");
        }
    }
}
