using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PSPropertySortAttribute : Attribute
    {

        private int _sortID = 1;

        /// <summary>
        /// This will define the sort order for this property. The property with the smallest 
        /// SortID will be sorted first, then the one with the next higher SortID and so on.
        /// </summary>
        public int SortID
        {
            get { return _sortID; }
            set { _sortID = value; }
        }
    }

}
