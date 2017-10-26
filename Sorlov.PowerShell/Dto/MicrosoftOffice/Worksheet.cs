namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class Worksheet
    {
        private string name;
        public string Name { get { return name; } internal set { name = value; } }

        private RowCollection rows;
        public RowCollection Rows { get { return rows; } internal set { rows = value; } }

        internal Worksheet()
        {
        }
    }
}
