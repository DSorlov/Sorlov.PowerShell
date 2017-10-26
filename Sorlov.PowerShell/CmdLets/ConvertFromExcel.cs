using System;
using System.Management.Automation;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Globalization;
using System.Threading;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.ConvertFrom, "Excel")]
    [CmdletDescription("Converts a document from Excel to PDF, XPS or ODF",
        "This converts a Excel document to PDF, XPS or ODF")]
    public class ConvertFromExcel : PSCmdlet
    {
        #region "Private variables"
        private string parFile;
        private string parFormat = "PDF";
        private bool parNoExit;
        private bool parDisplay;
        private int format;
        private string formatName;
        private bool parOverwrite;
        private CultureInfo currentCulture;
        private IDynamic msExcel; 
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
        [ValidateSet("Excel", "Excel97", "XPS", "PDF")]
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

            if (!(GetOfficeVersion.GetVersion("Excel")).IsInstalled)
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Excel is not installed"), "1", ErrorCategory.InvalidOperation, null));

            //if (!ValidLicense())
            //    ThrowTerminatingError(new ErrorRecord(new Licensing.InvalidLicenseException("Product licence could not be verified"), "10", ErrorCategory.SecurityError, null));

            msExcel = BindingFactory.CreateAutomationBinding("Excel.Application");
            if (parDisplay)
            {
                WriteVerbose("Display have been set, making application visible");
                msExcel.Property("Visible").Set(true);
            }

            currentCulture = Thread.CurrentThread.CurrentCulture;
            int langSettings = msExcel.Property("LanguageSettings").Get().Property("LanguageID").PropertyParam(2).Get<int>();

            CultureInfo newCulture = new CultureInfo(langSettings);
            WriteVerbose(string.Format("Changing culture ({0}->{1})", currentCulture.DisplayName, newCulture.DisplayName));
            Thread.CurrentThread.CurrentCulture = newCulture;

            switch (parFormat.ToLower())
            {
                case "excel": format = 51; formatName = "xlsx"; break;
                case "excel97": format = 56; formatName = "xls"; break;
                case "pdf": format = 57; formatName = "pdf"; break;
                case "xps": format = 58; formatName = "xps"; break;
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
                    IDynamic doc = msExcel.Property("Workbooks").Get().Method("Open").AddParameter(parFile).AddMissingParameters(14).Invoke();
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
                WriteVerbose("-NoExit specified, making excel visible if not already");
                msExcel.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting excel");
                msExcel.Method("Quit").Invoke();
            }

            WriteVerbose("Reverting culture");
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");

            if (parNoExit)
            {
                WriteVerbose("-NoExit specified, making excel visible if not already");
                msExcel.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting excel");
                msExcel.Method("Quit").Invoke();
            }

            WriteVerbose("Reverting culture");
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

    }
}
