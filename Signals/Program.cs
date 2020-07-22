using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Signals
{
    static class Program
    {

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainWindow = new MainForm();
            App.Initialize(mainWindow);
            Application.Run(mainWindow);
        }
    }
}
