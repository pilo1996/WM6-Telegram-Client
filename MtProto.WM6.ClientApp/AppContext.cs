using MtProto.WM6.Client;
using MtProto.WM6.Generated;
using MtProto.WM6.Session;
using MtProto.WM6.Runtime;
using MtProto.WM6.ClientApp.Services;

namespace MtProto.WM6.ClientApp
{
    public sealed class AppContext
    {
        private static readonly AppContext _current = new AppContext();
        public static AppContext Current { get { return _current; } }

        public readonly AppSettings Settings;
        public readonly DiagnosticLog Log;
        public readonly ResilientMtClient Client;
        public string PhoneNumber;
        public string PhoneCodeHash;
        public object LastAuthResult;

        private AppContext()
        {
            Settings = AppSettings.Load();
            Log = new DiagnosticLog(Settings.LogPath);
            Client = new ResilientMtClient(new MtProtoClient(), Settings, Log);
            if (System.IO.File.Exists(Settings.SessionPath))
            {
                MtSession s = SessionStore.Load(Settings.SessionPath);
                if (s != null) Client.Inner.UseSession(s);
            }
        }

        public void SaveSession()
        {
            SessionStore.Save(Settings.SessionPath, Client.Inner.Session);
        }
    }
}
