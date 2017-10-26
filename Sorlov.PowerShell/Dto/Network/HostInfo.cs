namespace Sorlov.PowerShell.Dto.Network
{
    public class HostInfo
    {

        private string hostName;
        private string dnsName;
        private System.Net.IPAddress[] adresses;

        public HostInfo(string hostName, string dnsName, System.Net.IPAddress[] adresses)
        {
            this.hostName = hostName;
            this.dnsName = dnsName;
            this.adresses = adresses;
        }

        public string Hostname
        {
            get { return hostName; }
        }

        public string DNSName
        {
            get { return dnsName; }
        }

        public System.Net.IPAddress[] Adresses
        {
            get { return adresses; }
        }

        public override string ToString()
        {
            return hostName;
        }

    }
}
