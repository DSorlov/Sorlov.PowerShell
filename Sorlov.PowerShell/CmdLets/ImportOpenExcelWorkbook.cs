using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Sorlov.PowerShell.Dto.MicrosoftOffice;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Excel;


namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Import, "OpenExcelWorkbook")]
    [CmdletDescription("Imports a file from excel to a native object",
        "This imports a excel sheet to powershell")]
    public class ImportOpenExcelWorkbook : PSCmdlet
    {
        private string parFile;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The document to convert to PDF", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string File
        {
            get { return parFile; }
            set { parFile = value; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (parFile.StartsWith(@".\"))
                parFile = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parFile.Substring(2));

            parFile = System.IO.Path.GetFullPath(parFile);

            if (!System.IO.File.Exists(parFile))
            {
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException(string.Format("The source file does not exist", parFile)), "200", ErrorCategory.OpenError, parFile));
            }

            FileInfo fileInfo = new System.IO.FileInfo(parFile);

            ExcelPackage excelPackage = new ExcelPackage(fileInfo);
            ExcelWorkbook excelWorkbook = excelPackage.Workbook;


            Workbook workbook = new Workbook();
            workbook.Filepath = parFile;
            workbook.FileIsReadOnly = fileInfo.IsReadOnly;
            workbook.FileLastModified = fileInfo.LastWriteTime;
            workbook.FileCreated = fileInfo.CreationTime;

            workbook.Author = excelWorkbook.Properties.Author;
            workbook.Title = excelWorkbook.Properties.Title;
            workbook.Comments = excelWorkbook.Properties.Comments;
            workbook.Keywords = excelWorkbook.Properties.Keywords;

            List<Worksheet> worksheets = new List<Worksheet>();

            foreach (ExcelWorksheet excelWorksheet in excelWorkbook.Worksheets)
            {
                DynamicTypeManager dynType = new DynamicTypeManager("Sorlov.PowerShell.MicrosoftOffice", "ExcelWorkbook#" + parFile.GetHashCode().ToString());

                Worksheet worksheet = new Worksheet();
                worksheet.Name = excelWorksheet.Name;

                int colCount = 1;
                while (true)
                {
                   if (excelWorksheet.Cell(1,colCount).Value==null) break;
                   if (excelWorksheet.Cell(1,colCount).Value.Trim() == string.Empty) break;
                   dynType.CreateProperty(excelWorksheet.Cell(1, colCount).Value.Trim(), typeof(string));

                   colCount++;
                }

                List<object> rowList = new List<object>();
                int rowCount = 2;
                while (true)
                {
                    bool foundSomething = false;
                    object newRow = dynType.Create();

                    for (int i = 1; i < colCount; i++)
                    {

                        string cellValue = excelWorksheet.Cell(rowCount, i).Value;
                        if (cellValue != null)
                            if (cellValue.Trim() != string.Empty)
                            {
                                DynamicTypeManager.SetProperty(newRow, excelWorksheet.Cell(1, i).Value.Trim(), cellValue);
                                foundSomething = true;
                            }
                    }

                    if (foundSomething == false) break;
                    rowList.Add(newRow);
                    rowCount++;
                }

                RowCollection rowCollection = new RowCollection(rowList);
                worksheet.Rows = rowCollection;

                worksheets.Add(worksheet);
            }

            workbook.Worksheets = new WorksheetCollection(worksheets);

            WriteObject(workbook);

            excelPackage.Dispose();
        }

        protected override void EndProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Done processing files");
        }

    }
}
