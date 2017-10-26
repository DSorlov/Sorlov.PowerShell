using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Sorlov.PowerShell.SelfHosted.Dto;
using Sorlov.PowerShell.SelfHosted.Lib.Application;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "SelfHostedPS",DefaultParameterSetName = "Default")]
    [CmdletDescription("Exports a PS1 to a EXE file",
        "This commandlet creates a self-hosted PS1 file as a EXE file")]
    [OutputType(typeof(SelfHostedPSInfo))]
    public class NewSelfHostedPS : PSCmdlet
    {

        #region "Private Parameters"

        private ThreadMode threadMode = ThreadMode.MTA;
        private string frameworkVersion = string.Empty;
        private string outputName = string.Empty;
        private string iconPath = string.Empty;
        private string sourceFile = string.Empty;
        private bool debug = false;
        private string version = "1.0.0.0";
        private bool hideConsole = false;
        private bool embedCore = false;
        private PSModuleInfo[] embedModules = null;
        private bool makeService = false;
        private string serviceName = string.Empty;
        private string serviceDesc = string.Empty;
        private string serviceDisplay = string.Empty;
        private Platform exePlatform = Platform.anycpu;
        private bool signFile = false;
        private X509Certificate2 signCert = null;
        private string signUrl = null;
        private int lcid = 0;
        private string ownerOrg;
        private string ownerName;
        private bool noClobber = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name and path to process", ValueFromPipelineByPropertyName=true, ParameterSetName = "Default")]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name and path to process", ValueFromPipelineByPropertyName = true, ParameterSetName = "Service")]
        [ValidateNotNullOrEmpty()]
        public string SourceFile
        {
            get { return sourceFile; }
            set { sourceFile = value; }
        }
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The name and path to output", ValueFromPipelineByPropertyName = true, ParameterSetName = "Default")]
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The name and path to output", ValueFromPipelineByPropertyName = true, ParameterSetName = "Service")]
        [ValidateNotNullOrEmpty()]
        public string DestinationFile
        {
            get { return outputName; }
            set { outputName = value; }
        }
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The icon to embed into the application", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The icon to embed into the application", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string IconPath
        {
            get { return iconPath; }
            set { iconPath = value; }
        }
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Do not show the console when running", ParameterSetName = "Default")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter HideConsole
        {
            get { return hideConsole; }
            set { hideConsole = value; }
        }
        [Parameter(Position = 3, Mandatory = true, HelpMessage = "The name of the service", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }
        [Parameter(Position = 4, Mandatory = true, HelpMessage = "The description for the service", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string ServiceDescription
        {
            get { return serviceDesc; }
            set { serviceDesc = value; }
        }
        [Parameter(Position = 5, Mandatory = true, HelpMessage = "The description for the service", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string ServiceDisplayName
        {
            get { return serviceDisplay; }
            set { serviceDisplay = value; }
        }
        [Parameter(Position = 6, Mandatory = true, HelpMessage = "Create a service instead of a standalone exe", ParameterSetName = "Service")]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Service
        {
            get { return makeService; }
            set { makeService = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Force framework version", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Force framework version", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("2.0", "3.5", "4.0")]
        public string FrameworkVersion
        {
            get { return frameworkVersion; }
            set { frameworkVersion = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "The LCID for the app", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "The LCID for the app", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int LCID
        {
            get { return lcid; }
            set { lcid = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Thread mode of application", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Thread mode of application", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public ThreadMode ThreadMode
        {
            get { return threadMode; }
            set { threadMode = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Target plattform", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Target plattform", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public Platform Platform
        {
            get { return exePlatform; }
            set { exePlatform = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Build debug files", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Build debug files", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter BuildDebug
        {
            get { return debug; }
            set { debug = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Version number for generated assembly", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Version number for generated assembly", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidatePattern("\\d*.\\d*.\\d*.\\d*")]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Embed core dll in output file", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Embed core dll in output file", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter EmbedCore
        {
            get { return embedCore; }
            set { embedCore = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Modules to embed", ParameterSetName = "Default")]
        [Parameter(Mandatory = false, HelpMessage = "Modules to embed", ParameterSetName = "Service")]
        [ValidateNotNullOrEmpty()]
        public PSModuleInfo[] EmbedModules
        {
            get { return embedModules; }
            set { embedModules = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Should the file be signed", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Should the file be signed", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter Sign
        {
            get { return signFile; }
            set { signFile = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "Manually specify a certificate file", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Manually specify a certificate file", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public X509Certificate2 Certificate
        {
            get { return signCert; }
            set { signCert = value; }
        }
        [Parameter(Mandatory = false, HelpMessage = "The timestamping URL", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "The timestamping URL", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        [ValidatePattern(@"(http(?:s)?\:\/\/[a-zA-Z0-9\-]+(?:\.[a-zA-Z0-9\-]+)*\.[a-zA-Z]{2,6}(?:\/?|(?:\/[\w\-]+)*)(?:\/?|\/\w+\.[a-zA-Z]{2,4}(?:\?[\w]+\=[\w\-]+)?)?(?:\&[\w]+\=[\w\-]+)*)")]
        public string TimestampURL
        {
            get { return signUrl; }
            set { signUrl = value; }
        }

        [Parameter(Mandatory = false, HelpMessage = "Do not overwrite", ParameterSetName = "Default", ValueFromPipelineByPropertyName = true)]
        [Parameter(Mandatory = false, HelpMessage = "Do not overwrite", ParameterSetName = "Service", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter NoClobber
        {
            get { return noClobber; }
            set { noClobber = value; }
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
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (sourceFile.StartsWith(@".\"))
                sourceFile = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, sourceFile.Substring(2));
            else
                sourceFile = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, sourceFile);

            sourceFile = System.IO.Path.GetFullPath(sourceFile);

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
                    if (noClobber)
                    {
                        WriteError(new ErrorRecord(new IOException(string.Format("File {0} already exists", outputName)), "100", ErrorCategory.OpenError, outputName));
                        return;                                       
                    }
                    else
                    {
                        File.Delete(outputName);
                    }
                }
            }
            else
            {
                string destPath = System.IO.Path.GetDirectoryName(sourceFile);
                string scriptName = System.IO.Path.GetFileNameWithoutExtension(sourceFile);
                outputName = System.IO.Path.Combine(destPath, string.Format("{0}.exe", scriptName));

                if (System.IO.File.Exists(outputName))
                {
                    if (noClobber)
                    {
                        WriteError(new ErrorRecord(new IOException(string.Format("File {0} already exists", outputName)), "100", ErrorCategory.OpenError, outputName));
                        return;
                    }
                    else
                    {
                        File.Delete(outputName);
                    }
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
                string tempIcon = sourceFile.ToLower().Replace(".ps1", ".ico");
                if (System.IO.File.Exists(tempIcon)) iconPath = tempIcon;
            }

            if (frameworkVersion == string.Empty)
            {
                frameworkVersion = Environment.Version.ToString().Substring(0, 3);
            }


            ApplicationData appData;
            if (makeService)
            {
                appData = new ServiceData();
                ((ServiceData)appData).Description = serviceDesc;
                ((ServiceData)appData).ServiceName = serviceName;
                ((ServiceData)appData).DisplayName = serviceDisplay;
            }
            else
            {
                appData = new ApplicationData();
            }

            appData.DebugBuild = debug;
            appData.EmbedCore = embedCore;
            appData.EmbedModules = embedModules;
            appData.Framework = EnumExtensions.FromString<Framework>(string.Format("Framework{0}", frameworkVersion.Replace(".", "")));
            appData.HideConsole = hideConsole;
            if (!string.IsNullOrWhiteSpace(iconPath)) appData.Icon = new Bitmap(iconPath);
            appData.LCID = lcid;
            appData.Mode = threadMode;
            appData.Platform = exePlatform;
            appData.PublisherName = ownerName;
            appData.PublisherOrganization = ownerOrg;
            appData.Version = new Version(version);
            appData.ApplicationName = System.IO.Path.GetFileNameWithoutExtension(sourceFile);

            if (signFile)
            {
                appData.SigningInformation = new SingingInformation();
                appData.SigningInformation.Certificate = signCert;
                appData.SigningInformation.TimestampServer = signUrl;
            }

            WriteObject(Compiler.CompileStandard(this,sourceFile,outputName,appData));




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
