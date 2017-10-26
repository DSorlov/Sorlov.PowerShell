using System.Collections;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class RowCollection : IEnumerable
    {
        private List<object> rows;

        internal RowCollection(List<object> rows)
        {
            this.rows = rows;
        }

        public object this[int index]
        {
            get
            {
                return rows[index];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new RowCollectionEnumerator(rows);
        }

    }
}
