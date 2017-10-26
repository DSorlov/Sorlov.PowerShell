using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "NotifyIcon")]
    [CmdletDescription("¨Shows a tray icon and a message balloon",
        "Perfect for displaying information from background scripts.")]
    [Example(Code = "Show-Balloon $Message", Remarks = "Shows a message containing $Message in the taskbar of the computer")]
    [Example(Code = "Show-Balloon $Message $Title Warning", Remarks = "Shows the $Message and the title $Title using the warning icon")]
    public class GetNotifyIcon: PSCmdlet
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

            foreach (PSObject psObject in SessionState.InvokeCommand.InvokeScript("Get-Variable SPSNotifyIcon_*"))
            {
                string name = psObject.Properties["Name"].Value as string;

                WriteObject(name.Replace("SPSNotifyIcon_", ""));
            }
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
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
