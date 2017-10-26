using System;
using System.Runtime.InteropServices;

namespace Sorlov.PowerShell.Lib.API
{
    public static class DWMAPI
    {
        [DllImport("dwmapi.dll", EntryPoint = "#127", PreserveSig = false)]
        public static extern void DwmGetColorizationParameters(out DWM_COLORIZATION_PARAMS parameters);

        [DllImport("dwmapi.dll", EntryPoint = "#131", PreserveSig = false)]
        public static extern void DwmSetColorizationParameters(ref DWM_COLORIZATION_PARAMS parameters, long uUnknown);

        [DllImport("dwmapi.dll")]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DwmIsCompositionEnabled();

        public const int DWM_BB_ENABLE = 0x00000001;

        public enum DWM_BB
        {
            Enable = 1,
            BlurRegion = 2,
            TransitionMaximized = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_COLORIZATION_PARAMS
        {
            public UInt32 ColorizationColor;
            public UInt32 ColorizationAfterglow;
            public UInt32 ColorizationColorBalance;
            public UInt32 ColorizationAfterglowBalance;
            public UInt32 ColorizationBlurBalance;
            public UInt32 ColorizationGlassReflectionIntensity;
            public UInt32 ColorizationOpaqueBlend;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DWM_BB dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }


    }
}
