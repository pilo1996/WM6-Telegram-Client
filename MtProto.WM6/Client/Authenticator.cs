using System;
using System.IO;
using MtProto.WM6.Crypto;
using MtProto.WM6.Generated;
using MtProto.WM6.Session;
using MtProto.WM6.TL;

namespace MtProto.WM6.Client
{
    public sealed class Authenticator
    {
        private readonly MtProtoClient _client;
        public byte[] Nonce { get; private set; }
        public byte[] ServerNonce { get; private set; }
        public byte[] NewNonce { get; private set; }

        public Authenticator(MtProtoClient client) { _client = client; }

        public AuthKey CreateAuthKey(RsaKey[] rsaKeys, int dcId)
        {
            Nonce = RandomHelper.Bytes(16); NewNonce = RandomHelper.Bytes(32);
            ReqPqMulti req = new ReqPqMulti(); req.Nonce = Nonce;
            ResPQ res = ResPQ.Deserialize(_client.SendPlain(req));
            if (!HashHelper.AreEqual(Nonce, res.Nonce)) throw new InvalidDataException("nonce mismatch in resPQ.");
            ServerNonce = res.ServerNonce;

            byte[] p, q; FactorPq(res.Pq, out p, out q);
            RsaKey rsa = SelectRsaKey(rsaKeys, res.Fingerprints); if (rsa == null) throw new InvalidDataException("No matching Telegram RSA key fingerprint.");

            PqInnerData inner = new PqInnerData(); inner.Pq = res.Pq; inner.P = p; inner.Q = q; inner.Nonce = Nonce; inner.ServerNonce = ServerNonce; inner.NewNonce = NewNonce; inner.Dc = dcId;
            byte[] encrypted = RsaEncryptInner(inner, rsa);
            ReqDHParams dhReq = new ReqDHParams(); dhReq.Nonce = Nonce; dhReq.ServerNonce = ServerNonce; dhReq.P = p; dhReq.Q = q; dhReq.PublicKeyFingerprint = rsa.Fingerprint; dhReq.EncryptedData = encrypted;
            byte[] dhBody = _client.SendPlain(dhReq);

            ServerDHInnerData dh = DecodeServerDhParams(dhBody);
            _client.Session.TimeOffset = dh.ServerTime - (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            byte[] b = RandomHelper.Bytes(256);
            byte[] gb = BigInteger.ModPow(new BigInteger(new byte[] { (byte)dh.G }), new BigInteger(b), new BigInteger(dh.DhPrime)).ToByteArrayBigEndian();
            byte[] authKeyBytes = BigInteger.ModPow(new BigInteger(dh.GA), new BigInteger(b), new BigInteger(dh.DhPrime)).ToByteArrayBigEndian();
            authKeyBytes = LeftPad(authKeyBytes, 256);

            ClientDHInnerData cdh = new ClientDHInnerData(); cdh.Nonce = Nonce; cdh.ServerNonce = ServerNonce; cdh.RetryId = 0; cdh.GB = gb;
            byte[] cdhPlain = SerializeWithSha1AndPad(cdh, 16); byte[] aesKey, aesIv; CalcTmpAes(out aesKey, out aesIv); byte[] cdhEncrypted = AesIge.Encrypt(cdhPlain, aesKey, aesIv);
            SetClientDHParams set = new SetClientDHParams(); set.Nonce = Nonce; set.ServerNonce = ServerNonce; set.EncryptedData = cdhEncrypted;
            byte[] setResult = _client.SendPlain(set); ValidateDhGen(setResult, authKeyBytes);

            AuthKey key = new AuthKey(authKeyBytes); _client.Session.AuthKey = key; _client.Session.DcId = dcId; _client.Session.ServerSalt = CalcServerSalt(NewNonce, ServerNonce); return key;
        }

        public byte[] Step1ReqPqMulti() { Nonce = RandomHelper.Bytes(16); ReqPqMulti req = new ReqPqMulti(); req.Nonce = Nonce; return _client.SendPlain(req); }
        public byte[] Step2ReqDhParams(byte[] serverNonce, byte[] p, byte[] q, ulong fingerprint, byte[] encryptedData) { ServerNonce = serverNonce; ReqDHParams req = new ReqDHParams(); req.Nonce = Nonce; req.ServerNonce = serverNonce; req.P = p; req.Q = q; req.PublicKeyFingerprint = fingerprint; req.EncryptedData = encryptedData; return _client.SendPlain(req); }
        public byte[] Step3SetClientDhParams(byte[] encryptedData) { SetClientDHParams req = new SetClientDHParams(); req.Nonce = Nonce; req.ServerNonce = ServerNonce; req.EncryptedData = encryptedData; return _client.SendPlain(req); }

        private ServerDHInnerData DecodeServerDhParams(byte[] body)
        {
            TLBinaryReader r = new TLBinaryReader(body); int c = r.ReadInt(); if (c == TLTypes.ServerDHParamsFail) throw new InvalidDataException("server_DH_params_fail."); if (c != TLTypes.ServerDHParamsOk) throw new InvalidDataException("Expected server_DH_params_ok.");
            byte[] nonce = r.ReadRaw(16); byte[] serverNonce = r.ReadRaw(16); if (!HashHelper.AreEqual(nonce, Nonce) || !HashHelper.AreEqual(serverNonce, ServerNonce)) throw new InvalidDataException("nonce mismatch in server_DH_params_ok.");
            byte[] encrypted = r.ReadBytes(); byte[] aesKey, aesIv; CalcTmpAes(out aesKey, out aesIv); byte[] plain = AesIge.Decrypt(encrypted, aesKey, aesIv);
            byte[] sha = HashHelper.SubArray(plain, 0, 20); byte[] data = HashHelper.SubArray(plain, 20, plain.Length - 20);
            // Padding is included in data; ServerDHInnerData consumes only needed bytes.
            if (!HashHelper.AreEqual(sha, HashHelper.Sha1(TrimToTlObject(data)))) throw new InvalidDataException("server_DH_inner_data SHA1 mismatch.");
            return ServerDHInnerData.Deserialize(data);
        }

        private void ValidateDhGen(byte[] body, byte[] authKey)
        {
            TLBinaryReader r = new TLBinaryReader(body); int c = r.ReadInt(); byte[] nonce = r.ReadRaw(16); byte[] serverNonce = r.ReadRaw(16); if (!HashHelper.AreEqual(nonce, Nonce) || !HashHelper.AreEqual(serverNonce, ServerNonce)) throw new InvalidDataException("nonce mismatch in dh_gen result.");
            byte[] aux = HashHelper.SubArray(HashHelper.Sha1(authKey), 0, 8);
            if (c == TLTypes.DhGenOk) { r.ReadRaw(16); return; }
            if (c == TLTypes.DhGenRetry) throw new InvalidDataException("dh_gen_retry returned by server.");
            if (c == TLTypes.DhGenFail) throw new InvalidDataException("dh_gen_fail returned by server.");
            throw new InvalidDataException("Unknown dh_gen result.");
        }

        private byte[] RsaEncryptInner(TLObject inner, RsaKey rsa)
        {
            byte[] data = SerializeObject(inner); byte[] block = HashHelper.Concat(HashHelper.Sha1(data), data); int target = rsa.Modulus.Length; if (block.Length > target) throw new InvalidDataException("RSA payload too large.");
            block = HashHelper.Concat(block, RandomHelper.Bytes(target - block.Length)); return rsa.Encrypt(block);
        }

        private static byte[] SerializeObject(TLObject o) { TLBinaryWriter w = new TLBinaryWriter(); w.WriteObject(o); return w.ToArray(); }
        private static byte[] SerializeWithSha1AndPad(TLObject o, int block)
        {
            byte[] data = SerializeObject(o); byte[] x = HashHelper.Concat(HashHelper.Sha1(data), data); int pad = block - (x.Length % block); if (pad == 0) pad = block; return HashHelper.Concat(x, RandomHelper.Bytes(pad));
        }
        private void CalcTmpAes(out byte[] key, out byte[] iv)
        {
            byte[] s1 = HashHelper.Sha1(HashHelper.Concat(NewNonce, ServerNonce)); byte[] s2 = HashHelper.Sha1(HashHelper.Concat(ServerNonce, NewNonce)); byte[] s3 = HashHelper.Sha1(NewNonce);
            key = HashHelper.Concat(s1, HashHelper.SubArray(s2, 0, 12)); iv = HashHelper.Concat(HashHelper.SubArray(s2, 12, 8), s3, HashHelper.SubArray(NewNonce, 0, 4));
        }
        private static long CalcServerSalt(byte[] newNonce, byte[] serverNonce)
        {
            byte[] x = new byte[8]; for (int i = 0; i < 8; i++) x[i] = (byte)(newNonce[i] ^ serverNonce[i]); return BitConverter.ToInt64(x, 0);
        }
        private static RsaKey SelectRsaKey(RsaKey[] keys, ulong[] fps)
        {
            if (keys == null || fps == null) return null; for (int i = 0; i < keys.Length; i++) for (int j = 0; j < fps.Length; j++) if (keys[i].Fingerprint == fps[j]) return keys[i]; return null;
        }
        private static byte[] LeftPad(byte[] data, int len) { if (data.Length >= len) return data; byte[] r = new byte[len]; Buffer.BlockCopy(data, 0, r, len - data.Length, data.Length); return r; }

        private static void FactorPq(byte[] pqBytes, out byte[] pBytes, out byte[] qBytes)
        {
            ulong pq = 0; for (int i = 0; i < pqBytes.Length; i++) pq = (pq << 8) | pqBytes[i]; ulong p = PollardRho(pq); ulong q = pq / p; if (p > q) { ulong t = p; p = q; q = t; } pBytes = ToMinimalBigEndian(p); qBytes = ToMinimalBigEndian(q);
        }
        private static ulong PollardRho(ulong n)
        {
            if ((n & 1) == 0) return 2; for (ulong c = 1; c < 10; c++) { ulong x = 2, y = 2, d = 1; while (d == 1) { x = (MulMod(x, x, n) + c) % n; y = (MulMod(y, y, n) + c) % n; y = (MulMod(y, y, n) + c) % n; d = Gcd(x > y ? x - y : y - x, n); } if (d != n) return d; } throw new InvalidDataException("Unable to factor pq.");
        }
        private static ulong MulMod(ulong a, ulong b, ulong m) { ulong r = 0; a %= m; while (b > 0) { if ((b & 1) != 0) r = (r + a) % m; a = (a << 1) % m; b >>= 1; } return r; }
        private static ulong Gcd(ulong a, ulong b) { while (b != 0) { ulong t = a % b; a = b; b = t; } return a; }
        private static byte[] ToMinimalBigEndian(ulong v) { byte[] tmp = BitConverter.GetBytes(v); Array.Reverse(tmp); int i = 0; while (i < tmp.Length - 1 && tmp[i] == 0) i++; byte[] r = new byte[tmp.Length - i]; Buffer.BlockCopy(tmp, i, r, 0, r.Length); return r; }
        private static byte[] TrimToTlObject(byte[] data)
        {
            TLBinaryReader r = new TLBinaryReader(data);
            r.ReadInt(); r.ReadRaw(16); r.ReadRaw(16); r.ReadInt(); r.ReadBytes(); r.ReadBytes(); r.ReadInt();
            int len = (int)r.Position; byte[] x = new byte[len]; Buffer.BlockCopy(data, 0, x, 0, len); return x;
        }
    }
}
