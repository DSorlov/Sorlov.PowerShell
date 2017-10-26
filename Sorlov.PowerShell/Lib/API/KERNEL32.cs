using System;
using System.Runtime.InteropServices;

namespace Sorlov.PowerShell.Lib.API
{
    public class KERNEL32
    {
        [Flags]
        public enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MoveFileEx(
            string lpExistingFileName,
            string lpNewFileName,
            MoveFileFlags dwFlags);
    }


}
