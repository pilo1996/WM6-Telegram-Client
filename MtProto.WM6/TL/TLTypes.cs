namespace MtProto.WM6.TL
{
    public static class TLTypes
    {
        public const int Vector = 0x1cb5c415;
        public const int ReqPqMulti = unchecked((int)0xbe7e8ef1);
        public const int ResPQ = unchecked((int)0x05162463);
        public const int PQInnerData = unchecked((int)0x83c95aec);
        public const int PQInnerDataDc = unchecked((int)0xa9f55f95);
        public const int ReqDHParams = unchecked((int)0xd712e4be);
        public const int ServerDHParamsOk = unchecked((int)0xd0e8075c);
        public const int ServerDHParamsFail = unchecked((int)0x79cb045d);
        public const int ServerDHInnerData = unchecked((int)0xb5890dba);
        public const int ClientDHInnerData = unchecked((int)0x6643b654);
        public const int SetClientDHParams = unchecked((int)0xf5045f1f);
        public const int DhGenOk = unchecked((int)0x3bcbf734);
        public const int DhGenRetry = unchecked((int)0x46dc1fb9);
        public const int DhGenFail = unchecked((int)0xa69dae02);
        public const int RpcResult = unchecked((int)0xf35c6d01);
        public const int RpcError = unchecked((int)0x2144ca19);
        public const int MsgContainer = unchecked((int)0x73f1f8dc);
        public const int BadServerSalt = unchecked((int)0xedab447b);
        public const int BadMsgNotification = unchecked((int)0xa7eff811);
        public const int AuthSendCode = unchecked((int)0xa677244f); // layer-dependent, check schema before production use
        public const int AuthSignIn = unchecked((int)0x8d52a951);   // layer-dependent, check schema before production use
        public const int MessagesSendMessage = unchecked((int)0xfe05dc9a); // layer-dependent
        public const int InputPeerUser = unchecked((int)0xdde8a54c);
        public const int InputPeerChat = unchecked((int)0x35a95cb9);
        public const int InputPeerChannel = unchecked((int)0x27bcbbfc);
        public const int CodeSettings = unchecked((int)0xad253d78);
        public const int AccountGetPassword = unchecked((int)0x548a30f5);
        public const int AuthCheckPassword = unchecked((int)0xd18b4d16);
        public const int InputCheckPasswordSRP = unchecked((int)0xd27ff082);

        public const int GzipPacked = unchecked((int)0x3072cfa1);
        public const int NewSessionCreated = unchecked((int)0x9ec20908);
        public const int Pong = unchecked((int)0x347773c5);
        public const int MsgsAck = unchecked((int)0x62d6b459);
        public const int Ping = unchecked((int)0x7abe77ec);
        public const int PasswordKdfAlgoUnknown = unchecked((int)0xd45ab096);
        public const int PasswordKdfAlgoModPow = unchecked((int)0x3a912d4a);
        public const int AccountPassword = unchecked((int)0x957b50fb);
    }
}
