using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Sorlov.SelfHostedPS.Application
{
    class Program
    {
        private static object psLocker;
        private static Host powershellHost;
        private static System.Management.Automation.PowerShell powershellEngine;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool AttachConsole(int processId);

        const bool hideCon = false;

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [MTAThread]
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            if (hideCon)
            {
                ConsoleHandler.ConsoleLoaded=false;
            }
            else
            {
                ConsoleHandler.ConsoleLoaded=true;

                if (!AttachConsole(-1))
                    AllocConsole();

                Console.CancelKeyPress += Console_CancelKeyPress;
                Console.TreatControlCAsInput = false;
            }

            psLocker = new object();

            string script = Common.GetScript("ScriptData");
            RunScript(script, args);

            Common.CleanUp();
        }

        private static void RunScript(string script, string[] args)
        {
            lock (psLocker)
            {
                powershellHost = new Host();
                powershellEngine = System.Management.Automation.PowerShell.Create();
            }

            try
            {
                InitialSessionState initalState = InitialSessionState.CreateDefault();

                List<ParameterItem> validCmds = new List<ParameterItem>();
                //AddValidCommands

                Commandline cmdLine = new Commandline(args);

                if (cmdLine["help"] == "true" || cmdLine["?"] == "true")
                {
                    AssemblyData assInfo = new AssemblyData(System.Reflection.Assembly.GetExecutingAssembly());

                    StringBuilder outputBuilder = new StringBuilder();
                    outputBuilder.AppendLine();
                    outputBuilder.AppendLine(string.Format("{0} v{1} by {2}",assInfo.Product, assInfo.Version, assInfo.Company));
                    outputBuilder.AppendLine(assInfo.Copyright);
                    outputBuilder.AppendLine();
                    outputBuilder.AppendLine(" [-Help]");
                    outputBuilder.AppendLine("    Show help");

                    foreach (ParameterItem cmdName in validCmds)
                    {
                        if (cmdName.Mandatory)
                            outputBuilder.AppendLine(string.Format(" -{0} <{1}>", cmdName.Name, cmdName.Type));
                        else
                            outputBuilder.AppendLine(string.Format(" [-{0} <{1}>]", cmdName.Name, cmdName.Type));

                        if (!string.IsNullOrWhiteSpace(cmdName.HelpText)) outputBuilder.AppendLine(string.Format("    {0}", cmdName.HelpText));
                    }

                    if (hideCon)
                        MessageBox.Show(outputBuilder.ToString(), "Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    else
                        ConsoleHandler.WriteLine(outputBuilder.ToString());

                    return;
                }

                Dictionary<string, object> cmdLineArgs = new Dictionary<string, object>();
                foreach (string arg in cmdLine.GetKeys())
                {
                    ParameterItem paramItem = validCmds.FirstOrDefault(x => String.Equals(x.Name, arg, StringComparison.CurrentCultureIgnoreCase));

                    if (paramItem!=null)
                    {
                        try
                        {
                            object realItem;
                            switch (paramItem.Type)
                            {
                                case "sbyte":
                                    realItem = sbyte.Parse(cmdLine[arg]);
                                    break;
                                case "byte":
                                    realItem = byte.Parse(cmdLine[arg]);
                                    break;
                                case "short":
                                    realItem = short.Parse(cmdLine[arg]);
                                    break;
                                case "ushort":
                                    realItem = ushort.Parse(cmdLine[arg]);
                                    break;
                                case "int":
                                    realItem = int.Parse(cmdLine[arg]);
                                    break;
                                case "uint":
                                    realItem = uint.Parse(cmdLine[arg]);
                                    break;
                                case "ulong":
                                    realItem = ulong.Parse(cmdLine[arg]);
                                    break;
                                case "long":
                                    realItem = long.Parse(cmdLine[arg]);
                                    break;
                                case "float":
                                    realItem = float.Parse(cmdLine[arg]);
                                    break;
                                case "double":
                                    realItem = double.Parse(cmdLine[arg]);
                                    break;
                                case "decimal":
                                    realItem = decimal.Parse(cmdLine[arg]);
                                    break;
                                case "char":
                                    realItem = char.Parse(cmdLine[arg]);
                                    break;
                                case "switch":
                                case "bool":
                                    realItem = bool.Parse(cmdLine[arg]);
                                    break;
                                case "boolean":
                                    realItem = Boolean.Parse(cmdLine[arg]);
                                    break;
                                default:
                                    realItem = cmdLine[arg];
                                    break;
                            }

                            cmdLineArgs.Add(arg, realItem);
                        }
                        catch (Exception)
                        {
                            string errorString = string.Format("Parameter '-{0}' was not in correct format: '{1}'", arg, paramItem.Type);

                            if (hideCon)
                                MessageBox.Show(errorString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                ConsoleHandler.WriteLine(errorString);

                            return;
                        }

                    }
                    else
                    {
                        StringBuilder outputBuilder = new StringBuilder();
                        outputBuilder.AppendLine(string.Format("Parameter '-{0}' is not valid. Use '-help' to show valid parameters.", arg));

                        if (hideCon)
                            MessageBox.Show(outputBuilder.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            ConsoleHandler.WriteLine(outputBuilder.ToString());

                        return;
                    }
                }

                foreach (ParameterItem paramItem in validCmds.Where(x => x.Mandatory == true))
                {
                    if (!cmdLineArgs.ContainsKey(paramItem.Name.ToLower()))
                    {
                        StringBuilder outputBuilder = new StringBuilder();
                        outputBuilder.AppendLine(string.Format("Parameter '-{0}' of type '{1}' is mandatory.", paramItem.Name, paramItem.Type));

                        if (hideCon)
                            MessageBox.Show(outputBuilder.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            ConsoleHandler.WriteLine(outputBuilder.ToString());

                        return;
                    }

                }

                List<string> modulesToLoad = Common.ProcessManifest(initalState);

                powershellEngine.Runspace = RunspaceFactory.CreateRunspace(powershellHost, initalState);
                powershellEngine.Runspace.ApartmentState = System.Threading.ApartmentState.MTA;
                powershellEngine.Runspace.Open();

                RunspaceInvoke runSpaceInvoker = new RunspaceInvoke(powershellEngine.Runspace);

                try
                {
                    runSpaceInvoker.Invoke("Set-ExecutionPolicy Unrestricted -Scope Process");
                }
                catch
                {
                }

                foreach (string module in modulesToLoad)
                    try
                    {
                        runSpaceInvoker.Invoke(string.Format("Import-Module {0} -Scope Global", module));
                    }
                    catch (Exception e)
                    {
                       ConsoleHandler.WriteLine(string.Format("Could import module {0}: {0}", e.Message, module));
                    }

                Pipeline pipeline = powershellEngine.Runspace.CreatePipeline();
                Command command = new Command(script, true, true);

                foreach (KeyValuePair<string,object> cmdLineData in cmdLineArgs)
                    command.Parameters.Add(cmdLineData.Key, cmdLineData.Value);                    
                
                pipeline.Commands.Add(command);
                Collection<PSObject> resultObjects = pipeline.Invoke();

                foreach (PSObject resultObject in resultObjects)
                    ConsoleHandler.WriteLine(resultObject.ToString());

            }
            catch(Exception e)
            {
                ConsoleHandler.WriteLine(string.Format("Internal error: {0}", e.Message));
            }
            finally
            {
                lock (psLocker)
                {
                    powershellEngine.Dispose();
                    powershellEngine = null;
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            try
            {
                lock (psLocker)
                {
                    if (powershellEngine != null && powershellEngine.InvocationStateInfo.State == PSInvocationState.Running)
                    {
                        powershellEngine.Stop();
                    }
                }
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                powershellHost.UI.WriteErrorLine(ex.ToString());
            }
        }
    }

 
}