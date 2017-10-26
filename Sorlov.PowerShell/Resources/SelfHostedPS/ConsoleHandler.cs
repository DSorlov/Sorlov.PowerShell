using System;
using System.Management.Automation.Host;
using System.Runtime.InteropServices;

namespace Sorlov.SelfHostedPS
{
    public static class ConsoleHandler
    {
        public class FakeConsoleData
        {
            public string WindowTitle = "SelfHostedPS";
            public Size WindowSize = new Size(80, 40);
            public Size BufferSize = new Size(80, 40);
            public int CursorSize;
            public Coordinates WindowPosition = new Coordinates(0, 0);
            public Coordinates CursorPosition = new Coordinates(0, 0);
            public ConsoleColor ForegroundColor = ConsoleColor.White;
            public ConsoleColor BackgroundColor = ConsoleColor.Black;
        }

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        private static extern short GetKeyState(int keyCode);

        private static FakeConsoleData fakeCon;
        public static FakeConsoleData FakeConsole
        {
            get
            {
                if (fakeCon == null) fakeCon = new FakeConsoleData();
                return fakeCon;
            }
            set
            {
                if (fakeCon == null) fakeCon = new FakeConsoleData();
                fakeCon = value;
            }
        }

        public static bool ConsoleLoaded = false;
        public delegate void ConsoleOutputEventHandler(string data);
        public static event ConsoleOutputEventHandler ConsoleOutputReceived;



        public static void WriteLine(string data)
        {
            if (ConsoleLoaded) Console.WriteLine(data);
            if (ConsoleOutputReceived != null) ConsoleOutputReceived(data);
        }


        public static void Write(string data)
        {
            if (ConsoleLoaded) Console.Write(data);
            if (ConsoleOutputReceived != null) ConsoleOutputReceived(data);
        }

 

        public static KeyInfo ReadKey(ReadKeyOptions options)
        {
            if (ConsoleLoaded)
            {
                bool noecho = false;
                if (options == ReadKeyOptions.NoEcho)
                    noecho = true;

                bool CapsLock = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
                bool NumLock = (((ushort)GetKeyState(0x90)) & 0xffff) != 0;
                bool ScrollLock = (((ushort)GetKeyState(0x91)) & 0xffff) != 0;

                ConsoleKeyInfo conKeyInfo = Console.ReadKey(noecho);

                ControlKeyStates cks = new ControlKeyStates();
                if (conKeyInfo.Modifiers == ConsoleModifiers.Shift) cks = cks | ControlKeyStates.ShiftPressed;
                if (conKeyInfo.Modifiers == ConsoleModifiers.Control) cks = cks | ControlKeyStates.LeftCtrlPressed | ControlKeyStates.RightCtrlPressed;
                if (conKeyInfo.Modifiers == ConsoleModifiers.Alt) cks = cks | ControlKeyStates.LeftAltPressed | ControlKeyStates.RightAltPressed;
                if (CapsLock) cks = cks | ControlKeyStates.CapsLockOn;
                if (NumLock) cks = cks | ControlKeyStates.NumLockOn;
                if (ScrollLock) cks = cks | ControlKeyStates.ScrollLockOn;

                return new KeyInfo(VkKeyScan(conKeyInfo.KeyChar), conKeyInfo.KeyChar, cks, true);
            }
            else
                return new KeyInfo();
        }

        public static void FlushInputBuffer()
        {
        }

        public static void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
        }

        public static void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
        }

        public static BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException();
        }

        public static void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
        }

        public static string ReadLine()
        {
            if (ConsoleLoaded)
                return Console.ReadLine();
            else
                return string.Empty;
        }


        public static ConsoleColor ForegroundColor
        {
            get { if (ConsoleLoaded) return Console.ForegroundColor; else return FakeConsole.ForegroundColor; }
            set { if (ConsoleLoaded) Console.ForegroundColor = value; else FakeConsole.ForegroundColor = value; }
        }

        public static ConsoleColor BackgroundColor
        {
            get { if (ConsoleLoaded) return Console.BackgroundColor; else return FakeConsole.BackgroundColor; }
            set { if (ConsoleLoaded) Console.BackgroundColor = value; else FakeConsole.BackgroundColor = value; }
        }

        public static Coordinates CursorPosition
        {
            get { if (ConsoleLoaded) return new Coordinates(Console.CursorLeft, Console.CursorTop); else return FakeConsole.CursorPosition; }
            set { if (ConsoleLoaded) Console.SetCursorPosition(value.X, value.Y); else FakeConsole.CursorPosition = value; }
        }

        public static Coordinates WindowPosition
        {
            get { if (ConsoleLoaded) return new Coordinates(Console.WindowLeft, Console.WindowTop); else return FakeConsole.WindowPosition; }
            set { if (ConsoleLoaded) Console.SetWindowPosition(value.X, value.Y); else FakeConsole.WindowPosition = value; }
        }

        public static int CursorSize
        {
            get { if (ConsoleLoaded) return Console.CursorSize; else return FakeConsole.CursorSize; }
            set { if (ConsoleLoaded) Console.CursorSize = value; else FakeConsole.CursorSize = value; }
        }

        public static Size BufferSize
        {
            get { if (ConsoleLoaded) return new Size(Console.BufferWidth, Console.BufferHeight); else return FakeConsole.BufferSize; }
            set { if (ConsoleLoaded) Console.SetBufferSize(value.Width, value.Height); else FakeConsole.BufferSize = value; }
        }

        public static Size WindowSize
        {
            get { if (ConsoleLoaded) return new Size(Console.WindowWidth, Console.WindowHeight); else return FakeConsole.WindowSize; }
            set { if (ConsoleLoaded) Console.SetWindowSize(value.Width, value.Height); else FakeConsole.WindowSize = value; }
        }

        public static Size MaxWindowSize
        {
            get { if (ConsoleLoaded) return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); else return new Size(65535, 65535); }
        }

        public static Size MaxPhysicalWindowSize
        {
            get { if (ConsoleLoaded) return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); else return new Size(65535, 65535); }
        }

        public static bool KeyAvailable
        {
            get { if (ConsoleLoaded) return Console.KeyAvailable; else return false; }
        }

        public static string WindowTitle
        {
            get { if (ConsoleLoaded) return Console.Title; else return FakeConsole.WindowTitle; }
            set { if (ConsoleLoaded) Console.Title = value; else FakeConsole.WindowTitle = value; }
        }

    }


}
