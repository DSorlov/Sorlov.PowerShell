using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Disable, "WindowControlBox")]
    [CmdletDescription("Disables the controlbox on the window",
        "Disables the controlbox on the current, or other specified hWnd, window.")]
    [Example(Code = "Disable-WindowControlBox", Remarks = "Disables the controlbox on the current window")]
    [Example(Code = "Disable-WindowControlBox (Get-Process 'notepad.exe').MainWindowHandle", Remarks = "Disables the controlbox on the notepad window")]
    [RelatedCmdlets(typeof(EnableWindowControlBox))]
    public class DisableWindowControlBox : PSCmdlet
    {
        #region "Private Parameters"
        IntPtr hWnd = IntPtr.Zero;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, HelpMessage = "The host or email to check", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        [Alias("hWnd","Window")]
        public IntPtr WindowHandle
        {
            get { return hWnd; }
            set { hWnd = value; }
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
            if (hWnd == IntPtr.Zero)
                hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            IntPtr hMenu = USER32.GetSystemMenu(hWnd, false);
            USER32.EnableMenuItem(hMenu, USER32.SC_CLOSE, USER32.MF_GRAYED); // Disables controlbox
            USER32.RemoveMenu(hMenu, USER32.SC_CLOSE, USER32.MF_BYCOMMAND); // Disables the exit menu
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
