using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "Alias", SupportsShouldProcess = true)]
    [CmdletDescription("Removes a defined alias")]
    public class RemoveAlias: PSCmdlet
    {
        #region "Private Parameters"
        string parFile = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The assembly to load",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string AliasName
        {
            get { return parFile; }
            set { parFile = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteObject(SessionState.InvokeCommand.InvokeScript(string.Format("Remove-Item alias:{0}",parFile)));
        
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Format("Touching {0}", parFile));
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
