using System;
using System.Runtime.InteropServices;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    public class CRYPTUI
    {
        [DllImport("cryptUI.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CryptUIWizDigitalSign(uint dwFlags, IntPtr hwndParentNotUsed, IntPtr pwszWizardTitleNotUsed, IntPtr pDigitalSignInfo, IntPtr ppSignContextNotUsed);
    }
}
