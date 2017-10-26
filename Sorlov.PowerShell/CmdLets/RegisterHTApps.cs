using System.Management.Automation;
using Sorlov.PowerShell.Lib.HTApps;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Register,"HTApps")]
    public class RegisterHTApps: PSCmdlet
    {
        protected override void BeginProcessing()
        {
            HTAppFiles.Associate();
        }
    }
}
