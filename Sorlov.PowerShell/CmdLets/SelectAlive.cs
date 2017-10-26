using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Select, "Alive")]
    [CmdletDescription("Selects all machines that are responding and sends on in the pipeline",
        "This cmdlet filters a list of computer objects and removes those which do not respond to ping.")]
    public class SelectAlive : PSCmdlet
    {

        #region "Private Parameters"
        object[] inputObject;
        #endregion

        #region "Public Parameters"
        [Parameter(Mandatory = true, HelpMessage = "The object to check if alive",ValueFromPipeline=true)]
        [Alias("Input")]
        [ValidateNotNullOrEmpty()]
        public object[] InputObject
        {
            get { return inputObject; }
            set { inputObject = value; }
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
            base.ProcessRecord();

            foreach (object currentObject in InputObject)
            {

                string host = string.Empty;
                switch (((PSObject) currentObject).BaseObject.GetType().Name)
                {
                    case "DirectoryEntry":
                        WriteVerbose("DirectoryEntry object found, will process..");
                        host = ((System.DirectoryServices.DirectoryEntry) currentObject).Properties["dnshostname"][0].ToString();
                        break;
                    case "SearchResult":
                        WriteVerbose("SearchResult object found, will process..");
                        host = ((System.DirectoryServices.SearchResult) currentObject).Properties["dnshostname"][0].ToString();
                        break;
                    case "IPHostEntry":
                        WriteVerbose("IPHostEntry object found, will process..");
                        host = ((System.Net.IPHostEntry) currentObject).HostName;
                        break;
                    case "PSCustomObject":
                    case "PSObject":
                        WriteVerbose("PSObject/PSCustomObject object found, will process..");
                        host = ((PSObject) currentObject).Properties["Name"].Value.ToString();
                        break;
                    case "String":
                        WriteVerbose("String object found, will process..");
                        host = ((string) currentObject).Trim();
                        break;
                    default:
                        WriteVerbose(string.Format("Unknown object type ({0}), will not process..", ((PSObject) currentObject).BaseObject.GetType().Name));
                        break;
                }

                if (host != string.Empty)
                {
                    WriteVerbose(string.Format("Sending ping to {0}", host));

                    System.Net.NetworkInformation.Ping pinger = new System.Net.NetworkInformation.Ping();
                    System.Net.NetworkInformation.PingReply reply = pinger.Send(host);

                    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    {
                        WriteVerbose(string.Format("Ping reply successfull at {0} ms", reply.RoundtripTime));
                        WriteObject(inputObject);
                    }
                    else
                    {
                        WriteVerbose(string.Format("Ping failed: {0}", reply.Status));
                    }
                }
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
