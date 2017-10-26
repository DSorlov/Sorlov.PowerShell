using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Sorlov.PowerShell.Lib
{
    public static class MachineSID
    {

    private static SecurityIdentifier GetAccountSid(string localAdminAccount)
    {
        NTAccount name = new NTAccount(localAdminAccount);
        SecurityIdentifier sid = (SecurityIdentifier)name.Translate(typeof(SecurityIdentifier));
        return sid;
    }


    // Get the Machine Sid for the PC the code is currently executing on
    public static string MachineSid
    {
        get
        {
            string machineSid = string.Empty;
            string domain;
            short accounttype;
            try
            {
                LookupAccountName(Environment.MachineName, 
                    Environment.MachineName, out machineSid, 
                    out domain, out accounttype);
            
            }
            catch (Exception)
            {
            }
            if (machineSid.Length < 1)
            {

                string account = Environment.MachineName + @"\Administrator";
                try
                {
                    SecurityIdentifier sid = GetAccountSid(account);
                    machineSid = sid.AccountDomainSid.ToString();
                }
                catch (Exception)
                {
                }
            }
            return machineSid;
        }
    }

    internal static bool LookupAccountName(string strServer, 
        string strAccountName, out string accountSid, 
        out string strDomainName, out short AccountType)
    {
        bool bRet = false;
        int lSidSize = 256;
        int lDomainNameSize = 256;

        accountSid = "";
        strDomainName = "";
        AccountType = 0;
        StringBuilder strName;
        lSidSize = 0;
        IntPtr Sid = IntPtr.Zero;

        int nRet = Win32API.LookupAccountName(
                            strServer,
                            strAccountName,
                            Sid,
                            ref lSidSize,
                            null,
                            ref lDomainNameSize,
                            ref AccountType);
        bRet = (0 != nRet);
        if (!bRet)
        {
            int nErr = Marshal.GetLastWin32Error();
            if (122 == nErr) 
            {
                strName = new StringBuilder(lDomainNameSize);
                Sid = Marshal.AllocHGlobal(lSidSize);
                nRet = Win32API.LookupAccountName(
                    strServer,
                    strAccountName,
                    Sid,
                    ref lSidSize,
                    strName,
                    ref lDomainNameSize,
                    ref AccountType);
                bRet = (0 != nRet);
                if (bRet)
                {
                    byte[] sidArray = new byte[lSidSize];
                    strDomainName = strName.ToString();
                    Marshal.Copy(Sid, sidArray, 0, lSidSize);
                    SecurityIdentifier sid = new SecurityIdentifier(sidArray, 0);
                    accountSid = sid.ToString();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(nErr);
            }
        }
        Marshal.FreeHGlobal(Sid);
        return bRet;
    }


    private class Win32API
    {
        #region Win32 API Interfaces

        [DllImport("Advapi32.dll", SetLastError = true)]
        public static extern int LookupAccountName(
            string ServerName,
            string AccountName,
            IntPtr Sid,
            ref int SidSize,
            StringBuilder DomainName,
            ref int DomainNameSize,
            ref short SidUse);


        #endregion
    }
 
    } 

}
