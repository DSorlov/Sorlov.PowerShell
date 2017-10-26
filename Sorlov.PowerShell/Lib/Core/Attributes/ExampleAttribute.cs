using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        public string Code { get; set; }
        public string Remarks { get; set; }
    }
}
