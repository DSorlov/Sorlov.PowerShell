using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Sorlov.SelfHostedPS;
using Sorlov.SelfHostedPS.Application;

namespace Sorlov.PowerShell.Lib.HTApps
{
    public partial class ScriptForm : Form
    {
        public System.Management.Automation.PowerShell PowerShellEngine;
        public Host PowerShellHost;
        private StringBuilder screenBuilder;
        public PipelineState PipelineStatus = PipelineState.NotStarted;
        public ScriptInterface ScriptInterface;
        public object ExitObject;
        public string ProviderPath;
        public string LoadedScript;

        private void InitializePowershell()
        {
            PowerShellHost = new Host();
            PowerShellEngine = System.Management.Automation.PowerShell.Create();
            screenBuilder = new StringBuilder();
            InitialSessionState initalState = InitialSessionState.CreateDefault();
            PowerShellEngine.Runspace = RunspaceFactory.CreateRunspace(PowerShellHost, initalState);
            PowerShellEngine.Runspace.ApartmentState = System.Threading.ApartmentState.STA;
            PowerShellEngine.Runspace.Open();
            ConsoleHandler.ConsoleOutputReceived += ConsoleHandler_ConsoleOutput;
        }

        public ScriptForm(string initalScript)
        {
            InitializeComponent();
            InitializePowershell();

            Visible = false;
            List<string> modulesToLoad = Common.ProcessManifest(this.PowerShellEngine.Runspace.InitialSessionState);

            foreach (string module in modulesToLoad)
                try
                {
                    this.PowerShellEngine.Runspace.SessionStateProxy.InvokeCommand.InvokeScript(string.Format("Import-Module {0} -Scope Global", module));
                }
                catch (Exception ex)
                {
                    this.ScriptInterface.ErrorMessage(string.Format("Could import module {0}: {0}", ex.Message, module));
                }

            this.ProviderPath = Common.GetDataDirectory();
            LoadApplication(Path.Combine(Common.GetDataDirectory(), initalScript));
            BringToFront();

        }
        public ScriptForm()
        {
                InitializeComponent();
                InitializePowershell();

        }

        public void ConsoleHandler_ConsoleOutput(string data)
        {
            screenBuilder.Append(data);
        }

        public void LoadNewApplication(string filePath)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(
                    delegate(object o, DoWorkEventArgs args)
                    {
                        while (PipelineStatus == PipelineState.Running || PipelineStatus == PipelineState.Stopping)
                        {
                            System.Windows.Forms.Application.DoEvents();
                            Thread.Sleep(1);
                        }

                        if (filePath.StartsWith(@".\"))
                            filePath = System.IO.Path.Combine(this.ProviderPath, filePath.Substring(2));

                        if (!System.IO.Path.IsPathRooted(filePath))
                            filePath = System.IO.Path.Combine(this.ProviderPath, filePath);

                        string script;
                        try {
                            script = File.ReadAllText(filePath);
                        }
                        catch (FileNotFoundException)
                        {
                            this.ShowDialog(this.Text, string.Format("Could not load '{0}' because the file was not found.", Path.GetFileName(filePath)), MessageBoxIcon.Error);
                            return;
                        }
                        catch (Exception)
                        {
                            this.ShowDialog(this.Text, string.Format("Could not load '{0}' due to a internal processing error.", Path.GetFileName(filePath)), MessageBoxIcon.Error);
                            return;                            
                        }

                        browser.SafeInvoke(brWnd => brWnd.DocumentText = PreprocessScript(script));
                        LoadedScript = filePath;

                    });

            bw.RunWorkerAsync();
        }

        public void LoadApplication(string filePath)
        {
            ScriptInterface = new ScriptInterface(this);
            browser.ObjectForScripting = ScriptInterface;

            PowerShellEngine.Runspace.SessionStateProxy.PSVariable.Set(new PSVariable("App", ScriptInterface, ScopedItemOptions.AllScope));
            PowerShellEngine.Runspace.SessionStateProxy.PSVariable.Set(new PSVariable("Document", browser.Document, ScopedItemOptions.AllScope));

            browser.DocumentText = PreprocessScript(File.ReadAllText(filePath));
            LoadedScript = filePath;
        }

        private void scriptForm_Load(object sender, EventArgs e)
        {
        }

        public void ShowDialog(string title, string message, MessageBoxIcon icon)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, icon);
        }

        private string ProcessClientSide(string fileData)
        {
            string result = fileData;
            result = result.Replace("PSRuntime:", "window.external.", StringComparison.InvariantCultureIgnoreCase);
            result = result.Replace("alert(", "window.external.InfoMessage(", StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        private string ProcessServerSide(string fileData)
        {
            string result = fileData;
            result = Regex.Replace(result, @"(<%=)(.*?)(%>)", match => string.Format("<%Write-Host ({0})%>", match.Groups[2].Value.Trim()), RegexOptions.Singleline);
            return result;
        }

        void tagPipeline_StateChanged(object sender, PipelineStateEventArgs e)
        {
            PipelineStatus = e.PipelineStateInfo.State;
        }

        public string ExecuteScriptOnThread(string script)
        {
            try
            {
                screenBuilder.Length = 0;

                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(
                    delegate(object o, DoWorkEventArgs args)
                        {

                            Pipeline tagPipeline = this.PowerShellEngine.Runspace.CreatePipeline();
                            tagPipeline.Commands.Add(new Command(script, true, false));
                            tagPipeline.Commands.Add(new Command("Out-Default", false, false));
                            tagPipeline.Invoke();
                        }
                    );
                bw.RunWorkerAsync();

                while (bw.IsBusy) {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(1);
                }

                return screenBuilder.ToString().Trim();
            }
            catch (Exception ex)
            {
                return string.Format("<b>[Internal Error while processing:</b> {0}<b>]</b>", ex.Message);
            }


        }

        private string ProcessScriptTags(string fileData)
        {
            return Regex.Replace(fileData, @"(<%)(.*?)(%>)", delegate(Match match)
            {
                return this.SafeInvoke(scrptFrm => scrptFrm.ExecuteScriptOnThread(match.Groups[2].Value.Trim()));
            }, RegexOptions.Singleline);
        }

        public string PreprocessScript(string fileData)
        {
            this.SuspendLayout();
            string result = fileData;
            result = ProcessServerSide(result);
            result = ProcessScriptTags(result);
            result = ProcessClientSide(result);
            this.ResumeLayout(true);
            return result;
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

    }
}
