using System;

namespace MtProto.WM6.Crypto
{
    public sealed class RsaKey
    {
        public ulong Fingerprint;
        public byte[] Modulus;
        public byte[] Exponent;

        public byte[] EncryptRaw(byte[] data)
        {
            if (Modulus == null || Exponent == null) throw new InvalidOperationException("RSA key is incomplete.");
            BigInteger m = BigInteger.FromBigEndian(data);
            BigInteger e = BigInteger.FromBigEndian(Exponent);
            BigInteger n = BigInteger.FromBigEndian(Modulus);
            BigInteger c = BigInteger.ModPow(m, e, n);
            byte[] raw = c.ToByteArrayBigEndian();
            int size = Modulus.Length;
            if (raw.Length > size) throw new InvalidOperationException("RSA output is larger than modulus.");
            byte[] padded = new byte[size];
            Buffer.BlockCopy(raw, 0, padded, size - raw.Length, raw.Length);
            return padded;
        }

        // Telegram auth uses RSA over a padded structure. The caller must pass the exact encoded block.
        public byte[] Encrypt(byte[] data) { return EncryptRaw(data); }
    }
}
