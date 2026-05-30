using MtProto.WM6.Generated;
using MtProto.WM6.Crypto;
using MtProto.WM6.Session;
using MtProto.WM6.TL;
using MtProto.WM6.Runtime;
using MtProto.WM6.Transport;

namespace MtProto.WM6.Client
{
    public sealed class MtProtoClient
    {
        private readonly IMtProtoTransport _transport;
        public MtSession Session { get; private set; }

        public MtProtoClient() : this(new TcpAbridgedTransport()) { }
        public MtProtoClient(IMtProtoTransport transport) { _transport = transport; Session = new MtSession(); TLBootstrap.RegisterCore(); }
        public void UseSession(MtSession session) { Session = session; }
        public void Connect(string host, int port) { _transport.Connect(host, port); }

        public byte[] SendPlain(TLObject request)
        {
            TLBinaryWriter body = new TLBinaryWriter(); body.WriteObject(request); byte[] packet = MessagePacker.PackPlain(Session.NextMessageId(), body.ToArray()); _transport.Send(packet); return MessagePacker.UnpackPlain(_transport.Receive());
        }

        public byte[] SendEncryptedRaw(TLObject request, bool contentRelated)
        {
            TLBinaryWriter body = new TLBinaryWriter();
            body.WriteObject(request);
            byte[] packet = MessagePacker.PackEncrypted(Session, body.ToArray(), contentRelated);
            _transport.Send(packet);
            return MessagePacker.UnpackEncrypted(Session, _transport.Receive()).Body;
        }

        public object SendEncrypted(TLObject request, bool contentRelated)
        {
            byte[] body = SendEncryptedRaw(request, contentRelated);
            return RpcParser.ParseRpcEnvelope(body);
        }

        public void MigrateToDc(int dcId, bool testMode)
        {
            DcEndpoint ep = testMode ? DcOptions.Test(dcId) : DcOptions.Production(dcId);
            _transport.Close();
            _transport.Connect(ep.Host, ep.Port);
            Session.DcId = dcId;
        }

        public object SendWithMigration(TLObject request, bool contentRelated, bool testMode)
        {
            try { return SendEncrypted(request, contentRelated); }
            catch (BadServerSaltException ex)
            {
                Session.ServerSalt = ex.Salt.NewServerSalt;
                return SendEncrypted(request, contentRelated);
            }
            catch (RpcException ex)
            {
                if (ex.MigrationDc > 0)
                {
                    MigrateToDc(ex.MigrationDc, testMode);
                    return SendEncrypted(request, contentRelated);
                }
                throw;
            }
        }

        public object AuthSendCode(string phoneNumber, int apiId, string apiHash, bool testMode)
        {
            AuthSendCode r = new AuthSendCode(); r.PhoneNumber = phoneNumber; r.ApiId = apiId; r.ApiHash = apiHash; return SendWithMigration(r, true, testMode);
        }
        public object AuthSignIn(string phoneNumber, string phoneCodeHash, string phoneCode, bool testMode)
        {
            AuthSignIn r = new AuthSignIn(); r.PhoneNumber = phoneNumber; r.PhoneCodeHash = phoneCodeHash; r.PhoneCode = phoneCode; return SendWithMigration(r, true, testMode);
        }
        public object AccountGetPassword(bool testMode)
        {
            return SendWithMigration(new AccountGetPassword(), true, testMode);
        }
        public object AuthCheckPassword(InputCheckPasswordSRP password, bool testMode)
        {
            AuthCheckPassword r = new AuthCheckPassword(); r.Password = password; return SendWithMigration(r, true, testMode);
        }
        public object AuthCheckPassword(AccountPassword accountPassword, string cloudPassword, bool testMode)
        {
            return AuthCheckPassword(PasswordSrp.Compute(accountPassword, cloudPassword), testMode);
        }
        public object SendMessage(InputPeer peer, string message, bool testMode)
        {
            MessagesSendMessage r = new MessagesSendMessage(); r.Flags = 0; r.Peer = peer; r.Message = message; r.RandomId = RandomHelper.Int64(); return SendWithMigration(r, true, testMode);
        }
        public void Close() { _transport.Close(); }
    }
}
