using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    public class NativeStructs
    {
        internal struct CRYPT_PROVIDER_CERT
        {
            private uint cbStruct;
            internal IntPtr pCert;
            private uint fCommercial;
            private uint fTrustedRoot;
            private uint fSelfSigned;
            private uint fTestCert;
            private uint dwRevokedReason;
            private uint dwConfidence;
            private uint dwError;
            private IntPtr pTrustListContext;
            private uint fTrustListSignerCert;
            private IntPtr pCtlContext;
            private uint dwCtlError;
            private uint fIsCyclic;
            private IntPtr pChainElement;
        }

        public struct WINTRUST_FILE_INFO
        {
            internal uint cbStruct;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pcwszFilePath;
            internal IntPtr hFileNotUsed;
            internal IntPtr pgKnownSubjectNotUsed;
        }

        public struct CRYPTUI_WIZ_DIGITAL_SIGN_INFO
        {
            internal uint dwSize;
            internal uint dwSubjectChoice;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pwszFileName;
            internal uint dwSigningCertChoice;
            internal IntPtr pSigningCertContext;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pwszTimestampURL;
            internal uint dwAdditionalCertChoice;
            internal IntPtr pSignExtInfo;
        }

        public struct CRYPT_OID_INFO
        {
            public uint cbSize;
            [MarshalAs(UnmanagedType.LPStr)] public string pszOID;
            [MarshalAs(UnmanagedType.LPWStr)] public string pwszName;
            public uint dwGroupId;
            public NativeStructs.Anonymous_a3ae7823_8a1d_432c_bc07_a72b6fc6c7d8 Union1;
            public NativeStructs.CRYPT_ATTR_BLOB ExtraInfo;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Anonymous_a3ae7823_8a1d_432c_bc07_a72b6fc6c7d8
        {
            [FieldOffset(0)] public uint dwValue;
            [FieldOffset(0)] public uint Algid;
            [FieldOffset(0)] public uint dwLength;
        }

        public struct CRYPT_ATTR_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }

        public struct CERT_ENHKEY_USAGE
        {
            internal uint cUsageIdentifier;
            internal IntPtr rgpszUsageIdentifier;
        }

        public enum SigningOption
        {
            AddOnlyCertificate = 0,
            AddFullCertificateChain = 1,
            AddFullCertificateChainExceptRoot = 2,
            Default = 2,
        }

        public static uint GetCertChoiceFromSigningOption(SigningOption option)
        {
            uint num;
            switch (option)
            {
                case SigningOption.AddOnlyCertificate:
                    num = 0U;
                    break;
                case SigningOption.AddFullCertificateChain:
                    num = 1U;
                    break;
                case SigningOption.AddFullCertificateChainExceptRoot:
                    num = 2U;
                    break;
                default:
                    num = 2U;
                    break;
            }
            return num;
        }

        public static NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_INFO InitSignInfoStruct(string fileName, X509Certificate2 signingCert, string timeStampServerUrl, string hashAlgorithm, SigningOption option)
        {
            NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_INFO wizDigitalSignInfo = new NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_INFO();
            wizDigitalSignInfo.dwSize = (uint) Marshal.SizeOf((object) wizDigitalSignInfo);
            wizDigitalSignInfo.dwSubjectChoice = 1U;
            wizDigitalSignInfo.pwszFileName = fileName;
            wizDigitalSignInfo.dwSigningCertChoice = 1U;
            wizDigitalSignInfo.pSigningCertContext = signingCert.Handle;
            wizDigitalSignInfo.pwszTimestampURL = timeStampServerUrl;
            wizDigitalSignInfo.dwAdditionalCertChoice = NativeStructs.GetCertChoiceFromSigningOption(option);
            NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO signExtendedInfo = NativeStructs.InitSignInfoExtendedStruct("", "", hashAlgorithm);
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object) signExtendedInfo));
            Marshal.StructureToPtr((object) signExtendedInfo, ptr, false);
            wizDigitalSignInfo.pSignExtInfo = ptr;
            return wizDigitalSignInfo;
        }

        public static NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO InitSignInfoExtendedStruct(string description, string moreInfoUrl, string hashAlgorithm)
        {
            NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO signExtendedInfo = new NativeStructs.CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO();
            signExtendedInfo.dwSize = (uint) Marshal.SizeOf((object) signExtendedInfo);
            signExtendedInfo.dwAttrFlagsNotUsed = 0U;
            signExtendedInfo.pwszDescription = description;
            signExtendedInfo.pwszMoreInfoLocation = moreInfoUrl;
            signExtendedInfo.pszHashAlg = (string) null;
            signExtendedInfo.pwszSigningCertDisplayStringNotUsed = IntPtr.Zero;
            signExtendedInfo.hAdditionalCertStoreNotUsed = IntPtr.Zero;
            signExtendedInfo.psAuthenticatedNotUsed = IntPtr.Zero;
            signExtendedInfo.psUnauthenticatedNotUsed = IntPtr.Zero;
            if (hashAlgorithm != null)
                signExtendedInfo.pszHashAlg = hashAlgorithm;
            return signExtendedInfo;
        }

        public struct CRYPTUI_WIZ_DIGITAL_SIGN_EXTENDED_INFO
        {
            internal uint dwSize;
            internal uint dwAttrFlagsNotUsed;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pwszDescription;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pwszMoreInfoLocation;
            [MarshalAs(UnmanagedType.LPStr)] internal string pszHashAlg;
            internal IntPtr pwszSigningCertDisplayStringNotUsed;
            internal IntPtr hAdditionalCertStoreNotUsed;
            internal IntPtr psAuthenticatedNotUsed;
            internal IntPtr psUnauthenticatedNotUsed;
        }

        public struct WINTRUST_BLOB_INFO
        {
            internal uint cbStruct;
            internal NativeGUID gSubject;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pcwszDisplayName;
            internal uint cbMemObject;
            internal IntPtr pbMemObject;
            internal uint cbMemSignedMsg;
            internal IntPtr pbMemSignedMsg;
        }

        public struct NativeGUID
        {
            internal uint Data1;
            internal ushort Data2;
            internal ushort Data3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] internal byte[] Data4;
        }

        public struct CRYPT_PROVIDER_SGNR
        {
            private uint cbStruct;
            private System.Runtime.InteropServices.ComTypes.FILETIME sftVerifyAsOf;
            private uint csCertChain;
            private IntPtr pasCertChain;
            private uint dwSignerType;
            private IntPtr psSigner;
            private uint dwError;
            internal uint csCounterSigners;
            internal IntPtr pasCounterSigners;
            private IntPtr pChainContext;
        }

        public struct WINTRUST_DATA
        {
            internal uint cbStruct;
            internal IntPtr pPolicyCallbackData;
            internal IntPtr pSIPClientData;
            internal uint dwUIChoice;
            internal uint fdwRevocationChecks;
            internal uint dwUnionChoice;
            internal WinTrust_Choice Choice;
            internal uint dwStateAction;
            internal IntPtr hWVTStateData;
            [MarshalAs(UnmanagedType.LPWStr)] internal string pwszURLReference;
            internal uint dwProvFlags;
            internal uint dwUIContext;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct WinTrust_Choice
        {
            [FieldOffset(0)] internal IntPtr pFile;
            [FieldOffset(0)] internal IntPtr pCatalog;
            [FieldOffset(0)] internal IntPtr pBlob;
            [FieldOffset(0)] internal IntPtr pSgnr;
            [FieldOffset(0)] internal IntPtr pCert;
        }
    }
}
