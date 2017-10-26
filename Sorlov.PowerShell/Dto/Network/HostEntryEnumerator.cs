using System.Collections;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Dto.Network
{
    public class HostEntryEnumerator: IEnumerator
    {
        private List<HostEntry> hostEntries;
        private int _index;

        public HostEntryEnumerator(List<HostEntry> hostEntries)
        {
            this.hostEntries = hostEntries;
            _index = -1;
        }

        public void Reset()
        {
            _index = -1;
        }


        public object Current
        {
            get
            {
                return hostEntries[_index];
            }
        }


        public bool MoveNext()
        {
            _index++;
            if (_index >= hostEntries.Count)

                return false;

            else

                return true;
        }
    }
}
