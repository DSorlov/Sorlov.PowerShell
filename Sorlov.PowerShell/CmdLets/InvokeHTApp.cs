using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "HTApp")]
    [CmdletDescription("Executes a HTApp")]

    public class InvokeHTApp: PSCmdlet
    {
        private string parPath;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parPath; }
            set { parPath = value; }
        }

        public void runForm(string path)
        {
        }

        protected override void BeginProcessing()
        {
            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2));

            parPath = System.IO.Path.GetFullPath(parPath);

            if (!File.Exists(parPath))
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("File was not found"), "100", ErrorCategory.ResourceUnavailable, parPath));

            // New form and browser
            ScriptForm form = new ScriptForm();
            form.LoadApplication(parPath);
            form.ProviderPath = CurrentProviderLocation("FileSystem").ProviderPath;
            form.Show();

            while (form.Visible && form.PipelineStatus == PipelineState.NotStarted)
            {
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(1);
            }

            if (form.ExitObject != null)
                WriteObject(form.ExitObject);
           
            }

    }
}
