using System;
using System.IO;
using System.Windows.Forms;

namespace MtProto.WM6.ClientApp
{
    public sealed class SettingsForm : Form
    {
        TextBox apiId, apiHash, host, port; CheckBox test;
        public SettingsForm()
        {
            Text = "Settings"; Width = 240; Height = 320;
            int y = 8;
            apiId = AddText("ApiId", ref y); apiHash = AddText("ApiHash", ref y); host = AddText("Host", ref y); port = AddText("Port", ref y);
            test = new CheckBox(); test.Text = "Test DC"; test.Left = 8; test.Top = y; test.Width = 220; y += 30; Controls.Add(test);
            Button save = new Button(); save.Text = "Salva"; save.Left = 8; save.Top = y; save.Width = 105; save.Click += Save_Click;
            Button log = new Button(); log.Text = "Log"; log.Left = 123; log.Top = y; log.Width = 105; log.Click += Log_Click; Controls.Add(save); Controls.Add(log);
            LoadValues();
        }
        TextBox AddText(string label, ref int y)
        {
            Label l = new Label(); l.Text = label; l.Left = 8; l.Top = y; l.Width = 220; Controls.Add(l); y += 20;
            TextBox t = new TextBox(); t.Left = 8; t.Top = y; t.Width = 220; Controls.Add(t); y += 30; return t;
        }
        void LoadValues()
        {
            Services.AppSettings s = AppContext.Current.Settings;
            apiId.Text = s.ApiId.ToString(); apiHash.Text = s.ApiHash; host.Text = s.Host; port.Text = s.Port.ToString(); test.Checked = s.TestMode;
        }
        void Save_Click(object sender, EventArgs e)
        {
            Services.AppSettings s = AppContext.Current.Settings;
            s.ApiId = int.Parse(apiId.Text.Trim()); s.ApiHash = apiHash.Text.Trim(); s.Host = host.Text.Trim(); s.Port = int.Parse(port.Text.Trim()); s.TestMode = test.Checked; s.Save();
            MessageBox.Show("Settings salvati."); Close();
        }
        void Log_Click(object sender, EventArgs e)
        {
            string p = AppContext.Current.Settings.LogPath;
            MessageBox.Show(File.Exists(p) ? File.ReadAllText(p) : "Log vuoto.");
        }
    }
}
