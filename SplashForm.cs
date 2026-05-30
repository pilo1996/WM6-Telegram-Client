using System;
using System.Drawing;
using System.Windows.Forms;

namespace MtProto.WM6.ClientApp
{
    public sealed class SplashForm : Form
    {
        Timer timer;
        Bitmap splash;
        public SplashForm()
        {
            Text = "Pocket MTProto"; Width = 240; Height = 320;
            splash = UiTheme.LoadBitmap("splash");
            timer = new Timer(); timer.Interval = 1200; timer.Tick += delegate { timer.Enabled = false; new LoginForm().Show(); Close(); };
            timer.Enabled = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (splash != null) e.Graphics.DrawImage(splash, 0, 0); else e.Graphics.Clear(UiTheme.Blue);
        }
    }
}
