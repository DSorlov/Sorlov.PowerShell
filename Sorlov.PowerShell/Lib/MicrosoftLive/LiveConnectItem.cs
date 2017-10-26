using System.Collections.Generic;
using Mjollnir;

namespace Sorlov.PowerShell.Lib.MicrosoftLive
{
    public abstract class LiveConnectItem
    {
        public LiveConnectItem(IDictionary<string, object> source)
        {
            ThrowArgumentException.IfNull(source, "source");
            ThrowArgumentException.IfOutOfRange(!source.ContainsKey("id"), "source");

            this.Properties = source;
        }

        public IDictionary<string, object> Properties { get; private set; }

        public string Id
        {
            get
            {
                return (string)this.Properties["id"];
            }
        }
    }
}
