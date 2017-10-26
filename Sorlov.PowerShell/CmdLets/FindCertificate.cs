using System;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Security.Cryptography.X509Certificates;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Find, "Certificate")]
    [CmdletDescription("Retrieves a certificate from the machine store",
        "This command simplified finding a specific certficate from the stores")]
    [OutputType(typeof(X509Certificate2))]
    public class FindCertificate: PSCmdlet
    {
        #region "Private Parameters"
        string storeLocation = "CurrentUser";
        string filter = null;
        X509FindType findBy;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 3, HelpMessage = "Which store to search", ValueFromPipeline = true)]
        [ValidateSet("LocalMachine", "CurrentUser")]
        public string StoreLocation
        {
            get { return storeLocation; }
            set { storeLocation = value.ToUpper(); }
        }

        [Parameter(Position = 0,Mandatory=true, HelpMessage = "What attribute should be used to find certificate", ValueFromPipeline = true)]
        [ValidateSet("Thumbprint", "SubjectName", "SubjectDistinguishedName", "IssuerName", "IssuerDistinguishedName","SerialNumber", "Extension", "KeyUsage", "SubjectKeyIdentifier")]
        public string FindBy
        {
            get { return findBy.ToString().Replace("FindBy",""); }
            set { findBy = (X509FindType)Enum.Parse(typeof(X509FindType), string.Format("FindBy{0}",value)); }
        }

        [Parameter(Position = 1,Mandatory=true, HelpMessage = "The value", ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string Value
        {
            get { return filter; }
            set { filter = value; }
        }



        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            foreach(StoreName storeName in Enum.GetValues(typeof(StoreName)))
            {
                X509Store searchStore;
                if (storeLocation == "LOCALMACHINE")
                    searchStore = new X509Store(storeName, System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine);
                else
                    searchStore = new X509Store(storeName, System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser);

                searchStore.Open(OpenFlags.ReadOnly);
                X509CertificateCollection result = searchStore.Certificates.Find(findBy, Value, false);
                foreach (X509Certificate cert in result)
                    WriteObject(cert);

                searchStore.Close();
            }

        }
        #endregion

    }
}
