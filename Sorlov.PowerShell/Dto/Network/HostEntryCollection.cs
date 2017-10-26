using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Sorlov.PowerShell.Dto.Network
{
    public class HostEntryCollection: IEnumerable
    {
        private List<HostEntry> hostEntries;

        internal HostEntryCollection()
        {
            this.hostEntries = new List<HostEntry>();
        }

        internal void Add(HostEntry hostEntry)
        {
            this.hostEntries.Add(hostEntry);
        }

        internal void Remove(HostEntry hostEntry)
        {
            this.hostEntries.Remove(hostEntry);
        }

        public HostEntry this[int i] { get { return hostEntries[i]; } }
        public int Count { get { return hostEntries.Count; } }

        public HostEntry this[string hostname]
        {
            get
            {
                foreach (HostEntry entry in hostEntries)
                    foreach(string entryHostname in entry.Hostnames)
                        if (entryHostname.ToLower() == hostname.ToLower()) return entry;

                return null;
            }
        }

        public HostEntry this[IPAddress address]
        {
            get
            {
                foreach (HostEntry entry in hostEntries)
                    if (entry.IPAddress.Equals(address)) return entry;

                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new HostEntryEnumerator(hostEntries);
        }
    }
}
