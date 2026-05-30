using System;

namespace MtProto.WM6.Crypto
{
    // Unsigned little-endian 32-bit-limb BigInteger for .NET Compact Framework.
    // Implements only what MTProto auth needs: parse, compare, add/sub, mul, divmod, modpow.
    public sealed class BigInteger
    {
        private uint[] _data; // least significant limb first

        public static readonly BigInteger Zero = new BigInteger(0u);
        public static readonly BigInteger One = new BigInteger(1u);

        public BigInteger(uint value)
        {
            _data = value == 0 ? new uint[0] : new uint[] { value };
        }

        public BigInteger(byte[] bigEndian)
        {
            if (bigEndian == null) throw new ArgumentNullException("bigEndian");
            int len = (bigEndian.Length + 3) / 4;
            _data = new uint[len];
            int byteIndex = bigEndian.Length;
            for (int i = 0; i < len; i++)
            {
                uint limb = 0;
                for (int j = 0; j < 4 && byteIndex > 0; j++)
                    limb |= (uint)bigEndian[--byteIndex] << (8 * j);
                _data[i] = limb;
            }
            Normalize();
        }

        private BigInteger(uint[] limbs, bool trusted)
        {
            _data = limbs;
            Normalize();
        }

        public static BigInteger FromBigEndian(byte[] data) { return new BigInteger(data); }
        public bool IsZero { get { return _data.Length == 0; } }
        public bool IsOdd { get { return _data.Length != 0 && ((_data[0] & 1) == 1); } }

        public byte[] ToByteArrayBigEndian()
        {
            if (_data.Length == 0) return new byte[] { 0 };
            int bytes = _data.Length * 4;
            uint top = _data[_data.Length - 1];
            while (bytes > 1 && ((top >> 24) & 0xff) == 0) { top <<= 8; bytes--; }
            byte[] r = new byte[bytes];
            int p = bytes;
            for (int i = 0; i < _data.Length; i++)
            {
                uint v = _data[i];
                for (int j = 0; j < 4 && p > 0; j++) r[--p] = (byte)(v >> (8 * j));
            }
            return r;
        }

        private void Normalize()
        {
            int n = _data.Length;
            while (n > 0 && _data[n - 1] == 0) n--;
            if (n != _data.Length)
            {
                uint[] t = new uint[n];
                Array.Copy(_data, t, n);
                _data = t;
            }
        }

        public int CompareTo(BigInteger other)
        {
            if (_data.Length != other._data.Length) return _data.Length < other._data.Length ? -1 : 1;
            for (int i = _data.Length - 1; i >= 0; i--)
                if (_data[i] != other._data[i]) return _data[i] < other._data[i] ? -1 : 1;
            return 0;
        }

        public static BigInteger Add(BigInteger a, BigInteger b)
        {
            int n = Math.Max(a._data.Length, b._data.Length);
            uint[] r = new uint[n + 1];
            ulong carry = 0;
            for (int i = 0; i < n; i++)
            {
                ulong av = i < a._data.Length ? a._data[i] : 0;
                ulong bv = i < b._data.Length ? b._data[i] : 0;
                ulong s = av + bv + carry;
                r[i] = (uint)s;
                carry = s >> 32;
            }
            r[n] = (uint)carry;
            return new BigInteger(r, true);
        }

        public static BigInteger Sub(BigInteger a, BigInteger b)
        {
            if (a.CompareTo(b) < 0) throw new ArgumentException("Unsigned subtraction underflow.");
            uint[] r = new uint[a._data.Length];
            long borrow = 0;
            for (int i = 0; i < a._data.Length; i++)
            {
                long av = (long)a._data[i];
                long bv = i < b._data.Length ? (long)b._data[i] : 0;
                long v = av - bv - borrow;
                if (v < 0) { v += 0x100000000L; borrow = 1; } else borrow = 0;
                r[i] = (uint)v;
            }
            return new BigInteger(r, true);
        }

        public static BigInteger Mul(BigInteger a, BigInteger b)
        {
            if (a.IsZero || b.IsZero) return Zero;
            uint[] r = new uint[a._data.Length + b._data.Length];
            for (int i = 0; i < a._data.Length; i++)
            {
                ulong carry = 0;
                for (int j = 0; j < b._data.Length; j++)
                {
                    ulong cur = r[i + j] + carry + (ulong)a._data[i] * (ulong)b._data[j];
                    r[i + j] = (uint)cur;
                    carry = cur >> 32;
                }
                int k = i + b._data.Length;
                while (carry != 0)
                {
                    ulong cur = (ulong)r[k] + carry;
                    r[k] = (uint)cur;
                    carry = cur >> 32;
                    k++;
                }
            }
            return new BigInteger(r, true);
        }

        public int BitLength()
        {
            if (_data.Length == 0) return 0;
            uint top = _data[_data.Length - 1];
            int bits = 32;
            while (bits > 0 && ((top >> (bits - 1)) & 1) == 0) bits--;
            return (_data.Length - 1) * 32 + bits;
        }

        private bool TestBit(int bit)
        {
            int limb = bit / 32;
            int off = bit % 32;
            return limb < _data.Length && ((_data[limb] >> off) & 1) != 0;
        }

        private static BigInteger ShiftLeftOne(BigInteger a)
        {
            if (a.IsZero) return Zero;
            uint[] r = new uint[a._data.Length + 1];
            uint carry = 0;
            for (int i = 0; i < a._data.Length; i++)
            {
                uint next = a._data[i] >> 31;
                r[i] = (a._data[i] << 1) | carry;
                carry = next;
            }
            r[a._data.Length] = carry;
            return new BigInteger(r, true);
        }

        public static void DivMod(BigInteger n, BigInteger d, out BigInteger q, out BigInteger r)
        {
            if (d.IsZero) throw new DivideByZeroException();
            q = Zero; r = Zero;
            int bits = n.BitLength();
            for (int i = bits - 1; i >= 0; i--)
            {
                r = ShiftLeftOne(r);
                if (n.TestBit(i)) r = Add(r, One);
                q = ShiftLeftOne(q);
                if (r.CompareTo(d) >= 0)
                {
                    r = Sub(r, d);
                    q = Add(q, One);
                }
            }
        }

        public static BigInteger Mod(BigInteger a, BigInteger m)
        {
            BigInteger q, r; DivMod(a, m, out q, out r); return r;
        }

        public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus)
        {
            if (modulus.IsZero) throw new DivideByZeroException();
            BigInteger result = One;
            BigInteger b = Mod(value, modulus);
            int bits = exponent.BitLength();
            for (int i = 0; i < bits; i++)
            {
                if (exponent.TestBit(i)) result = Mod(Mul(result, b), modulus);
                b = Mod(Mul(b, b), modulus);
            }
            return result;
        }
    }
}
