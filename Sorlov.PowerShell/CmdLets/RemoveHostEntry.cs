using System;
using System.Management.Automation;
using System.Security.Principal;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Net;
using Sorlov.PowerShell.Dto.Network;
using Sorlov.PowerShell.Lib.Network;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "HostEntry")]
    [CmdletDescription("This removed a entry from the host file",
        "This cmdlet deletes a specified host or ip from the host file. You must be administrator to use this command")]
    public class RemoveHostEntry : PSCmdlet
    {
        private string hostFile = string.Empty;
        private string removeIP = string.Empty;
        private string removeHostname = string.Empty;

        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string IPAddress
        {
            get { return removeIP; }
            set { removeIP = value; }
        }

        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Hostname
        {
            get { return removeHostname; }
            set { removeHostname = value; }
        }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Hostfile
        {
            get { return hostFile; }
            set { hostFile = value; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            if (hostFile == string.Empty)
                hostFile = Path.Combine(Environment.SystemDirectory, "drivers\\etc\\hosts");

            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ThrowTerminatingError(new ErrorRecord(new UnauthorizedAccessException("You must have Administrative permissions to run this command"), "100", ErrorCategory.PermissionDenied, null));
            }
            else
            {
                HostEntryCollection entryCollection = HostFileHandler.GetHostEntryColletion(hostFile);
                
                if (removeIP == string.Empty && removeHostname == string.Empty)
                        ThrowTerminatingError(new ErrorRecord(new ArgumentException("You must supply either IP or host to remove"),"200",ErrorCategory.InvalidArgument,null));

                if (removeIP != string.Empty)
                {
                    IPAddress removeAddress = System.Net.IPAddress.Parse(removeIP);
                    HostEntry hostEntry = entryCollection[removeAddress];
                    if (hostEntry!=null) entryCollection.Remove(hostEntry);
                }

                if (removeHostname != string.Empty)
                {
                    HostEntry hostEntry = entryCollection[removeHostname];
                    if (hostEntry != null) hostEntry.RemoveHostname(removeHostname);

                    if (hostEntry.Count == 0)
                        entryCollection.Remove(hostEntry);
                }

                HostFileHandler.PersistHostEntryCollection(hostFile, entryCollection);
            }

        }



    }
}
