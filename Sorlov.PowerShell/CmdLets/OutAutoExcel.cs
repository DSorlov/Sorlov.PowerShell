using System;
using System.Management.Automation;
using System.IO;
using System.Threading;
using System.Globalization;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, "AutoExcel")]
    [CmdletDescription("Output handler to create Microsoft Excel compatible documents.",
        "You may use the Out-Excel handler to create documents and provide basic formatting. ")]
    [Example(Code="Get-Process | Select Name | Out-Excel Processes.xlsx",Remarks="Will produce a Excel document in the current directory listing all processes in the system")]
    [Example(Code = "Get-Process | Select Name | Out-Excel Processes.xlxs -Overwrite", Remarks = "Will produce a Excel document in the current directory listing all processes in the system and overwriting any existing file")]
    [OutputType(typeof(System.IO.FileInfo))]
    public class OutAutoExcel : PSCmdlet
    {
        #region "Private variables"
        private string parPath = String.Empty;
        private bool parDisplay = false;
        private bool parNoExit = false;
        private bool parOverwrite = false;
        private PSObject parObject;
        private Type lastObject;
        private string tableStyle = string.Empty;
        private string parFormat = "Excel";
        private bool isFirstObject = true;

        private IDynamic workBook;
        private IDynamic msExcel;
        private CultureInfo currentCulture;

        private IDynamic currentSheet;

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
        [ValidateSet("Excel", "Excel2007", "XPS", "PDF")]
        public string Format
        {
            get { return parFormat; }
            set { parFormat = value; }
        }

        [Parameter(HelpMessage = "The style to use for the table")]
        [ValidateNotNullOrEmpty()]
        public string TableStyle
        {
            get { return tableStyle; }
            set { tableStyle = value; }
        }

        [Parameter(HelpMessage = "Display Excel during processing")]
        public SwitchParameter Display
        {
            get { return parDisplay; }
            set { parDisplay = value; }
        }

        [Parameter(HelpMessage = "Do not exit Excel after processing")]
        public SwitchParameter NoExit
        {
            get { return parNoExit; }
            set { parNoExit = value; }
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

            if (!(GetOfficeVersion.GetVersion("Excel")).IsInstalled)
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Excel is not installed use Out-OExcel instead."), "1", ErrorCategory.InvalidOperation, null));


            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2));

            parPath = System.IO.Path.GetFullPath(parPath);

            WriteVerbose(string.Format("Checking for existance of file {0}",parPath));
            WriteProgress(new ProgressRecord(10, "Out-Excel", "Initializing.."));

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

            WriteVerbose("Starting Microsoft Excel");
            msExcel = BindingFactory.CreateAutomationBinding("Excel.Application");

            currentCulture = Thread.CurrentThread.CurrentCulture;
            int langSettings = msExcel.Property("LanguageSettings").Get().Property("LanguageID").PropertyParam(2).Get<int>();

            CultureInfo newCulture = new CultureInfo(langSettings);
            WriteVerbose(string.Format("Changing culture ({0}->{1})",currentCulture.DisplayName,newCulture.DisplayName));
            Thread.CurrentThread.CurrentCulture = newCulture;

            if (parDisplay)
            {
                WriteVerbose("Display have been set, making application visible");
                msExcel.Property("Visible").Set(true);
            }

            WriteProgress(new ProgressRecord(10, "Out-Excel", "Creating document.."));
            try
            {
                WriteVerbose("Creating new document");
                workBook = msExcel.Property("Workbooks").Get().Method("Add").Invoke();
            }
            catch
            {
                workBook.Method("Close").AddParameter(0).Invoke();
                msExcel.Method("Quit").AddParameter(0).Invoke();
                WriteVerbose("Document creation failed ");
                ThrowTerminatingError(new ErrorRecord(new System.IO.IOException("Document creation failed"), "101", ErrorCategory.ResourceUnavailable, msExcel));
            }

            WriteVerbose("Activating document");
            workBook.Method("Activate").Invoke();

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
                    WriteProgress(new ProgressRecord(10, "Out-Excel", "Creating new table.."));

                    lastObject = parObject.GetType();

                    int columns = 0;
                    foreach (PSMemberInfo property in parObject.Properties)
                        columns++;

                        WriteVerbose(string.Format("Adding header (#{0})", headerCount++));
                        WriteProgress(new ProgressRecord(10, "Out-Excel", string.Format("Adding header (#{0})", headerCount)));

                        if (isFirstObject)
                        {
                            currentSheet = msExcel.Property("Worksheets").Get().Index(1).Get();
                            isFirstObject = false;
                        }
                        else
                        {
                            currentSheet = msExcel.Property("Worksheets").Get().Method("Add").Invoke();
                        }
                        rowCount = 2;

                        int headerCounter = 1;
                        foreach (PSMemberInfo property in parObject.Properties)
                        {
                            currentSheet.Property("Cells").Get().Index(1,headerCounter).Get().Property("Value").Set(property.Name);
                            headerCounter++;
                        }
                }

                WriteVerbose(string.Format("Adding object row (#{0})", totalCount++));
                WriteProgress(new ProgressRecord(10, "Out-Excel", string.Format("Adding row (#{0})", totalCount)));

                int rowCounter = 1;
                foreach (PSMemberInfo property in parObject.Properties)
                {

                    try
                    {
                        currentSheet.Property("Cells").Get().Index(rowCount,rowCounter).Get().Property("Value").Set(property.Value.ToString());
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

        private string GetExcelCellName(int columnNumber, int rowNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

           return columnName+rowNumber;
        }

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();

            if (tableStyle == string.Empty)
            {
                tableStyle = "TableStyleMedium9";
            }
            WriteVerbose("Setting table styles");
            WriteProgress(new ProgressRecord(10, "Out-Excel", "Formatting tables.."));

            IDynamic dynamicWorksheets = workBook.Property("Worksheets").Get();
            int tableCount = dynamicWorksheets.Property("Count").Get<int>();

            for (int i = 1; (i - 1) < tableCount; i++)
            {
                //worksheet.ListObjects.Add(XlListObjectSourceType.xlSrcRange,worksheet.Range("A1",worksheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell)), Type.Missing, XlYesNoGuess.xlYes,"Table1",tableStyle);
                IDynamic worksheet = dynamicWorksheets.Index(i).Get();

                IDynamic endCell = worksheet.Property("Cells").Get().Property("SpecialCells").PropertyParam(11).Get();
                IDynamic range = worksheet.Property("Range").PropertyParam("A1").PropertyParam(GetExcelCellName(endCell.Property("Column").Get<int>(),endCell.Property("Row").Get<int>())).Get();

                worksheet.Property("Columns").Get().Method("AutoFit").Invoke();
                worksheet.Property("ListObjects").Get().Method("Add").AddParameter(1).AddParameter(range.InstanceObject).AddMissingParameters(1).AddParameter(1).AddParameter("Table1").AddParameter(tableStyle).Invoke();
            }

            WriteProgress(new ProgressRecord(10, "Out-Excel", "Saving document.."));
            int format = 0;
            switch (parFormat.ToLower())
            {
                case "excel": format = 51; break;
                case "excel97": format = 56; break;
                case "pdf": format = 57; break;
                case "xps": format = 58; break;
            }

            WriteVerbose(string.Format("Saving document in {0}", parFormat));

            try
            {
                workBook.Method("SaveAs").AddRefParameter(parPath).AddRefParameter(format).Invoke();
            }
            catch(Exception ex)
            {

                WriteVerbose("Save failed, see error log for details");

                if (parNoExit)
                {
                    WriteVerbose("-NoExit specified, making word visible if not already");
                    msExcel.Property("Visible").Set(true);
                }
                else
                {
                    WriteVerbose("-NoExit not specified, quitting Excel");
                    workBook.Method("Close").AddParameter(0).Invoke();
                    msExcel.Method("Quit").Invoke();
                }

                
                ThrowTerminatingError(new ErrorRecord(ex, "300", ErrorCategory.FromStdErr, parPath));
            }

            if (parNoExit)
            {
                WriteVerbose("-NoExit specified, making Excel visible if not already");
                msExcel.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting word");
                workBook.Method("Close").AddParameter(0).Invoke();
                msExcel.Method("Quit").Invoke();
            }

            WriteVerbose("Reverting culture");
            Thread.CurrentThread.CurrentCulture = currentCulture;

            WriteProgress(new ProgressRecord(10, "Out-Excel", "Done"));
            WriteVerbose("Done");

            WriteObject(new FileInfo(parPath));

        }
        #endregion
    }
}
