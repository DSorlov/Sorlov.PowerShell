using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Security.Principal;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "AdminStatus", SupportsShouldProcess = true)]
    [CmdletDescription("Returns a list of all loaded assemblies")]
    public class GetAdminStatus: PSCmdlet
    {
        #region "Private Parameters"
        string parFile = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The file or directory to touch",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            WindowsPrincipal wp = new WindowsPrincipal(wi);
            base.BeginProcessing();

            WriteObject(wp.IsInRole(WindowsBuiltInRole.Administrator));

        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Format("Touching {0}", parFile));
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stop processing");
        }
        #endregion

    }
}
