using System;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.SelfHosted.HTApps
{
    static class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new ScriptForm("$$FILENAME$$"));
           

        }
    }

 
}