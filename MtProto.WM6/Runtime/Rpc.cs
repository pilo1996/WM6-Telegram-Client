using System;
using System.IO;
using MtProto.WM6.TL;

namespace MtProto.WM6.Runtime
{
    public sealed class RpcResult
    {
        public long RequestMsgId;
        public object Result;
    }

    public sealed class RpcError
    {
        public int ErrorCode;
        public string ErrorMessage;
        public static RpcError Deserialize(TLBinaryReader r)
        {
            RpcError e = new RpcError();
            e.ErrorCode = r.ReadInt();
            e.ErrorMessage = r.ReadString();
            return e;
        }
    }

    public class RpcException : Exception
    {
        public int ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public int MigrationDc { get; private set; }
        public int FloodWaitSeconds { get; private set; }

        public RpcException(RpcError error) : base(error.ErrorCode + ": " + error.ErrorMessage)
        {
            ErrorCode = error.ErrorCode;
            ErrorMessage = error.ErrorMessage;
            MigrationDc = ParseSuffix(error.ErrorMessage, "_MIGRATE_");
            FloodWaitSeconds = ParseSuffix(error.ErrorMessage, "FLOOD_WAIT_");
        }

        private static int ParseSuffix(string text, string marker)
        {
            int p = text == null ? -1 : text.IndexOf(marker);
            if (p < 0) return 0;
            p += marker.Length;
            int v = 0;
            while (p < text.Length && text[p] >= '0' && text[p] <= '9')
            {
                v = (v * 10) + (text[p] - '0');
                p++;
            }
            return v;
        }
    }

    public sealed class SessionPasswordNeededException : RpcException
    {
        public SessionPasswordNeededException(RpcError error) : base(error) { }
    }

    public static class RpcParser
    {
        public static object ParseRpcEnvelope(byte[] body)
        {
            TLBinaryReader r = new TLBinaryReader(body);
            int c = r.ReadInt();
            if (c == TLTypes.RpcResult)
            {
                RpcResult rr = new RpcResult();
                rr.RequestMsgId = r.ReadLong();
                int inner = r.ReadInt();
                rr.Result = ParseInner(r, inner);
                return rr;
            }
            return ParseInner(r, c);
        }

        public static object ParseInner(TLBinaryReader r, int c)
        {
            if (c == TLTypes.RpcError)
            {
                RpcError e = RpcError.Deserialize(r);
                if (e.ErrorMessage == "SESSION_PASSWORD_NEEDED") throw new SessionPasswordNeededException(e);
                throw new RpcException(e);
            }
            if (c == TLTypes.BadServerSalt) throw new BadServerSaltException(BadServerSalt.Deserialize(r));
            if (c == TLTypes.BadMsgNotification) throw new BadMessageException(BadMsgNotification.Deserialize(r));
            if (c == TLTypes.NewSessionCreated) return NewSessionCreated.Deserialize(r);
            if (c == TLTypes.Pong) return Pong.Deserialize(r);
            if (c == TLTypes.GzipPacked) return GzipPacked.Deserialize(r);
            if (c == TLTypes.MsgContainer) return MessageContainer.Deserialize(r);
            return TLRegistry.DeserializeKnown(r, c);
        }
    }
}
