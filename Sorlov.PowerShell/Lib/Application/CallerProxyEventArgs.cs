﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sorlov.PowerShell.Lib.Application
{
    public class CallerProxyEventArgs : EventArgs
    {
        public string StreamName { get; set; }
        public string Text { get; set; }
    }
}
