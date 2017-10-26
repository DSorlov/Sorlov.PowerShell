using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using Sorlov.PowerShell.Lib.API;
using System.Drawing;
using Sorlov.PowerShell.Dto;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Window")]
    [CmdletDescription("Gets the position of a windows form",
        "Returns the current, or selected window, position")]
    [Example(Code = "Get-WindowPosition", Remarks = "Returns the position for current powershell host")]
    [Example(Code = "Get-WindowPosition (Get-Process 'notepad.exe').MainWindowHandle", Remarks = "Returns the window position for notepad")]
    public class GetWindow : PSCmdlet
    {
        #region "Private Parameters"
        IntPtr hWnd = IntPtr.Zero;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

        internal static Rectangle GetWindowRectangle(IntPtr hWnd)
        {
            USER32.Rect rect = new USER32.Rect();
            if (USER32.GetWindowRect(hWnd, ref rect))
            {
                return rect.ToRectangle();
            }
            else
            {
                return new Rectangle();
            }
        }


        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            if (hWnd == IntPtr.Zero)
                hWnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;


            WindowInformation result = new WindowInformation();

            Rectangle rect = GetWindowRectangle(hWnd);
            result.X = rect.X;
            result.Y = rect.Y;
            result.Height = rect.Height;
            result.Width = rect.Width;

            if (USER32.IsIconic(hWnd))
                result.State = WindowState.Minimized;
            else if (USER32.IsZoomed(hWnd))
                result.State = WindowState.Maximized;


            WriteObject(result);
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
