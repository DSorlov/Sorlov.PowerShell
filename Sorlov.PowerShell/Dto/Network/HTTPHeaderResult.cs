using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Dto.Network
{
    public class HTTPHeaderResult
    {
        private string header;
        private string value;
        public string Header { get { return header; } }
        public string Value { get { return value; } }
        public HTTPHeaderResult(string Header, string Value)
        {
            header = Header;
            value = Value;
        }
    }
}
