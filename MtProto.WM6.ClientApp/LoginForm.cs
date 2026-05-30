using System;
using System.Windows.Forms;
using MtProto.WM6.Runtime;

namespace MtProto.WM6.ClientApp
{
    public sealed class LoginForm : Form
    {
        TextBox phone; Button send; Button settings; Label status;
        public LoginForm()
        {
            UiTheme.Apply(this, "Pocket MTProto - Login", "connect");
            Label l = UiTheme.Label("Telefono (+391234...)", 8, 62, 220);
            phone = new TextBox(); phone.Left = 8; phone.Top = 84; phone.Width = 220;
            send = UiTheme.IconButton("Invia codice", "send", 8, 120, 105); send.Click += Send_Click;
            settings = UiTheme.IconButton("Settings", "settings", 123, 120, 105); settings.Click += Settings_Click;
            status = UiTheme.Label("Configura ApiId/ApiHash prima del login.", 8, 164, 220); status.Height = 80;
            Controls.Add(l); Controls.Add(phone); Controls.Add(send); Controls.Add(settings); Controls.Add(status);
        }
        void Settings_Click(object sender, EventArgs e) { new SettingsForm().ShowDialog(); }
        void Send_Click(object sender, EventArgs e)
        {
            try
            {
                AppContext ctx = AppContext.Current;
                if (ctx.Settings.ApiId == 0 || ctx.Settings.ApiHash.Length == 0) { MessageBox.Show("ApiId/ApiHash mancanti."); return; }
                ctx.PhoneNumber = phone.Text.Trim(); status.Text = "Invio codice...";
                object result = ctx.Client.Call(delegate { return ctx.Client.Inner.AuthSendCode(ctx.PhoneNumber, ctx.Settings.ApiId, ctx.Settings.ApiHash, ctx.Settings.TestMode); });
                ctx.LastAuthResult = result; ctx.PhoneCodeHash = ResultText.ExtractPhoneCodeHash(result);
                if (ctx.PhoneCodeHash.Length == 0) { MessageBox.Show("phone_code_hash non tipizzato: serve generazione TL completa per auth.sentCode. Vedi log."); return; }
                new CodeForm().Show(); Close();
            }
            catch (RpcException ex) { if (ex.FloodWaitSeconds > 0) MessageBox.Show("Telegram chiede attesa: " + ex.FloodWaitSeconds + " secondi."); else MessageBox.Show(ex.Message); }
            catch (Exception ex) { AppContext.Current.Log.Error("Login send code failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
