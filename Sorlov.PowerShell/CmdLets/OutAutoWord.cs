using System;
using System.Management.Automation;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation;


namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, "AutoWord")]
    [CmdletDescription("Output handler to create Microsoft Word compatible documents.",
        "You may use the Out-Word handler to create documents and provide formatting. The supported document types are Word, Word97, XPS, PDF, ODF, RTF")]
    [Example(Code="Get-Process | Select Name | Out-Word Processes.docx",Remarks="Will produce a Word document in the current directory listing all processes in the system")]
    [Example(Code = "Get-Process | Select Name | Out-Word Processes.pdf -Format PDF", Remarks = "Will produce a PDF document in the current directory listing all processes in the system")]
    [Example(Code = "Get-Process | Select Name | Out-Word Processes.docx -Overwrite", Remarks = "Will produce a Word document in the current directory listing all processes in the system and overwriting any existing file")]
    [Example(Code = "Get-Process | Select Name | Out-Word Processes.docx -Title \"System processes\"", Remarks = "Will produce a Word document in the current directory listing all processes in the system and create a title in the document")]
    [OutputType(typeof(System.IO.FileInfo))]
    public class OutAutoWord : PSCmdlet
    {
        #region "Private variables"
        private string parPath = String.Empty;
        private string parFormat = "Word";
        private string parTitle = String.Empty;
        private string parHeader = String.Empty;
        private string parFooter = String.Empty;
        private string parTemplate = "normal.dotm";
        private bool parDisplay = false;
        private bool parNoExit = false;
        private bool parOverwrite = false;
        private PSObject parObject;
        private Type lastObject;

        private string wordTitleStyle = string.Empty;
        private string wordHeaderStyle = string.Empty;
        private string wordFooterStyle = string.Empty;
        private string wordTableStyle = string.Empty;
        private string wordTableHeaderStyle = string.Empty;
        private string wordTableRowStyle = string.Empty;

        private IDynamic wordDoc;
        private IDynamic msWord;

        private IDynamic currentTable;
        private IDynamic currentRange;

        private int rowCount = 0;
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
        [ValidateSet("Word", "Word97", "XPS", "PDF", "ODF", "RTF")]
        public string Format
        {
            get { return parFormat; }
            set { parFormat = value; }
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

        [Parameter(HelpMessage = "Document template to use. Must exist.")]
        [ValidateNotNullOrEmpty()]
        public string Template
        {
            get { return parTemplate; }
            set { parTemplate = value; }
        }

        [Parameter(HelpMessage = "The style to use for the title field")]
        [ValidateNotNullOrEmpty()]
        public string TitleStyle
        {
            get { return wordTitleStyle; }
            set { wordTitleStyle = value; }
        }

        [Parameter(HelpMessage = "The style to use for the header field")]
        [ValidateNotNullOrEmpty()]
        public string HeaderStyle
        {
            get { return wordHeaderStyle; }
            set { wordHeaderStyle = value; }
        }

        [Parameter(HelpMessage = "The style to use for the footer field")]
        [ValidateNotNullOrEmpty()]
        public string FooterStyle
        {
            get { return wordFooterStyle; }
            set { wordFooterStyle = value; }
        }

        [Parameter(HelpMessage = "The style to use for the table")]
        [ValidateNotNullOrEmpty()]
        public string TableStyle
        {
            get { return wordTableStyle; }
            set { wordTableStyle = value; }
        }

        [Parameter(HelpMessage = "The style to use for the table header rows")]
        [ValidateNotNullOrEmpty()]
        public string TableHeaderStyle
        {
            get { return wordTableHeaderStyle; }
            set { wordTableHeaderStyle = value; }
        }

        [Parameter(HelpMessage = "The style to use for the table rows")]
        [ValidateNotNullOrEmpty()]
        public string TableRowStyle
        {
            get { return wordTableRowStyle; }
            set { wordTableRowStyle = value; }
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

            if (!(GetOfficeVersion.GetVersion("Word")).IsInstalled)
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Excel is not installed use Out-OWord instead."), "1", ErrorCategory.InvalidOperation, null));


            if (parPath.StartsWith(@".\"))
                parPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, parPath.Substring(2)); 
             
            parPath = System.IO.Path.GetFullPath(parPath);

            WriteVerbose(string.Format("Checking for existance of file {0}",parPath));
            WriteProgress(new ProgressRecord(10, "Out-Word", "Initializing.."));

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

            WriteVerbose("Starting Microsoft Word");
            msWord = BindingFactory.CreateAutomationBinding("Word.Application");

            if (parDisplay)
            {
                WriteVerbose("Display have been set, making application visible");
                msWord.Property("Visible").Set(true);
            }

            WriteProgress(new ProgressRecord(10, "Out-Word", "Creating document.."));
            try
            {
                WriteVerbose("Creating new document");
                wordDoc = msWord.Property("Documents").Get().Method("Add").AddParameter(Template).AddParameter(false).AddParameter(0).AddParameter(true).Invoke();
            }
            catch
            {
                msWord.Method("Quit").AddParameter(0).Invoke();
                WriteVerbose("Document creation failed (error in template most likely)");
                ThrowTerminatingError(new ErrorRecord(new System.IO.IOException("Template not valid/found"), "101", ErrorCategory.ResourceUnavailable, parTemplate));
            }

            //WriteVerbose("Activating document");
            //msWord.Method("Activate").Invoke();
            

            if (wordTitleStyle == string.Empty)
            {
                wordTitleStyle = wordDoc.Property("Styles").Get().Index(-63).Get().Property("NameLocal").Get<string>();
                //wordTitleStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleTitle].NameLocal;
                WriteVerbose(string.Format("Default localized TITLE style is: {0}", wordTitleStyle));
            }
            if (wordHeaderStyle == string.Empty)
            {
                //wordHeaderStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleNormal].NameLocal;
                wordHeaderStyle = wordDoc.Property("Styles").Get().Index(-1).Get().Property("NameLocal").Get<string>();
                WriteVerbose(string.Format("Default localized HEADER style is: {0}", wordHeaderStyle));
            }
            if (wordFooterStyle == string.Empty)
            {
                //wordFooterStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleQuote].NameLocal;
                wordFooterStyle = wordDoc.Property("Styles").Get().Index(-181).Get().Property("NameLocal").Get<string>();
                WriteVerbose(string.Format("Default localized FOOTER style is: {0}", wordFooterStyle));
            }
            if (wordTableStyle == string.Empty)
            {
                //wordTableStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleTableMediumList1Accent1].NameLocal;
                wordTableStyle = wordDoc.Property("Styles").Get().Index(-178).Get().Property("NameLocal").Get<string>();
                WriteVerbose(string.Format("Default localized TABLE style is: {0}", wordTableStyle));
            }
            if (wordTableHeaderStyle == string.Empty)
            {
                //wordTableHeaderStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleIntenseEmphasis].NameLocal;
                wordTableHeaderStyle = wordDoc.Property("Styles").Get().Index(-262).Get().Property("NameLocal").Get<string>();
                WriteVerbose(string.Format("Default localized TABLEHEADER style is: {0}", wordTableHeaderStyle));
            }
            if (wordTableRowStyle == string.Empty)
            {
                //wordTableRowStyle = wordDoc.Styles[WdBuiltinStyle.wdStyleEmphasis].NameLocal;
                wordTableRowStyle = wordDoc.Property("Styles").Get().Index(-89).Get().Property("NameLocal").Get<string>();
                WriteVerbose(string.Format("Default localized TABLEROW style is: {0}", wordTableRowStyle));
            }

            currentRange = msWord.Property("Selection").Get().Property("Range").Get();

            WriteProgress(new ProgressRecord(10, "Out-Word", "Creating title and headers.."));

            if (parTitle != String.Empty)
            {
                WriteVerbose("Adding title");
                IDynamic newPara = wordDoc.Property("Paragraphs").Get().Method("Add").Invoke();
                try
                {
                    newPara.Property("Style").Set(wordTitleStyle);
                }
                catch
                {
                    WriteWarning(string.Format("The style specified for document title ('{0}') was not found", wordTitleStyle));
                }
                newPara.Property("Range").Get().Property("Text").Set(parTitle);
                newPara.Property("Range").Get().Method("InsertParagraphAfter").Invoke();
                currentRange = newPara.Property("Range").Get();
            }

            if (parHeader != String.Empty)
            {
                WriteVerbose("Adding header");
                IDynamic newPara = wordDoc.Property("Paragraphs").Get().Method("Add").Invoke();
                try
                {
                    newPara.Property("Style").Set(wordHeaderStyle);
                }
                catch
                {
                    WriteWarning(string.Format("The style specified for document header ('{0}') was not found", wordHeaderStyle));
                }
                newPara.Property("Range").Get().Property("Text").Set(parHeader);
                newPara.Property("Range").Get().Method("InsertParagraphAfter").Invoke();
                currentRange = newPara.Property("Range").Get();
            }


        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (parObject == null)
            {
                WriteVerbose("A null object found in the pipe, ignoring it.");
            }
            else
            {
                if (parObject.GetType() != lastObject)
                {
                    WriteVerbose("New object type detected");
                    WriteProgress(new ProgressRecord(10, "Out-Word", "Creating new table.."));

                    lastObject = parObject.GetType();

                    int columns = 0;
                    foreach (PSMemberInfo property in parObject.Properties)
                        columns++;

                    if (columns > 63)
                    {
                        wordDoc.Method("Close").AddParameter(0).Invoke();
                        msWord.Method("Quit").AddParameter(0).Invoke();
                        WriteVerbose("To many properties detected on the object");
                        ThrowTerminatingError(new ErrorRecord(new IndexOutOfRangeException("One or more objects exceeds 63 properties which is maximum in Word"), "200", ErrorCategory.InvalidData, parObject));
                    }
                    else
                    {
                        WriteVerbose(string.Format("Adding header (#{0})", headerCount++));
                        WriteProgress(new ProgressRecord(10, "Out-Word", string.Format("Adding header (#{0})", headerCount)));

                        currentTable = wordDoc.Property("Tables").Get().Method("Add").AddParameter(currentRange.InstanceObject).AddParameter(1).AddParameter(columns).Invoke();
                        currentRange = currentTable.Property("Range").Get();

                        int headerCounter = 1;
                        foreach (PSMemberInfo property in parObject.Properties)
                        {
                            IDynamic cell = currentTable.Method("Cell").AddParameter(1).AddParameter(headerCounter).Invoke().Property("Range").Get();
                            try
                            {
                                cell.Property("Style").Set(wordTableHeaderStyle);
                            }
                            catch
                            {
                                WriteWarning(string.Format("The style specified for table header rows ('{0}') was not found", wordTableHeaderStyle));
                            }
                            cell.Method("InsertAfter").AddParameter(property.Name).Invoke();
                            headerCounter++;
                        }
                    }

                }

                WriteVerbose(string.Format("Adding object row (#{0})", rowCount++));
                WriteProgress(new ProgressRecord(10, "Out-Word", string.Format("Adding row (#{0})", rowCount)));
                currentTable.Property("Rows").Get().Method("Add").Invoke();

                int colCounter = 1;
                foreach (PSMemberInfo property in parObject.Properties)
                {

                    IDynamic cell = currentTable.Method("Cell").AddParameter(rowCount+1).AddParameter(colCounter).Invoke().Property("Range").Get();

                    try
                    {
                        cell.Property("Style").Set(wordTableRowStyle);
                    }
                    catch
                    {
                        WriteWarning(string.Format("The style specified for table rows ('{0}') was not found", wordTableRowStyle));
                    }

                    try
                    {
                        cell.Method("InsertAfter").AddParameter(property.Value.ToString()).Invoke();
                    }
                    catch
                    {
                        //this.WriteError(new ErrorRecord(new NullReferenceException(string.Format("Property ({0}) contains null value",property.Name)),"900",ErrorCategory.InvalidData,parObject));
                    }
                    colCounter++;
                }


            }


        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();

            WriteProgress(new ProgressRecord(10, "Out-Word", "Creating footers.."));
            if (parFooter != string.Empty)
            {
                WriteVerbose("Adding footer");
                IDynamic newPara = wordDoc.Property("Paragraphs").Get().Method("Add").Invoke();
                try
                {
                    newPara.Property("Style").Set(wordFooterStyle);
                }
                catch
                {
                    WriteWarning(string.Format("The style specified for document footer ('{0}') was not found", wordFooterStyle));
                }
                newPara.Property("Range").Get().Property("Text").Set("");
                newPara.Property("Range").Get().Method("InsertParagraphAfter").Invoke();
                currentRange = newPara.Property("Range").Get();
                try
                {
                    newPara.Property("Style").Set(wordFooterStyle);
                }
                catch
                {
                    WriteWarning(string.Format("The style specified for document footer ('{0}') was not found", wordFooterStyle));
                }

                newPara.Property("Range").Get().Property("Text").Set(parFooter);
                newPara.Property("Range").Get().Method("InsertParagraphAfter").Invoke();
                currentRange = newPara.Property("Range").Get();
            }

            WriteVerbose("Setting table styles");
            WriteProgress(new ProgressRecord(10, "Out-Word", "Formatting tables.."));

            IDynamic tables = wordDoc.Property("Tables").Get();
            int tableCount = tables.Property("Count").Get<int>();

            for(int i=1;(i-1)<tableCount; i++)
            {
                IDynamic table = tables.Index(i).Get();
                try
                {
                    table.Property("Style").Set(wordTableStyle);
                }
                catch
                {
                    WriteWarning(string.Format("The style specified for tables ('{0}') was not found", wordTableStyle));
                }

                table.Property("Columns").Get().Method("AutoFit").Invoke();
            }



            WriteProgress(new ProgressRecord(10, "Out-Word", "Saving document.."));
            int format = 0;
            switch (parFormat.ToLower())
            {
                case "word": format = 12; break;
                case "word97": format = 0; break;
                case "pdf": format = 17; break;
                case "xps": format = 18; break;
                case "odf": format = 23; break;
                case "rtf": format = 6; break;
            }

            WriteVerbose(string.Format("Saving document in {0}", parFormat));

            try
            {
                wordDoc.Method("SaveAs").AddRefParameter(parPath).AddRefParameter(format).Invoke();
            }
            catch(Exception ex)
            {

                WriteVerbose("Save failed, see error log for details");

                if (parNoExit)
                {
                    WriteVerbose("-NoExit specified, making word visible if not already");
                    msWord.Property("Visible").Set(true);
                }
                else
                {
                    WriteVerbose("-NoExit not specified, quitting word");
                    wordDoc.Method("Close").AddParameter(0).Invoke();
                    msWord.Method("Quit").AddParameter(0).Invoke();
                }

                
                ThrowTerminatingError(new ErrorRecord(ex, "300", ErrorCategory.FromStdErr, parPath));
            }

            if (parNoExit)
            {
                WriteVerbose("-NoExit specified, making word visible if not already");
                msWord.Property("Visible").Set(true);
            }
            else
            {
                WriteVerbose("-NoExit not specified, quitting word");
                wordDoc.Method("Close").AddParameter(0).Invoke();
                msWord.Method("Quit").AddParameter(0).Invoke();
            }

            WriteProgress(new ProgressRecord(10, "Out-Word", "Done"));
            WriteVerbose("Done");

            WriteObject(new FileInfo(parPath));

        }
        #endregion
    }
}
