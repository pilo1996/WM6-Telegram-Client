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
            UiTheme.Apply(this, "Chat", "chat");
            Label p = UiTheme.Label("User/Chat/Channel ID", 8, 56, 220);
            peerId = new TextBox(); peerId.Left = 8; peerId.Top = 76; peerId.Width = 220;
            Label a = UiTheme.Label("Access hash se richiesto", 8, 102, 220);
            accessHash = new TextBox(); accessHash.Left = 8; accessHash.Top = 122; accessHash.Width = 220;
            channel = new CheckBox(); channel.Text = "Channel/User con access hash"; channel.Left = 8; channel.Top = 148; channel.Width = 220; channel.BackColor = UiTheme.Surface;
            message = new TextBox(); message.Left = 8; message.Top = 174; message.Width = 220; message.Height = 42; message.Multiline = true;
            Button send = UiTheme.IconButton("Invia", "send", 8, 222, 105); send.Click += Send_Click;
            Button settings = UiTheme.IconButton("Settings", "settings", 123, 222, 105); settings.Click += delegate { new SettingsForm().ShowDialog(); };
            log = new TextBox(); log.Left = 8; log.Top = 258; log.Width = 220; log.Height = 32; log.Multiline = true;
            Controls.Add(p); Controls.Add(peerId); Controls.Add(a); Controls.Add(accessHash); Controls.Add(channel); Controls.Add(message); Controls.Add(send); Controls.Add(settings); Controls.Add(log);
        }
        void Send_Click(object sender, EventArgs e)
        {
            try
            {
                InputPeer peer; long id = long.Parse(peerId.Text.Trim()); long hash = accessHash.Text.Trim().Length == 0 ? 0 : long.Parse(accessHash.Text.Trim());
                if (channel.Checked) { InputPeerUser u = new InputPeerUser(); u.UserId = id; u.AccessHash = hash; peer = u; }
                else { InputPeerChat c = new InputPeerChat(); c.ChatId = id; peer = c; }
                AppContext ctx = AppContext.Current;
                object result = ctx.Client.Call(delegate { return ctx.Client.Inner.SendMessage(peer, message.Text, ctx.Settings.TestMode); });
                ctx.SaveSession(); log.Text = "Inviato: " + ResultText.ToShort(result);
            }
            catch (Exception ex) { AppContext.Current.Log.Error("Send message failed", ex); MessageBox.Show(ex.Message); }
        }
    }
}
