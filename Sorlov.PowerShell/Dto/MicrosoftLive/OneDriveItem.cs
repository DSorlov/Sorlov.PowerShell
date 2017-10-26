using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Dto.MicrosoftLive
{
    class OneDriveItem
    {
        public OneDriveItem(IDictionary<string, object> source)
        {
            this.Properties = source;
        }

        public IDictionary<string, object> Properties { get; private set; }

        public string Id
        {
            get { return (string)this.Properties["id"]; }
        }

        public string Type
        {
            get { return (string)this.Properties["type"]; }
        }

        public string Name
        {
            get { return (string)this.Properties["name"]; }
        }

        public bool IsFolder
        {
            get { return this.Type == "folder"; }
        }

        public bool IsAlbum
        {
            get { return this.Type == "album"; }
        }

        public DateTimeOffset UpdatedTime
        {
            get { return DateTimeOffset.Parse((string)this.Properties["updated_time"]); }
        }

        public string SharedWith
        {
            get { return ((IDictionary<string, object>)this.Properties["shared_with"])["access"].ToString(); }
        }
    }

}
