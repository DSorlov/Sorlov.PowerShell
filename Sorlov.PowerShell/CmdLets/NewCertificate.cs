using System;
using System.Collections.Generic;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "Certificate")]  
    [CmdletDescription("Creates a new certificate")]
    [OutputType(typeof(System.Security.Cryptography.X509Certificates.X509Certificate2))]    
    public class NewCertificate: PSCmdlet
    {
        #region "Private Parameters"
        private string subjectName;
        private string algorithm = "SHA256WithRSA";
        private int keyLength = 0;
        private DateTime validFrom = DateTime.UtcNow.Date;
        private DateTime validTo = DateTime.UtcNow.Date.AddYears(2);
        private string method = "pipeline";
        private bool certificateAuthority = false;
        private System.Security.Cryptography.X509Certificates.X509Certificate2 signingCert;
        private bool purposeAny = false;
        private bool purposeClientAuth = false;
        private bool purposeCodeSigning = false;
        private bool purposeEmail = false;
        private bool purposeIpsecEnd = false;
        private bool purposeIpsecTunnel = false;
        private bool purposeIpsecUser = false;
        private bool purposeOcsp = false;
        private bool purposeServerAuth = false;
        private bool purposeSmartCardLogon = false;
        private bool purposeTimestamping = false;
        private string friendlyName = string.Empty;
        private bool trust = false;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The subject name to use for the certificate", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string SubjectName
        {
            get { return subjectName; }
            set { subjectName = value; }
        }
        [Parameter(Position = 1, HelpMessage = "Friendly name to use for the certificate", ValueFromPipelineByPropertyName = true)]
        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }
        [Parameter(HelpMessage = "Where to deliver the created certificate")]
        [ValidateSet("PIPELINE","CURRENTUSER","LOCALMACHINE")]
        public string Target
        {
            get { return method; }
            set { method = value.ToLower(); }
        }
        [Parameter(HelpMessage = "The algorithm to use for the key", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("MD2withRSA", "MD5withRSA", "SHA1withRSA", "RIPEMD128withRSA", "RIPEMD160withRSA", "RIPEMD256withRSA","SHA256WithRSA", "SHA384withRSA", "SHA512withRSA", "SHA1withRSAandMGF1","SHA256withRSAandMGF1", "SHA384withRSAandMGF1","SHA512withRSAandMGF1")]
        public string Algorithm
        {
            get { return algorithm; }
            set { algorithm = value; }
        }
        [Parameter(HelpMessage = "Length to use for the key", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("1024","2048","4096","8192")]
        public string KeyLength
        {
            get { return keyLength.ToString(); }
            set { keyLength = int.Parse(value); }
        }
        [Parameter(HelpMessage = "First valid date of the cert", ValueFromPipelineByPropertyName = true)]
        public DateTime ValidFrom
        {
            get { return validFrom; }
            set { validFrom = value; }
        }
        [Parameter(HelpMessage = "Last valid date of the cert", ValueFromPipelineByPropertyName = true)]
        public DateTime ValidTo
        {
            get { return validTo; }
            set { validTo = value; }
        }
        [Parameter(HelpMessage = "This is a certificate authority ", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter CertificateAuthority
        {
            get { return certificateAuthority; }
            set { certificateAuthority = value; }
        }
        [Parameter(HelpMessage = "Certificate used to sign the certificate, if not specified a X509 self-signed will be generated", ValueFromPipelineByPropertyName = true)]
        public System.Security.Cryptography.X509Certificates.X509Certificate2 SigningCertificate
        {
            get { return signingCert; }
            set { signingCert = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for any purpose", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter AnyPurpose
        {
            get { return purposeAny; }
            set { purposeAny = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for client authentication", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter ClientAuthentication
        {
            get { return purposeClientAuth; }
            set { purposeClientAuth = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for server authentication", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter ServerAuthentication
        {
            get { return purposeServerAuth; }
            set { purposeServerAuth = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for code signing ", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter CodeSigning
        {
            get { return purposeCodeSigning; }
            set { purposeCodeSigning = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for email protection", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter EmailProtection
        {
            get { return purposeEmail; }
            set { purposeEmail = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for oscp", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Ocsp
        {
            get { return purposeOcsp; }
            set { purposeOcsp = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for smart card login", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter SmartCardLogin
        {
            get { return purposeSmartCardLogon; }
            set { purposeSmartCardLogon = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for timestamping", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Timestamping
        {
            get { return purposeTimestamping; }
            set { purposeTimestamping = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for ipsec end systems", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter IPSecEnd
        {
            get { return purposeIpsecEnd; }
            set { purposeIpsecEnd = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for ipsec tunnels", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter IPSecTunnel
        {
            get { return purposeIpsecTunnel; }
            set { purposeIpsecTunnel = value; }
        }
        [Parameter(HelpMessage = "The certificate can be used for users", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter IPSecUser
        {
            get { return purposeIpsecUser; }
            set { purposeIpsecUser = value; }
        }
        [Parameter(HelpMessage = "Add trust for the certificate", ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Trust
        {
            get { return trust; }
            set { trust = value; }
        }


        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {

        }
        #endregion

        #region "BeginProcessing"
        protected override void ProcessRecord()
        {
            // Create friendly name if not specified
            if (friendlyName == string.Empty) friendlyName = subjectName;

            // Check if key length is specified
            if (keyLength == 0)
                if (certificateAuthority) keyLength = 2048; else keyLength = 1024;

            // init random
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);

            // generate a new key pair for the cert
            KeyGenerationParameters keyGenerationParameters = new KeyGenerationParameters(random, keyLength);
            RsaKeyPairGenerator keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            AsymmetricCipherKeyPair subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

            // setup the name and serial number we are creating
            X509Name subjectDN = new X509Name(subjectName);
            X509Name issuerDN = subjectDN;
            BigInteger subjectSerialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
            BigInteger issuerSerialNumber = subjectSerialNumber;

            // setup the generator
            X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

            // Check if we are signing a real certificate
            if (signingCert != null)
            {
                issuerDN = new X509Name(signingCert.Subject);
                issuerKeyPair = DotNetUtilities.GetKeyPair(signingCert.PrivateKey);
                issuerSerialNumber = new BigInteger(signingCert.GetSerialNumber());
            }

            // General options
            certificateGenerator.SetSerialNumber(subjectSerialNumber);
            certificateGenerator.SetSignatureAlgorithm(algorithm);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);
            certificateGenerator.SetNotBefore(validFrom);
            certificateGenerator.SetNotAfter(validTo);
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            List<KeyPurposeID> certPurpose = new List<KeyPurposeID>();

            // Is it a CA cert?
            certificateGenerator.AddExtension(X509Extensions.BasicConstraints.Id, true, new BasicConstraints(certificateAuthority));
            if (certificateAuthority)
            {
                certificateGenerator.AddExtension(X509Extensions.KeyUsage.Id, false, new KeyUsage(KeyUsage.KeyCertSign | KeyUsage.CrlSign));
                certPurpose.Add(KeyPurposeID.IdKPServerAuth);
                certPurpose.Add(KeyPurposeID.IdKPClientAuth);
                certPurpose.Add(KeyPurposeID.IdKPEmailProtection);
                certPurpose.Add(KeyPurposeID.IdKPTimeStamping);
                certPurpose.Add(KeyPurposeID.IdKPCodeSigning);
            }
            else
            {
                certificateGenerator.AddExtension(X509Extensions.KeyUsage.Id, false, new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.DataEncipherment | KeyUsage.KeyEncipherment | KeyUsage.KeyAgreement));
                if (purposeAny) certPurpose.Add(KeyPurposeID.AnyExtendedKeyUsage);
                if (purposeClientAuth) certPurpose.Add(KeyPurposeID.IdKPClientAuth);
                if (purposeCodeSigning) certPurpose.Add(KeyPurposeID.IdKPCodeSigning);
                if (purposeEmail) certPurpose.Add(KeyPurposeID.IdKPEmailProtection);
                if (purposeIpsecEnd) certPurpose.Add(KeyPurposeID.IdKPIpsecEndSystem);
                if (purposeIpsecTunnel) certPurpose.Add(KeyPurposeID.IdKPIpsecTunnel);
                if (purposeIpsecUser) certPurpose.Add(KeyPurposeID.IdKPIpsecUser);
                if (purposeOcsp) certPurpose.Add(KeyPurposeID.IdKPOcspSigning);
                if (purposeServerAuth) certPurpose.Add(KeyPurposeID.IdKPServerAuth);
                if (purposeSmartCardLogon) certPurpose.Add(KeyPurposeID.IdKPSmartCardLogon);
                if (purposeTimestamping) certPurpose.Add(KeyPurposeID.IdKPTimeStamping);
                if (!purposeClientAuth && !purposeCodeSigning && !purposeEmail && !purposeIpsecEnd && !purposeIpsecTunnel && !purposeIpsecUser && !purposeOcsp && !purposeServerAuth && !purposeSmartCardLogon && !purposeTimestamping && !purposeAny) certPurpose.Add(KeyPurposeID.AnyExtendedKeyUsage);
            }
 
            certificateGenerator.AddExtension(X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(certPurpose.ToArray()));

            // Set the SKI
            SubjectKeyIdentifier subjectKeyIdentifier = new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public));
            certificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifier);

            // Authority key identifier
            AuthorityKeyIdentifier authorityKeyIdentifier = new AuthorityKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public), new GeneralNames(new GeneralName(issuerDN)), issuerSerialNumber);
            certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifier);
            
            // Generate the cert
            X509Certificate certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
            System.Security.Cryptography.X509Certificates.X509Certificate2 privateKeyCert = ConvertCertificate(subjectKeyPair.Private, certificate, friendlyName);
            switch (method)
            {
                case "pipeline":
                    WriteObject(privateKeyCert);
                    break;
                case "currentuser":
                case "localmachine":
                    AddToStore(privateKeyCert, System.Security.Cryptography.X509Certificates.StoreName.My, method);
                    break;
            }

            if (trust)
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 publicOnlyCert = ConvertCertificatePublic(certificate, friendlyName);
                switch (method)
                {
                    case "pipeline":
                        WriteWarning("Trust was specified by can only be used together with target PERSONAL or MACHINE, no trust created");
                        break;
                    case "currentuser":
                    case "localmachine":
                        if (certificateAuthority)
                        {
                            AddToStore(publicOnlyCert,System.Security.Cryptography.X509Certificates.StoreName.Root, method);
                        }
                        else if (purposeAny || purposeCodeSigning)
                        {
                            AddToStore(publicOnlyCert, System.Security.Cryptography.X509Certificates.StoreName.TrustedPeople, method);
                            AddToStore(publicOnlyCert, System.Security.Cryptography.X509Certificates.StoreName.TrustedPublisher, method);
                        }
                        else
                        {
                            AddToStore(publicOnlyCert, System.Security.Cryptography.X509Certificates.StoreName.TrustedPeople, method);
                        }
                        break;
                }
            }

        }
        #endregion

        private void AddToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName storeName, string storeLocation)
        {
            System.Security.Cryptography.X509Certificates.StoreLocation location;

            if (storeLocation == "currentuser") location = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser; else location = System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine;

            System.Security.Cryptography.X509Certificates.X509Store store = new System.Security.Cryptography.X509Certificates.X509Store(storeName, location);
            store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();

        }

        private void ExportToPFX(AsymmetricKeyParameter privateKey, X509Certificate input, string fileName, string password)
        {
            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);
            Pkcs12Store store = new Pkcs12Store();
            X509CertificateEntry certificateEntry = new X509CertificateEntry(input);

            string friendlyName = input.SubjectDN.ToString();
            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey), new[] { certificateEntry });

            var stream = new MemoryStream();

            // Get the cert
            store.Save(stream, password.ToCharArray(), random);
            File.WriteAllBytes(fileName, stream.ToArray());
            
            stream.Close();
        }

        private System.Security.Cryptography.X509Certificates.X509Certificate2 ConvertCertificatePublic(X509Certificate input, string friendlyName)
        {
            const string password = "password";

            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);
            Pkcs12Store store = new Pkcs12Store();
            X509CertificateEntry certificateEntry = new X509CertificateEntry(input);

            store.SetCertificateEntry(friendlyName, certificateEntry);
            //store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey), new[] { certificateEntry });

            var stream = new MemoryStream();
            store.Save(stream, password.ToCharArray(), random);
            System.Security.Cryptography.X509Certificates.X509Certificate2 result = new System.Security.Cryptography.X509Certificates.X509Certificate2(stream.ToArray(), password);
            stream.Close();

            return result;
        }

        private System.Security.Cryptography.X509Certificates.X509Certificate2 ConvertCertificate(AsymmetricKeyParameter privateKey, X509Certificate input, string friendlyName)
        {
            const string password = "password";

            CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
            SecureRandom random = new SecureRandom(randomGenerator);
            Pkcs12Store store = new Pkcs12Store();
            X509CertificateEntry certificateEntry = new X509CertificateEntry(input);

            store.SetCertificateEntry(friendlyName, certificateEntry);
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(privateKey), new[] { certificateEntry });

            var stream = new MemoryStream();
            store.Save(stream,password.ToCharArray(), random);
            System.Security.Cryptography.X509Certificates.X509Certificate2 result = new System.Security.Cryptography.X509Certificates.X509Certificate2(stream.ToArray(), password, System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.PersistKeySet | System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.Exportable);
            stream.Close();

            return result;
        }

    }
}
