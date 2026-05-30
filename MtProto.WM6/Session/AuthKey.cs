using System;
using MtProto.WM6.Crypto;

namespace MtProto.WM6.Session
{
    public sealed class AuthKey
    {
        public byte[] Key;
        public ulong KeyId;

        public AuthKey(byte[] key)
        {
            Key = key;
            byte[] sha1 = HashHelper.Sha1(key);
            KeyId = BitConverter.ToUInt64(sha1, 12);
        }
    }
}
