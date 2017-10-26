using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "NotifyIcon")]
    [CmdletDescription("¨Shows a tray icon and a message balloon",
        "Perfect for displaying information from background scripts.")]
    [Example(Code = "Show-Balloon $Message", Remarks = "Shows a message containing $Message in the taskbar of the computer")]
    [Example(Code = "Show-Balloon $Message $Title Warning", Remarks = "Shows the $Message and the title $Title using the warning icon")]
    public class NewNotifyIcon: PSCmdlet
    {
        #region "Private Parameters"
        private string name = "SPSNotifyIcon_Default";
        private NotifyIcon notifyIcon;
        private string toolTip = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,Mandatory=false,HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get { return name; }
            set { name = string.Format("SPSNotifyIcon_{0}",value); }
        }
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string TooltipText
        {
            get { return toolTip; }
            set { toolTip = value; }
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
                WriteError(new ErrorRecord(new IOException("The icon already exists"), "100", ErrorCategory.InvalidArgument, null));
            }
            else
            {
                container = new System.ComponentModel.Container();
                notifyIcon = new NotifyIcon(container);
                string path = System.Diagnostics.Process.GetCurrentProcess().Modules[0].FileName;
                notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                notifyIcon.Visible = true;
                if (toolTip != string.Empty) notifyIcon.Text = toolTip;
                SessionState.PSVariable.Set(name, notifyIcon);
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
