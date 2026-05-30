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
            UiTheme.Apply(this, "Verifica codice", "key");
            Label l = UiTheme.Label("Codice SMS/app", 8, 62, 220);
            code = new TextBox(); code.Left = 8; code.Top = 84; code.Width = 220;
            Button ok = UiTheme.IconButton("Accedi", "connect", 8, 120, 105); ok.Click += Ok_Click;
            Button back = UiTheme.IconButton("Indietro", "user", 123, 120, 105); back.Click += delegate { new LoginForm().Show(); Close(); };
            status = UiTheme.Label("Inserisci il codice ricevuto da Telegram.", 8, 164, 220); status.Height = 80;
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
