using System;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.Lib.HTApps
{
    public static class HTAppFiles
    {
        [DllExport("RunScriptStd",System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static void RunScript(IntPtr hWnd, IntPtr hInst, string command, int cmdVisible)
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.AssemblyResolve += new ResolveEventHandler(currentDomain_AssemblyResolve);

                Thread staThread = new Thread(new ParameterizedThreadStart(RunScriptWorker));
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Start(command.Replace("\"", ""));

                staThread.Join();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(),"Sorlov.PowerShell.HTApps: Initialization Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        public static Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string requestedAssembly = string.Format("{0}.dll", args.Name.Substring(0, args.Name.IndexOf(",")));

            if (requestedAssembly.StartsWith("sorlov.powershell",true,Thread.CurrentThread.CurrentCulture))
            {
                string assemblyPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), requestedAssembly);
                return Assembly.LoadFrom(assemblyPath); ;                
            }
            else
            {
                return Assembly.Load(requestedAssembly);
            }

            

        }


        public static void RunScriptWorker(object indata)
        {
            try
            {
                string command = (string) indata;
                ScriptForm form = new ScriptForm();
                form.LoadApplication(command);
                form.ProviderPath = Path.GetDirectoryName(command);
                form.Show();

                while (form.Visible && form.PipelineStatus == PipelineState.NotStarted)
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(1);
                }            
            
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Sorlov.PowerShell.HTApps: Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public static void Associate()
        {
            string rundll32 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "rundll32.exe");
            string powerShell = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0\\PowerShell.exe");
            string notepad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "notepad.exe");
            string libPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            Registry.CurrentUser.CreateSubKey(@"Software\Classes\.htapp").SetValue("", "Sorlov.PowerShell");
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Sorlov.PowerShell"))
            {
                key.SetValue("", "PowerShell HTML Application");
                key.CreateSubKey("DefaultIcon").SetValue("", powerShell);

                key.CreateSubKey(@"Shell\Open\Command").SetValue("", rundll32 + " \"" + libPath + "\",RunScriptStd \"%1\"");
                key.CreateSubKey(@"Shell\Edit\Command").SetValue("", notepad + " \"%1\"");
            }


            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.htapp"))
            {
                //key.OpenSubKey("UserChoice", RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl);
                //key.DeleteSubKey("UserChoice", false);
                key.SetValue("Progid", "Sorlov.PowerShell", RegistryValueKind.String);
            }

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    
    }
}
