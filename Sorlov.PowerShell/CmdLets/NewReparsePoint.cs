using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.FileSystem;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "ReparsePoint",SupportsShouldProcess=true)]
    [CmdletDescription("Creates a symbolic link from a existing file or directory to a new name",
        "This command creates a new reparse point (symbolic link).")]
    public class NewReparsePoint: PSCmdlet
    {
        #region "Private Parameters"
        string sourcePath = string.Empty;
        string destinationPath = string.Empty;
        bool overwrite = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The orginal path",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string SourcePath
        {
            get { return sourcePath; }
            set { sourcePath = value; }
        }
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The target path to create", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string DestinationPath
        {
            get { return destinationPath; }
            set { destinationPath = value; }
        }
        [Parameter(Position = 2, HelpMessage = "Overwrite existing reparse point or empty directory", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Overwrite
        {
            get { return overwrite; }
            set { overwrite = value; }
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
            if (ShouldProcess("Creating link from {0} to {1}"))
                JunctionPoint.Create(sourcePath, destinationPath, overwrite);
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
