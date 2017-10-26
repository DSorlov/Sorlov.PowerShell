using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "Assembly", SupportsShouldProcess = true)]
    [CmdletDescription("Returns a list of all loaded assemblies")]
    public class GetAssembly: PSCmdlet
    {

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            WriteObject(AppDomain.CurrentDomain.GetAssemblies());
        }
        #endregion

    }
}
