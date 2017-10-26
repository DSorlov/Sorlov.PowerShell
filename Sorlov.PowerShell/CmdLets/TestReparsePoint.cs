using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.FileSystem;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsDiagnostic.Test, "ReparsePoint")]
    [CmdletDescription("Tests a symbolic link",
        "This command tests a reparse point (symbolic link).")]
    public class TestReparsePoint: PSCmdlet
    {
        #region "Private Parameters"
        string path = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The path",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteObject(JunctionPoint.Exists(path));
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
