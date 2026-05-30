using System;
using System.Windows.Forms;
using MtProto.WM6.Generated;

namespace MtProto.WM6.ClientApp
{
    public sealed class Password2FAForm : Form
    {
        TextBox password; Label hint;
        public Password2FAForm()
        {
            Text = "Password 2FA"; Width = 240; Height = 320;
            hint = new Label(); hint.Left = 8; hint.Top = 10; hint.Width = 220; hint.Height = 40; hint.Text = "Password cloud Telegram";
            password = new TextBox(); password.Left = 8; password.Top = 55; password.Width = 220; password.PasswordChar = '*';
            Button ok = new Button(); ok.Text = "Verifica"; ok.Left = 8; ok.Top = 90; ok.Width = 105; ok.Click += Ok_Click;
            Button cancel = new Button(); cancel.Text = "Annulla"; cancel.Left = 123; cancel.Top = 90; cancel.Width = 105; cancel.Click += delegate { new LoginForm().Show(); Close(); };
            Controls.Add(hint); Controls.Add(password); Controls.Add(ok); Controls.Add(cancel);
        }
        void Ok_Click(object sender, EventArgs e)
        {
            try
            {
                AppContext ctx = AppContext.Current;
                object pwdObj = ctx.Client.Call(delegate { return ctx.Client.Inner.AccountGetPassword(ctx.Settings.TestMode); });
                AccountPassword p = pwdObj as AccountPassword;
                if (p == null) throw new InvalidOperationException("account.password non tipizzato: rigenerare schema TL completo.");
                ctx.Client.Call(delegate { return ctx.Client.Inner.AuthCheckPassword(p, password.Text, ctx.Settings.TestMode); });
                ctx.SaveSession(); new ChatForm().Show(); Close();
            }
            catch (Exception ex) { AppContext.Current.Log.Error("2FA failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
