using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Enable, "WindowControlBox")]
    [CmdletDescription("Enables the controlbox on the window",
        "Enables the controlbox on the current, or other specified hWnd, window.")]
    [Example(Code="Enable-WindowControlBox",Remarks="Enables the controlbox on the current window")]
    [Example(Code="Enable-WindowControlBox (Get-Process 'notepad.exe').MainWindowHandle", Remarks="Enables the controlbox on the notepad window")]
    [RelatedCmdlets(typeof(DisableWindowControlBox))]
    public class EnableWindowControlBox : PSCmdlet
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
            USER32.EnableMenuItem(hMenu, USER32.SC_CLOSE, USER32.MF_ENABLED); //Enables controlbox
            USER32.AppendMenu(hMenu, USER32.MenuFlags.MF_STRING, USER32.SC_CLOSE, "Exit"); // Enables the exit menu
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
