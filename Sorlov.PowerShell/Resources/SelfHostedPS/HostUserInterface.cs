using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Sorlov.SelfHostedPS.Application
{
    class HostUserInterface : PSHostUserInterface, IHostUISupportsMultipleChoiceSelection
    {
        private PSHostRawUserInterface rawUserInterface = new HostRawUserInterface();

        #pragma warning disable 0649
        private static ITaskbarList3 taskBarList;
        #pragma warning restore 0649

        internal enum TBPFLAG
        {
            TBPF_NOPROGRESS = 0,
            TBPF_INDETERMINATE = 0x1,
            TBPF_NORMAL = 0x2,
            TBPF_ERROR = 0x4,
            TBPF_PAUSED = 0x8
        }


        internal enum TBATFLAG
        {
            TBATF_USEMDITHUMBNAIL = 0x1,
            TBATF_USEMDILIVEPREVIEW = 0x2
        }

        internal enum THBMASK
        {
            THB_BITMAP = 0x1,
            THB_ICON = 0x2,
            THB_TOOLTIP = 0x4,
            THB_FLAGS = 0x8
        }

        internal enum THBFLAGS
        {
            THBF_ENABLED = 0,
            THBF_DISABLED = 0x1,
            THBF_DISMISSONCLICK = 0x2,
            THBF_NOBACKGROUND = 0x4,
            THBF_HIDDEN = 0x8
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct THUMBBUTTON
        {
            [MarshalAs(UnmanagedType.U4)]
            public THBMASK dwMask;
            public uint iId;
            public uint iBitmap;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szTip;
            [MarshalAs(UnmanagedType.U4)]
            public THBFLAGS dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [ComImportAttribute()]
        [GuidAttribute("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ITaskbarList3
        {
            // ITaskbarList
            [PreserveSig]
            void HrInit();
            [PreserveSig]
            void AddTab(IntPtr hwnd);
            [PreserveSig]
            void DeleteTab(IntPtr hwnd);
            [PreserveSig]
            void ActivateTab(IntPtr hwnd);
            [PreserveSig]
            void SetActiveAlt(IntPtr hwnd);

            // ITaskbarList2
            [PreserveSig]
            void MarkFullscreenWindow(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

            // ITaskbarList3
            void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
            void SetProgressState(IntPtr hwnd, TBPFLAG tbpFlags);
            void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
            void UnregisterTab(IntPtr hwndTab);
            void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
            void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, TBATFLAG tbatFlags);
            void ThumbBarAddButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
            void ThumbBarUpdateButtons(
                IntPtr hwnd,
                uint cButtons,
                [MarshalAs(UnmanagedType.LPArray)] THUMBBUTTON[] pButtons);
            void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
            void SetOverlayIcon(
              IntPtr hwnd,
              IntPtr hIcon,
              [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
            void SetThumbnailTooltip(
                IntPtr hwnd,
                [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
            void SetThumbnailClip(
                IntPtr hwnd,
                /*[MarshalAs(UnmanagedType.LPStruct)]*/ ref RECT prcClip);
        }


        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr ptr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere, int authError, ref uint authPackage, IntPtr InAuthBuffer, uint InAuthBufferSize, out IntPtr refOutAuthBuffer, out uint refOutAuthBufferSize, ref bool fSave, int flags);


        public override PSHostRawUserInterface RawUI
        {
            get { return rawUserInterface; }
        }

        public override string ReadLine()
        {
            if (ConsoleHandler.ConsoleLoaded)
                return Console.ReadLine();
            else
                return string.Empty;
        }

        public override SecureString ReadLineAsSecureString()
        {
            SecureString pwd = new SecureString();

            if (ConsoleHandler.ConsoleLoaded)
            {
                while (true)
                {
                    ConsoleKeyInfo i = Console.ReadKey(true);
                    if (i.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    else if (i.Key == ConsoleKey.Backspace)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                    else
                    {
                        pwd.AppendChar(i.KeyChar);
                        Console.Write("*");
                    }
                }
            }
            return pwd;
        }

        public override void Write(string value)
        {
            ConsoleHandler.Write(value ?? "null");
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            ConsoleColor origFgColor = ConsoleHandler.ForegroundColor;
            ConsoleColor origBgColor = ConsoleHandler.BackgroundColor;
            ConsoleHandler.ForegroundColor = foregroundColor;
            ConsoleHandler.BackgroundColor = backgroundColor;
            ConsoleHandler.Write(value ?? "null");
            ConsoleHandler.ForegroundColor = origFgColor;
            ConsoleHandler.BackgroundColor = origBgColor;
        }

        public override void WriteLine(string value)
        {
            ConsoleHandler.WriteLine(value ?? "null");
        }

        public override void WriteErrorLine(string value)
        {
            string output = value ?? "null";
            ConsoleColor origFgColor = ConsoleHandler.ForegroundColor;
            ConsoleHandler.ForegroundColor = ConsoleColor.Red;
            ConsoleHandler.WriteLine(output);
            ConsoleHandler.ForegroundColor = origFgColor;
        }

        public override void WriteDebugLine(string message)
        {
            Write(ConsoleColor.Yellow, ConsoleHandler.BackgroundColor, string.Format("DEBUG: {0}", message ?? "null"));
        }

        public override void WriteVerboseLine(string message)
        {
            Write(ConsoleColor.Yellow, ConsoleHandler.BackgroundColor, string.Format("VERBOSE: {0}", message ?? "null"));
        }

        public override void WriteWarningLine(string message)
        {
            Write(ConsoleColor.Yellow, ConsoleHandler.BackgroundColor, string.Format("WARNING: {0}", message ?? "null"));
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            IntPtr hWnd = GetConsoleWindow();

           if (record.PercentComplete == 100)
               taskBarList.SetProgressState(hWnd, TBPFLAG.TBPF_NOPROGRESS);
           else
               taskBarList.SetProgressState(hWnd, TBPFLAG.TBPF_NORMAL);

           taskBarList.SetProgressValue(hWnd, unchecked((ulong)record.PercentComplete), unchecked((ulong)100));
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            if (String.IsNullOrEmpty(caption) && String.IsNullOrEmpty(message) && descriptions.Count > 0)
            {
                ConsoleHandler.Write(descriptions[0].Name + ": ");
            }
            else
            {
                this.Write(ConsoleColor.DarkCyan, ConsoleColor.Black, caption + "\n" + message + " ");
            }
            Dictionary<string, PSObject> results = new Dictionary<string, PSObject>();
            foreach (FieldDescription fd in descriptions)
            {
                string[] label = GetHotkeyAndLabel(fd.Label);
                this.WriteLine(label[1]);
                string userData = ConsoleHandler.ReadLine();
                if (userData == null)
                {
                    return null;
                }

                results[fd.Name] = PSObject.AsPSObject(userData);
            }

            return results;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            return PromptForCredential(caption, message, userName, targetName, PSCredentialTypes.Default, PSCredentialUIOptions.Default);
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            PSCredential result = null;
            CREDUI_INFO credui = new CREDUI_INFO();
            credui.pszCaptionText = caption;
            credui.pszMessageText = message;
            credui.cbSize = Marshal.SizeOf(credui);
            uint authPackage = 0;
            IntPtr outCredBuffer = new IntPtr();
            uint outCredSize;
            bool save = false;
            int w32Result = CredUIPromptForWindowsCredentials(ref credui, 0, ref authPackage, IntPtr.Zero, 0, out outCredBuffer, out outCredSize, ref save, 1);

            int maxLength = 100;
            StringBuilder usernameBuf = new StringBuilder(maxLength);
            StringBuilder passwordBuf = new StringBuilder(maxLength);
            StringBuilder domainBuf = new StringBuilder(maxLength);

            if (w32Result == 0)
            {
                if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxLength, domainBuf, ref maxLength, passwordBuf, ref maxLength))
                {
                    CoTaskMemFree(outCredBuffer);

                    SecureString password = new SecureString();
                    foreach (char c in passwordBuf.ToString())
                        password.AppendChar(c);

                    string domainName = domainBuf.ToString();

                    if (domainName != null && domainName != string.Empty)
                        result = new PSCredential(string.Format("{0}\\{1}", domainName, usernameBuf.ToString()), password);
                    else
                        result = new PSCredential(usernameBuf.ToString(), password);
                }
            }

            return result;

        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            // Write the caption and message strings in Blue.
            this.WriteLine(ConsoleColor.Blue, ConsoleColor.Black, caption + "\n" + message + "\n");

            // Convert the choice collection into something that is
            // easier to work with. See the BuildHotkeysAndPlainLabels 
            // method for details.
            string[,] promptData = BuildHotkeysAndPlainLabels(choices);

            // Format the overall choice prompt string to display.
            StringBuilder sb = new StringBuilder();
            for (int element = 0; element < choices.Count; element++)
            {
                sb.Append(String.Format(CultureInfo.CurrentCulture, "|{0}> {1} ", promptData[0, element], promptData[1, element]));
            }

            sb.Append(String.Format(CultureInfo.CurrentCulture, "[Default is ({0}]", promptData[0, defaultChoice]));

            // Read prompts until a match is made, the default is
            // chosen, or the loop is interrupted with ctrl-C.
            while (true)
            {
                this.WriteLine(sb.ToString());
                string data = ConsoleHandler.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

                // If the choice string was empty, use the default selection.
                if (data.Length == 0)
                {
                    return defaultChoice;
                }

                // See if the selection matched and return the
                // corresponding index if it did.
                for (int i = 0; i < choices.Count; i++)
                {
                    if (promptData[0, i] == data)
                    {
                        return i;
                    }
                }

                this.WriteErrorLine("Invalid choice: " + data);
            }
        }

        #region IHostUISupportsMultipleChoiceSelection Members

        public Collection<int> PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, IEnumerable<int> defaultChoices)
        {
            this.WriteLine(ConsoleColor.Blue, ConsoleColor.Black, caption + "\n" + message + "\n");

            string[,] promptData = BuildHotkeysAndPlainLabels(choices);

            StringBuilder sb = new StringBuilder();
            for (int element = 0; element < choices.Count; element++)
            {
                sb.Append(String.Format(CultureInfo.CurrentCulture, "|{0}> {1} ", promptData[0, element], promptData[1, element]));
            }

            Collection<int> defaultResults = new Collection<int>();
            if (defaultChoices != null)
            {
                int countDefaults = 0;
                foreach (int defaultChoice in defaultChoices)
                {
                    ++countDefaults;
                    defaultResults.Add(defaultChoice);
                }

                if (countDefaults != 0)
                {
                    sb.Append(countDefaults == 1 ? "[Default choice is " : "[Default choices are ");
                    foreach (int defaultChoice in defaultChoices)
                    {
                        sb.AppendFormat(CultureInfo.CurrentCulture, "\"{0}\",", promptData[0, defaultChoice]);
                    }
                    sb.Remove(sb.Length - 1, 1);
                    sb.Append("]");
                }
            }

            this.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, sb.ToString());

            Collection<int> results = new Collection<int>();
            while (true)
            {
            ReadNext:
                string prompt = string.Format(CultureInfo.CurrentCulture, "Choice[{0}]:", results.Count);
                this.Write(ConsoleColor.Cyan, ConsoleColor.Black, prompt);
                string data = ConsoleHandler.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

                if (data.Length == 0)
                {
                    return (results.Count == 0) ? defaultResults : results;
                }

                for (int i = 0; i < choices.Count; i++)
                {
                    if (promptData[0, i] == data)
                    {
                        results.Add(i);
                        goto ReadNext;
                    }
                }

                this.WriteErrorLine("Invalid choice: " + data);
            }
        }

        #endregion

        private static string[,] BuildHotkeysAndPlainLabels(Collection<ChoiceDescription> choices)
        {
            // Allocate the result array
            string[,] hotkeysAndPlainLabels = new string[2, choices.Count];

            for (int i = 0; i < choices.Count; ++i)
            {
                string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
                hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
                hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
            }

            return hotkeysAndPlainLabels;
        }

        private static string[] GetHotkeyAndLabel(string input)
        {
            string[] result = new string[] { String.Empty, String.Empty };
            string[] fragments = input.Split('&');
            if (fragments.Length == 2)
            {
                if (fragments[1].Length > 0)
                {
                    result[0] = fragments[1][0].ToString().
                    ToUpper(CultureInfo.CurrentCulture);
                }

                result[1] = (fragments[0] + fragments[1]).Trim();
            }
            else
            {
                result[1] = input;
            }

            return result;
        }
    }

}
