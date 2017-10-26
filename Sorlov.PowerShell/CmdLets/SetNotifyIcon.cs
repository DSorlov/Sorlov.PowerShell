using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Windows.Forms;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "NotifyIcon")]
    [CmdletDescription("¨Shows a tray icon and a message balloon",
        "Perfect for displaying information from background scripts.")]
    [Example(Code = "Show-Balloon $Message", Remarks = "Shows a message containing $Message in the taskbar of the computer")]
    [Example(Code = "Show-Balloon $Message $Title Warning", Remarks = "Shows the $Message and the title $Title using the warning icon")]
    public class SetNotifyIcon: PSCmdlet
    {
        #region "Private Parameters"
        private string name = "SPSNotifyIcon_Default";
        private NotifyIcon notifyIcon;
        private string tooltipText = null;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,Mandatory=false,HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get { return name; }
            set { name = string.Format("SPSNotifyIcon_{0}",value); }
        }
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string TooltipText
        {
            get { return tooltipText; }
            set { tooltipText = value; }
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
            if (savedIcon != null)
            {
                notifyIcon = (NotifyIcon)savedIcon.Value;
                if (tooltipText!=null) notifyIcon.Text = tooltipText;
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
