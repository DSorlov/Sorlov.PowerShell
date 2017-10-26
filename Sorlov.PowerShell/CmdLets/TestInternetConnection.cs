using Sorlov.PowerShell.Lib.API;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System;
using System.Management.Automation;
using System.Net;
using Heijden.DNS;

namespace Sorlov.PowerShell.Cmdlets
{

    
    [Cmdlet(VerbsDiagnostic.Test, "InternetConnection")]
    [CmdletDescription("Checks for a valid internet connection",
        "This cmdlet returns the status from a internet connection")]
    [OutputType(typeof(bool))]
    public class TestInternetConnection: PSCmdlet
    {
        private LookupMethod method = LookupMethod.API;
        private string dnsServer = "8.8.8.8";
        private string hostName = "www.google.com";

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "Method used to check internet connection.", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public LookupMethod Method
        {
            get { return method; }
            set { method = value; }
        }

        [Parameter(Mandatory = false, HelpMessage = "The hostname to use when checking using DNS or WEB", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string DnsServer
        {
            get { return dnsServer; }
            set { dnsServer = value; }
        }

        [Parameter(Mandatory = false, HelpMessage = "The hostname to use when checking WEB", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Hostname
        {
            get { return hostName; }
            set { hostName = value; }
        }

        protected override void ProcessRecord()
        {
            switch (method)
            {
                case LookupMethod.API:
                    int returnValue = 0;
                    WININET.InternetGetConnectedState(out returnValue, 0);
                    WriteObject(Convert.ToBoolean(returnValue));
                    break;

                case LookupMethod.DNS:
                    Resolver dnsResolver = new Resolver(dnsServer);
                    IPHostEntry dnsResult = dnsResolver.Resolve(hostName);
                    WriteObject(dnsResult.AddressList.Length != 0);
                    break;

                case LookupMethod.WEB:
                    Resolver httpResolver = new Resolver(dnsServer);
                    IPHostEntry httpResult = httpResolver.Resolve(hostName);

                    if (httpResult.AddressList.Length == 0)
                        WriteObject(false);
                    else
                    {
                        try
                        {
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.google.com");
                            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        }
                        catch
                        {
                            WriteObject(false);
                        }
                        WriteObject(true);

                    }

                    break;
            }
        }

        public enum LookupMethod
        {
            API = 0,
            DNS = 1,
            WEB = 2
        }
    }
}
