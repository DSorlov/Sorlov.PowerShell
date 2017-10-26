using System;
using System.Text;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Xml;
using System.Collections;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Convert, "XMLtoJSON")]
    [CmdletDescription("Converts a XML document to JSON",
        "Converts a XML document to JSON")]
    public class ConvertXmlToJson : PSCmdlet
    {

        #region "Private Parameters"
        private string filePath;
        private XmlDocument xmlDocument;
        private string xmlData;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0,HelpMessage = "The name of file",ParameterSetName="__AllParameterSets")]
        public string File
        {
            get { return filePath; }
            set { filePath = value; }
        }
        [Parameter(Position = 0, HelpMessage = "The name of file",ParameterSetName="XMLDocument")]
        public XmlDocument XmlDocument
        {
            get { return xmlDocument; }
            set { xmlDocument = value; }
        }
        [Parameter(Position = 0, HelpMessage = "The name of file",ParameterSetName="XmlData")]
        public string Xml
        {
            get { return xmlData; }
            set { xmlData = value; }
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
            XmlDocument xmlDoc = new XmlDocument();

            if (filePath != null)
                xmlDoc.Load(filePath);
            else if (xmlData != null)
                xmlDoc.LoadXml(xmlData);
            else if (xmlDocument != null)
                xmlDoc = xmlDocument;
            else
                ThrowTerminatingError(new ErrorRecord(new ArgumentException("You must specify a xmlFile, xmlData or a xmlDocument"), "100", ErrorCategory.InvalidData,""));

            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, xmlDoc.DocumentElement, true);
            sbJSON.Append("}");
            WriteObject(sbJSON.ToString());

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

        private void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
            sbJSON.Append("{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList childNodeNames = new SortedList();

            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode.ChildNodes[0] is XmlCDataSection)
                    StoreChildNode(childNodeNames, cnode.Name, cnode.ChildNodes[0].InnerText);
                else if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                ArrayList alChild = (ArrayList)childNodeNames[childname];
                if (alChild.Count == 1)
                    OutputNode(childname, alChild[0], sbJSON, true);
                else
                {
                    sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ ");
                    foreach (object Child in alChild)
                        OutputNode(childname, Child, sbJSON, false);
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            object oValuesAL = childNodeNames[nodeName];
            ArrayList ValuesAL;
            if (oValuesAL == null)
            {
                ValuesAL = new ArrayList();
                childNodeNames[nodeName] = ValuesAL;
            }
            else
                ValuesAL = (ArrayList)oValuesAL;
            ValuesAL.Add(nodeValue);
        }

        private void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();

                Double temp;
                if (Double.TryParse(sChild, out temp))
                    sbJSON.Append(SafeJSON(sChild));
                else
                    sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
            }
            else
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);

            string temp2 = sbJSON.ToString().Trim();
            if (temp2.Substring(temp2.Length - 1) != ",")
                sbJSON.Append(", ");
        }

        // Make a string safe for JSON
        private string SafeJSON(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }



    }
}
