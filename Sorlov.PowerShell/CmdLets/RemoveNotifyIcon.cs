using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "NotifyIcon")]
    [CmdletDescription("¨Shows a tray icon and a message balloon",
        "Perfect for displaying information from background scripts.")]
    [Example(Code = "Show-Balloon $Message", Remarks = "Shows a message containing $Message in the taskbar of the computer")]
    [Example(Code = "Show-Balloon $Message $Title Warning", Remarks = "Shows the $Message and the title $Title using the warning icon")]
    public class RemoveNotifyIcon: PSCmdlet
    {
        #region "Private Parameters"
        private string name = "SPSNotifyIcon_Default";
        private NotifyIcon notifyIcon;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,Mandatory=false,HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get { return name; }
            set { name = string.Format("SPSNotifyIcon_{0}",value); }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

            try
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.EnableVisualStyles();
            }
            catch
            {
            }


            PSVariable savedIcon = SessionState.PSVariable.Get(name);
            IContainer container;
            if (savedIcon != null)
            {
                notifyIcon = (NotifyIcon)savedIcon.Value;
                notifyIcon.Visible = false;
                container = notifyIcon.Container;
                container.Dispose();
                SessionState.PSVariable.Remove(name);
            }
            else
            {
                WriteError(new ErrorRecord(new IOException("The icon does not exists"), "100", ErrorCategory.InvalidArgument, null));
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
