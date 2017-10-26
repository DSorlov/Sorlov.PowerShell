using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Network;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "HostEntry")]
    [CmdletDescription("Gets the current hostfile config",
        "This cmdlet returns the current hostfile if you are logged in as a administrator.")]
    public class GetHostEntry: PSCmdlet
    {
        private string hostFile = string.Empty;

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Hostfile
        {
            get { return hostFile; }
            set { hostFile = value; }
        }

        protected override void BeginProcessing()
        {           
            WriteObject(HostFileHandler.GetHostEntryColletion(hostFile));   
        }



    }
}
