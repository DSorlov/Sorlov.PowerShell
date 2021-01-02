using System;
using System.Linq;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Word;


namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, "OpenWord")]
    [CmdletDescription("Output handler to create Microsoft Word compatible documents.",
        "You may use the Out-OpenWord handler to create documents and provide basic formatting. ")]
    [Example(Code = "Get-Process | Select Name | Out-OpenWord Processes.xlsx", Remarks = "Will produce a Excel document in the current directory listing all processes in the system")]
    [Example(Code = "Get-Process | Select Name | Out-OpenWord Processes.xlxs -Overwrite", Remarks = "Will produce a Excel document in the current directory listing all processes in the system and overwriting any existing file")]
    [OutputType(typeof(System.IO.FileInfo))]
    public class OutOpenWord : PSCmdlet
    {
        #region "Private variables"
        private string parPath = String.Empty;
        private bool parOverwrite = false;
        private string parTitle = string.Empty; 
        private string parHeader = string.Empty;
        private string parFooter = string.Empty;
        private PSObject parObject;
        private Type lastObject;
        private TableDesign tableStyle = TableDesign.MediumList1Accent1;
        private string parFormat = "Word2007";

        private DocX wordDocument;
        private Table currentTable;

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
        [ValidateSet("Word2007")]
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

        [Parameter(HelpMessage = "The style to use for the table")]
        [ValidateNotNullOrEmpty()]
        public TableDesign TableStyle
        {
            get { return tableStyle; }
            set { tableStyle = value; }
        }

        [Parameter(ValueFromPipeline = true)]
        public PSObject inputObject
        {
            get { return parObject; }
            set { parObject = value; }
        }

        [Parameter(HelpMessage = "The title to insert into the document")]
        public string Title
        {
            get { return parTitle; }
            set { parTitle = value; }
        }

        [Parameter(HelpMessage = "The header to add before the table")]
        public string Header
        {
            get { return parHeader; }
            set { parHeader = value; }
        }

        [Parameter(HelpMessage = "The footer to add after the table")]
        public string Footer
        {
            get { return parFooter; }
            set { parFooter = value; }
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
            WriteProgress(new ProgressRecord(10, "Out-OWord", "Initializing.."));

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

            WriteVerbose("Creating Word object");


            WriteProgress(new ProgressRecord(10, "Out-OWord", "Creating document.."));
            try
            {
                WriteVerbose("Creating new document");
                wordDocument = DocX.Create(parPath);
            }
            catch(Exception)
            {
                WriteVerbose("Document creation failed ");
                ThrowTerminatingError(new ErrorRecord(new System.IO.IOException("Document creation failed"), "101", ErrorCategory.ResourceUnavailable, parPath));
            }
            
            if (parTitle != string.Empty)
            {
                wordDocument.InsertParagraph(parTitle).FontSize(28);
                wordDocument.InsertParagraph();
            }

            if (parHeader != string.Empty)
            {
                wordDocument.InsertParagraph(parHeader);
                wordDocument.InsertParagraph();
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
                    if (currentTable != null) wordDocument.InsertTable(currentTable);
                    WriteVerbose("New object type detected");
                    WriteProgress(new ProgressRecord(10, "Out-OWord", "Creating new table.."));

                    lastObject = parObject.GetType();

                    int columns = 0;
                    foreach (PSMemberInfo property in parObject.Properties)
                        columns++;

                    WriteVerbose(string.Format("Adding header (#{0})", headerCount++));
                    WriteProgress(new ProgressRecord(10, "Out-OWord", string.Format("Adding header (#{0})", headerCount)));

                    currentTable = wordDocument.AddTable(1, parObject.Properties.Count());
                    currentTable.Design = TableDesign.MediumList1Accent1;
                    currentTable.Alignment = Lib.MicrosoftOffice.Word.Alignment.left;

                    rowCount = 1;

                    int headerCounter = 0;
                    foreach (PSMemberInfo property in parObject.Properties)
                    {
                        currentTable.Rows[0].Cells[headerCounter].Paragraphs[0].Append(property.Name);
                        headerCounter++;
                    }
                }

                WriteVerbose(string.Format("Adding object row (#{0})", totalCount++));
                WriteProgress(new ProgressRecord(10, "Out-OWord", string.Format("Adding row (#{0})", totalCount)));

                currentTable.InsertRow();
                int rowCounter = 0;
                foreach (PSMemberInfo property in parObject.Properties)
                {

                    try
                    {
                        currentTable.Rows[rowCount].Cells[rowCounter].Paragraphs[0].Append(property.Value.ToString());
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

            wordDocument.InsertTable(currentTable);

            wordDocument.InsertParagraph();

            if (parFooter != string.Empty)
                wordDocument.InsertParagraph(parFooter).Italic().FontSize(11).Alignment = Lib.MicrosoftOffice.Word.Alignment.center;

            WriteProgress(new ProgressRecord(10, "Out-OWord", "Saving document.."));
            WriteVerbose(string.Format("Saving document in {0}", parFormat));

            switch (parFormat.ToLower())
            {
                case "word2007":
                    {
                        wordDocument.Save();
                    }
                    break;
            }

            WriteProgress(new ProgressRecord(10, "Out-OWord", "Done"));
            WriteVerbose("Done");

            WriteObject(new FileInfo(parPath));


        }
        #endregion
    }
}
