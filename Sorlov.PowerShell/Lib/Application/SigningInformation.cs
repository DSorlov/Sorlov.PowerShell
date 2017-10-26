using System.Security.Cryptography.X509Certificates;

namespace Sorlov.PowerShell.SelfHosted.Lib.Application
{
    public class SingingInformation
    {
        public X509Certificate2 Certificate = null;
        public string TimestampServer = string.Empty;
    }
}
