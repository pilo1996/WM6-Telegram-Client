using System;
using System.Security.Cryptography;
using MtProto.WM6.Crypto;
using MtProto.WM6.Generated;

namespace MtProto.WM6.Runtime
{
    public static class PasswordSrp
    {
        public static InputCheckPasswordSRP FromKnownProof(long srpId, byte[] a, byte[] m1)
        {
            InputCheckPasswordSRP p = new InputCheckPasswordSRP();
            p.SrpId = srpId;
            p.A = a;
            p.M1 = m1;
            return p;
        }

        public static InputCheckPasswordSRP Compute(AccountPassword accountPassword, string password)
        {
            if (accountPassword == null) throw new ArgumentNullException("accountPassword");
            PasswordKdfAlgoModPow algo = accountPassword.CurrentAlgo as PasswordKdfAlgoModPow;
            if (algo == null) throw new NotSupportedException("Unsupported Telegram password KDF algorithm.");
            return Compute(accountPassword.SrpId, accountPassword.SrpB, algo, password);
        }

        public static InputCheckPasswordSRP Compute(long srpId, byte[] srpB, PasswordKdfAlgoModPow algo, string password)
        {
            if (srpB == null || srpB.Length == 0) throw new ArgumentException("srp_B is empty.");
            ValidatePrimeAndGenerator(algo.P, algo.G);

            byte[] pBytes = Pad(algo.P, 256);
            byte[] gBytes = Pad(ToBigEndian((uint)algo.G), 256);
            BigInteger p = new BigInteger(algo.P);
            BigInteger g = new BigInteger(new byte[] { (byte)algo.G });
            BigInteger B = new BigInteger(srpB);
            if (B.IsZero || B.CompareTo(p) >= 0) throw new ArgumentException("Invalid SRP B value.");

            byte[] xBytes = ComputeX(algo.Salt1, algo.Salt2, password);
            BigInteger x = new BigInteger(xBytes);
            byte[] aBytes = RandomHelper.Bytes(256);
            BigInteger a = new BigInteger(aBytes);
            byte[] ABytes = Pad(BigInteger.ModPow(g, a, p).ToByteArrayBigEndian(), 256);
            BigInteger A = new BigInteger(ABytes);
            if (A.IsZero) throw new ArgumentException("Invalid SRP A value.");

            byte[] kBytes = HashHelper.Sha256(HashHelper.Concat(pBytes, gBytes));
            BigInteger k = new BigInteger(kBytes);
            byte[] uBytes = HashHelper.Sha256(HashHelper.Concat(ABytes, Pad(srpB, 256)));
            BigInteger u = new BigInteger(uBytes);
            if (u.IsZero) throw new ArgumentException("Invalid SRP u value.");

            BigInteger gx = BigInteger.ModPow(g, x, p);
            BigInteger kgx = BigInteger.Mod(BigInteger.Mul(k, gx), p);
            BigInteger baseValue = B.CompareTo(kgx) >= 0 ? BigInteger.Sub(B, kgx) : BigInteger.Sub(BigInteger.Add(B, p), kgx);
            BigInteger exp = BigInteger.Add(a, BigInteger.Mul(u, x));
            BigInteger S = BigInteger.ModPow(baseValue, exp, p);
            byte[] K = HashHelper.Sha256(Pad(S.ToByteArrayBigEndian(), 256));

            byte[] hP = HashHelper.Sha256(pBytes);
            byte[] hG = HashHelper.Sha256(gBytes);
            byte[] xor = HashHelper.Xor(hP, hG);
            byte[] m1 = HashHelper.Sha256(HashHelper.Concat(
                xor,
                HashHelper.Sha256(algo.Salt1),
                HashHelper.Sha256(algo.Salt2),
                ABytes,
                Pad(srpB, 256),
                K));

            return FromKnownProof(srpId, ABytes, m1);
        }

        private static byte[] ComputeX(byte[] salt1, byte[] salt2, string password)
        {
            byte[] pwd = System.Text.Encoding.UTF8.GetBytes(password == null ? "" : password);
            byte[] x = HashHelper.Sha256(HashHelper.Concat(salt1, pwd, salt1));
            x = HashHelper.Sha256(HashHelper.Concat(salt1, x, salt1));
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(x, salt2, 100000))
            {
                x = pbkdf2.GetBytes(64);
            }
            return HashHelper.Sha256(HashHelper.Concat(salt2, x, salt2));
        }

        private static void ValidatePrimeAndGenerator(byte[] p, int g)
        {
            if (p == null || p.Length < 128) throw new ArgumentException("SRP prime is too small.");
            if (g < 2 || g > 7) throw new ArgumentException("Unsupported SRP generator.");
            if ((p[p.Length - 1] & 1) == 0) throw new ArgumentException("SRP prime must be odd.");
        }

        private static byte[] Pad(byte[] value, int len)
        {
            if (value == null) value = new byte[0];
            if (value.Length == len) return value;
            if (value.Length > len)
            {
                byte[] cut = new byte[len];
                Buffer.BlockCopy(value, value.Length - len, cut, 0, len);
                return cut;
            }
            byte[] r = new byte[len];
            Buffer.BlockCopy(value, 0, r, len - value.Length, value.Length);
            return r;
        }

        private static byte[] ToBigEndian(uint value)
        {
            byte[] b = BitConverter.GetBytes(value);
            Array.Reverse(b);
            int i = 0; while (i < b.Length - 1 && b[i] == 0) i++;
            byte[] r = new byte[b.Length - i];
            Buffer.BlockCopy(b, i, r, 0, r.Length);
            return r;
        }
    }
}
