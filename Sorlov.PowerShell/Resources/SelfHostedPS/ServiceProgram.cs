using System;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace Sorlov.SelfHostedPS.Service
{
    public class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool AttachConsole(int processId);

        [MTAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            if (System.Diagnostics.Process.GetCurrentProcess().SessionId == 0)
            {
                ConsoleHandler.ConsoleLoaded = false;
                ServiceBase.Run(new ServiceBase[] { new PSService() });              
            }
            else
            {
                ConsoleHandler.ConsoleLoaded = true;
                if (!AttachConsole(-1))
                    AllocConsole();

                Commandline commandline = new Commandline(args);

                if (commandline["debug"] == "true")
                {
                    PSService app = new PSService();
                    app.StartInternal();

                    string input = string.Empty;
                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("Application started in debug mode.");
                    ConsoleHandler.WriteLine("");

                    while (input.ToLower().Trim() != "quit")
                    {
                        ConsoleHandler.Write("Type 'quit' to stop the application: ");
                        input = ConsoleHandler.ReadLine();
                    }

                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("Exiting...");
                    app.StopInternal("OnStop");
                    ConsoleHandler.WriteLine("");
                }
                else if (commandline["install"] == "true")
                {
                    ConsoleHandler.WriteLine("");

                    ServiceStartMode startMode = ServiceStartMode.Disabled;
                    switch (commandline["startmode"].ToLower())
                    {
                        case "automatic":
                            startMode = ServiceStartMode.Automatic;
                            break;
                        case "disabled":
                            startMode = ServiceStartMode.Disabled;                            
                            break;
                        default:
                            startMode = ServiceStartMode.Manual;
                            break;
                    }

                    ServiceAccount serviceAccount = ServiceAccount.LocalSystem;
                    switch (commandline["account"].ToLower())
                    {
                        case "localservice":
                            serviceAccount = ServiceAccount.LocalService;
                            break;
                        case "networkservice":
                            serviceAccount = ServiceAccount.NetworkService;
                            break;
                        case "user":
                            serviceAccount = ServiceAccount.User;
                            break;
                        default:
                            serviceAccount = ServiceAccount.LocalSystem;
                            break;
                    }

                    PSService.InstallService(serviceAccount, startMode, commandline["username"], commandline["password"]);
                    ConsoleHandler.WriteLine("");
                }
                else if (commandline["uninstall"] == "true")
                {
                    ConsoleHandler.WriteLine("");
                    PSService.UninstallService();
                    ConsoleHandler.WriteLine("");
                }
                else
                {
                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("Usage information:");
                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("  /install      Installs the service, with optional arguments:");
                    ConsoleHandler.WriteLine("       /startmode:[automatic|manual|disabled]                    Start mode for the service");
                    ConsoleHandler.WriteLine("       /account:[localsystem|localservice|networkservice|user]   The account for the service");
                    ConsoleHandler.WriteLine("       /username:<username>                                      Only if /account:user");
                    ConsoleHandler.WriteLine("       /password:<password>                                      Only if /account:password");
                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("  /uninstall    Removes the application as a service in Windows");
                    ConsoleHandler.WriteLine("");
                    ConsoleHandler.WriteLine("  /debug        Starts the application as a local console application for debug");
                    ConsoleHandler.WriteLine("");
                }
            }
        }

    }
}
