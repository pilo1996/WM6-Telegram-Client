using System;
using System.Windows.Forms;
using MtProto.WM6.Runtime;

namespace MtProto.WM6.ClientApp
{
    public sealed class CodeForm : Form
    {
        TextBox code; Label status;
        public CodeForm()
        {
            Text = "Codice Telegram"; Width = 240; Height = 320;
            Label l = new Label(); l.Text = "Codice SMS/app"; l.Left = 8; l.Top = 12; l.Width = 220;
            code = new TextBox(); code.Left = 8; code.Top = 36; code.Width = 220;
            Button ok = new Button(); ok.Text = "Accedi"; ok.Left = 8; ok.Top = 70; ok.Width = 105; ok.Click += Ok_Click;
            Button back = new Button(); back.Text = "Indietro"; back.Left = 123; back.Top = 70; back.Width = 105; back.Click += delegate { new LoginForm().Show(); Close(); };
            status = new Label(); status.Left = 8; status.Top = 110; status.Width = 220; status.Height = 120;
            Controls.Add(l); Controls.Add(code); Controls.Add(ok); Controls.Add(back); Controls.Add(status);
        }
        void Ok_Click(object sender, EventArgs e)
        {
            try
            {
                AppContext ctx = AppContext.Current; status.Text = "Verifica codice...";
                object result = ctx.Client.Call(delegate { return ctx.Client.Inner.AuthSignIn(ctx.PhoneNumber, ctx.PhoneCodeHash, code.Text.Trim(), ctx.Settings.TestMode); });
                ctx.LastAuthResult = result; ctx.SaveSession(); new ChatForm().Show(); Close();
            }
            catch (RpcException ex)
            {
                if (ex.Message.IndexOf("SESSION_PASSWORD_NEEDED") >= 0) { new Password2FAForm().Show(); Close(); return; }
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex) { AppContext.Current.Log.Error("SignIn failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
