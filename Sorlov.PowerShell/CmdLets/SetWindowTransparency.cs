using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "WindowsTransparency")]
    [CmdletDescription("Sets transparancy for the current or selected window",
        "Sets transparancy for the current or selected window (Windows Vista & Windows 7 only)")]
    public class SetWindowTransparency : PSCmdlet
    {
        #region "Private Parameters"
        IntPtr hWnd = IntPtr.Zero;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

            if (DWMAPI.DwmIsCompositionEnabled())
                {
                    DWMAPI.DWM_BLURBEHIND bb;
                    bb.dwFlags = (DWMAPI.DWM_BB)DWMAPI.DWM_BB_ENABLE;
                    bb.fEnable = true;
                    bb.fTransitionOnMaximized = true;
                    bb.hRgnBlur = IntPtr.Zero;

                    DWMAPI.DwmEnableBlurBehindWindow(hWnd, ref bb);
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
