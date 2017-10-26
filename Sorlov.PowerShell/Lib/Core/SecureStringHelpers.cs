using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Sorlov.PowerShell.Lib.Core
{
    public static class SecureStringHelpers
    {
        public static String ToUnsecureString(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToBSTR(value);
                string clearText = Marshal.PtrToStringBSTR(valuePtr);
                return clearText;
            }
            finally
            {
                Marshal.FreeBSTR(valuePtr);
            }
        }

        public static SecureString ToSecureString(this string Source)
        {
            SecureString Result = new SecureString();
            foreach (char c in Source.ToCharArray())
                Result.AppendChar(c);
            return Result;
        }
    }
}
