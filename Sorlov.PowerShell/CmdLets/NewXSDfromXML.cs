using System.Text;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "XSDfromXML")]
    [CmdletDescription("Creates a XSD from XML",
        "This cmdlet creates a new XSD based on XML document")]
    public class NewXSDfromXML : PSCmdlet
    {

        #region "Private Parameters"
        private string filePath;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The name of file",ValueFromPipelineByPropertyName=true)]
        [ValidateNotNullOrEmpty()]
        public string Name
        {
            get { return filePath; }
            set { filePath = value; }
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

            XmlReader xmlReader = XmlReader.Create(filePath);

            XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
            XmlSchemaInference xmlSchemaInference = new XmlSchemaInference();

            xmlSchemaSet = xmlSchemaInference.InferSchema(xmlReader);

            foreach (XmlSchema xmlSchema in xmlSchemaSet.Schemas())
            {
                MemoryStream stream = new MemoryStream();
                xmlSchema.Write(stream);
                WriteObject(Encoding.UTF8.GetString(stream.ToArray()));
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
