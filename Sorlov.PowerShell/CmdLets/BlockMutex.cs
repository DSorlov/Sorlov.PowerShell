using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Threading;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsSecurity.Block, "Mutex")]
    [CmdletDescription("Blocks a mutex or wait for it to be free",
        "This commandlet reserves a mutex and returns or if the mutex is already reserved it waits for it to be released.")]
    public class BlockMutex : PSCmdlet
    {

        #region "Private Parameters"
        private string mutexName = "DSPSMutex";
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The name of the mutex")]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get { return mutexName; }
            set { mutexName = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();


            try
	        {
		        Mutex mutex = Mutex.OpenExisting(mutexName);
		        mutex.WaitOne();
	        }
            catch(WaitHandleCannotBeOpenedException)
            {
                Mutex mutex = new Mutex(false, mutexName);
            }
        	catch(Exception ex)
	        {
                WriteError(new ErrorRecord(ex,"100",ErrorCategory.InvalidOperation, mutexName));
	        }            
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
