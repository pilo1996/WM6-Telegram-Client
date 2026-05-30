using System;
using System.Windows.Forms;
using MtProto.WM6.Generated;

namespace MtProto.WM6.ClientApp
{
    public sealed class ChatForm : Form
    {
        TextBox peerId, accessHash, message, log;
        CheckBox channel;
        public ChatForm()
        {
            Text = "MTProto WM6 - Chat"; Width = 240; Height = 320;
            Label p = new Label(); p.Text = "User/Chat/Channel ID"; p.Left = 8; p.Top = 8; p.Width = 220;
            peerId = new TextBox(); peerId.Left = 8; peerId.Top = 30; peerId.Width = 220;
            Label a = new Label(); a.Text = "Access hash se richiesto"; a.Left = 8; a.Top = 58; a.Width = 220;
            accessHash = new TextBox(); accessHash.Left = 8; accessHash.Top = 80; accessHash.Width = 220;
            channel = new CheckBox(); channel.Text = "Channel/User con access hash"; channel.Left = 8; channel.Top = 108; channel.Width = 220;
            message = new TextBox(); message.Left = 8; message.Top = 134; message.Width = 220; message.Height = 50; message.Multiline = true;
            Button send = new Button(); send.Text = "Invia"; send.Left = 8; send.Top = 190; send.Width = 105; send.Click += Send_Click;
            Button settings = new Button(); settings.Text = "Settings"; settings.Left = 123; settings.Top = 190; settings.Width = 105; settings.Click += delegate { new SettingsForm().ShowDialog(); };
            log = new TextBox(); log.Left = 8; log.Top = 225; log.Width = 220; log.Height = 55; log.Multiline = true;
            Controls.Add(p); Controls.Add(peerId); Controls.Add(a); Controls.Add(accessHash); Controls.Add(channel); Controls.Add(message); Controls.Add(send); Controls.Add(settings); Controls.Add(log);
        }
        void Send_Click(object sender, EventArgs e)
        {
            try
            {
                InputPeer peer;
                long id = long.Parse(peerId.Text.Trim());
                long hash = accessHash.Text.Trim().Length == 0 ? 0 : long.Parse(accessHash.Text.Trim());
                if (channel.Checked)
                {
                    InputPeerUser u = new InputPeerUser(); u.UserId = id; u.AccessHash = hash; peer = u;
                }
                else
                {
                    InputPeerChat c = new InputPeerChat(); c.ChatId = id; peer = c;
                }
                AppContext ctx = AppContext.Current;
                object result = ctx.Client.Call(delegate { return ctx.Client.Inner.SendMessage(peer, message.Text, ctx.Settings.TestMode); });
                ctx.SaveSession(); log.Text = "Inviato: " + ResultText.ToShort(result);
            }
            catch (Exception ex) { AppContext.Current.Log.Error("Send message failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
