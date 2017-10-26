using System;
using System.Collections.Generic;
using System.Linq;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.SelfHosted.Dto
{
    [Serializable]
    [PSReturnableObject]
    public class SelfHostedPSInfo
    {
        private string version;
        private string script;
        private string copyright;
        private string company;
        private string validVersion;
        private string appType = "Application";
        private string signatureStatus;
        private string signedBy;
        private string timestampBy;
        private string path;
        private string architecture;

        private DateTime? generatedDate;
        private string generatedBy;
        
        private string generatorOwner;
        private string generatorCompany;
        private string generatorVersion;


        private Dictionary<string,List<string>> manifest;

        public override string ToString()
        {
            return script;
        }

        public SelfHostedPSInfo(Dictionary<string, string> assemblyInfo, Dictionary<string,List<string>> manifest, string signatureStatus, string signedBy, string timestampBy, string path)
        {

            version = assemblyInfo["AssemblyFileVersionAttribute"];
            script = assemblyInfo["AssemblyTitleAttribute"];
            company = assemblyInfo["AssemblyCompanyAttribute"];
            copyright = assemblyInfo["AssemblyCopyrightAttribute"];
            architecture = assemblyInfo["AssemblyArchitecture"];
            validVersion = "1.1.0.0";

            try
            {
                string[] tempData = assemblyInfo["AssemblyDescriptionAttribute"].Split(';');

                generatedDate = DateTime.Parse(tempData[0]);
                generatedBy = tempData[1];
                generatorCompany = tempData[2];
                generatorOwner = tempData[3];
                generatorVersion = tempData[4];

                if (tempData.Count() >= 6)
                {
                    validVersion = "1.2.0.0";
                    if (tempData[5] == "True")
                        appType = "Service";
                }
            }
            catch
            {
                generatedDate = null;
                generatedBy = null;
                generatorOwner = null;
                generatorCompany = null;
                generatorVersion = null;
                validVersion = "1.0.1.0";
            }

            this.manifest = manifest;
            if (manifest == null) validVersion = "1.0.0.0";

            if (validVersion=="1.0.0.0" && !script.Contains("ps1"))
            {
                validVersion = "Not_Valid";
                version = null;
                script = null;
                company = null;
                copyright = null;
            }

            this.signatureStatus = signatureStatus;
            this.signedBy = signedBy;
            this.timestampBy = timestampBy;
            this.path = path;
        }

        private string[] GetManifestArray(string key)
        {
            if (manifest == null)
                return new string[] { };

            if (!manifest.ContainsKey(key))
                return new string[] {};

            return manifest[key].ToArray();
        }

        [PSPropertyView(ColumnName = "File", Default = true, ColumnOutput = true, Sequence = 1)]
        public string Path { get { return path; } }
        [PSPropertyView(ColumnOutput = false)]
        public string ImageVersion { get { return validVersion; } }

        [PSPropertyView(ColumnOutput = false)]
        public string ScriptName { get { return script; } }
        [PSPropertyView(ColumnOutput = true, ColumnName = "Version", Sequence = 2, ColumnWidth = 10)]
        public string ScriptVersion { get { return version; } }
        [PSPropertyView(ColumnOutput = false)]
        public string Copyright { get { return copyright; } }
        [PSPropertyView(ColumnOutput = false)]
        public string Company { get { return company; } }

        [PSPropertyView(ColumnName = "Generated", ColumnOutput = true, Sequence = 5, ColumnWidth = 20)]
        public DateTime? GeneratedDate { get { return generatedDate; } }
        [PSPropertyView(ColumnOutput = false)]
        public string GeneratedBy { get { return generatedBy; } }
        [PSPropertyView(ColumnOutput = false)]
        public string GeneratorVersion { get { return generatorVersion; } }
        [PSPropertyView(ColumnOutput = false)]
        public string GeneratorOwner { get { return generatorOwner; } }
        [PSPropertyView(ColumnOutput = false)]
        public string GeneratorCompany { get { return generatorCompany; } }
        [PSPropertyView(ColumnOutput = true, ColumnName = "Type", Sequence = 4, ColumnWidth = 12)]
        public string ApplicationType { get { return appType; } }

        [PSPropertyView(ColumnName = "Signature", ColumnOutput = true, Sequence = 3, ColumnWidth = 10)]
        public string SignatureStatus { get { return signatureStatus; } }
        [PSPropertyView(ColumnOutput = false)]
        public string SignedBy { get { return signedBy; } }
        [PSPropertyView(ColumnOutput = false)]
        public string TimestampedBy { get { return timestampBy; } }

        [PSPropertyView(ColumnOutput = false)]
        public string[] Modules { get { return GetManifestArray("Module"); } }
        [PSPropertyView(ColumnOutput = false)]
        public string[] Files { get { return GetManifestArray("Copy"); } }
        [PSPropertyView(ColumnOutput = false)]
        public string Architecture { get { return architecture; } }

    }

}
