using MtProto.WM6.Runtime;
using MtProto.WM6.TL;

namespace MtProto.WM6.Generated
{
    public static class TLBootstrap
    {
        private static bool _done;
        public static void RegisterCore()
        {
            if (_done) return;
            _done = true;
            TLRegistry.Register(TLTypes.RpcError, "rpc_error", delegate(TLBinaryReader r) { return RpcError.Deserialize(r); });
            TLRegistry.Register(TLTypes.MsgContainer, "msg_container", delegate(TLBinaryReader r) { return MessageContainer.Deserialize(r); });

            TLRegistry.Register(TLTypes.BadServerSalt, "bad_server_salt", delegate(TLBinaryReader r) { return BadServerSalt.Deserialize(r); });
            TLRegistry.Register(TLTypes.BadMsgNotification, "bad_msg_notification", delegate(TLBinaryReader r) { return BadMsgNotification.Deserialize(r); });
            TLRegistry.Register(TLTypes.NewSessionCreated, "new_session_created", delegate(TLBinaryReader r) { return NewSessionCreated.Deserialize(r); });
            TLRegistry.Register(TLTypes.Pong, "pong", delegate(TLBinaryReader r) { return Pong.Deserialize(r); });
            TLRegistry.Register(TLTypes.GzipPacked, "gzip_packed", delegate(TLBinaryReader r) { return GzipPacked.Deserialize(r); });
            TLRegistry.Register(TLTypes.AccountPassword, "account.password", delegate(TLBinaryReader r) { return AccountPassword.Deserialize(r); });
            TLRegistry.Register(TLTypes.ResPQ, "resPQ", delegate(TLBinaryReader r) {
                ResPQ x = new ResPQ(); x.Nonce = r.ReadRaw(16); x.ServerNonce = r.ReadRaw(16); x.Pq = r.ReadBytes(); x.Fingerprints = r.ReadULongVector(); return x;
            });
        }
    }
}
