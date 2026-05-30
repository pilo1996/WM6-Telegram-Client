using System;
using System.IO;
using MtProto.WM6.Crypto;

namespace MtProto.WM6.Session
{
    public static class SessionStore
    {
        private const int Magic = 0x574d3653; // WM6S
        public static void Save(string path, MtSession session)
        {
            using (FileStream fs = File.Create(path))
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write(Magic); w.Write(1); w.Write(session.ServerSalt); w.Write(session.SessionId); w.Write(session.TimeOffset); w.Write(session.DcId);
                byte[] key = session.AuthKey == null ? new byte[0] : session.AuthKey.Key; w.Write(key.Length); w.Write(key);
            }
        }
        public static MtSession Load(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            using (BinaryReader r = new BinaryReader(fs))
            {
                if (r.ReadInt32() != Magic) throw new InvalidDataException("Not a MtProto.WM6 session file."); r.ReadInt32();
                MtSession s = new MtSession(); s.ServerSalt = r.ReadInt64(); s.SessionId = r.ReadInt64(); s.TimeOffset = r.ReadInt32(); s.DcId = r.ReadInt32(); int len = r.ReadInt32(); if (len > 0) s.AuthKey = new AuthKey(r.ReadBytes(len)); return s;
            }
        }
    }
}
