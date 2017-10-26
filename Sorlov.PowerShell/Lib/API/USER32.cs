using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Sorlov.PowerShell.Lib.API
{
    static class USER32
    {
        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        public static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);
        [DllImport("user32.dll")]
        public static extern bool AppendMenu(IntPtr hMenu, MenuFlags uFlags, uint uIDNewItem, string lpNewItem);
        [DllImport("user32.dll")]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);
        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int X, int Y, int width, int height, uint flags);
        [DllImport("User32")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, SWP flags);
        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32")]
        public static extern bool IsIconic(IntPtr window);
        [DllImport("User32")]
        public static extern bool IsZoomed(IntPtr window);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, WM Msg, SC wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, WM Msg, int wParam, int lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("User32", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, WM Msg, IntPtr wParam, IntPtr lParam);



        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect(int left_, int top_, int right_, int bottom_)
            {
                Left = left_;
                Top = top_;
                Right = right_;
                Bottom = bottom_;
            }

            public int Height
            {
                get { return (Bottom - Top) + 1; }
                set { Bottom = (Top + value) - 1; }
            }
            public int Width
            {
                get { return (Right - Left) + 1; }
                set { Right = (Left + value) - 1; }
            }
            public Size Size { get { return new Size(Width, Height); } }

            public Point Location { get { return new Point(Left, Top); } }

            // Handy method for converting to a System.Drawing.Rectangle
            public Rectangle ToRectangle()
            { return Rectangle.FromLTRB(Left, Top, Right, Bottom); }

            public static Rect FromRectangle(Rectangle rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public override int GetHashCode()
            {
                return Left ^ ((Top << 13) | (Top >> 0x13))
                  ^ ((Width << 0x1a) | (Width >> 6))
                  ^ ((Height << 7) | (Height >> 0x19));
            }

            #region Operator overloads

            public static implicit operator Rectangle(Rect rect)
            {
                return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            public static implicit operator Rect(Rectangle rect)
            {
                return new Rect(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            #endregion
        }



        public const UInt32 FLASHW_STOP = 0;
        public const UInt32 FLASHW_CAPTION = 1;
        public const UInt32 FLASHW_TRAY = 2;
        public const UInt32 FLASHW_ALL = 3;
        public const UInt32 FLASHW_TIMER = 4;
        public const UInt32 FLASHW_TIMERNOFG = 12;

        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;
        public static IntPtr HWND_TOP = IntPtr.Zero;
        public const int SWP_SHOWWINDOW = 64; // 0×0040

        public const uint SC_CLOSE = 0xF060;

        internal const UInt32 MF_ENABLED = 0x00000000;
        internal const UInt32 MF_GRAYED = 0x00000001;
        internal const UInt32 MF_DISABLED = 0x00000002;

        internal const UInt32 MF_BYCOMMAND = 0x00000000;
        internal const UInt32 MF_BYPOSITION = 0x00000400;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public Int32 dwTimeout;
        }

        [Flags]
        public enum MenuFlags : uint
        {
            MF_STRING = 0,
            MF_BYPOSITION = 0x400,
            MF_SEPARATOR = 0x800,
            MF_REMOVE = 0x1000,
        }

        public enum SWP : uint
        {
            /// <summary>Retains the current size (ignores width and height parameters).</summary>
            NOSIZE = 0x0001,
            /// <summary>Retains the current positions (ignores the x and y parameters).</summary>
            NOMOVE = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            NOZORDER = 0x0004,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs.</summary>
            NOREDRAW = 0x0008,
            /// <summary>Does not activate the window.</summary>
            NOACTIVATE = 0x0010,
            /// <summary>Applies new frame styles set using the SetWindowLong function. 
            /// Sends a WM_NCCALCSIZE message to the window, 
            /// even if the window's size is not being changed.
            /// </summary>
            FRAMECHANGED = 0x0020,
            /// <summary>Displays the window.</summary>
            SHOWWINDOW = 0x0040,
            /// <summary>Hides the window.</summary>
            HIDEWINDOW = 0x0080,
            /// <summary>Discards the entire contents of the client area. 
            /// If this flag is not specified, the valid contents of the client area 
            /// are saved and copied back into the client area 
            /// after the window is sized or repositioned.
            /// </summary>
            NOCOPYBITS = 0x0100,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            NOOWNERZORDER = 0x0200,
            /// <summary>
            /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            NOSENDCHANGING = 0x0400,
            //      , DRAWFRAME      =FRAMECHANGED
            //      , NOREPOSITION   =NOOWNERZORDER
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            DEFERERASE = 0x2000,
            /// <summary>If the calling thread and the thread that owns the window 
            /// are attached to different input queues, the system posts the request
            /// to the thread that owns the window. Prevents blocking thread execution.
            /// </summary>
            ASYNCWINDOWPOS = 0x4000
        }

        public enum SC : uint
        {
            Minimize = 0xf020,
            Maximize = 0xf030,
            Restore = 0xf120,
            Close = 0xf060

            //Size = 0xf000,
            //Move = 0xf010,
            //NextWindow = 0xf040,
            //PrevWindow   =0xf050,
            //VScroll      =0xf070,
            //HScroll      =0xf080,
            //MouseMenu    =0xf090,
            //KeyMenu      =0xf100,
            //Arrange      =0xf110,
            //TaskList     =0xf130,
            //ScreenSave   =0xf140,
            //hotkey       =0xf150,
            //default      =0xf160,
            //monitorpower =0xf170,
            //contexthelp  =0xf180,
            //separator    =0xf00f
        }

        public enum WM : uint
        {
            LeftButtonDown = 0x201,
            LeftButtonUp = 0x202,
            LeftButtonDoubleClick = 0x203,

            RightButtonDown = 0x204,
            RightButtonUp = 0x205,
            RightButtonDoubleClick = 0x206,

            MiddleButtonDown = 0x207,
            MiddleButtonUp = 0x208,
            MiddleButtonDoubleClick = 0x209,

            XButtonDoubleClick = 0x20D,
            XButtonDown = 0x20B,
            XButtonUp = 0x20C,

            KeyDown = 0x100,
            KeyFirst = 0x100,
            KeyLast = 0x108,
            KeyUp = 0x101,

            NonClientHitTest = 0x084,

            //NCACTIVATE         = 0x086,
            //NCCALCSIZE         = 0x083,
            //NCCREATE           = 0x081,
            //NCDESTROY          = 0x082,
            //NCMOUSEMOVE        = 0x0A0,
            //NCPAINT				= 0x085,

            NonClientLeftButtonDown = 0x0A1,
            NonClientLeftButtonUp = 0x0A2,
            NonClientLeftButtonDoubleClick = 0x0A3,

            NonClientRightButtonDown = 0x0A4,
            NonClientRightButtonUp = 0x0A5,
            NonClientRightButtonDoubleClick = 0x0A6,

            NonClientMiddleButtonDown = 0x0A7,
            NonClientMiddleButtonUp = 0x0A8,
            NonClientMiddleButtonDoubleClick = 0x0A9,

            NonClientXButtonDown = 0x0AB,
            NonClientXButtonUp = 0x0AC,
            NonClientXButtonDoubleClick = 0x0AD,

            Activate = 0x006,
            ActivateApp = 0x01C,
            SysCommand = 0x112,
            GetText = 0x00D,
            GetTextLength = 0x00E,
        }



    }
}
