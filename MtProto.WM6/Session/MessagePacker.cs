using System;
using MtProto.WM6.Crypto;
using MtProto.WM6.TL;

namespace MtProto.WM6.Session
{
    public sealed class EncryptedMessage
    {
        public ulong AuthKeyId; public byte[] MsgKey; public long ServerSalt; public long SessionId; public long MsgId; public int SeqNo; public byte[] Body;
    }

    public static class MessagePacker
    {
        public static byte[] PackPlain(long msgId, byte[] body)
        {
            TLBinaryWriter w = new TLBinaryWriter(); w.WriteLong(0); w.WriteLong(msgId); w.WriteInt(body.Length); w.WriteRaw(body); return w.ToArray();
        }

        public static byte[] UnpackPlain(byte[] packet)
        {
            TLBinaryReader r = new TLBinaryReader(packet); long keyId = r.ReadLong(); if (keyId != 0) throw new ArgumentException("Not a plain MTProto packet."); r.ReadLong(); int len = r.ReadInt(); return r.ReadRaw(len);
        }

        public static byte[] PackEncrypted(MtSession session, byte[] body, bool contentRelated)
        {
            long msgId = session.NextMessageId(); int seqNo = session.NextSeqNo(contentRelated);
            TLBinaryWriter inner = new TLBinaryWriter(); inner.WriteLong(session.ServerSalt); inner.WriteLong(session.SessionId); inner.WriteLong(msgId); inner.WriteInt(seqNo); inner.WriteInt(body.Length); inner.WriteRaw(body);
            byte[] innerData = Pad16(inner.ToArray()); byte[] msgKey = HashHelper.SubArray(HashHelper.Sha256(HashHelper.Concat(HashHelper.SubArray(session.AuthKey.Key, 88, 32), innerData)), 8, 16);
            byte[] aesKey, aesIv; CalcKey(session.AuthKey.Key, msgKey, true, out aesKey, out aesIv); byte[] encrypted = AesIge.Encrypt(innerData, aesKey, aesIv);
            TLBinaryWriter outer = new TLBinaryWriter(); outer.WriteULong(session.AuthKey.KeyId); outer.WriteRaw(msgKey); outer.WriteRaw(encrypted); return outer.ToArray();
        }

        public static EncryptedMessage UnpackEncrypted(MtSession session, byte[] packet)
        {
            TLBinaryReader r = new TLBinaryReader(packet); EncryptedMessage m = new EncryptedMessage(); m.AuthKeyId = r.ReadULong(); if (m.AuthKeyId != session.AuthKey.KeyId) throw new ArgumentException("Auth key id mismatch.");
            m.MsgKey = r.ReadRaw(16); byte[] encrypted = r.ReadRaw((int)(r.Length - r.Position)); byte[] aesKey, aesIv; CalcKey(session.AuthKey.Key, m.MsgKey, false, out aesKey, out aesIv);
            byte[] plain = AesIge.Decrypt(encrypted, aesKey, aesIv); byte[] check = HashHelper.SubArray(HashHelper.Sha256(HashHelper.Concat(HashHelper.SubArray(session.AuthKey.Key, 96, 32), plain)), 8, 16);
            if (!HashHelper.AreEqual(check, m.MsgKey)) throw new ArgumentException("Message key check failed.");
            TLBinaryReader ir = new TLBinaryReader(plain); m.ServerSalt = ir.ReadLong(); m.SessionId = ir.ReadLong(); m.MsgId = ir.ReadLong(); m.SeqNo = ir.ReadInt(); int len = ir.ReadInt(); m.Body = ir.ReadRaw(len); return m;
        }

        public static void CalcKey(byte[] authKey, byte[] msgKey, bool client, out byte[] aesKey, out byte[] aesIv)
        {
            int x = client ? 0 : 8; byte[] sha256a = HashHelper.Sha256(HashHelper.Concat(msgKey, HashHelper.SubArray(authKey, x, 36))); byte[] sha256b = HashHelper.Sha256(HashHelper.Concat(HashHelper.SubArray(authKey, 40 + x, 36), msgKey));
            aesKey = HashHelper.Concat(HashHelper.SubArray(sha256a, 0, 8), HashHelper.SubArray(sha256b, 8, 16), HashHelper.SubArray(sha256a, 24, 8));
            aesIv = HashHelper.Concat(HashHelper.SubArray(sha256b, 0, 8), HashHelper.SubArray(sha256a, 8, 16), HashHelper.SubArray(sha256b, 24, 8));
        }

        public static byte[] Pad16(byte[] data)
        {
            int pad = 16 - (data.Length % 16); if (pad == 0) pad = 16; return HashHelper.Concat(data, RandomHelper.Bytes(pad));
        }
    }
}
