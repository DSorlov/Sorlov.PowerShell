using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Text;
using Sorlov.PowerShell.Lib.API;
using Sorlov.PowerShell.Lib.MicrosoftOffice.Automation.Exceptions;

namespace Sorlov.PowerShell.CmdLets
{
    [Cmdlet(VerbsLifecycle.Invoke,"AsProcess",DefaultParameterSetName="ById")]
    public class InvokeAsProcess: PSCmdlet
    {
        private ScriptBlock scriptBlock = null;
        private int processId = 0;
        private string processName = string.Empty;
        private Process processObject = null;

        [Parameter(Mandatory=true, Position = 0, ParameterSetName="ById")]
        [ValidateNotNullOrEmpty()]
        public int Id
        {
            set { processId = value; }
            get { return processId; }
        }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            set { processName = value; }
            get { return processName; }
        }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ProcessObject", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public Process Process
        {
            set { processObject = value; }
            get { return processObject; }
        }


        [Parameter(Mandatory=true, Position = 1, ParameterSetName="ById")]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ByName")]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "ProcessObject")]
        [ValidateNotNullOrEmpty()]
        public ScriptBlock ScriptBlock
        {
            set { scriptBlock = value; }
            get { return scriptBlock; }
        }

        private void AdjustToken(IntPtr hToken, string privilege)
        {
            ADVAPI32.TokPriv1Luid tokenPriv1Luid = new ADVAPI32.TokPriv1Luid() { Count = 1, Attr = ADVAPI32.SE_PRIVILEGE_ENABLED, Luid = 0 };
            bool lookupResult = ADVAPI32.LookupPrivilegeValue(null, privilege, ref tokenPriv1Luid.Luid);

            if (!lookupResult)
            {
                int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                ThrowTerminatingError(new ErrorRecord(new InstanceNotFoundException(string.Format("Privilege lookup failed for '{1}': {0}", win32Error,privilege)), "100", ErrorCategory.ResourceUnavailable, null));
            }

            bool adjustTokenResult = ADVAPI32.AdjustTokenPrivileges(hToken, false, ref tokenPriv1Luid, 12, IntPtr.Zero, IntPtr.Zero);
            if (!adjustTokenResult)
            {
                int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException(string.Format("Cannot adjust token privilege '{1}': {0}", win32Error, privilege)), "100", ErrorCategory.AuthenticationError, null));
            }
        }

        protected override void ProcessRecord()
        {
 	        base.ProcessRecord();

            IntPtr hToken = IntPtr.Zero;
            bool openProcessToken = ADVAPI32.OpenProcessToken(ADVAPI32.GetCurrentProcess(), ADVAPI32.TOKEN_ALL_ACCESS, ref hToken);
            if (!openProcessToken)
            {
                int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                ThrowTerminatingError(new ErrorRecord(new InvalidOperationException(string.Format("Cannot open process token: {0}", win32Error)), "100", ErrorCategory.ResourceUnavailable, null));
            }

            

            Process lsassProcess = null;
            try
            {
                switch (this.ParameterSetName)
                {
                    case "ById":
                        lsassProcess = Process.GetProcessById(processId);
                        break;
                    case "ByName":
                        lsassProcess = Process.GetProcessesByName(processName)[0];
                        break;
                    case "ProcessObject":
                        lsassProcess = processObject;
                        break;
                }
            }
            catch
            {
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Specified cannot be located."), "100", ErrorCategory.ObjectNotFound, null));                                
            }

            IntPtr lsaToken = IntPtr.Zero;
            bool processOpen = ADVAPI32.OpenProcessToken(lsassProcess.Handle, ADVAPI32.TOKEN_IMPERSONATE | ADVAPI32.TOKEN_DUPLICATE | ADVAPI32.TOKEN_QUERY , ref lsaToken);
            if (!processOpen)
            {
                int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException(string.Format("Cannot open process: {0}", win32Error)), "100", ErrorCategory.ConnectionError, null));
            }

            try
            {
                WindowsIdentity newId = new WindowsIdentity(hToken);
                using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                {
                    IntPtr dupeToken = IntPtr.Zero;
                    int tokenDuplication = ADVAPI32.DuplicateToken(lsaToken, 2, ref dupeToken);
                    if (tokenDuplication<0)
                    {
                        int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                        ThrowTerminatingError(new ErrorRecord(new OperationCallFailedException(string.Format("Could not duplicate token: {0}", win32Error)), "100", ErrorCategory.SecurityError, null));
                    }
                    AdjustToken(hToken, "SeIncreaseQuotaPrivilege");
                    AdjustToken(hToken, "SeSecurityPrivilege");
                    AdjustToken(hToken, "SeTakeOwnershipPrivilege");
                    AdjustToken(hToken, "SeLoadDriverPrivilege");
                    AdjustToken(hToken, "SeSystemProfilePrivilege");
                    AdjustToken(hToken, "SeSystemtimePrivilege");
                    AdjustToken(hToken, "SeProfileSingleProcessPrivilege");
                    AdjustToken(hToken, "SeIncreaseBasePriorityPrivilege");
                    AdjustToken(hToken, "SeCreatePagefilePrivilege");
                    AdjustToken(hToken, "SeBackupPrivilege");
                    AdjustToken(hToken, "SeRestorePrivilege");
                    AdjustToken(hToken, "SeShutdownPrivilege");
                    AdjustToken(hToken, "SeDebugPrivilege");
                    AdjustToken(hToken, "SeSystemEnvironmentPrivilege");
                    AdjustToken(hToken, "SeChangeNotifyPrivilege");
                    AdjustToken(hToken, "SeRemoteShutdownPrivilege");
                    AdjustToken(hToken, "SeUndockPrivilege");
                    AdjustToken(hToken, "SeManageVolumePrivilege");
                    AdjustToken(hToken, "SeImpersonatePrivilege");
                    AdjustToken(hToken, "SeCreateGlobalPrivilege");
                    AdjustToken(hToken, "SeTimeZonePrivilege");
                    AdjustToken(hToken, "SeCreateSymbolicLinkPrivilege");
                    AdjustToken(hToken, "SeIncreaseWorkingSetPrivilege");


                    bool setToken = ADVAPI32.SetThreadToken(IntPtr.Zero, dupeToken);
                    if (!setToken)
                    {
                        int win32Error = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
                        ThrowTerminatingError(new ErrorRecord(new SecurityAccessDeniedException(string.Format("Could not set thread token: {0}", win32Error)), "100", ErrorCategory.SecurityError, null));
                    }


                    try
                    {
                        Collection<PSObject> results = scriptBlock.Invoke();
                        WriteObject(results, true);
                    }
                    catch (Exception ex)
                    {
                        ThrowTerminatingError(new ErrorRecord(new ApplicationException(string.Format("An internal error occured",ex.Message),ex), "200", ErrorCategory.NotSpecified, null ));
                    }
                }

            }
            catch (Exception)
            {
                ThrowTerminatingError(new ErrorRecord(new SecurityException(string.Format("Could not change identity")), "100", ErrorCategory.SecurityError, null));
            }

        }
    }
}
