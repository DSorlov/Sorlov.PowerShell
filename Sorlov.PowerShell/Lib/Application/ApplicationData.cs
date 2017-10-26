using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Automation;

namespace Sorlov.PowerShell.SelfHosted.Lib.Application
{
    public class ApplicationData
    {
        public string ApplicationName = string.Empty;
        public Bitmap Icon = null;
        public bool HideConsole = false;
        public bool DebugBuild = false;
        public bool EmbedCore = false;
        public int LCID = 0;
        public Framework Framework;
        public Platform Platform = Platform.anycpu;
        public ThreadMode Mode = ThreadMode.MTA;
        public PSModuleInfo[] EmbedModules;
        public Version Version = new Version(1, 0, 0, 0);
        public string PublisherName = string.Empty;
        public string PublisherOrganization = string.Empty;
        public SingingInformation SigningInformation = null;

        public string ReplaceProgram = string.Empty;
        public List<string> AdditionalCode = new List<string>();
        public List<string> AdditionalFiles = new List<string>();
        public List<string> AdditionalAssemblies = new List<string>();
    }
}
