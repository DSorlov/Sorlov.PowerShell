using System;
using System.Management.Automation;
using System.Net;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Dto;
using Sorlov.PowerShell.Dto.Network;

namespace Sorlov.PowerShell.Cmdlets
{


    [Cmdlet(VerbsCommon.Get, "HTTPHeader")]
    [CmdletDescription("Sends a standard http request and returns the headers from the server",
        "This command returns all headers returned by the server.")]
    [Example(Code="Get-HTTPHeader http://www.test.com",Remarks="Downloads headers from the http://www.test.com site.")]
    public class GetHTTPHeader: PSCmdlet
    {
        #region "Private Parameters"
        string url = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WebClient client = new WebClient();

            if (Url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
            {

                try
                {
                    client.DownloadData(Url);
                    for (int i = 0; i < client.ResponseHeaders.Count; i++)
                    {
                        WriteObject(new HTTPHeaderResult(client.ResponseHeaders.GetKey(i), client.ResponseHeaders.Get(i)));
                    }
                }
                catch(Exception ex)
                {
                    WriteError(new ErrorRecord(ex,"100",ErrorCategory.InvalidResult,Url));
                }

            }
            else
            {
                WriteError(new ErrorRecord(new ArgumentException("The url was not in a valid format"), "200", ErrorCategory.InvalidArgument, Url));
            }
            

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
