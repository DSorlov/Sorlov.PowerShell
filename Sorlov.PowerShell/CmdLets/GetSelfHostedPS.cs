using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.SelfHosted.Dto;
using Sorlov.PowerShell.SelfHosted.Lib;
using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security.Policy;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "SelfHostedPS")]
    [CmdletDescription("Gets information about a self hosted PS exe file",
        "This cmdlet displays information about a self-hosted file")]
    [OutputType(typeof(SelfHostedPSInfo))]
    public class GetSelfHostedPS : PSCmdlet
    {
        private string path;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name and path to process", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }


        private string GetDLLLocation(string name)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name == name) return a.Location;
            }
            return null;
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (path.StartsWith(@".\"))
                path = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, path.Substring(2));
            else
                path = System.IO.Path.Combine(this.CurrentProviderLocation("FileSystem").ProviderPath, path);

            path = System.IO.Path.GetFullPath(path);

            if (!System.IO.File.Exists(path))
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException(string.Format("File {0} not found", path)), "100", ErrorCategory.OpenError, path));

            

            
            string domainName = "GetSelfHostedPSDomain-" + Guid.NewGuid().ToString().GetHashCode().ToString("x");
            Evidence domainEvidence = new Evidence(AppDomain.CurrentDomain.Evidence);
            AppDomainSetup domainInfo = new AppDomainSetup();
            domainInfo.ApplicationName = domainName;
            domainInfo.ApplicationBase = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            AppDomain subDomain = AppDomain.CreateDomain(domainName, domainEvidence, domainInfo);
            Type type = typeof(ProxyDomain);

            ProxyDomain proxyDomain = (ProxyDomain)subDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

            proxyDomain.LoadAssembly(GetDLLLocation("System"));
            proxyDomain.LoadAssembly(GetDLLLocation("System.Management"));
            proxyDomain.LoadAssembly(GetDLLLocation("System.Management.Automation"));
            proxyDomain.LoadAssembly(Assembly.GetExecutingAssembly().Location);

            WriteObject(proxyDomain.GetAssembly(path));

            AppDomain.Unload(subDomain);
            subDomain = null;

        }

    }
}
