using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using Sorlov.PowerShell.Cmdlets;
using Sorlov.PowerShell.Lib.Core;

namespace Sorlov.PowerShell
{
    public class Initializer : IModuleAssemblyInitializer
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        private System.Management.Automation.PowerShell powerShell;
        private void WriteWarning(string warning)
        {
            powerShell.Streams.Warning.Add(new WarningRecord(warning));
        }

        private Collection<PSObject> InvokeCommand(string cmdLet)
        {
            return InvokeCommand(cmdLet, null);
        }
        private Collection<PSObject> InvokeCommand(string cmdLet, Dictionary<string,object> parameters)
        {
            powerShell.Commands.Clear();
            powerShell.AddCommand(cmdLet, false);
            if (parameters!=null) powerShell.AddParameters(parameters);
            return powerShell.Invoke();
        }

        private Collection<PSObject> InvokeScript(string script)
        {
            powerShell.Commands.Clear();
            powerShell.AddScript(script, false);
            return powerShell.Invoke();
        }

        private void InitAliases()
        {
            try
            {
                if (GetOfficeVersion.GetVersion("Word").IsInstalled)
                    InvokeScript("New-Alias Out-Word Out-AutoWord");
                else
                    InvokeScript("New-Alias Out-Word Out-OpenWord");
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Coult not register alias for Out-Word: {0}", ex.Message));
            }

            try
            {
                if (GetOfficeVersion.GetVersion("Excel").IsInstalled)
                    InvokeScript("New-Alias Out-Excel Out-AutoExcel");
                else
                    InvokeScript("New-Alias Out-Excel Out-OpenExcel");
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not register alias for Out-Excel: {0}", ex.Message));
            }            
        }

        private void InitSupportFiles(string localDir)
        {
            if (!File.Exists(Path.Combine(localDir, "Sorlov.PowerShell-help.xml")))
            {
                XMLHelpGenerator.GenerateHelp(Path.Combine(localDir, "Sorlov.PowerShell.dll"), localDir, true);
            }
            if (!File.Exists(Path.Combine(localDir, "Sorlov.PowerShell.Format.ps1xml")) || !File.Exists(Path.Combine(localDir, "Sorlov.PowerShell.Types.ps1xml")))
            {
                PS1XMLGenerator.GeneratePS1XML(localDir, localDir, "Sorlov.PowerShell");
            }
            InvokeCommand("Update-TypeData", new Dictionary<string, object>() { { "AppendPath", System.IO.Path.Combine(localDir, "Sorlov.PowerShell.Types.ps1xml") } });
            InvokeCommand("Update-FormatData", new Dictionary<string, object>() { { "AppendPath", System.IO.Path.Combine(localDir, "Sorlov.PowerShell.Format.ps1xml") } });            
        }

        public void OnImport()
        {
            try
            {
                powerShell = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);

                string localDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string pathName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WindowsPowershell\\Modules\\Sorlov.PowerShell");

                InitSupportFiles(localDir);
                InitAliases();

            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Initialization failed: {0}", ex.Message));
            }
        }
    }
}
