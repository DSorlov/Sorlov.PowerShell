using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Windows.Forms;
using System.ComponentModel;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Show, "Balloon")]
    [CmdletDescription("¨Shows a tray icon and a message balloon",
        "Perfect for displaying information from background scripts.")]
    [Example(Code = "Show-Balloon $Message", Remarks = "Shows a message containing $Message in the taskbar of the computer")]
    [Example(Code = "Show-Balloon $Message $Title Warning", Remarks = "Shows the $Message and the title $Title using the warning icon")]
    public class ShowBalloon: PSCmdlet
    {
        #region "Private Parameters"
        private string name = "SPSNotifyIcon_Default";
        string message = string.Empty;
        string title = "Windows PowerShell";
        ToolTipIcon icon = ToolTipIcon.None;
        int duration = 10000;
        public static NotifyIcon notifyIcon;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,Mandatory=true,HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        [Parameter(Position = 1, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        [Parameter(Position = 3, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        [ValidateSet("Info","Warning","None","Error")]
        public string Icon
        {
            get { return icon.ToString(); }
            set { icon = (ToolTipIcon)Enum.Parse(typeof(ToolTipIcon),string.Concat(value.Substring(0, 1).ToUpper(), value.Substring(1).ToLower())); }
        }
        [Parameter(Position = 4, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        [ValidateRange(1,60)]
        public int Duration
        {
            get { return duration/1000; }
            set { duration = value*1000; }
        }
        [Parameter(Position = 5, HelpMessage = "The host or email to check")]
        [ValidateNotNullOrEmpty()]
        public string NotifyIconName
        {
            get { return name; }
            set { name = string.Format("SPSNotifyIcon_{0}", value); }
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
            }
            else
            {
                container = new System.ComponentModel.Container();
                notifyIcon = new NotifyIcon(container);
                string path = System.Diagnostics.Process.GetCurrentProcess().Modules[0].FileName;
                notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                notifyIcon.Visible = true;

                SessionState.PSVariable.Set(name, notifyIcon);
            }


               
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(duration);
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
