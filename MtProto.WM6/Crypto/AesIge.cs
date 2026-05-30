using System;
using System.IO;
using System.Security.Cryptography;

namespace MtProto.WM6.Crypto
{
    public static class AesIge
    {
        private const int BlockSize = 16;

        public static byte[] Encrypt(byte[] plain, byte[] key, byte[] iv32)
        {
            if (plain.Length % BlockSize != 0) throw new ArgumentException("Plaintext length must be multiple of 16.");
            byte[] result = new byte[plain.Length];
            byte[] xPrev = HashHelper.SubArray(iv32, 0, 16);
            byte[] yPrev = HashHelper.SubArray(iv32, 16, 16);

            for (int i = 0; i < plain.Length; i += BlockSize)
            {
                byte[] block = HashHelper.SubArray(plain, i, BlockSize);
                XorInPlace(block, xPrev);
                byte[] enc = AesEcb(block, key, true);
                XorInPlace(enc, yPrev);
                Buffer.BlockCopy(enc, 0, result, i, BlockSize);
                xPrev = enc;
                yPrev = HashHelper.SubArray(plain, i, BlockSize);
            }
            return result;
        }

        public static byte[] Decrypt(byte[] cipher, byte[] key, byte[] iv32)
        {
            if (cipher.Length % BlockSize != 0) throw new ArgumentException("Ciphertext length must be multiple of 16.");
            byte[] result = new byte[cipher.Length];
            byte[] xPrev = HashHelper.SubArray(iv32, 0, 16);
            byte[] yPrev = HashHelper.SubArray(iv32, 16, 16);

            for (int i = 0; i < cipher.Length; i += BlockSize)
            {
                byte[] block = HashHelper.SubArray(cipher, i, BlockSize);
                byte[] tmp = (byte[])block.Clone();
                XorInPlace(tmp, yPrev);
                byte[] dec = AesEcb(tmp, key, false);
                XorInPlace(dec, xPrev);
                Buffer.BlockCopy(dec, 0, result, i, BlockSize);
                xPrev = block;
                yPrev = dec;
            }
            return result;
        }

        private static byte[] AesEcb(byte[] input, byte[] key, bool encrypt)
        {
            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = key.Length * 8;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;
                aes.Key = key;
                using (ICryptoTransform tr = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor())
                {
                    return tr.TransformFinalBlock(input, 0, input.Length);
                }
            }
        }

        private static void XorInPlace(byte[] a, byte[] b)
        {
            for (int i = 0; i < a.Length; i++) a[i] ^= b[i];
        }
    }
}
