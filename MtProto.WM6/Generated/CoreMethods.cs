using MtProto.WM6.TL;

namespace MtProto.WM6.Generated
{
    public sealed class ReqPqMulti : TLObject
    {
        public byte[] Nonce;
        public override int ConstructorId { get { return TLTypes.ReqPqMulti; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteRaw(Nonce); }
    }

    public sealed class ResPQ
    {
        public byte[] Nonce; public byte[] ServerNonce; public byte[] Pq; public ulong[] Fingerprints;
        public static ResPQ Deserialize(byte[] body)
        {
            TLBinaryReader r = new TLBinaryReader(body);
            int c = r.ReadInt(); if (c != TLTypes.ResPQ) throw new System.IO.InvalidDataException("Expected resPQ.");
            ResPQ x = new ResPQ(); x.Nonce = r.ReadRaw(16); x.ServerNonce = r.ReadRaw(16); x.Pq = r.ReadBytes(); x.Fingerprints = r.ReadULongVector(); return x;
        }
    }

    public sealed class ReqDHParams : TLObject
    {
        public byte[] Nonce; public byte[] ServerNonce; public byte[] P; public byte[] Q; public ulong PublicKeyFingerprint; public byte[] EncryptedData;
        public override int ConstructorId { get { return TLTypes.ReqDHParams; } }
        public override void Serialize(TLBinaryWriter writer)
        { writer.WriteRaw(Nonce); writer.WriteRaw(ServerNonce); writer.WriteBytes(P); writer.WriteBytes(Q); writer.WriteULong(PublicKeyFingerprint); writer.WriteBytes(EncryptedData); }
    }

    public sealed class SetClientDHParams : TLObject
    {
        public byte[] Nonce; public byte[] ServerNonce; public byte[] EncryptedData;
        public override int ConstructorId { get { return TLTypes.SetClientDHParams; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteRaw(Nonce); writer.WriteRaw(ServerNonce); writer.WriteBytes(EncryptedData); }
    }

    public sealed class PqInnerData : TLObject
    {
        public byte[] Pq; public byte[] P; public byte[] Q; public byte[] Nonce; public byte[] ServerNonce; public byte[] NewNonce; public int Dc;
        public override int ConstructorId { get { return Dc == 0 ? TLTypes.PQInnerData : TLTypes.PQInnerDataDc; } }
        public override void Serialize(TLBinaryWriter writer)
        { writer.WriteBytes(Pq); writer.WriteBytes(P); writer.WriteBytes(Q); writer.WriteRaw(Nonce); writer.WriteRaw(ServerNonce); writer.WriteRaw(NewNonce); if (Dc != 0) writer.WriteInt(Dc); }
    }

    public sealed class ServerDHInnerData
    {
        public byte[] Nonce; public byte[] ServerNonce; public int G; public byte[] DhPrime; public byte[] GA; public int ServerTime;
        public static ServerDHInnerData Deserialize(byte[] plain)
        {
            TLBinaryReader r = new TLBinaryReader(plain); int c = r.ReadInt(); if (c != TLTypes.ServerDHInnerData) throw new System.IO.InvalidDataException("Expected server_DH_inner_data.");
            ServerDHInnerData x = new ServerDHInnerData(); x.Nonce = r.ReadRaw(16); x.ServerNonce = r.ReadRaw(16); x.G = r.ReadInt(); x.DhPrime = r.ReadBytes(); x.GA = r.ReadBytes(); x.ServerTime = r.ReadInt(); return x;
        }
    }

    public sealed class ClientDHInnerData : TLObject
    {
        public byte[] Nonce; public byte[] ServerNonce; public long RetryId; public byte[] GB;
        public override int ConstructorId { get { return TLTypes.ClientDHInnerData; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteRaw(Nonce); writer.WriteRaw(ServerNonce); writer.WriteLong(RetryId); writer.WriteBytes(GB); }
    }

    public sealed class AuthCodeSettings : TLObject
    {
        public override int ConstructorId { get { return TLTypes.CodeSettings; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteInt(0); }
    }

    public sealed class AuthSendCode : TLObject
    {
        public string PhoneNumber; public int ApiId; public string ApiHash; public AuthCodeSettings Settings = new AuthCodeSettings();
        public override int ConstructorId { get { return TLTypes.AuthSendCode; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteString(PhoneNumber); writer.WriteInt(ApiId); writer.WriteString(ApiHash); writer.WriteObject(Settings); }
    }

    public sealed class AuthSignIn : TLObject
    {
        public string PhoneNumber; public string PhoneCodeHash; public string PhoneCode;
        public override int ConstructorId { get { return TLTypes.AuthSignIn; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteInt(1); writer.WriteString(PhoneNumber); writer.WriteString(PhoneCodeHash); writer.WriteString(PhoneCode); }
    }

    public abstract class InputPeer : TLObject { }
    public sealed class InputPeerUser : InputPeer
    {
        public long UserId; public long AccessHash;
        public override int ConstructorId { get { return TLTypes.InputPeerUser; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteLong(UserId); writer.WriteLong(AccessHash); }
    }
    public sealed class InputPeerChat : InputPeer
    {
        public long ChatId; public override int ConstructorId { get { return TLTypes.InputPeerChat; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteLong(ChatId); }
    }
    public sealed class InputPeerChannel : InputPeer
    {
        public long ChannelId; public long AccessHash;
        public override int ConstructorId { get { return TLTypes.InputPeerChannel; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteLong(ChannelId); writer.WriteLong(AccessHash); }
    }

    public sealed class MessagesSendMessage : TLObject
    {
        public int Flags; public InputPeer Peer; public string Message; public long RandomId;
        public override int ConstructorId { get { return TLTypes.MessagesSendMessage; } }
        public override void Serialize(TLBinaryWriter writer) { writer.WriteInt(Flags); writer.WriteObject(Peer); writer.WriteString(Message); writer.WriteLong(RandomId); }
    }
}

namespace MtProto.WM6.Generated
{
    public sealed class AccountGetPassword : MtProto.WM6.TL.TLObject
    {
        public override int ConstructorId { get { return MtProto.WM6.TL.TLTypes.AccountGetPassword; } }
        public override void Serialize(MtProto.WM6.TL.TLBinaryWriter writer) { }
    }

    public sealed class InputCheckPasswordSRP : MtProto.WM6.TL.TLObject
    {
        public long SrpId;
        public byte[] A;
        public byte[] M1;
        public override int ConstructorId { get { return MtProto.WM6.TL.TLTypes.InputCheckPasswordSRP; } }
        public override void Serialize(MtProto.WM6.TL.TLBinaryWriter writer)
        {
            writer.WriteLong(SrpId);
            writer.WriteBytes(A);
            writer.WriteBytes(M1);
        }
    }

    public sealed class AuthCheckPassword : MtProto.WM6.TL.TLObject
    {
        public InputCheckPasswordSRP Password;
        public override int ConstructorId { get { return MtProto.WM6.TL.TLTypes.AuthCheckPassword; } }
        public override void Serialize(MtProto.WM6.TL.TLBinaryWriter writer) { writer.WriteObject(Password); }
    }
}

namespace MtProto.WM6.Generated
{
    public abstract class PasswordKdfAlgo { }

    public sealed class PasswordKdfAlgoUnknown : PasswordKdfAlgo { }

    public sealed class PasswordKdfAlgoModPow : PasswordKdfAlgo
    {
        public byte[] Salt1;
        public byte[] Salt2;
        public int G;
        public byte[] P;
        public static PasswordKdfAlgoModPow Deserialize(MtProto.WM6.TL.TLBinaryReader r)
        {
            PasswordKdfAlgoModPow x = new PasswordKdfAlgoModPow();
            x.Salt1 = r.ReadBytes();
            x.Salt2 = r.ReadBytes();
            x.G = r.ReadInt();
            x.P = r.ReadBytes();
            return x;
        }
    }

    public sealed class AccountPassword
    {
        public int Flags;
        public PasswordKdfAlgo CurrentAlgo;
        public byte[] SrpB;
        public long SrpId;
        public string Hint;
        public string EmailUnconfirmedPattern;
        public byte[] SecureRandom;

        public static AccountPassword Deserialize(MtProto.WM6.TL.TLBinaryReader r)
        {
            AccountPassword x = new AccountPassword();
            x.Flags = r.ReadInt();
            if ((x.Flags & (1 << 2)) != 0)
            {
                int algo = r.ReadInt();
                if (algo == MtProto.WM6.TL.TLTypes.PasswordKdfAlgoModPow) x.CurrentAlgo = PasswordKdfAlgoModPow.Deserialize(r);
                else x.CurrentAlgo = new PasswordKdfAlgoUnknown();
                x.SrpB = r.ReadBytes();
                x.SrpId = r.ReadLong();
            }
            if ((x.Flags & (1 << 3)) != 0) x.Hint = r.ReadString();
            if ((x.Flags & (1 << 4)) != 0) x.EmailUnconfirmedPattern = r.ReadString();
            // The full account.password object also carries new_algo/new_secure_algo/secure_random.
            // For WM6 login only current_algo, srp_B and srp_id are needed.
            return x;
        }
    }
}
