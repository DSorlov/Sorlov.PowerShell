using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.IO.Packaging;

namespace Sorlov.PowerShell.Cmdlets 
{
    [Cmdlet(VerbsData.Expand, "ZIPFile",SupportsShouldProcess=true)]
    [CmdletDescription("Extracts the content of a ZIP File",
        "With this command you can get all the files contained in a ZIP file.")]
    [Example(Code="Expand-ZIPFile c:\\temp\\trial.zip -List",Remarks="Returns a list with all the files in the zip-file")]
    [Example(Code="Expand-ZIPFile c:\\temp\\trial.zip c:\\temp\\newdir", Remarks = "Expands trial.zip into c:\\temp\\newdir")]
    [OutputType(null,typeof(Uri))]
    public class ExpandZIPFile : PSCmdlet
    {
        #region "Private Parameters"
        string parFile = @".\";
        bool list = false;
        string outputFolder = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The file to create",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Base directory to expand to", ValueFromPipeline = true)]
        public string OutputRoot
        {
            get { return outputFolder; }
            set { outputFolder = value; }
        }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "Do not expand, only list files", ValueFromPipeline = true)]
        public SwitchParameter List
        {
            get { return list; }
            set { list = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing, will create some ZIP files");
      
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Format("Processing {0}", parFile));

            if (outputFolder.StartsWith(@".\"))
                outputFolder = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, outputFolder.Substring(2)); 

            if (!File.Exists(parFile))
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("File does not exist or access denied"), "100", ErrorCategory.DeviceError, parFile));

            Package package = ZipPackage.Open(parFile, FileMode.Open);
            if (list)
            {
                foreach (PackagePart packagePart in package.GetParts())
                {
                    WriteObject(packagePart.Uri);
                }
            }
            else
            {
                foreach (PackagePart packagePart in package.GetParts())
                {
                    Uri relUri = packagePart.Uri;
                    string savePath = System.IO.Path.Combine(outputFolder, relUri.OriginalString.Trim('/').Replace('/', '\\'));
                    string saveFolder = System.IO.Path.GetDirectoryName(savePath);

                    if (!Directory.Exists(saveFolder))
                    {
                        Directory.CreateDirectory(saveFolder);
                    }

                    if (ShouldProcess(string.Format("Expanding {0} to {1}", packagePart.Uri, savePath)))
                    {
                        using (FileStream fileStream = File.Create(savePath))
                        {
                            using (BinaryReader reader = new BinaryReader(packagePart.GetStream()))
                            {
                                using (BinaryWriter writer = new BinaryWriter(fileStream))
                                {
                                    byte[] buffer = new byte[16384];
                                    int read;
                                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        writer.Write(buffer, 0, read);

                                    }
                                }
                            }
                        }
                    }
                }
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
