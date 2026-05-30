using System;
using System.Windows.Forms;

namespace MtProto.WM6.ClientApp
{
    static class Program
    {
        [MTAThread]
        static void Main()
        {
            Application.Run(new LoginForm());
        }
    }
}
