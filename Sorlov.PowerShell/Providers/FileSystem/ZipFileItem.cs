using System;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    public class ZipFileItem
    {
        private string name;
        private DateTime lastModifiedDate;
        private string itemType;

        public ZipFileItem(string name, DateTime lastModifiedDate, string itemType)
        {
          this.name = name;
          this.lastModifiedDate = lastModifiedDate;
          this.itemType = itemType;
        }

        public string Name
        {
          get { return name; }
          set { name = value; }
        }

        public DateTime LastModifiedDate
        {
            get { return lastModifiedDate; }
            set { lastModifiedDate = value; }
        }

        public string Type
        {
          get { return itemType; }
          set { itemType = value; }
        }
    }
}
