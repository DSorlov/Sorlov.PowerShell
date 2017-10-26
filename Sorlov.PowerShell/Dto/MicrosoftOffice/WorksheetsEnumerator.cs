using System.Collections;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class WorksheetsEnmuerator : IEnumerator
    {
        private List<Worksheet> worksheets;
        private int _index;

        public WorksheetsEnmuerator(List<Worksheet> worksheets)
        {
            this.worksheets = worksheets;
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
                return worksheets[_index];
            }
        }


        public bool MoveNext()
        {
            _index++;
            if (_index >= worksheets.Count)

                return false;

            else

                return true;
        }
    }
}
