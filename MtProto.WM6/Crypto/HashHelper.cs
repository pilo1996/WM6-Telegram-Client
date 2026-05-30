using System;
using System.Security.Cryptography;

namespace MtProto.WM6.Crypto
{
    public static class HashHelper
    {
        public static byte[] Sha1(byte[] data) { using (SHA1 sha = SHA1.Create()) return sha.ComputeHash(data); }
        public static byte[] Sha256(byte[] data) { using (SHA256 sha = SHA256.Create()) return sha.ComputeHash(data); }
        public static byte[] Concat(params byte[][] arrays)
        {
            int len = 0; for (int i = 0; i < arrays.Length; i++) if (arrays[i] != null) len += arrays[i].Length;
            byte[] result = new byte[len]; int p = 0; for (int i = 0; i < arrays.Length; i++) { if (arrays[i] == null) continue; Buffer.BlockCopy(arrays[i], 0, result, p, arrays[i].Length); p += arrays[i].Length; } return result;
        }
        public static byte[] SubArray(byte[] data, int offset, int count) { byte[] r = new byte[count]; Buffer.BlockCopy(data, offset, r, 0, count); return r; }
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false; int diff = 0; for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i]; return diff == 0;
        }
        public static byte[] Xor(byte[] a, byte[] b)
        {
            byte[] r = new byte[a.Length]; for (int i = 0; i < a.Length; i++) r[i] = (byte)(a[i] ^ b[i]); return r;
        }
    }
}
