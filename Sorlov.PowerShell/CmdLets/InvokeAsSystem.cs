using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace Sorlov.PowerShell.CmdLets
{
    [Cmdlet(VerbsLifecycle.Invoke,"AsSystem")]
    public class InvokeAsSystem: PSCmdlet
    {
        private ScriptBlock scriptBlock = null;

        [Parameter(Mandatory = true, Position = 0)]
        [ValidateNotNullOrEmpty()]
        public ScriptBlock ScriptBlock
        {
            set { scriptBlock = value; }
            get { return scriptBlock; }
        }

        protected override void BeginProcessing()
        {
            Pipeline pipeline = Runspace.DefaultRunspace.CreateNestedPipeline();
            
            Command command = new Command("Invoke-AsProcess");
            command.Parameters.Add("ScriptBlock", scriptBlock);
            command.Parameters.Add("Name", "lsass");

            pipeline.Commands.Add(command);

            Collection<PSObject> results = pipeline.Invoke();
            WriteObject(results, true);

        }



    }
}
