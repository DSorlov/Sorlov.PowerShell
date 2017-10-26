using Sorlov.PowerShell.Dto.MicrosoftOffice;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sorlov.PowerShell.Cmdlets
{

    [Cmdlet(VerbsData.Import, "ExcelWorkbook")]
    [CmdletDescription("Imports a file from excel to a native object",
        "This imports a excel cheet to powershell")]
    [OutputType(typeof(Workbook))]
    public class ImportExcelWorkbook : PSCmdlet
    {
        private CultureInfo currentCulture;
        private IDynamic msExcel;
        private string parFile;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string File
        {
            get { return parFile; }
            set { parFile = value; }
        }

        private RowCollection GetRows(IDynamic workSheet)
        {
            List<object> result = new List<object>();
            DynamicTypeManager dynType = new DynamicTypeManager("Sorlov.PowerShell.MicrosoftOffice", "ExcelWorkbook#" + parFile.GetHashCode().ToString());


            IDynamic endCell = workSheet.Property("Cells").Get().Property("SpecialCells").PropertyParam(11).Get();
            IDynamic range = workSheet.Property("Range").PropertyParam("A1").PropertyParam(GetExcelCellName(endCell.Property("Column").Get<int>(), endCell.Property("Row").Get<int>())).Get();

            IDynamic columns = range.Property("Columns").Get();
            int columnCount = columns.Property("Count").Get<int>();

            IDynamic rows = range.Property("Rows").Get();
            int rowCount = rows.Property("Count").Get<int>();

            int startingRow = range.Property("Row").Get<int>();
            int startingColumn = range.Property("Column").Get<int>();

            for (int i = 0; i < columnCount; i++)
            {

                string colName = workSheet.Property("Cells").Get().Index(startingRow,startingColumn+i).Get().Property("Value").Get<string>();
                if (colName == null) break;
                dynType.CreateProperty(colName, typeof(string));
            }


            for (int i = 1; i < rowCount; i++)
            {
                object newRow = dynType.Create();

                for (int j = 0; j < columnCount; j++)
                {
                    string colName = workSheet.Property("Cells").Get().Index(startingRow, startingColumn + j).Get().Property("Text").Get<string>();
                    if (colName == null) break;

                    string cellData = workSheet.Property("Cells").Get().Index(startingRow + i, startingColumn + j).Get().Property("Text").Get<string>();
                    DynamicTypeManager.SetProperty(newRow, colName, cellData);
                }

                result.Add(newRow);
            }


            return new RowCollection(result);
        }

        private string SanitizePropertyName(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            str = rgx.Replace(str, "");
            return str;
        }

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

            return columnName + rowNumber;
        }

        private WorksheetCollection GetWorksheets(IDynamic workBook)
        {
            List<Worksheet> result = new List<Worksheet>();

            IDynamic nativeWorksheets = workBook.Property("Worksheets").Get();
            int sheetCount = nativeWorksheets.Property("Count").Get<int>();

            for (int i = 1; (i - 1) < sheetCount; i++)
            {
                Worksheet newSheet = new Worksheet();

                IDynamic nativeworkSheet = nativeWorksheets.Index(i).Get();
                newSheet.Name = nativeworkSheet.Property("Name").Get<string>();

                newSheet.Rows = GetRows(nativeworkSheet);

                result.Add(newSheet);
            }

            return new WorksheetCollection(result);
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (!(GetOfficeVersion.GetVersion("Excel")).IsInstalled)
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Excel is not installed use Import-OExcelWorkbook instead."), "1", ErrorCategory.InvalidOperation, null));

            msExcel = BindingFactory.CreateAutomationBinding("Excel.Application");

            currentCulture = Thread.CurrentThread.CurrentCulture;
            int langSettings = msExcel.Property("LanguageSettings").Get().Property("LanguageID").PropertyParam(2).Get<int>();

            CultureInfo newCulture = new CultureInfo(langSettings);
            WriteVerbose(string.Format("Changing culture ({0}->{1})", currentCulture.DisplayName, newCulture.DisplayName));
            Thread.CurrentThread.CurrentCulture = newCulture;

            if (parFile.StartsWith(@".\"))
                parFile = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parFile.Substring(2));

            parFile = System.IO.Path.GetFullPath(parFile);

            if (!System.IO.File.Exists(parFile))
            {
                msExcel.Method("Quit").Invoke();
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException(string.Format("The source file does not exist", parFile)), "200", ErrorCategory.OpenError, parFile));
            }

            IDynamic nativeWorkbook = null;
            try  
            {
                nativeWorkbook = msExcel.Property("Workbooks").Get().Method("Open").AddParameter(parFile).AddMissingParameters(14).Invoke();
                FileInfo fileInfo = new FileInfo(parFile);

                Workbook workbook = new Workbook();
                workbook.Filepath = parFile;
                workbook.FileIsReadOnly = fileInfo.IsReadOnly;
                workbook.FileLastModified = fileInfo.LastWriteTime;
                workbook.FileCreated = fileInfo.CreationTime;

                workbook.Author = nativeWorkbook.Property("Author").Get<string>();
                workbook.Title = nativeWorkbook.Property("Title").Get<string>();
                workbook.Comments = nativeWorkbook.Property("Comments").Get<string>();
                workbook.Keywords = nativeWorkbook.Property("Keywords").Get<string>();

                workbook.Worksheets = GetWorksheets(nativeWorkbook);

                WriteObject(workbook);

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(new InvalidDataException(string.Format("Conversion failed: {0}", ex.Message)), "201", ErrorCategory.ParserError, parFile));
            }
            finally
            {
                if (nativeWorkbook!=null) nativeWorkbook.Method("Close").AddParameter(0).Invoke();
                msExcel.Method("Quit").Invoke();
            }


        }

        protected override void EndProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");

            msExcel.Method("Quit").Invoke();

            WriteVerbose("Reverting culture");
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");

            msExcel.Method("Quit").Invoke();

            WriteVerbose("Reverting culture");
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }

    }
}
