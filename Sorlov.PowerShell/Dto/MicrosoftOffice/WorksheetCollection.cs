using System;
using System.Collections;
using System.Collections.Generic;

namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class WorksheetCollection: IEnumerable
    {
        private List<Worksheet> worksheets;

        internal WorksheetCollection(List<Worksheet> worksheets)
        {
            this.worksheets = worksheets;
        }

        public Worksheet this[int index]
        {
            get
            {
                return worksheets[index];
            }
        }

        public Worksheet this[string name]
        {
            get
            {
                foreach (Worksheet worksheet in worksheets)
                    if (worksheet.Name == name) return worksheet;

                throw new IndexOutOfRangeException(string.Format("There is no sheet with name '{0}'", name));
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new WorksheetsEnmuerator(worksheets);
        }
    }
}
