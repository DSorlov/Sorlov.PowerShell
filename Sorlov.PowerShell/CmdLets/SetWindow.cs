using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Runtime.InteropServices;
using Sorlov.PowerShell.Lib.API;
using System.Drawing;
using Sorlov.PowerShell.Dto;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Set, "Window")]
    [CmdletDescription("Gets the position of a windows form",
        "Returns the current, or selected window, position")]
    [Example(Code = "Get-WindowPosition", Remarks = "Returns the position for current powershell host")]
    [Example(Code = "Get-WindowPosition (Get-Process 'notepad.exe').MainWindowHandle", Remarks = "Returns the window position for notepad")]
    public class SetWindow : PSCmdlet
    {
        #region "Private Parameters"
        IntPtr hWnd = IntPtr.Zero;
        private int x = int.MinValue;
        private int y = int.MinValue;
        private int width = int.MinValue;
        private int height = int.MinValue;
        private int state = -1;
        private int flash = -1;
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

        [Parameter(Position = 1, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int X
        {
            get { return x; }
            set { x = value; }
        }
        [Parameter(Position = 2, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        [Parameter(Position = 3, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        [Parameter(Position = 4, HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        [Parameter(HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public WindowState State
        {
            get { return (WindowState)state; }
            set { state = (int)value; }
        }
        [Parameter(HelpMessage = "The host or email to check", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public WindowFlash Flash
        {
            get { return (WindowFlash)flash; }
            set { flash = (int)value; }
        }



        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");
        }
        #endregion

        public void SetPosition(IntPtr hWnd, int x, int y, int width, int height)
        {
            USER32.SWP param = USER32.SWP.NOACTIVATE;
            if (width + height <= 0)
            {
                param |= USER32.SWP.NOSIZE;
            }

            USER32.SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, USER32.SWP.NOACTIVATE);
        }

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


            if (x != int.MinValue || y != int.MinValue || width != int.MinValue || Height != int.MinValue)
            {
                Rectangle pos = GetWindowRectangle(hWnd);
                SetPosition(hWnd, x != int.MinValue ? x : pos.X, y != int.MinValue ? y : pos.Y, width != int.MinValue ? width : pos.Width, height != int.MinValue ? height : pos.Height);
            }

            if (state != -1)
            {
                switch (state)
                {
                    case (int)WindowState.Maximized:
                        USER32.PostMessage(hWnd, USER32.WM.SysCommand, USER32.SC.Maximize, IntPtr.Zero);
                        break;
                    case (int)WindowState.Minimized:
                        USER32.PostMessage(hWnd, USER32.WM.SysCommand, USER32.SC.Minimize, IntPtr.Zero);
                        break;
                    case (int)WindowState.Normal:
                        USER32.PostMessage(hWnd, USER32.WM.SysCommand, USER32.SC.Restore, IntPtr.Zero);
                        break;
                }
            }

            if (flash != -1)
            {
                USER32.FLASHWINFO fInfo = new USER32.FLASHWINFO();
                switch (flash)
                {
                    case (int)WindowFlash.Stop:
                        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                        fInfo.hwnd = hWnd;
                        fInfo.dwFlags = USER32.FLASHW_STOP;
                        fInfo.uCount = UInt32.MaxValue;
                        fInfo.dwTimeout = 0;
                        USER32.FlashWindowEx(ref fInfo); 
                        break;
                    case (int)WindowFlash.Infinite:
                        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                        fInfo.hwnd = hWnd;
                        fInfo.dwFlags = USER32.FLASHW_ALL;
                        fInfo.uCount = UInt32.MaxValue;
                        fInfo.dwTimeout = 0;
                        USER32.FlashWindowEx(ref fInfo); 
                        break;
                    case (int)WindowFlash.Quick:
                        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                        fInfo.hwnd = hWnd;
                        fInfo.dwFlags = USER32.FLASHW_ALL;
                        fInfo.uCount = 5;
                        fInfo.dwTimeout = 0;
                        USER32.FlashWindowEx(ref fInfo);
                        break;
                    case (int)WindowFlash.Long:
                        fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                        fInfo.hwnd = hWnd;
                        fInfo.dwFlags = USER32.FLASHW_ALL;
                        fInfo.uCount = 15;
                        fInfo.dwTimeout = 0;
                        USER32.FlashWindowEx(ref fInfo);
                        break;
                }
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
