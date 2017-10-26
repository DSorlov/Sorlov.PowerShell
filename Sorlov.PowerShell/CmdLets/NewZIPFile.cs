using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.IO.Packaging;

namespace Sorlov.PowerShell.Cmdlets 
{
    [Cmdlet(VerbsCommon.New, "ZIPFile",SupportsShouldProcess=true)]
    [CmdletDescription("Creates a new empty file",
        "This command creates a new empty ZIP file. If the file exists the last write-time is set, if it does not exists then a empty file is created.")]
    [Example(Code="New-ZIPFile c:\\temp\\trial.zip",Remarks="Creates the file trial.zip if it does not exist")]
    [Example(Code="New-ZIPFile c:\\temp\\trial.zip -Overwrite", Remarks = "Creates or overwrites the file trial.zip")]
    public class NewZIPFile : PSCmdlet
    {
        #region "Private Parameters"
        string parFile = string.Empty;
        bool overwrite = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The file to create",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Should the file overwrite", ValueFromPipeline = true)]
        public bool Overwrite
        {
            get { return overwrite; }
            set { overwrite = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing, will create some ZIP files");        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Format("Creating {0}", parFile));

            if (File.Exists(parFile))
            {
                WriteVerbose("File already exists.");
                if (overwrite)
                    if (ShouldProcess(parFile, "Creating new ZIP file"))
                        ZipPackage.Open(parFile, FileMode.Create);
            }
            else
            {
                WriteVerbose("File does not exists.");
                if (ShouldProcess(parFile,"Creating a new ZIP file"))
                    ZipPackage.Open(parFile, FileMode.CreateNew);
            }

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
