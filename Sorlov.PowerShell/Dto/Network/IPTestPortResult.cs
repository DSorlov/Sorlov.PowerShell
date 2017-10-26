using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Sorlov.PowerShell.Dto.Network
{
    public class IPTestPortResult
    {
        public string Host { get { return host; } }
        public int Port { get { return port; } }
        public IPAddress IPAddress { get { return ipAddress; } }
        public bool IsOpen { get { return isOpen; } }
        public Exception Error { get { return error; } }
        public string Banner { get { return banner; } }
        public ProtocolType Protocol { get { return protocol; } }
        public TimeSpan Duration { get { return duration; } }
        public IPStatus PingReply { get { return pingReply; } }

        public IPTestPortResult(string Host, int Port, IPAddress IPAddress, bool IsOpen, Exception Error, string Banner, ProtocolType Protocol, TimeSpan Duration, IPStatus PingResult)
        {
            host = Host;
            port = Port;
            ipAddress = IPAddress;
            isOpen = IsOpen;
            error = Error;
            banner = Banner;
            protocol = Protocol;
            duration = Duration;
            pingReply = PingReply;
        }

        public override string ToString()
        {
            return isOpen.ToString();
        }

        private string host;
        private int port;
        private IPAddress ipAddress;
        private bool isOpen;
        private Exception error;
        private string banner;
        private ProtocolType protocol;
        private TimeSpan duration;
        private IPStatus pingReply;
    }

}
