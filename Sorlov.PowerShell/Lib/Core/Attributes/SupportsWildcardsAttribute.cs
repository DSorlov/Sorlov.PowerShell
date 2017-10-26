using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SupportsWildcardsAttribute : Attribute
    {
    }
}
