using System;
using System.Runtime.InteropServices;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    public class CRYPT32
    {
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CertOpenSystemStore(IntPtr hCryptProv, string storename);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertCloseStore(IntPtr hCertStore,uint dwFlags);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertFindCertificateInStore(IntPtr hCertStore,uint dwCertEncodingType,uint dwFindFlags,uint dwFindType, [In, MarshalAs(UnmanagedType.LPWStr)]String pszFindString, IntPtr pPrevCertCntxt);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertFreeCertificateContext(IntPtr hCertStore);

		[DllImport("crypt32.dll", CharSet=CharSet.Auto, SetLastError=true)]
		public static extern IntPtr CertOpenStore(IntPtr storeProvider, uint dwMsgAndCertEncodingType,IntPtr hCryptProv,uint dwFlags,String cchNameString) ;

        [DllImport("crypt32.dll")]
        public static extern IntPtr CryptFindOIDInfo(uint dwKeyType, IntPtr pvKey, uint dwGroupId);

        [DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CertGetEnhancedKeyUsage(IntPtr pCertContext, uint dwFlags, IntPtr pUsage, out int pcbUsage);

    }

}
