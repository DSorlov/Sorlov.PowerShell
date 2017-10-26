using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace Sorlov.PowerShell.Builder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BuilderForm());
        }
    }
}
