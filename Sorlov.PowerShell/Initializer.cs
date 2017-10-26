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
        private ProgressRecord progressRecord;

        private void WriteProgress(string progressStatus, int completePercentage)
        {
            if (progressRecord == null) progressRecord = new ProgressRecord(100, "Initializing Sorlov.PowerShell..", progressStatus);
            progressRecord.PercentComplete = completePercentage;
            progressRecord.StatusDescription = progressStatus;

            powerShell.Streams.Progress.Add(progressRecord);            
        }

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

        private void Init_Office()
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
                WriteWarning(string.Format("Could not process StateRestore, error: {0}", ex.Message));
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
                WriteWarning(string.Format("Could not process StateRestore, error: {0}", ex.Message));
            }            
        }

        private void Init_Prompt()
        {
            try
            {
                WriteProgress("Setting up prompt..", 25);
                if (LibraryConfiguration.IsNotNull("ReplacePrompt"))
                    if (LibraryConfiguration.IsTrue("ReplacePrompt"))
                        InvokeScript((string)Resources.General.Prompt);
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process Prompt, error: {0}", ex.Message));
            }
            
        }

        private void Init_Window()
        {
            try
            {
                WriteProgress("Setting console colors..", 30);
                if (LibraryConfiguration.IsNotNull("SetColors"))
                    if (LibraryConfiguration.IsTrue("SetColors"))
                    {
                        InvokeScript(string.Format("$host.UI.RawUI.ForegroundColor = \"{0}\"", LibraryConfiguration.IsNotNull("ForegroundColor") ? LibraryConfiguration.Setting<string>("ForegroundColor") : "White"));
                        InvokeScript(string.Format("$host.UI.RawUI.BackgroundColor = \"{0}\"", LibraryConfiguration.IsNotNull("BackgroundColor") ? LibraryConfiguration.Setting<string>("BackgroundColor") : "DarkBlue"));
                    }
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process ConsoleColors, error: {0}", ex.Message));
            }

            try
            {
                WriteProgress("Disabling window control box..", 35);
                if (LibraryConfiguration.IsNotNull("DisableControlBox"))
                    if (LibraryConfiguration.IsTrue("DisableControlBox"))
                        InvokeScript("Disable-WindowControlBox");
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process ControlBox, error: {0}", ex.Message));
            }
            
        }

        private void Init_PersistedData(string pathName)
        {
            try
            {
                WriteProgress("Restoring data..", 50);
                string configFile = System.IO.Path.Combine(pathName, "Sorlov.PowerShell.PersistedState.xml");
                if (System.IO.File.Exists(configFile))
                {
                    WriteProgress("Loading persisted state..", 55);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(configFile);

                    try
                    {
                        if (LibraryConfiguration.IsNotNull("PersistPath"))
                            if (LibraryConfiguration.IsTrue("PersistPath"))
                            {
                                WriteProgress("Processing general items..", 60);
                                XmlNode pwdNode = xmlDoc.SelectSingleNode("/PSState/General/Pwd");
                                InvokeScript(string.Format("Set-Location \"{0}\"", pwdNode.Attributes["Value"].Value));
                            }
                    }
                    catch (Exception ex)
                    {
                        WriteWarning(string.Format("Could not process PersistPath, error: {0}", ex.Message));
                    }

                    try
                    {
                        if (LibraryConfiguration.IsNotNull("PersistHistory"))
                            if (LibraryConfiguration.IsTrue("PersistHistory"))
                            {
                                WriteProgress("Restoring history..", 65);
                                int id = 0;
                                string historyFile = "@'" + Environment.NewLine;
                                historyFile += "#TYPE Microsoft.PowerShell.Commands.HistoryInfo" + Environment.NewLine;
                                historyFile += "\"Id\",\"CommandLine\",\"ExecutionStatus\",\"StartExecutionTime\",\"EndExecutionTime\"" + Environment.NewLine;
                                XmlNodeList history = xmlDoc.SelectNodes("/PSState/HistoryList/History");
                                foreach (XmlNode historyNode in history)
                                {
                                    historyFile += string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"" + Environment.NewLine, historyNode.Attributes["Id"].Value, historyNode.Attributes["CommandLine"].Value, historyNode.Attributes["ExecutionStatus"].Value, historyNode.Attributes["StartExecutionTime"].Value, historyNode.Attributes["EndExecutionTime"].Value);
                                    id++;
                                }
                                historyFile += "'@ | ConvertFrom-CSV | Add-History";
                                InvokeScript(historyFile);
                            }
                    }
                    catch (Exception ex)
                    {
                        WriteWarning(string.Format("Could not process PersistHistory, error: {0}", ex.Message));
                    }

                    try
                    {
                        if (LibraryConfiguration.IsNotNull("PersistAliases"))
                            if (LibraryConfiguration.IsTrue("PersistAliases"))
                            {
                                WriteProgress("Restoring Aliases..", 75);
                                XmlNodeList aliases = xmlDoc.SelectNodes("/PSState/AliasList/Alias");
                                foreach (XmlNode alias in aliases)
                                    InvokeScript(string.Format("Set-Alias -Name {0} -Value {1} -Scope Global -Force -ErrorAction SilentlyContinue", alias.Attributes["Name"].Value, alias.Attributes["Definition"].Value));
                            }
                    }
                    catch (Exception ex)
                    {
                        WriteWarning(string.Format("Could not process PersistAliases, error: {0}", ex.Message));
                    }

                }
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process StateRestore, error: {0}", ex.Message));
            }
            
        }

        private void Init_SupportFiles(string localDir)
        {
            WriteProgress("Extracting help content..", 10);
            if (!File.Exists(Path.Combine(localDir, "Sorlov.PowerShell-help.xml")))
            {
                XMLHelpGenerator.GenerateHelp(Path.Combine(localDir, "Sorlov.PowerShell.dll"), localDir, true);
            }

            WriteProgress("Extracting type and format data..", 15);
            if (!File.Exists(Path.Combine(localDir, "Sorlov.PowerShell.Format.ps1xml")) || !File.Exists(Path.Combine(localDir, "Sorlov.PowerShell.Types.ps1xml")))
            {
                PS1XMLGenerator.GeneratePS1XML(localDir, localDir, "Sorlov.PowerShell");
            }
            InvokeCommand("Update-TypeData", new Dictionary<string, object>() { { "AppendPath", System.IO.Path.Combine(localDir, "Sorlov.PowerShell.Types.ps1xml") } });
            InvokeCommand("Update-FormatData", new Dictionary<string, object>() { { "AppendPath", System.IO.Path.Combine(localDir, "Sorlov.PowerShell.Format.ps1xml") } });            
        }

        private void Init_ExitHandler()
        {
            try
            {
                WriteProgress("Attatching exit handler..", 45);
                InvokeScript("Register-EngineEvent -SourceIdentifier PowerShell.Exiting -SupportEvent -Action { Exit-PSEnvironment $PSStateXMLPath }");
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process RegisterExitHandler, error: {0}", ex.Message));
            }            
        }

        public void Init_EnvionmentVariables(string pathName)
        {
            try
            {
                WriteProgress("Setting PSStateXMLPath..", 40);
                InvokeScript(string.Format("Set-Variable PSStateXMLPath \"{0}\" -Scope Global", pathName));
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process PSStateXMLPath, error: {0}", ex.Message));
            }
            
        }

        private void Init_Banner()
        {
            try
            {
                WriteProgress("Finishing touches..", 95);
                if (LibraryConfiguration.IsNotNull("PrintBanner"))
                    if (LibraryConfiguration.IsTrue("PrintBanner"))
                    {
                        Version vInfo = Assembly.GetExecutingAssembly().GetName().Version;
                        InvokeScript("Clear-Host");
                        InvokeScript(string.Format("Write-Host \"Sorlov PowerShell Utilities (v{0}.{1}.{2}.{3})\" -ForegroundColor White", vInfo.Major, vInfo.Minor, vInfo.Build, vInfo.Revision));
                        InvokeScript("Write-Host \"Copyright (C) 2010-2017 by Daniel Sörlöv \" -ForegroundColor White");
                        InvokeScript("Write-Host \"All rights reserved. Freeware version.\" -ForegroundColor White"); 
                        InvokeScript("Write-Host");
                        InvokeScript("Write-Host");

                    }
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process PrintBanner, error: {0}", ex.Message));
            }            
        }

        private void Init_Upgrade(string localDir)
        {
            bool deleted = false;
            WriteProgress("Cleaning old files..", 5);
            foreach (string module in System.IO.Directory.GetFiles(localDir, "Sorlov*.old"))
            {
                deleted = true;
                File.Delete(module);
            }
            if (deleted)
            {
                File.Delete(Path.Combine(localDir, "Sorlov.PowerShell-help.xml"));
                File.Delete(Path.Combine(localDir, "Sorlov.PowerShell.Format.ps1xml"));
                File.Delete(Path.Combine(localDir, "Sorlov.PowerShell.Types.ps1xml"));
            }

        }

        public void OnImport()
        {
            try
            {
                powerShell = System.Management.Automation.PowerShell.Create(RunspaceMode.CurrentRunspace);
                WriteProgress("Starting..", 0 );

                string localDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string pathName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WindowsPowershell\\Modules\\Sorlov.PowerShell");

                Init_Upgrade(localDir);
                Init_EnvionmentVariables(pathName);
                Init_SupportFiles(localDir);
                Init_Window();
                Init_Prompt();
                Init_Office();
                Init_PersistedData(pathName);
                Init_ExitHandler();
                Init_Banner();

                WriteProgress("Completed!", 100);
            }
            catch (Exception ex)
            {
                WriteWarning(string.Format("Could not process AutoStart, error: {0}", ex.Message));
            }

            powerShell.Streams.Progress.Clear();
        }
    }
}
