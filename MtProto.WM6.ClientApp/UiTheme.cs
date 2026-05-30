using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MtProto.WM6.ClientApp
{
    internal sealed class HeaderControl : Control
    {
        string _title; Bitmap _icon;
        public HeaderControl(string title, string iconName)
        {
            _title = title; _icon = UiTheme.LoadBitmap(iconName); Height = 48; Dock = DockStyle.Top;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(UiTheme.Blue);
            if (_icon != null) g.DrawImage(_icon, 8, 8, 32, 32);
            using (Brush b = new SolidBrush(Color.White)) g.DrawString(_title, UiTheme.HeaderFont, b, 48, 14);
        }
    }

    internal static class UiTheme
    {
        public static readonly Color Blue = Color.FromArgb(55,145,210);
        public static readonly Color DarkBlue = Color.FromArgb(28,78,124);
        public static readonly Color Surface = Color.FromArgb(246,250,253);
        public static readonly Color Panel = Color.FromArgb(230,244,252);
        public static readonly Font HeaderFont = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold);

        public static string ResourcePath(string file)
        {
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            return Path.Combine(Path.Combine(dir, "Resources"), file);
        }

        public static Bitmap LoadBitmap(string name)
        {
            try
            {
                string p = ResourcePath(name + ".png");
                return File.Exists(p) ? new Bitmap(p) : null;
            }
            catch { return null; }
        }

        public static void Apply(Form f, string title, string iconName)
        {
            f.BackColor = Surface;
            f.Text = title;
            f.Width = 240;
            f.Height = 320;
            f.Controls.Add(new HeaderControl(title, iconName));
        }

        public static Button IconButton(string text, string iconName, int left, int top, int width)
        {
            Button b = new Button(); b.Text = text; b.Left = left; b.Top = top; b.Width = width; b.Height = 30;
            try { Bitmap img = LoadBitmap(iconName); if (img != null) b.Image = img; }
            catch { }
            return b;
        }

        public static Label Label(string text, int left, int top, int width)
        {
            Label l = new Label(); l.Text = text; l.Left = left; l.Top = top; l.Width = width; l.BackColor = Surface; return l;
        }
    }
}
