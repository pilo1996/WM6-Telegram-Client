using System;
using System.IO;
using MtProto.WM6.TL;

namespace MtProto.WM6.Runtime
{
    public sealed class BadServerSalt
    {
        public long BadMsgId;
        public int BadMsgSeqNo;
        public int ErrorCode;
        public long NewServerSalt;
        public static BadServerSalt Deserialize(TLBinaryReader r)
        {
            BadServerSalt x = new BadServerSalt();
            x.BadMsgId = r.ReadLong();
            x.BadMsgSeqNo = r.ReadInt();
            x.ErrorCode = r.ReadInt();
            x.NewServerSalt = r.ReadLong();
            return x;
        }
    }

    public sealed class BadMsgNotification
    {
        public long BadMsgId;
        public int BadMsgSeqNo;
        public int ErrorCode;
        public static BadMsgNotification Deserialize(TLBinaryReader r)
        {
            BadMsgNotification x = new BadMsgNotification();
            x.BadMsgId = r.ReadLong();
            x.BadMsgSeqNo = r.ReadInt();
            x.ErrorCode = r.ReadInt();
            return x;
        }
    }

    public sealed class NewSessionCreated
    {
        public long FirstMsgId;
        public long UniqueId;
        public long ServerSalt;
        public static NewSessionCreated Deserialize(TLBinaryReader r)
        {
            NewSessionCreated x = new NewSessionCreated();
            x.FirstMsgId = r.ReadLong();
            x.UniqueId = r.ReadLong();
            x.ServerSalt = r.ReadLong();
            return x;
        }
    }

    public sealed class Pong
    {
        public long MsgId;
        public long PingId;
        public static Pong Deserialize(TLBinaryReader r)
        {
            Pong x = new Pong(); x.MsgId = r.ReadLong(); x.PingId = r.ReadLong(); return x;
        }
    }

    public sealed class GzipPacked
    {
        public byte[] PackedData;
        public static GzipPacked Deserialize(TLBinaryReader r)
        {
            GzipPacked x = new GzipPacked(); x.PackedData = r.ReadBytes(); return x;
        }
    }

    public sealed class BadServerSaltException : Exception
    {
        public BadServerSalt Salt { get; private set; }
        public BadServerSaltException(BadServerSalt salt) : base("bad_server_salt: " + salt.ErrorCode) { Salt = salt; }
    }

    public sealed class BadMessageException : Exception
    {
        public BadMsgNotification Notification { get; private set; }
        public BadMessageException(BadMsgNotification notification) : base("bad_msg_notification: " + notification.ErrorCode) { Notification = notification; }
    }
}
