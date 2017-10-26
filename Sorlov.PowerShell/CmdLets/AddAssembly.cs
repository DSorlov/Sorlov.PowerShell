using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Add, "Assembly", SupportsShouldProcess = true)]
    [CmdletDescription("Load a assembly into domain","Loads a specified assembly into memory. This is permanent until you restart PowerShell. Equivivalent to loading a assembly by using reflection.")]
    [Example(Code = "Add-Assembly System.Security", Remarks = "Loads System.Security into memory from GAC")]
    [Example(Code = "Add-Assembly c:\\someassembly.dll -ByLocation", Remarks = "Loads assembly from c:\\someassembly.dll")]
    public class AddAssembly : PSCmdlet
    {
        #region "Private Parameters"
        string parFile = string.Empty;
        bool byLocation = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name or path to assembly for load",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string AssemblyName
        {
            get { return parFile; }
            set { parFile = value; }
        }
        [Parameter(Position = 1, Mandatory = false, HelpMessage = "Will load the assembly from a specific location (not GAC)", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public SwitchParameter ByLocation
        {
            get { return byLocation; }
            set { byLocation = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            string commandString = string.Empty;

            if (byLocation)
                commandString = "[System.Reflection.Assembly]::LoadFrom(\"{0}\")";
            else
                commandString = "[System.Reflection.Assembly]::Load(\"{0}\")";

            WriteObject(SessionState.InvokeCommand.InvokeScript(string.Format(commandString,parFile)));

        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            WriteVerbose(string.Format("Touching {0}", parFile));
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stop processing");
        }
        #endregion

    }
}
