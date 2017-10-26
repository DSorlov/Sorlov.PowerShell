using System;
using System.Drawing;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Sorlov.PowerShell.SelfHosted.Dto;
using Sorlov.PowerShell.SelfHosted.Lib.Application;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "SelfHostedHTApp")]
    [CmdletDescription("Exports a HTApp to a EXE file",
        "This commandlet creates a self-hosted HTApp file as a EXE file")]
    [OutputType(typeof(SelfHostedPSInfo))]
    public class NewSelfHostedHTApp : PSCmdlet
    {

        #region "Private Parameters"

        private string frameworkVersion = string.Empty;
        private string outputName = string.Empty;
        private string iconPath = string.Empty;
        private string sourceFile = string.Empty;
        private bool debug = false;
        private string version = "1.0.0.0";
        private bool embedCore = false;
        private PSModuleInfo[] embedModules = null;
        private Platform exePlatform = Platform.anycpu;
        private bool signFile = false;
        private X509Certificate2 signCert = null;
        private string signUrl = null;
        private int lcid = 0;
        private string ownerOrg;
        private string ownerName;
        private string[] additionalFiles = new string[]{};
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name and path to process", ValueFromPipelineByPropertyName=true, ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty()]
        public string SourceFile
        {
            get { return sourceFile; }
            set { sourceFile = value; }
        }
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The name and path to output", ValueFromPipelineByPropertyName = true, ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty()]
        public string DestinationFile
        {
            get { return outputName; }
            set { outputName = value; }
        }
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The icon to embed into the application", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string IconPath
        {
            get { return iconPath; }
            set { iconPath = value; }
        }
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Any array of additional files to include", ValueFromPipelineByPropertyName = true, ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty()]
        public string[] AdditionalFiles
        {
            get { return additionalFiles; }
            set { additionalFiles = value; }
        }        
        [Parameter(Mandatory = false, HelpMessage = "Force framework version", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("2.0", "3.5", "4.0")]
        public string FrameworkVersion
        {
            get { return frameworkVersion; }
            set { frameworkVersion = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "The LCID for the app", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int LCID
        {
            get { return lcid; }
            set { lcid = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Target plattform", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public Platform Platform
        {
            get { return exePlatform; }
            set { exePlatform = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Build debug files", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter BuildDebug
        {
            get { return debug; }
            set { debug = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Version number for generated assembly", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidatePattern("\\d*.\\d*.\\d*.\\d*")]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Embed core dll in output file", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter EmbedCore
        {
            get { return embedCore; }
            set { embedCore = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Modules to embed", ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty()]
        public PSModuleInfo[] EmbedModules
        {
            get { return embedModules; }
            set { embedModules = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Should the file be signed", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Sign
        {
            get { return signFile; }
            set { signFile = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Manually specify a certificate file", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public X509Certificate2 Certificate
        {
            get { return signCert; }
            set { signCert = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "The timestamping URL", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidatePattern(@"(http(?:s)?\:\/\/[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,6}(?:\/?|(?:\/[\w\-]+)*)(?:\/?|\/\w+\.[a-zA-Z]{2,4}(?:\?[\w]+\=[\w\-]+)?)?(?:\&[\w]+\=[\w\-]+)*)")]
        public string TimestampURL
        {
            get { return signUrl; }
            set { signUrl = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

            WriteVerbose("Reading registry for metadata..");
            ownerOrg = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion", "RegisteredOrganization", string.Empty);
            ownerName = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion", "RegisteredOwner", string.Empty);

        }
        #endregion


        #region "ProcessRecord"

        private string FixPath(string inPath)
        {
            if (inPath.StartsWith(@".\"))
                inPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, inPath.Substring(2));
            else
                inPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, inPath);

            return System.IO.Path.GetFullPath(inPath);

        }
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            sourceFile = FixPath(sourceFile);

            if (!System.IO.File.Exists(sourceFile))
            {
                WriteError(new ErrorRecord(new FileNotFoundException(string.Format("File {0} not found", sourceFile)), "100", ErrorCategory.OpenError, sourceFile));
                return;               
            }

            if (outputName != string.Empty)
            {
                if (outputName.StartsWith(@".\"))
                    outputName = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, outputName.Substring(2));
                else
                    outputName = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, outputName);

                outputName = System.IO.Path.GetFullPath(outputName);

                if (System.IO.File.Exists(outputName))
                {
                    WriteError(new ErrorRecord(new IOException(string.Format("File {0} already exists", outputName)), "100", ErrorCategory.OpenError, outputName));
                    return;               
                }
            }
            else
            {
                string destPath = System.IO.Path.GetDirectoryName(sourceFile);
                string scriptName = System.IO.Path.GetFileNameWithoutExtension(sourceFile);
                outputName = System.IO.Path.Combine(destPath, string.Format("{0}.exe", scriptName));

                if (System.IO.File.Exists(outputName))
                {
                    WriteError(new ErrorRecord(new IOException(string.Format("File {0} already exists", outputName)), "100", ErrorCategory.OpenError, outputName));
                    return;
                }
            }


            if (iconPath != string.Empty)
            {
                if (iconPath.StartsWith(@".\"))
                    iconPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, iconPath.Substring(2));
                else
                    iconPath = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, iconPath);

                iconPath = System.IO.Path.GetFullPath(iconPath);

                if (!System.IO.File.Exists(iconPath))
                {
                    WriteError(new ErrorRecord(new FileNotFoundException(string.Format("File {0} not found", iconPath)), "100", ErrorCategory.OpenError, iconPath));
                    return;
                }

            }
            else
            {
                string tempIcon = sourceFile.ToLower().Replace(".htapp", ".ico");
                if (System.IO.File.Exists(tempIcon)) iconPath = tempIcon;
            }

            if (frameworkVersion == string.Empty)
            {
                frameworkVersion = Environment.Version.ToString().Substring(0, 3);
            }


            ApplicationData appData = new ApplicationData();

            appData.DebugBuild = debug;
            appData.EmbedCore = embedCore;
            appData.EmbedModules = embedModules;
            appData.Framework = EnumExtensions.FromString<Framework>(string.Format("Framework{0}", frameworkVersion.Replace(".", "")));
            appData.HideConsole = true;
            appData.Icon = new Bitmap(iconPath);
            appData.LCID = lcid;
            appData.Mode  = ThreadMode.STA;
            appData.Platform = exePlatform;
            appData.PublisherName = ownerName;
            appData.PublisherOrganization = ownerOrg;
            appData.Version = new Version(version);

            if (signFile)
            {
                appData.SigningInformation = new SingingInformation();
                appData.SigningInformation.Certificate = signCert;
                appData.SigningInformation.TimestampServer = signUrl;
            }

            string replaceProgram = Resources.Templates.Program;
            replaceProgram = replaceProgram.Replace("$$FILENAME$$", Path.GetFileName(sourceFile));

            appData.ReplaceProgram = replaceProgram;
            appData.AdditionalCode.Add(Resources.Templates.ScriptForm);
            appData.AdditionalCode.Add(Resources.Templates.ScriptForm_Designer);
            appData.AdditionalCode.Add(Resources.Templates.ScriptInterface);
            appData.AdditionalCode.Add(Resources.Templates.CrossThread);
            appData.AdditionalCode.Add(Resources.Templates.StringExtension);

            appData.AdditionalFiles.Add(sourceFile);

            foreach (string additionalFile in additionalFiles)
            {
                string additionalFilePath = FixPath(additionalFile);

                if (!System.IO.File.Exists(additionalFilePath))
                    WriteWarning(string.Format("Additional file {0} not found", additionalFilePath));
                else
                    appData.AdditionalFiles.Add(additionalFilePath);
            }

            appData.AdditionalAssemblies.Add("System.Windows.Forms");
            appData.AdditionalAssemblies.Add("System.Drawing");

            WriteObject(Compiler.CompileAdvanced(this, outputName, appData));

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
