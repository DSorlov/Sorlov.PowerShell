using System;
using System.Text;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "RandomFile",SupportsShouldProcess=true)]
    [CmdletDescription("Creates a random file for you to use",
        "This command generates a random file in a directory. The file is touched and ready to use. By default the file is created in the System temporary directory.")]
    [Example(Code = "Get-RandomFile", Remarks = "Generates a random file at the system temp directory")]
    [Example(Code = "Get-RandomFile -Path c:\\heyho", Remarks = "Generates a random file in the c:\\heyho directory")]
    [Example(Code = "Get-RandomFile -Pattern '????????.tmp'", Remarks = "Creates a file 8+3 chars att the system directory")]
    [Example(Code = "Get-RandomFile -Pattern 'TEST????.???'", Remarks = "Creates a file 8+3 chars att the system directory, name starting with TEST")]
    [OutputType(typeof(FileInfo))]
    public class GetRandomFile : PSCmdlet
    {
        #region "Private Parameters"
        private string parFile = System.IO.Path.GetTempPath();
        private string parPattern = "????????.tmp";
        private static Random random = new Random();
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The directory to create temp file",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The pattern to use when creating the file",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Pattern
        {
            get { return parPattern; }
            set { parPattern = value; }
        }
        #endregion

        #region "Privates"
        private static char GetRandomChar()
         {
          string allowedChars = "abcdefghijkmnopqrstuvwxyz0123456789";
          return allowedChars[random.Next(0, allowedChars.Length)];
         }       

        private string MakeUniqueFileName()
        {
            StringBuilder tempFile = new StringBuilder();
            foreach(char currentChar in parPattern.ToCharArray())
            {
                if (currentChar == '?')
                    tempFile.Append(GetRandomChar());
                else
                    tempFile.Append(currentChar);

            }
            return tempFile.ToString();
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing, will create some files today");        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {


            string tempFile = System.IO.Path.Combine(parFile, MakeUniqueFileName());

            while (System.IO.File.Exists(tempFile))
            {
                tempFile = System.IO.Path.Combine(parFile, MakeUniqueFileName());
            }

            WriteVerbose(string.Format("Random file will be: {0}", tempFile));
            if (ShouldProcess(tempFile, "Creating a new file"))
                File.Create(tempFile);

            WriteObject(new FileInfo(tempFile));

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
