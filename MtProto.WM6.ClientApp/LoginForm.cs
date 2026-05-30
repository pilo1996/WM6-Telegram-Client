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
            Text = "MTProto WM6 - Login"; Width = 240; Height = 320;
            Label l = new Label(); l.Text = "Telefono (+391234...)"; l.Left = 8; l.Top = 12; l.Width = 220;
            phone = new TextBox(); phone.Left = 8; phone.Top = 36; phone.Width = 220;
            send = new Button(); send.Text = "Invia codice"; send.Left = 8; send.Top = 70; send.Width = 105; send.Click += Send_Click;
            settings = new Button(); settings.Text = "Settings"; settings.Left = 123; settings.Top = 70; settings.Width = 105; settings.Click += Settings_Click;
            status = new Label(); status.Left = 8; status.Top = 110; status.Width = 220; status.Height = 120; status.Text = "Configura ApiId/ApiHash prima del login.";
            Controls.Add(l); Controls.Add(phone); Controls.Add(send); Controls.Add(settings); Controls.Add(status);
        }
        void Settings_Click(object sender, EventArgs e) { new SettingsForm().ShowDialog(); }
        void Send_Click(object sender, EventArgs e)
        {
            try
            {
                AppContext ctx = AppContext.Current;
                if (ctx.Settings.ApiId == 0 || ctx.Settings.ApiHash.Length == 0) { MessageBox.Show("ApiId/ApiHash mancanti."); return; }
                ctx.PhoneNumber = phone.Text.Trim();
                status.Text = "Invio codice...";
                object result = ctx.Client.Call(delegate { return ctx.Client.Inner.AuthSendCode(ctx.PhoneNumber, ctx.Settings.ApiId, ctx.Settings.ApiHash, ctx.Settings.TestMode); });
                ctx.LastAuthResult = result;
                ctx.PhoneCodeHash = ResultText.ExtractPhoneCodeHash(result);
                if (ctx.PhoneCodeHash.Length == 0) { MessageBox.Show("phone_code_hash non tipizzato: serve generazione TL completa per auth.sentCode. Vedi log."); return; }
                new CodeForm().Show(); Close();
            }
            catch (RpcException ex) { if (ex.FloodWaitSeconds > 0) MessageBox.Show("Telegram chiede attesa: " + ex.FloodWaitSeconds + " secondi."); else MessageBox.Show(ex.Message); }
            catch (Exception ex) { AppContext.Current.Log.Error("Login send code failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
