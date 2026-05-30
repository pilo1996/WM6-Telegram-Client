using System;
using System.IO;

namespace MtProto.WM6.ClientApp.Services
{
    public sealed class AppSettings
    {
        public int ApiId;
        public string ApiHash;
        public bool TestMode;
        public int DcId;
        public string Host;
        public int Port;
        public int TimeoutSeconds;
        public string SessionPath;
        public string LogPath;

        public static string RootPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MtProtoWM6"); }
        }

        public static AppSettings Default()
        {
            string root = RootPath;
            return new AppSettings
            {
                ApiId = 0,
                ApiHash = "",
                TestMode = true,
                DcId = 2,
                Host = "149.154.167.40",
                Port = 443,
                TimeoutSeconds = 30,
                SessionPath = Path.Combine(root, "session.dat"),
                LogPath = Path.Combine(root, "client.log")
            };
        }

        public static AppSettings Load()
        {
            AppSettings s = Default();
            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);
            string path = Path.Combine(RootPath, "settings.ini");
            if (!File.Exists(path)) { s.Save(); return s; }
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int p = line.IndexOf('=');
                if (p <= 0) continue;
                string k = line.Substring(0, p).Trim();
                string v = line.Substring(p + 1).Trim();
                if (k == "ApiId") s.ApiId = ToInt(v, s.ApiId);
                else if (k == "ApiHash") s.ApiHash = v;
                else if (k == "TestMode") s.TestMode = v == "1" || v.ToLower() == "true";
                else if (k == "DcId") s.DcId = ToInt(v, s.DcId);
                else if (k == "Host") s.Host = v;
                else if (k == "Port") s.Port = ToInt(v, s.Port);
                else if (k == "TimeoutSeconds") s.TimeoutSeconds = ToInt(v, s.TimeoutSeconds);
            }
            return s;
        }

        public void Save()
        {
            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);
            using (StreamWriter w = new StreamWriter(Path.Combine(RootPath, "settings.ini"), false))
            {
                w.WriteLine("ApiId=" + ApiId);
                w.WriteLine("ApiHash=" + ApiHash);
                w.WriteLine("TestMode=" + (TestMode ? "1" : "0"));
                w.WriteLine("DcId=" + DcId);
                w.WriteLine("Host=" + Host);
                w.WriteLine("Port=" + Port);
                w.WriteLine("TimeoutSeconds=" + TimeoutSeconds);
            }
        }

        private static int ToInt(string v, int def) { try { return int.Parse(v); } catch { return def; } }
    }
}
