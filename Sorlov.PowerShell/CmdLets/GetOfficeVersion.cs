using Microsoft.Win32;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using Sorlov.PowerShell.Dto;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "OfficeVersion")]
    [CmdletDescription("Gets the version of office installed",
        "This gets the office version installed")]
    [OutputType(typeof(ApplicationVersion))]
    public class GetOfficeVersion : PSCmdlet
    {
        private bool checkWord = false;
        private bool checkExcel = false;
        private bool checkPower = false;
        private bool checkOnenote = false;
        private bool checkOutlook = false;
        private bool checkVisio = false;


        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Word
        {
            get { return checkWord; }
            set { checkWord = value; }
        }

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Excel
        {
            get { return checkExcel; }
            set { checkExcel = value; }
        }

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter PowerPoint
        {
            get { return checkPower; }
            set { checkPower = value; }
        }

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter OneNote
        {
            get { return checkOnenote; }
            set { checkOnenote = value; }
        }
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Outlook
        {
            get { return checkOutlook; }
            set { checkOutlook = value; }
        }
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The output path/file")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Visio
        {
            get { return checkVisio; }
            set { checkVisio = value; }
        }

        internal static ApplicationVersion GetVersion(string appName)
        {
            string fileToFind = string.Empty;
            switch (appName)
            {
                case "Word":
                    fileToFind = "winword.exe";
                    break;
                case "Excel":
                    fileToFind = "excel.exe";
                    break;
                case "PowerPoint":
                    fileToFind = "powerpnt.exe";
                    break;
                case "OneNote":
                    fileToFind = "onenote.exe";
                    break;
                case "Outlook":
                    fileToFind = "outlook.exe";
                    break;
                case "Visio":
                    fileToFind = "visio.exe";
                    break;
                default:
                    fileToFind = "winword.exe";
                    break;
            }

            try
            {
                RegistryKey filePathKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\App Paths\" + fileToFind);
                string fileName = filePathKey.GetValue(string.Empty).ToString();

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fileName);
                return new ApplicationVersion(versionInfo, appName);
            }
            catch
            {
                return new ApplicationVersion(null, appName);
            }
        }

        private bool CheckAll()
        {
            if (!checkExcel && !checkOnenote && !checkOutlook && !checkPower && !checkVisio && !checkWord) return true; else return false;
        }

        protected override void BeginProcessing()
        {
            List<ApplicationVersion> result = new List<ApplicationVersion>();

            if (CheckAll() || checkWord) result.Add(GetVersion("Word"));
            if (CheckAll() || checkExcel) result.Add(GetVersion("Excel"));
            if (CheckAll() || checkPower) result.Add(GetVersion("PowerPoint"));
            if (CheckAll() || checkOnenote) result.Add(GetVersion("OneNote"));
            if (CheckAll() || checkOutlook) result.Add(GetVersion("Outlook"));
            if (CheckAll() || checkVisio) result.Add(GetVersion("Visio"));

            foreach(ApplicationVersion version in result)
                WriteObject(version);

        }
    }
}
