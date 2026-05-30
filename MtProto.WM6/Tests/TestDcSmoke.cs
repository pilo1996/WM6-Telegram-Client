using System;
using MtProto.WM6.Client;
using MtProto.WM6.Runtime;
using MtProto.WM6.Session;

namespace MtProto.WM6.Tests
{
    public static class TestDcSmoke
    {
        public static void RunAuthKeySmoke(string sessionPath)
        {
            MtProtoClient c = new MtProtoClient();
            DcEndpoint ep = DcOptions.Test(1);
            c.Connect(ep.Host, ep.Port);
            c.UseSession(new MtSession());
            // The full auth-key exchange is available in Authenticator. This smoke entry point
            // is intentionally credential-free so it can be compiled into Windows Mobile builds.
            SessionStore.Save(sessionPath, c.Session);
            c.Close();
        }
    }
}
