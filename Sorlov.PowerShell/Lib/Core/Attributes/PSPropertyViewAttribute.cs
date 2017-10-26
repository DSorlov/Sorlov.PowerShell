using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PSPropertyViewAttribute : Attribute
    {
        private bool _default = false;

        /// <summary>
        /// Defines if this property is the default property
        /// </summary>
        public bool Default
        {
            get { return _default; }
            set { _default = value; }
        }

        private int _sequence = 100;

        /// <summary>
        /// This will define where this property will appear in a table-like view in PowerShell. The 
        /// smaller the value, the more left the property will appear. 
        /// </summary>
        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }


        private string _alias = "";

        /// <summary>
        /// The alias name of this property 
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        private bool _columnOutput = true;

        /// <summary>
        /// If true, the output of this property is included in a table view 
        /// </summary>
        public bool ColumnOutput
        {
            get { return _columnOutput; }
            set { _columnOutput = value; }
        }



        private bool _colRightAligned = false;

        /// <summary>
        /// If true, the output of this property in a table view (as a column) is aligned to the right, else to the left 
        /// </summary>
        public bool ColumnRightAligned
        {
            get { return _colRightAligned; }
            set { _colRightAligned = value; }
        }


        private int _colWidth = 0;

        /// <summary>
        /// The width (in characters) of the output of this property used for a table view
        /// </summary>
        public int ColumnWidth
        {
            get { return _colWidth; }
            set { _colWidth = value; }
        }


        private string _colName = "";

        /// <summary>
        /// The name of this property in a table view. If not specified and ColumnOutput=TRUE, the alias
        /// for this property is used. If Alias is not set, the name of the property is used
        /// </summary>
        public string ColumnName
        {
            get { return _colName; }
            set { _colName = value; }
        }


        private string _colScript = "";

        /// <summary>
        /// This defines a PowerShell script that runs to retrieve the value of the property in a table view. For example:
        /// "[int]($_.Size / 1024)". If empty, the direct value from the property is used
        /// </summary>
        public string ColumnScript
        {
            get { return _colScript; }
            set { _colScript = value; }
        }




    }

}
