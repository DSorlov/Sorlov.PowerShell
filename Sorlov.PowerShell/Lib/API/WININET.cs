using System.Runtime.InteropServices;

namespace Sorlov.PowerShell.Lib.API
{
    public class WININET
    {
        [DllImport("wininet.dll")]
        public extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

    }
}
