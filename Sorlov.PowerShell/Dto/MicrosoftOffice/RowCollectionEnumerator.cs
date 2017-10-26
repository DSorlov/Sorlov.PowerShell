using System.Collections;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class RowCollectionEnumerator : IEnumerator
    {
        private List<object> rows;
        private int _index;

        public RowCollectionEnumerator(List<object> rows)
        {
            this.rows = rows;
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
                return rows[_index];
            }
        }


        public bool MoveNext()
        {
            _index++;
            if (_index >= rows.Count)

                return false;

            else

                return true;
        }
    }
}
