using System.ComponentModel;

namespace Sorlov.PowerShell.SelfHosted.Lib.Application
{
    public enum Framework
    {
        [Description("2.0")]
        Framework20 = 0,
        [Description("3.5")]
        Framework35 = 1,
        [Description("4.0")]
        Framework40 = 2
    }

}
