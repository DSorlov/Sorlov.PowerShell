using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    public class CmdletDescriptionAttribute : Attribute
    {
        public string Description { get; set; }

        public string Synopsis { get; set; }

        public CmdletDescriptionAttribute(string desc)
        {
            Description = desc;
            Synopsis = desc;
        }

        public CmdletDescriptionAttribute(string synopsis, string desc)
        {
            Description = desc;
            Synopsis = synopsis;
        }
    }
}
