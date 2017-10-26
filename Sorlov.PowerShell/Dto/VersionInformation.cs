using System;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Dto
{
    [PSReturnableObject()]
    public class VersionInformation
    {
        private string componentInfo;
        private Version installedVersion;
        private Version releasedVersion;
        private string url;
        private string md5;
        private DateTime releaseDate;
        private string assemblyPath;
        private bool stableRelease;


        internal VersionInformation(string componentInfo, Version installedVersion, Version releasedVersion, string url, string md5, DateTime releaseDate, string assemblyPath, bool stableRelease)
        {
            this.componentInfo = componentInfo;
            this.installedVersion = installedVersion;
            this.releasedVersion = releasedVersion;
            this.url = url;
            this.md5 = md5;
            this.releaseDate = releaseDate;
            this.assemblyPath = assemblyPath;
            this.stableRelease = stableRelease;
        }

        [PSPropertyView(ColumnOutput = true, Default = true, Sequence = 1, ColumnName = "ComponentName")]
        public string Component { get { return componentInfo; } }

        [PSPropertyView(ColumnOutput = true, Sequence = 2, ColumnName = "InstalledVersion")]
        public Version Installed { get { return installedVersion; } }

        [PSPropertyView(ColumnOutput = true, Sequence = 3, ColumnName = "ReleasedVersion")]
        public Version Released { get { return releasedVersion; } }
        internal string URL { get { return url; } }
        internal string MD5 { get { return md5; } }
        internal DateTime ReleaseDate { get { return releaseDate; } }
        internal string AssemblyPath { get { return assemblyPath; } }

        [PSPropertyView(ColumnOutput = true, Sequence = 4, ColumnName = "UpgradeRecomended")]
        public bool ShouldUpgrade { get { if (installedVersion == null) return false; else return installedVersion.CompareTo(releasedVersion) != 0; } }

        [PSPropertyView(ColumnOutput = true, Sequence = 5, ColumnName = "IsInstalled")]
        public bool IsInstalled { get { if (installedVersion == null) return false; else return true; } }

        [PSPropertyView(ColumnOutput = true, Sequence = 6, ColumnName = "IsStableRelease")]
        public bool Stable { get { return stableRelease; } }
        
    }
}
