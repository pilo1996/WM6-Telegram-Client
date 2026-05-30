using System;
using System.IO;

namespace MtProto.WM6.ClientApp.Services
{
    public sealed class DiagnosticLog
    {
        private readonly string _path;
        public DiagnosticLog(string path) { _path = path; }
        public void Info(string message) { Write("INFO", message); }
        public void Error(string message, Exception ex) { Write("ERROR", message + " | " + ex.GetType().Name + ": " + ex.Message); }
        public void Write(string level, string message)
        {
            try
            {
                string dir = Path.GetDirectoryName(_path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                using (StreamWriter w = new StreamWriter(_path, true))
                    w.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + level + "] " + message);
            }
            catch { }
        }
    }
}
