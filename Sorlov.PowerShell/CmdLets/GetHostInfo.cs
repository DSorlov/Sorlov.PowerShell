using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Dto.Network;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "HostInfo")]
    [CmdletDescription("Gets the current system hostname",
        "This cmdlet returns the name of the current machine")]
    public class GetHostInfo : PSCmdlet
    {

        #region "Private Parameters"
        #endregion

        #region "Public Parameters"
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
            base.ProcessRecord();

            string envName = Environment.MachineName;
            string dnsName = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] hostAddresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
			
            WriteObject(new HostInfo(envName,dnsName,hostAddresses));
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
