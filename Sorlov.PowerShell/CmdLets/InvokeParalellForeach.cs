using System;
using System.Collections.Generic;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation.Runspaces;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsLifecycle.Invoke, "ParalellForeach")]
    [Example(Code="Get-ChildItem -Path D:\\Files | ForEach-Parallel -MaxThreads 100 -ScriptBlock {Copy-Item -Path $_.FullName -Destination E:\\Company\\Files}")]
    [Example(Code="1..500 | Foreach-Parallel -MaxThreads 20 -ScriptBlock {New-VM –Name VM$_ –MemoryStartupBytes 512MB}")]
    [Example(Code="ForEach-Parallel -InputObject (Get-ChildItem -Path \"D:\\Files\") -MaxThreads 100 -ScriptBlock {Copy-Item -Path $_.FullName -Destination E:\\Company\\Files}")]
    [CmdletDescription("This function can be used to execute tasks in parallel.",
        "his function can be used to execute tasks in parallel with more than 5 parallel tasks at once. The number of parallel tasks can be defined by parameter. The input is accepted by defining it by using the -InputObject parameter which also accepts input from the pipeline.")]
    public class InvokeParalellForeach : PSCmdlet
    {

        #region "Private Parameters"
            private InitialSessionState sessionState;
            private RunspacePool runspacePool;
            private List<KeyValuePair<System.Management.Automation.PowerShell, IAsyncResult>> threads;
            private ScriptBlock scriptBlock;
            private PSObject inputObject;
            private int maxThreads = 5;
        #endregion

        #region "Public Parameters"
        [Parameter(Mandatory = true, HelpMessage = "The document to convert to PDF",ValueFromPipeline=true)]
        [Alias("Input")]
        [ValidateNotNullOrEmpty()]
        public PSObject InputObject
        {
            get { return inputObject; }
            set { inputObject = value; }
        }
        [Parameter(Position = 1, HelpMessage = "The document to convert to PDF")]
        [Alias("Threads")]
        [ValidateNotNullOrEmpty()]
        public int MaxThreads
        {
            get { return maxThreads; }
            set { maxThreads = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            sessionState = InitialSessionState.CreateDefault();
            WriteVerbose("Created initial session state..");

            runspacePool = RunspaceFactory.CreateRunspacePool(1, maxThreads, sessionState, this.Host);
            runspacePool.Open();
            WriteVerbose("Runspace pool created..");
            
            scriptBlock = InvokeCommand.NewScriptBlock(string.Format("param($_)\r\n{0}", scriptBlock.ToString()));
            WriteVerbose("Modified scriptblock..");
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create().AddScript(scriptBlock.ToString()).AddArgument(inputObject);
            ps.RunspacePool = runspacePool;
            WriteVerbose("Created runspace in runspace pool..");

            threads.Add(new KeyValuePair<System.Management.Automation.PowerShell, IAsyncResult>(ps,ps.BeginInvoke()));
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            
            base.EndProcessing();
            WriteVerbose("Waiting for runspace completion..");

            bool running = true;

            while(running)
            {
                running = false;
                foreach(KeyValuePair<System.Management.Automation.PowerShell, IAsyncResult> thread in threads)
                {
                    if (thread.Value.IsCompleted)
                    {
                        WriteVerbose("Runspace completed..");
                        thread.Key.EndInvoke(thread.Value);
                        thread.Key.Dispose();
                        threads.Remove(thread);
                    }
                    else
                    {
                        running = true;
                    }
                }
            }

            WriteVerbose("All runspaces completed..");

        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stopping processing..");


        }
        #endregion


    }
}
