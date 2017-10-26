using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Dto.Adfs
{
    public class AdfsAudience
    {
        private string server;
        private string audience;

        public string Server { get { return server; } }
        public string Audience { get { return audience; } }

        public AdfsAudience(string server, string audience)
        {
            this.server = server;
            this.audience = audience;
        }
    }
}
