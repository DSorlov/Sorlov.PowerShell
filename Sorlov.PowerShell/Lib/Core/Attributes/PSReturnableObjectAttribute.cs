using System;

namespace Sorlov.PowerShell.Lib.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PSReturnableObjectAttribute : Attribute
    {
    }
}
