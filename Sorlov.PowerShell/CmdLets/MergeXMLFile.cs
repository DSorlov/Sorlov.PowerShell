using System.Text;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Xml;
using System.Data;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Merge, "XMLFile")]
    [CmdletDescription("Merges two XML files",
        "This cmdlet merges the XML files")]
    [Example(Code="file1.xml, file2.xml, file3.xml | Merge-XMLFile | Out-File mergedXML.xml",Remarks="Merges all the files sent via pipe")]
    public class MergeXMLFile : PSCmdlet
    {

        #region "Private Parameters"
        private string filePath;
        private DataSet resultData;
        #endregion

        #region "Public Parameters"
        [Parameter(ValueFromPipeline = true)]
        public PSObject inputObject
        {
            get { return PSObject.AsPSObject(filePath); }
            set { filePath = value.ToString(); }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");

            resultData = new DataSet();
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            XmlTextReader reader = new XmlTextReader(filePath);
            DataSet newDataSet = new DataSet();
            newDataSet.ReadXml(reader);

            resultData.Merge(newDataSet);
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");

            MemoryStream stream = new MemoryStream();
            resultData.WriteXml(stream);

            WriteObject(Encoding.UTF8.GetString(stream.ToArray()));
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
