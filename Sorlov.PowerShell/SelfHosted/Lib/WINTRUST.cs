using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Sorlov.PowerShell.SelfHosted.Lib
{
    public class WINTRUST
    {
        [DllImport("wintrust.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern uint WinVerifyTrust(IntPtr hWndNotUsed, IntPtr pgActionID, IntPtr pWinTrustData);

        [DllImport("wintrust.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr WTHelperProvDataFromStateData(IntPtr hStateData);

        [DllImport("wintrust.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr WTHelperGetProvSignerFromChain(IntPtr pProvData, uint idxSigner, uint fCounterSigner, uint idxCounterSigner);

        [DllImport("wintrust.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr WTHelperGetProvCertFromChain(IntPtr pSgnr, uint idxCert);

        public static NativeStructs.WINTRUST_DATA InitWintrustDataStructFromBlob(NativeStructs.WINTRUST_BLOB_INFO wbi)
        {
            NativeStructs.WINTRUST_DATA wintrustData = new NativeStructs.WINTRUST_DATA();
            wintrustData.cbStruct = (uint)Marshal.SizeOf((object)wbi);
            wintrustData.pPolicyCallbackData = IntPtr.Zero;
            wintrustData.pSIPClientData = IntPtr.Zero;
            wintrustData.dwUIChoice = 2U;
            wintrustData.fdwRevocationChecks = 0U;
            wintrustData.dwUnionChoice = 3U;
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)wbi));
            Marshal.StructureToPtr((object)wbi, ptr, false);
            wintrustData.Choice.pBlob = ptr;
            wintrustData.dwStateAction = 1U;
            wintrustData.hWVTStateData = IntPtr.Zero;
            wintrustData.pwszURLReference = (string)null;
            wintrustData.dwProvFlags = 0U;
            return wintrustData;
        }

        public static NativeStructs.WINTRUST_DATA InitWintrustDataStructFromFile(NativeStructs.WINTRUST_FILE_INFO wfi)
        {
            NativeStructs.WINTRUST_DATA wintrustData = new NativeStructs.WINTRUST_DATA();
            wintrustData.cbStruct = (uint)Marshal.SizeOf((object)wintrustData);
            wintrustData.pPolicyCallbackData = IntPtr.Zero;
            wintrustData.pSIPClientData = IntPtr.Zero;
            wintrustData.dwUIChoice = 2U;
            wintrustData.fdwRevocationChecks = 0U;
            wintrustData.dwUnionChoice = 1U;
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)wfi));
            Marshal.StructureToPtr((object)wfi, ptr, false);
            wintrustData.Choice.pFile = ptr;
            wintrustData.dwStateAction = 1U;
            wintrustData.hWVTStateData = IntPtr.Zero;
            wintrustData.pwszURLReference = (string)null;
            wintrustData.dwProvFlags = 0U;
            return wintrustData;
        }

        public static NativeStructs.WINTRUST_FILE_INFO InitWintrustFileInfoStruct(string fileName)
        {
            NativeStructs.WINTRUST_FILE_INFO wintrustFileInfo = new NativeStructs.WINTRUST_FILE_INFO();
            wintrustFileInfo.cbStruct = (uint)Marshal.SizeOf((object)wintrustFileInfo);
            wintrustFileInfo.pcwszFilePath = fileName;
            wintrustFileInfo.hFileNotUsed = IntPtr.Zero;
            wintrustFileInfo.pgKnownSubjectNotUsed = IntPtr.Zero;
            return wintrustFileInfo;
        }

        public static NativeStructs.WINTRUST_BLOB_INFO InitWintrustBlobInfoStruct(string fileName, string content)
        {
            NativeStructs.WINTRUST_BLOB_INFO wintrustBlobInfo = new NativeStructs.WINTRUST_BLOB_INFO();
            byte[] bytes = Encoding.Unicode.GetBytes(content);
            wintrustBlobInfo.gSubject.Data1 = 1614531615U;
            wintrustBlobInfo.gSubject.Data2 = (ushort)19289;
            wintrustBlobInfo.gSubject.Data3 = (ushort)19976;
            wintrustBlobInfo.gSubject.Data4 = new byte[8]
              {
                (byte) 183,
                (byte) 36,
                (byte) 210,
                (byte) 198,
                (byte) 41,
                (byte) 126,
                (byte) 243,
                (byte) 81
              };
            wintrustBlobInfo.cbStruct = (uint)Marshal.SizeOf((object)wintrustBlobInfo);
            wintrustBlobInfo.pcwszDisplayName = fileName;
            wintrustBlobInfo.cbMemObject = (uint)bytes.Length;
            wintrustBlobInfo.pbMemObject = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, wintrustBlobInfo.pbMemObject, bytes.Length);
            return wintrustBlobInfo;
        }

        public static uint DestroyWintrustDataStruct(NativeStructs.WINTRUST_DATA wtd)
        {
            uint num1 = 2147500037U;
            IntPtr num2 = IntPtr.Zero;
            IntPtr num3 = IntPtr.Zero;
            Guid guid = new Guid("00AAC56B-CD44-11d0-8CC2-00C04FC295EE");
            try
            {
                num2 = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)guid));
                Marshal.StructureToPtr((object)guid, num2, false);
                wtd.dwStateAction = 2U;
                num3 = Marshal.AllocCoTaskMem(Marshal.SizeOf((object)wtd));
                Marshal.StructureToPtr((object)wtd, num3, false);
                num1 = WINTRUST.WinVerifyTrust(IntPtr.Zero, num2, num3);
                wtd = (NativeStructs.WINTRUST_DATA)Marshal.PtrToStructure(num3, typeof(NativeStructs.WINTRUST_DATA));
            }
            finally
            {
                Marshal.DestroyStructure(num3, typeof(NativeStructs.WINTRUST_DATA));
                Marshal.FreeCoTaskMem(num3);
                Marshal.DestroyStructure(num2, typeof(Guid));
                Marshal.FreeCoTaskMem(num2);
            }
            if ((int)wtd.dwUnionChoice == 3)
            {
                Marshal.FreeCoTaskMem(((NativeStructs.WINTRUST_BLOB_INFO)Marshal.PtrToStructure(wtd.Choice.pBlob, typeof(NativeStructs.WINTRUST_BLOB_INFO))).pbMemObject);
                Marshal.DestroyStructure(wtd.Choice.pBlob, typeof(NativeStructs.WINTRUST_BLOB_INFO));
                Marshal.FreeCoTaskMem(wtd.Choice.pBlob);
            }
            else
            {
                Marshal.DestroyStructure(wtd.Choice.pFile, typeof(NativeStructs.WINTRUST_FILE_INFO));
                Marshal.FreeCoTaskMem(wtd.Choice.pFile);
            }
            return num1;
        }

    }
}
