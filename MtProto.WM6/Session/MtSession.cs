using System;
using MtProto.WM6.Crypto;

namespace MtProto.WM6.Session
{
    public sealed class MtSession
    {
        public AuthKey AuthKey;
        public long ServerSalt;
        public long SessionId;
        public int TimeOffset;
        public int DcId;
        private int _seqNo;
        private long _lastMessageId;

        public MtSession() { SessionId = RandomHelper.Int64(); }

        public int NextSeqNo(bool contentRelated) { int value = contentRelated ? _seqNo * 2 + 1 : _seqNo * 2; if (contentRelated) _seqNo++; return value; }

        public long NextMessageId()
        {
            long unix = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + TimeOffset; long id = unix << 32; id &= ~3L; if (id <= _lastMessageId) id = _lastMessageId + 4; _lastMessageId = id; return id;
        }
    }
}
