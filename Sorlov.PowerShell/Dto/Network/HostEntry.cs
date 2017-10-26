using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Sorlov.PowerShell.Dto.Network
{
    public class HostEntry
    {
        private IPAddress ipAddress;
        private List<string> hostName;
        private string comment;

        public IPAddress IPAddress { get { return ipAddress; } internal set { ipAddress = value; } }
        public string[] Hostnames { get { return hostName.ToArray(); } }
        public string Comment { get { return comment; } internal set { comment = value; } }
        public int Count { get { return hostName.Count(); } }

        internal HostEntry()
        {
            hostName = new List<string>();
        }

        public string this[int index]
        {
            get { return hostName[index]; }
        }

        internal void AddHostname(string hostname)
        {
            hostName.Add(hostname);
        }

        internal void RemoveHostname(string hostname)
        {
            hostName.Remove(hostname);
        }

        internal bool ContainsHostname(string hostname)
        {
            return hostName.Contains(hostname);
        }


    }
}
