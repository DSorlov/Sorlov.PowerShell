using System;
using System.Management.Automation;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.ConvertFrom, "Word")]
    [CmdletDescription("Converts a document from Word to PDF, XPS or ODF",
        "This converts a Word document to PDF, XPS or ODF")]
    public class ConvertFromWord : PSCmdlet
    {
        #region "Private variables"
        private string parFile;
        private string parFormat = "PDF";
        private bool parNoExit;
        private bool parDisplay;
        private int format;
        private string formatName;
        private bool parOverwrite;
        private IDynamic msWord; 
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The document to convert to PDF",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string File
        {
            get { return parFile; }
            set { parFile = value; }
        }

        [Parameter(Position = 1, HelpMessage = "Format to save in")]
        [ValidateSet("Word", "Word97", "XPS", "PDF", "ODF", "RTF")]
        public string Format
        {
            get { return parFormat; }
            set { parFormat = value; }
        }

        [Parameter(HelpMessage = "Should the target file be overwritten if it exists")]
        public SwitchParameter Overwrite
        {
            get { return parOverwrite; }
            set { parOverwrite = value; }
        }

        [Parameter(HelpMessage = "Display word during processing")]
        public SwitchParameter Display
        {
            get { return parDisplay; }
            set { parDisplay = value; }
        }

        [Parameter(HelpMessage = "Do not exit word after processing")]
        public SwitchParameter NoExit
        {
            get { return parNoExit; }
            set { parNoExit = value; }
        }
        #endregion


        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (!(GetOfficeVersion.GetVersion("Word")).IsInstalled)
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Excel is not installed"), "1", ErrorCategory.InvalidOperation, null));


            msWord = BindingFactory.CreateAutomationBinding("Word.Application");
            if (parDisplay)
            {
                WriteVerbose("Display have been set, making application visible");
                msWord.Property("Visible").Set(true);
            }

            switch (parFormat.ToLower())
            {
                case "word": format = 12; formatName = "docx"; break;
                case "word97": format = 0; formatName = "doc"; break;
                case "pdf": format = 17; formatName = "pdf"; break;
                case "xps": format = 18; formatName = "xps"; break;
                case "odf": format = 23; formatName = "odf"; break;
                case "rtf": format = 6; formatName = "rtf"; break;
            }

            WriteVerbose(string.Format("Setting output format to {0} (.{1})",parFormat,formatName));

        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            if (parFile.StartsWith(@".\"))
                parFile = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parFile.Substring(2));

            parFile = System.IO.Path.GetFullPath(parFile);
            
            try
            {
                string destName = string.Format("{0}\\{1}.{2}", Path.GetDirectoryName(parFile),Path.GetFileNameWithoutExtension(parFile), formatName);
                WriteVerbose(string.Format("Will convert {0} to {1}", parFile, destName));

                if (System.IO.File.Exists(destName) && !parOverwrite)
                {
                    WriteError(new ErrorRecord(new UnauthorizedAccessException(string.Format("File already exists and overwrite not specified",destName)),"200",ErrorCategory.PermissionDenied,destName));
                }
                else
                {
                    IDynamic doc = msWord.Property("Documents").Get().Method("Open").AddParameter(parFile).AddMissingParameters(15).Invoke();
                    doc.Method("SaveAs").AddRefParameter(destName).AddRefParameter(format).Invoke();
                    if (!parNoExit) doc.Method("Close").AddParameter(0).Invoke();
                }

            }
            catch(Exception ex)
            {
                WriteError(new ErrorRecord(new InvalidDataException(string.Format("Conversion failed: {0}",ex.Message)),"201",ErrorCategory.ParserError,parFile));
            }
        }
        #endregion

        protected override void EndProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");

            if (parNoExit)
            {
                WriteVerbose("-NoExit specified, making word visible if not already");
                msWord.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting word");
                msWord.Method("Quit").AddParameter(0).Invoke();
            }
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");

            if (parNoExit)
            {
                WriteVerbose("-NoExit specified, making word visible if not already");
                msWord.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting word");
                msWord.Method("Quit").AddParameter(0).Invoke();
            }

        }

    }
}
