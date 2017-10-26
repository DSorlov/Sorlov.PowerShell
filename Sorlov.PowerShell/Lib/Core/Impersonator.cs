using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Sorlov.PowerShell.Lib.API;

namespace Sorlov.PowerShell.Lib.Core
{


 public class Impersonator : IDisposable
 {
     public enum LogonType
     {
          LOGON32_LOGON_INTERACTIVE = 2,
          LOGON32_LOGON_NETWORK = 3,
          LOGON32_LOGON_BATCH = 4,
          LOGON32_LOGON_SERVICE = 5,
          LOGON32_LOGON_UNLOCK = 7,
          LOGON32_LOGON_NETWORK_CLEARTEXT = 8, // Win2K or higher
          LOGON32_LOGON_NEW_CREDENTIALS = 9 // Win2K or higher
     };
 
     public enum LogonProvider
     {
          LOGON32_PROVIDER_DEFAULT = 0,
          LOGON32_PROVIDER_WINNT35 = 1,
          LOGON32_PROVIDER_WINNT40 = 2,
          LOGON32_PROVIDER_WINNT50 = 3
     };
 
     public enum ImpersonationLevel
     {
          SecurityAnonymous = 0,
          SecurityIdentification = 1,
          SecurityImpersonation = 2,
          SecurityDelegation = 3
     }

    private WindowsImpersonationContext _wic;
 
    public Impersonator(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
    {
        Impersonate(userName, domainName, password, logonType, logonProvider);
    }
 
    public Impersonator(string userName, string domainName, string password)
    {
        Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
    }
 
    public Impersonator()
    {
    }
 
    public void Dispose()
    {
        UndoImpersonation();
    }
 
    public void Impersonate(string userName, string domainName, string password)
    {
        Impersonate(userName, domainName, password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT);
    }
 
    public void Impersonate(string userName, string domainName, string password, LogonType logonType, LogonProvider logonProvider)
    {
        UndoImpersonation();
 
        IntPtr logonToken = IntPtr.Zero;
        IntPtr logonTokenDuplicate = IntPtr.Zero;
        try
        {
            _wic = WindowsIdentity.Impersonate(IntPtr.Zero);
 
            if (ADVAPI32.LogonUser(userName, domainName, password, (int)logonType, (int)logonProvider, ref logonToken) != 0)
            {
                if (ADVAPI32.DuplicateToken(logonToken, (int)ImpersonationLevel.SecurityIdentification, ref logonTokenDuplicate) != 0)
                {
                    var wi = new WindowsIdentity(logonTokenDuplicate);
                    wi.Impersonate();
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            else
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            if (logonToken != IntPtr.Zero)
                KERNEL32.CloseHandle(logonToken);
 
            if (logonTokenDuplicate != IntPtr.Zero)
                KERNEL32.CloseHandle(logonTokenDuplicate);
        }
    }
 
    private void UndoImpersonation()
    {
        if (_wic != null)
            _wic.Undo();
            _wic = null;
        }
    }

}
