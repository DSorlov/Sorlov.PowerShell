using System;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Excel;


namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, "OpenExcel")]
    [CmdletDescription("Output handler to create Microsoft Excel compatible documents.",
        "You may use the Out-OpenExcel handler to create documents and provide basic formatting. ")]
    [Example(Code = "Get-Process | Select Name | Out-OpenExcel Processes.xlsx", Remarks = "Will produce a Excel document in the current directory listing all processes in the system")]
    [Example(Code = "Get-Process | Select Name | Out-OpenExcel Processes.xlxs -Overwrite", Remarks = "Will produce a Excel document in the current directory listing all processes in the system and overwriting any existing file")]
    [OutputType(typeof(System.IO.FileInfo))]
    public class OutOpenExcel : PSCmdlet
    {
        #region "Private variables"
        private string parPath = String.Empty;
        private bool parOverwrite = false;
        private PSObject parObject;
        private Type lastObject;
        private string tableStyle = string.Empty;
        private string parFormat = "Excel2007";

        private ExcelPackage excelPackage;
        private ExcelWorksheet currentSheet;

        private int rowCount = 0;
        private int totalCount = 0;
        private int headerCount = 0;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parPath; }
            set { parPath = value; }
        }

        [Parameter(Position = 1, HelpMessage = "Format to save in")]
        [ValidateSet("Excel2007")]
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

        [Parameter(ValueFromPipeline = true)]
        public PSObject inputObject
        {
            get { return parObject; }
            set { parObject = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2));

            parPath = System.IO.Path.GetFullPath(parPath);

            WriteVerbose(string.Format("Checking for existance of file {0}", parPath));
            WriteProgress(new ProgressRecord(10, "Out-OExcel", "Initializing.."));

            if (File.Exists(parPath))
            {
                if (!parOverwrite)
                {
                    WriteVerbose("File exists and -Overwrite have not been specified");
                    ThrowTerminatingError(new ErrorRecord(new System.IO.IOException("The file already exists and -Overwrite have not been specified"), "100", ErrorCategory.ResourceExists, parPath.ToString()));
                }
                else
                {
                    WriteVerbose("File exists and -Override have been specified, will delete file");
                    File.Delete(Path);
                }
            }

            WriteVerbose("Creating Excel object");


            WriteProgress(new ProgressRecord(10, "Out-OExcel", "Creating document.."));
            try
            {
                WriteVerbose("Creating new document");
                excelPackage = new ExcelPackage(new FileInfo(parPath));
            }
            catch
            {
                WriteVerbose("Document creation failed ");
                ThrowTerminatingError(new ErrorRecord(new System.IO.IOException("Document creation failed"), "101", ErrorCategory.ResourceUnavailable, parPath));
            }
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            if (parObject == null)
            {
                WriteVerbose("A null object found in the pipe, ignoring it.");
            }
            else
            {
                if (parObject.GetType() != lastObject)
                {
                    WriteVerbose("New object type detected");
                    WriteProgress(new ProgressRecord(10, "Out-OExcel", "Creating new table.."));

                    lastObject = parObject.GetType();

                    int columns = 0;
                    foreach (PSMemberInfo property in parObject.Properties)
                        columns++;

                    WriteVerbose(string.Format("Adding header (#{0})", headerCount++));
                    WriteProgress(new ProgressRecord(10, "Out-OExcel", string.Format("Adding header (#{0})", headerCount)));

                    currentSheet = excelPackage.Workbook.Worksheets.Add(string.Format("Sheet {0}",excelPackage.Workbook.Worksheets.Count));

                    rowCount = 2;

                    int headerCounter = 1;
                    foreach (PSMemberInfo property in parObject.Properties)
                    {
                        currentSheet.Cell(1, headerCounter).Value = property.Name;
                        headerCounter++;
                    }
                }

                WriteVerbose(string.Format("Adding object row (#{0})", totalCount++));
                WriteProgress(new ProgressRecord(10, "Out-OExcel", string.Format("Adding row (#{0})", totalCount)));

                int rowCounter = 1;
                foreach (PSMemberInfo property in parObject.Properties)
                {

                    try
                    {
                        currentSheet.Cell(rowCount, rowCounter).Value = property.Value.ToString();
                    }
                    catch
                    {
                    }
                    rowCounter++;
                }
                rowCount++;


            }


        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();

            WriteProgress(new ProgressRecord(10, "Out-OExcel", "Saving document.."));
            WriteVerbose(string.Format("Saving document in {0}", parFormat));

            switch (parFormat.ToLower())
            {
                case "excel2007":
                    {
                        excelPackage.Save();
                    }
                    break;
            }

            WriteProgress(new ProgressRecord(10, "Out-OExcel", "Done"));
            WriteVerbose("Done");

            excelPackage.Dispose();

            WriteObject(new FileInfo(parPath));


        }
        #endregion
    }
}
