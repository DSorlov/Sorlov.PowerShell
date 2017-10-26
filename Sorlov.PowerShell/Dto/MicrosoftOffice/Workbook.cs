using System;

namespace Sorlov.PowerShell.Dto.MicrosoftOffice
{
    public class Workbook
    {
        private string filename;
        public string Filepath { get { return filename; } internal set { filename = value; } }

        private string author;
        public string Author { get { return author; } internal set { author = value; } }

        private string title;
        public string Title { get { return title; } internal set { title = value; } }

        private string comments;
        public string Comments { get { return comments; } internal set { comments = value; } }

        private string keywords;
        public string Keywords { get { return keywords; } internal set { keywords = value; } }

        private DateTime fileLastModified;
        public DateTime FileLastModified { get { return fileLastModified; } internal set { fileLastModified = value; } }

        private DateTime fileCreated;
        public DateTime FileCreated { get { return fileCreated; } internal set { fileCreated = value; } }

        private bool fileIsReadOnly;
        public bool FileIsReadOnly { get { return fileIsReadOnly; } internal set { fileIsReadOnly = value; } }

        private bool isReadOnly;
        public bool IsReadOnly { get { return isReadOnly; } internal set { isReadOnly = value; } }

        private WorksheetCollection worksheets;
        public WorksheetCollection Worksheets { get { return worksheets; } internal set { worksheets = value; } }

        public object this[int index, int id]
        {
            get
            {
                return worksheets[index].Rows[id];
            }
        }

        public object this[string name, int id]
        {
            get
            {
                return worksheets[name].Rows[id];
            }
        }

        public RowCollection this[int index]
        {
            get
            {
                return worksheets[index].Rows;
            }
        }

        public RowCollection this[string name]
        {
            get
            {
                return worksheets[name].Rows;
            }
        }

        internal Workbook()
        {
            isReadOnly = true;
        }

        public override string ToString()
        {
            return string.Format("Excel.Workbook::{0}", filename);
        }

    }
}
