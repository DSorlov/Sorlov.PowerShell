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
    [Cmdlet(VerbsCommon.Add, "HostEntry")]
    [CmdletDescription("Add HostEntry to the host file",
        "This cmdlet adds a HostEntry to the host file, you need to be local admin to perform this action.")]
    [Example(Code ="Add-HostEntry 127.0.0.1 myhost.com", Remarks = "Will create a new entry and point myhost.com to 127.0.0.1 ")]
    public class AddHostEntry: PSCmdlet
    {
        private string hostFile = string.Empty;
        private string setIP;
        private string[] setHostnames;
        private string comment;

        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string IPAddress
        {
            get { return setIP; }
            set { setIP = value; }
        }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string[] Hostname
        {
            get { return setHostnames; }
            set { setHostnames = value; }
        }

        [Parameter(Position = 2, Mandatory = false, HelpMessage = "An optional comment for the line in the hostfile", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The hostfile to process", ValueFromPipeline = true)]
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
                hostFile = Path.Combine(Environment.SystemDirectory,"drivers\\etc\\hosts");

            WindowsPrincipal wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            if (!wp.IsInRole(WindowsBuiltInRole.Administrator))
            {
                ThrowTerminatingError(new ErrorRecord(new UnauthorizedAccessException("You must have Administrative permissions to run this command"),"100",ErrorCategory.PermissionDenied,null));
            }
            else
            {
                HostEntryCollection entryCollection = HostFileHandler.GetHostEntryColletion(hostFile);
                IPAddress setAddress = System.Net.IPAddress.Parse(setIP);

                foreach (string newHost in setHostnames)
                {

                    if (entryCollection[newHost] == null)
                    {
                        if (entryCollection[setAddress]==null)
                        {
                            HostEntry newEntry = new HostEntry();
                            newEntry.IPAddress = setAddress;
                            newEntry.AddHostname(newHost);
                            newEntry.Comment = comment;
                            entryCollection.Add(newEntry);
                        }
                        else
                        {
                            entryCollection[setAddress].AddHostname(newHost);
                        }
                    }
                    else
                    {
                        WriteError(new ErrorRecord(new InvalidDataException(string.Format("Host '{0}' already present in the host file and will not be added",newHost)),"200",ErrorCategory.InvalidData,null));
                    }

                }

                HostFileHandler.PersistHostEntryCollection(hostFile, entryCollection);
            }

        } 



    }
}
