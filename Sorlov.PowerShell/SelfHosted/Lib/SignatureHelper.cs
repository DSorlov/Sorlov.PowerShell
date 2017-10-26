using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;

namespace Sorlov.PowerShell.SelfHosted.Lib
{

    public static class SignatureHelper
    {

        public static string SetFileSignatureSimple(string file, X509Certificate2 cert, string timestamp)
        {
            Signature result = SetFileSignature(file, cert, timestamp);
            return result.Status.ToString();
        }

        public static Signature SetFileSignature(string file, X509Certificate2 cert, string timestamp)
        {
            NativeStructs.SigningOption options = NativeStructs.SigningOption.AddFullCertificateChain;
            string defaultAlgorithm = "SHA1";   

            if (cert==null)
            {
                X509Store store = new X509Store("MY", StoreLocation.CurrentUser);
                store.Open(OpenFlags.MaxAllowed);
                foreach (X509Certificate2 storedCert in store.Certificates)
                    if (CertIsGoodForSigning(storedCert))
                    { cert = storedCert; break; }

                if (cert == null)
                    throw new InvalidDataException("Could not locate a appropriate certificate");
            }

            if (timestamp==null)
                timestamp = "http://timestamp.comodoca.com/authenticode";

            return SignFile(options, file, cert, timestamp, defaultAlgorithm); 
        }

        private static bool CertIsGoodForSigning(X509Certificate2 c)
        {
            if (!c.HasPrivateKey)
                return false;
            foreach (string str in GetCertEKU(c))
            {
                if (str == "1.3.6.1.5.5.7.3.3")
                    return true;
            }
            return false;
        }

        private static Collection<string> GetCertEKU(X509Certificate2 cert)
        {
            Collection<string> collection = new Collection<string>();
            IntPtr handle = cert.Handle;
            int pcbUsage = 0;
            IntPtr pUsage = IntPtr.Zero;
            if (!CRYPT32.CertGetEnhancedKeyUsage(handle, 0U, pUsage, out pcbUsage))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            if (pcbUsage > 0)
            {
                IntPtr num = Marshal.AllocHGlobal(pcbUsage);
                try
                {
                    if (!CRYPT32.CertGetEnhancedKeyUsage(handle, 0U, num, out pcbUsage))
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    NativeStructs.CERT_ENHKEY_USAGE certEnhkeyUsage = (NativeStructs.CERT_ENHKEY_USAGE)Marshal.PtrToStructure(num, typeof(NativeStructs.CERT_ENHKEY_USAGE));
                    IntPtr ptr = certEnhkeyUsage.rgpszUsageIdentifier;
                    for (int index = 0; (long)index < (long)certEnhkeyUsage.cUsageIdentifier; ++index)
                    {
                        string str = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(ptr, index * Marshal.SizeOf((object)ptr)));
                        collection.Add(str);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(num);
                }
            }
            return collection;
        }

        private static Signature SignFile(NativeStructs.SigningOption option, string fileName, X509Certificate2 certificate, string timeStampServerUrl, string hashAlgorithm)
        {
            System.Management.Automation.Signature signature = (System.Management.Automation.Signature)null;
            IntPtr num = IntPtr.Zero;
            uint error = 0U;
            string hashAlgorithm1 = (string)null;
            CheckArgForNullOrEmpty(fileName, "fileName");
            CheckArgForNull((object)certificate, "certificate");

            if (!string.IsNullOrEmpty(timeStampServerUrl) && (timeStampServerUrl.Length <= 7 || timeStampServerUrl.IndexOf("http://", StringComparison.OrdinalIgnoreCase) != 0))
                throw new ArgumentException("Time stamp server url required");

            if (!string.IsNullOrEmpty(hashAlgorithm))
            {
                IntPtr oidInfo = CRYPT32.CryptFindOIDInfo(2U, Marshal.StringToHGlobalUni(hashAlgorithm), 0U);
                if (oidInfo == IntPtr.Zero)
                    throw new ArgumentException("Invalid hash algorithm");

                hashAlgorithm1 = ((NativeStructs.CRYPT_OID_INFO)Marshal.PtrToStructure(oidInfo, typeof(NativeStructs.CRYPT_OID_INFO))).pszOID;
            }
            if (!CertIsGoodForSigning(certificate))
                throw new ArgumentException("Supplied certificate cannot be used to sign files.");
            
            CheckIfFileExists(fileName);
            try
            {
                string timeStampServerUrl1 = (string)null;
                if (!string.IsNullOrEmpty(timeStampServerUrl))
                    timeStampServerUrl1 = timeStampServerUrl;
                NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_INFO wizDigitalSignInfo = NativeStructs.InitSignInfoStruct(fileName, certificate, timeStampServerUrl1, hashAlgorithm1, option);
                num = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)wizDigitalSignInfo));
                Marshal.StructureToPtr((object)wizDigitalSignInfo, num, false);
                bool flag = CRYPTUI.CryptUIWizDigitalSign(1U, IntPtr.Zero, IntPtr.Zero, num, IntPtr.Zero);
                Marshal.DestroyStructure(wizDigitalSignInfo.pSignExtInfo, typeof(NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO));
                Marshal.FreeCoTaskMem(wizDigitalSignInfo.pSignExtInfo);
                if (!flag)
                {
                    error = SignatureHelper.GetLastWin32Error();
                    switch (error)
                    {
                        case 2147500037U:
                        case 2147942401U:
                        case 2147954407U:
                            flag = true;
                            break;
                        case 2148073480U:
                            throw new ArgumentException("InvalidHashAlgorithm");
                        default:
                            throw new ArgumentException(string.Format("CryptUIWizDigitalSign: failed: {0:x}", new object[1]
                              {
                                (object) error
                              }));
                    }
                }
                signature = !flag ? SignatureProxy.GenerateSignature(fileName, error) : SignatureHelper.GetSignature(fileName);
            }
            finally
            {
                Marshal.DestroyStructure(num, typeof(NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_INFO));
                Marshal.FreeCoTaskMem(num);
            }
            return signature;
        }

        private static void CheckIfFileExists(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);
        }

        private static void CheckArgForNullOrEmpty(string arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
            if (arg.Length == 0)
                throw new ArgumentException(argName);
        }

        private static void CheckArgForNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }


        public static Signature GetSignature(string fileName)
        {
            CheckArgForNullOrEmpty(fileName, "fileName");
            CheckIfFileExists(fileName);
            System.Management.Automation.Signature signature;
            try
            {
                NativeStructs.WINTRUST_DATA wtData;
                uint winTrustData = SignatureHelper.GetWinTrustData(fileName, out wtData);
                //if ((int)winTrustData != 0)
                //    SignatureHelper.tracer.WriteLine("GetWinTrustData failed: {0:x}", new object[1]
                //      {
                //        (object) winTrustData
                //      });
                signature = SignatureHelper.GetSignatureFromWintrustData(fileName, winTrustData, wtData);
                uint num = WINTRUST.DestroyWintrustDataStruct(wtData);
                //if ((int)num != 0)
                //    SignatureHelper.tracer.WriteLine("DestroyWinTrustDataStruct failed: {0:x}", new object[1]
                  //{
                  //  (object) num
                  //});
            }
            catch
            {
                signature = SignatureProxy.GenerateSignature(fileName, 2148204800U);
            }
            return signature;
        }

        private static uint GetWinTrustData(string fileName, out NativeStructs.WINTRUST_DATA wtData)
        {
            uint num1 = 2147500037U;
            IntPtr num2 = IntPtr.Zero;
            IntPtr num3 = IntPtr.Zero;
            Guid guid = new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");
            try
            {
                num2 = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)guid));
                Marshal.StructureToPtr((object)guid, num2, false);
                //NativeStructs.WINTRUST_DATA wintrustData = fileContent != null ? WINTRUST.InitWintrustDataStructFromBlob(WINTRUST.InitWintrustBlobInfoStruct(fileName, fileContent)) : WINTRUST.InitWintrustDataStructFromFile(WINTRUST.InitWintrustFileInfoStruct(fileName));
                NativeStructs.WINTRUST_DATA wintrustData = WINTRUST.InitWintrustDataStructFromFile(WINTRUST.InitWintrustFileInfoStruct(fileName));
                num3 = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)wintrustData));
                Marshal.StructureToPtr((object)wintrustData, num3, false);
                num1 = WINTRUST.WinVerifyTrust(new IntPtr(-1), num2, num3);
                wtData = (NativeStructs.WINTRUST_DATA)Marshal.PtrToStructure(num3, typeof(NativeStructs.WINTRUST_DATA));
            }
            finally
            {
                Marshal.DestroyStructure(num2, typeof(Guid));
                Marshal.FreeCoTaskMem(num2);
                Marshal.DestroyStructure(num3, typeof(NativeStructs.WINTRUST_DATA));
                Marshal.FreeCoTaskMem(num3);
            }
            return num1;
        }

        private static X509Certificate2 GetCertFromChain(IntPtr pSigner)
        {
            X509Certificate2 x509Certificate2 = (X509Certificate2)null;
            IntPtr provCertFromChain = WINTRUST.WTHelperGetProvCertFromChain(pSigner, 0U);
            if (provCertFromChain != IntPtr.Zero)
                x509Certificate2 = new X509Certificate2(((NativeStructs.CRYPT_PROVIDER_CERT)Marshal.PtrToStructure(provCertFromChain, typeof(NativeStructs.CRYPT_PROVIDER_CERT))).pCert);
            return x509Certificate2;
        }

        private static System.Management.Automation.Signature GetSignatureFromWintrustData(string filePath, uint error, NativeStructs.WINTRUST_DATA wtd)
        {
            System.Management.Automation.Signature signature = (System.Management.Automation.Signature)null;
            X509Certificate2 timestamper = (X509Certificate2)null;

            IntPtr pProvData = WINTRUST.WTHelperProvDataFromStateData(wtd.hWVTStateData);
            if (pProvData != IntPtr.Zero)
            {
                IntPtr provSignerFromChain = WINTRUST.WTHelperGetProvSignerFromChain(pProvData, 0U, 0U, 0U);
                if (provSignerFromChain != IntPtr.Zero)
                {
                    X509Certificate2 certFromChain = SignatureHelper.GetCertFromChain(provSignerFromChain);
                    if (certFromChain != null)
                    {
                        NativeStructs.CRYPT_PROVIDER_SGNR cryptProviderSgnr = (NativeStructs.CRYPT_PROVIDER_SGNR)Marshal.PtrToStructure(provSignerFromChain, typeof(NativeStructs.CRYPT_PROVIDER_SGNR));
                        if ((int)cryptProviderSgnr.csCounterSigners == 1)
                            timestamper = SignatureHelper.GetCertFromChain(cryptProviderSgnr.pasCounterSigners);
                        signature = timestamper == null ? SignatureProxy.GenerateSignature(filePath, error, certFromChain) : SignatureProxy.GenerateSignature(filePath, error, certFromChain, timestamper);
                    }
                }
            }
            if (signature == null && (int)error != 0)
                signature = SignatureProxy.GenerateSignature(filePath, error);
            return signature;
        }

        private static uint GetLastWin32Error()
        {
            return BitConverter.ToUInt32(BitConverter.GetBytes(Marshal.GetLastWin32Error()), 0);
        }
    }
}
