using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Sorlov.PowerShell.Lib.Core;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation.Runspaces;
using System.Xml;
using System.Reflection;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommon.Exit, "PSEnvironment", SupportsShouldProcess = true)]
    [CmdletDescription("Internal exit hook for the Sorlov.PowerShell environment",
        "This command is mostly for internal use or to save the enviroment during a session")]
    public class ExitPSEnvironment: PSCmdlet
    {
        #region "Private Parameters"
        string parFile = ".";
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = false, HelpMessage = "The file or directory to touch",ValueFromPipeline=true)]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }
        #endregion

        private XmlNode CreateXmlNode(XmlNode parentNode, string name, Dictionary<string,string> attributes)
        {
            XmlNode newNode = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, name, "");

            if (attributes!=null)
                foreach(KeyValuePair<string,string> attribute in attributes)
                {
                    XmlAttribute attrib = parentNode.OwnerDocument.CreateAttribute(attribute.Key);
                    attrib.Value = attribute.Value;
                    newNode.Attributes.Append(attrib);
                }

            parentNode.AppendChild(newNode);
            return newNode;
        }

        private XmlNode CreateXmlNode(XmlNode parentNode, string name)
        {
            return CreateXmlNode(parentNode, name, null);
        }

        private Dictionary<string, string> CreateSimpleDictionary(string name, string value)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add(name, value);
            return result;
        }




        private ProgressRecord progressRecord = null;

        private void WriteProgress(string progressStatus, int completePercentage)
        {
            if (progressRecord == null) progressRecord = new ProgressRecord(200, "Exiting Sorlov.PowerShell..", progressStatus);
            progressRecord.PercentComplete = completePercentage;
            progressRecord.StatusDescription = progressStatus;

            WriteProgress(progressRecord);
        }


        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            string pathName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WindowsPowershell\\Modules\\Sorlov.PowerShell");

            base.BeginProcessing();

                WriteProgress("Initializing..",0);

                WriteProgress("Creating configuration..",10);

                XmlDocument xmlDocument = new XmlDocument();
                XmlNode xmlRoot = xmlDocument.CreateNode(XmlNodeType.Element, "PSState", "");
                xmlDocument.AppendChild(xmlRoot);

                if (LibraryConfiguration.IsTrue("PersistPath"))
                {

                    WriteProgress("General settings..",20);
                    XmlNode xmlGeneral = CreateXmlNode(xmlRoot, "General");
                    CreateXmlNode(xmlGeneral, "Pwd", CreateSimpleDictionary("Value", SessionState.Path.CurrentLocation.ProviderPath));
                }

                if (LibraryConfiguration.IsTrue("PersistHistory"))
                {
                    WriteProgress("History..",30);
                    List<PSObject> history = SessionState.InvokeCommand.InvokeScript("Get-History -Count 100").ToList<PSObject>();
                    XmlNode xmlHistory = CreateXmlNode(xmlRoot, "HistoryList");
                    foreach (PSObject historyObject in history)
                    {
                        Dictionary<string, string> props = CreateSimpleDictionary("Id", historyObject.Properties["Id"].Value.ToString());
                        props.Add("CommandLine", historyObject.Properties["CommandLine"].Value.ToString());
                        props.Add("ExecutionStatus", historyObject.Properties["ExecutionStatus"].Value.ToString());
                        props.Add("StartExecutionTime", historyObject.Properties["StartExecutionTime"].Value.ToString());
                        props.Add("EndExecutionTime", historyObject.Properties["EndExecutionTime"].Value.ToString());
                        CreateXmlNode(xmlHistory, "History", props);
                    }
                }

                if (LibraryConfiguration.IsTrue("PersistAliases"))
                {
                    SessionStateAliasEntry[] aliasInfo = typeof(System.Management.Automation.Runspaces.InitialSessionState).GetProperty("BuiltInAliases", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, new object[0]) as SessionStateAliasEntry[];

                    Dictionary<string, SessionStateAliasEntry> builtInAliases = new Dictionary<string, SessionStateAliasEntry>();
                    foreach (SessionStateAliasEntry entry in aliasInfo)
                        builtInAliases.Add(entry.Name.ToLower(), entry);

                    WriteProgress("Aliases..",50);
                    XmlNode xmlAlias = CreateXmlNode(xmlRoot, "AliasList");
                    List<PSObject> alias = SessionState.InvokeCommand.InvokeScript("Get-Alias").ToList<PSObject>();
                    foreach (PSObject aliasObject in alias)
                    {
                        if (!builtInAliases.ContainsKey(aliasObject.Properties["Name"].Value.ToString().ToLower()))
                        {
                            Dictionary<string, string> props = CreateSimpleDictionary("Name", aliasObject.Properties["Name"].Value.ToString());
                            props.Add("Definition", aliasObject.Properties["Definition"].Value.ToString());
                            CreateXmlNode(xmlAlias, "Alias", props);
                        }
                    }
                }

                WriteProgress("Saving..",90);
                string savePath = System.IO.Path.Combine(pathName, "Sorlov.PowerShell.PersistedState.xml");
                xmlDocument.Save(savePath);

                WriteProgress("Done",100);
            }

        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
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
