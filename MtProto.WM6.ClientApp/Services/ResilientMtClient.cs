using System;
using System.Threading;
using MtProto.WM6.Client;
using MtProto.WM6.Runtime;
using MtProto.WM6.Generated;

namespace MtProto.WM6.ClientApp.Services
{
    public sealed class ResilientMtClient
    {
        public readonly MtProtoClient Inner;
        private readonly AppSettings _settings;
        private readonly DiagnosticLog _log;
        private bool _connected;

        public ResilientMtClient(MtProtoClient inner, AppSettings settings, DiagnosticLog log)
        {
            Inner = inner; _settings = settings; _log = log;
        }

        public void Connect()
        {
            if (_connected) return;
            _log.Info("Connecting to " + _settings.Host + ":" + _settings.Port);
            Inner.Connect(_settings.Host, _settings.Port);
            _connected = true;
        }

        public delegate object ClientAction();
        public object Call(ClientAction action)
        {
            int attempts = 0;
            while (true)
            {
                attempts++;
                try
                {
                    Connect();
                    return action();
                }
                catch (BadServerSaltException)
                {
                    if (attempts >= 2) throw;
                    _log.Info("bad_server_salt handled by MtProtoClient, retrying app call.");
                }
                catch (RpcException ex)
                {
                    if (ex.FloodWaitSeconds > 0) { _log.Info("FLOOD_WAIT_" + ex.FloodWaitSeconds); throw; }
                    if (ex.MigrationDc > 0 && attempts < 3)
                    {
                        _log.Info("Migrating to DC " + ex.MigrationDc);
                        Inner.MigrateToDc(ex.MigrationDc, _settings.TestMode);
                        continue;
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    _log.Error("Call failed", ex);
                    if (attempts >= 2) throw;
                    Reconnect();
                }
            }
        }

        public void Reconnect()
        {
            try { Inner.Close(); } catch { }
            _connected = false;
            Thread.Sleep(500);
            Connect();
        }
    }
}
