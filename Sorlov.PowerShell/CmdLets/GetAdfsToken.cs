using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.Protocols.WSTrust.Bindings;
using Microsoft.IdentityModel.Tokens;
using Sorlov.PowerShell.Cmdlets;
using Sorlov.PowerShell.Lib.Core;
using Sorlov.PowerShell.Lib.Core.Attributes;
using WSTrustChannelFactory = Microsoft.IdentityModel.Protocols.WSTrust.WSTrustChannelFactory;


namespace Sorlov.PowerShell.CmdLets
{
    [Cmdlet(VerbsCommon.Get, "AdfsToken", DefaultParameterSetName = "Cleartext")]
    [OutputType(typeof(System.IdentityModel.Tokens.SamlSecurityToken),ParameterSetName=new string[]{"Cleartext","Securestring","Credentials"})]
    [CmdletDescription("Gets a token for usage with adfs requests",
        "This cmdlet makes a call to and adfs server and requests a ticket that can be used to request services from claims protected http resources.")]
    public class GetAdfsToken : PSCmdlet
    {
        #region "Private Parameters"
        private string adfsRequestUrl = "/adfs/services/trust/13/usernamemixed";
        private string adfsServer;
        private string requestProtocol = "https";
        private string adfsAudience = "urn:federation:MicrosoftOnline";
        private string username;
        private string password;
        private string outputFormat = "XmlDocument";
        private SecureString securePassword;
        private PSCredential credentials;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of the ADFS server", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName=true)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of the ADFS server", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of the ADFS server", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string AdfsServer
        {
            get { return adfsServer; }
            set { adfsServer = value; }
        }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The username", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The username", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The password in cleartext", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The password in password", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public SecureString SecurePassword
        {
            get { return securePassword ; }
            set { securePassword = value; }
        }

        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The password in password", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public PSCredential Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The ADFS audience", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "The ADFS audience", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 2, Mandatory = false, HelpMessage = "The ADFS audience", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string AdfsAudience
        {
            get { return adfsAudience; }
            set { adfsAudience = value; }
        }

        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Provide a custom url for ADFS server url", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Provide a custom url for ADFS server url", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 3, Mandatory = false, HelpMessage = "Provide a custom url for ADFS server url", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string AdfsRequestUrl
        {
            get { return adfsRequestUrl; }
            set { adfsRequestUrl = value; }
        }

        [Parameter(Position = 5, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 5, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 4, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("Http", "Https")]
        [ValidateNotNullOrEmpty()]
        public string RequestProtocol
        {
            get { return requestProtocol; }
            set { requestProtocol = value; }
        }

        [Parameter(Position = 6, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Cleartext", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 6, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Securestring", ValueFromPipelineByPropertyName = true)]
        [Parameter(Position = 5, Mandatory = false, HelpMessage = "Protocol for request to ADFS server", ParameterSetName = "Credentials", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("XmlDocument", "XmlToken", "SamlToken")]
        [ValidateNotNullOrEmpty()]
        public string OutputFormat
        {
            get { return outputFormat; }
            set { outputFormat = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            WriteVerbose("Process record");

            string connectionPassword;
            string connectionUsername;
            switch (this.ParameterSetName)
            {
                case "Securestring":
                {
                    connectionPassword = password;
                    connectionUsername = securePassword.ToUnsecureString();
                    break;
                }
                case "Credentials":
                {
                    connectionPassword = credentials.Password.ToUnsecureString();
                    connectionUsername = credentials.UserName;
                    break;
                }
                default:
                {
                    connectionPassword = password;
                    connectionUsername = username;
                    break;
                }                   
            }

            string serverUrl = string.Format("{0}://{1}{2}", requestProtocol, adfsServer, adfsRequestUrl);
            try
            {
                var factory = new WSTrustChannelFactory(new UserNameWSTrustBinding(SecurityMode.TransportWithMessageCredential), new EndpointAddress(serverUrl)) { TrustVersion = TrustVersion.WSTrust13 };
                factory.Credentials.UserName.UserName = connectionUsername;
                factory.Credentials.UserName.Password = connectionPassword;
                var rst = new RequestSecurityToken
                {
                    RequestType = WSTrust13Constants.RequestTypes.Issue,
                    AppliesTo = new EndpointAddress(adfsAudience),
                    KeyType = WSTrust13Constants.KeyTypes.Bearer
                };

                var channel = factory.CreateChannel();
                var genericToken = channel.Issue(rst) as GenericXmlSecurityToken;

                if (outputFormat.ToLower() == "xmltoken")
                {
                    WriteObject(genericToken.TokenXml.OuterXml);
                }
                else if (outputFormat.ToLower() == "samltoken")
                {
                    var tokenHandler = System.IdentityModel.Tokens.SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
                    var tokenString = genericToken.TokenXml.OuterXml;
                    WriteObject(tokenHandler.ReadToken(new XmlTextReader(new StringReader(tokenString))));
                }
                else
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(genericToken.TokenXml.OuterXml);
                    WriteObject(xmlDocument);                    
                }

            }
            catch (Exception ex)
            {
              WriteError(new ErrorRecord(new SecurityException(ex.Message),"100",ErrorCategory.AuthenticationError, null));
            }
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stop processing");
        }
        #endregion
    }
}
