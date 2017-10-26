using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Reflection;
using System.Xml;
using System.IO;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Lib.Core
{
    public class XMLHelpGenerator
    {
        private static string copyright = null;

        private static XmlTextWriter writer = null;


        public static void GenerateHelp(string inputFile, string outputPath, bool oneFile)
        {
            GenerateHelp(Assembly.LoadFrom(inputFile), outputPath, oneFile);
        }

        public static void GenerateHelp(Assembly asm, string outputPath, bool oneFile)
        {
            var attr = asm.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false);
            if (attr.Length == 1)
                copyright = ((AssemblyCopyrightAttribute) attr[0]).Copyright;

            attr = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attr.Length == 1)
                copyright += ((AssemblyDescriptionAttribute)attr[0]).Description;


            StringBuilder sb = new StringBuilder();
            writer = new XmlTextWriter(new StringWriter(sb));
            writer.Formatting = Formatting.Indented;

            if (oneFile)
            {
                writer.WriteStartElement("helpItems");
                writer.WriteAttributeString("xmlns", "http://msh");
                writer.WriteAttributeString("schema", "maml");
            }

            foreach (Type type in asm.GetExportedTypes())
            {
                CmdletAttribute ca = GetAttribute<CmdletAttribute>(type);
                NoAutoDocAttribute ha = GetAttribute<NoAutoDocAttribute>(type);
                if (ca != null && ha == null)
                {

                    if (!oneFile)
                    {
                        writer.WriteStartElement("helpItems");
                        writer.WriteAttributeString("xmlns", "http://msh");
                        writer.WriteAttributeString("schema", "maml");
                    }


                    writer.WriteStartElement("command", "command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
                    writer.WriteAttributeString("xmlns", "maml", null, "http://schemas.microsoft.com/maml/2004/1");
                    writer.WriteAttributeString("xmlns", "dev", null, "http://schemas.microsoft.com/maml/dev/2004/10");

                    writer.WriteStartElement("command", "details", null);

                    writer.WriteElementString("command", "name", null, string.Format("{0}-{1}", ca.VerbName, ca.NounName));

                    WriteDescription(type, true, false);

                    WriteCopyright();

                    writer.WriteElementString("command", "verb", null, ca.VerbName);
                    writer.WriteElementString("command", "noun", null, ca.NounName);

                    writer.WriteElementString("dev", "version", null, asm.GetName().Version.ToString());

                    writer.WriteEndElement(); //command:details

                    WriteDescription(type, false, true);

                    WriteSyntax(ca, type);
                    
                    writer.WriteStartElement("command", "parameters", null);
                    
                    foreach (PropertyInfo pi in type.GetProperties())
                    {
                        List<ParameterAttribute> pas = GetAttribute<ParameterAttribute>(pi);
                        if (pas == null)
                            continue;

                        ParameterAttribute pa = null;
                        if (pas.Count == 1)
                            pa = pas[0];
                        else
                        {
                            // Determine the defualt property parameter set to use for details.
                            ParameterAttribute defaultPA = null;
                            foreach (ParameterAttribute temp in pas)
                            {
                                string defaultSet = ca.DefaultParameterSetName;
                                if (string.IsNullOrEmpty(ca.DefaultParameterSetName))
                                    defaultSet = string.Empty;

                                string set = temp.ParameterSetName;
                                if (string.IsNullOrEmpty(set) || set == DEFAULT_PARAMETER_SET_NAME)
                                {
                                    set = string.Empty;
                                    defaultPA = temp;
                                }
                                if (set.ToLower() == defaultSet.ToLower())
                                {
                                    pa = temp;
                                    defaultPA = temp;
                                    break;
                                }
                            }
                            if (pa == null && defaultPA != null)
                                pa = defaultPA;
                            if (pa == null)
                                pa = pas[0];
                        }
                        
                        writer.WriteStartElement("command", "parameter", null);
                        writer.WriteAttributeString("required", pa.Mandatory.ToString().ToLower());

                        bool supportsWildcard = GetAttribute<PowerShell.Lib.Core.Attributes.SupportsWildcardsAttribute>(pi) != null;
                        writer.WriteAttributeString("globbing", supportsWildcard.ToString().ToLower());

                        if (!pa.ValueFromPipeline && !pa.ValueFromPipelineByPropertyName)
                            writer.WriteAttributeString("pipelineInput", "false");
                        else if (pa.ValueFromPipeline && pa.ValueFromPipelineByPropertyName)
                            writer.WriteAttributeString("pipelineInput", "true (ByValue, ByPropertyName)");
                        else if (!pa.ValueFromPipeline && pa.ValueFromPipelineByPropertyName)
                            writer.WriteAttributeString("pipelineInput", "true (ByPropertyName)");
                        else if (pa.ValueFromPipeline && !pa.ValueFromPipelineByPropertyName)
                            writer.WriteAttributeString("pipelineInput", "true (ByValue)");

                        if (pa.Position < 0)
                            writer.WriteAttributeString("position", "named");
                        else
                            writer.WriteAttributeString("position", (pa.Position + 1).ToString());

                        bool variableLength = pi.PropertyType.IsArray;
                        writer.WriteAttributeString("variableLength", variableLength.ToString().ToLower());

                        writer.WriteElementString("maml", "name", null, pi.Name);

                        if (pi.PropertyType.Name == "SPAssignmentCollection")
                            WriteSPAssignmentCollectionDescription();
                        else
                            WriteDescription(pa.HelpMessage, false);

                        writer.WriteStartElement("command", "parameterValue", null);
                        writer.WriteAttributeString("required", pa.Mandatory.ToString().ToLower());
                        writer.WriteAttributeString("variableLength", variableLength.ToString().ToLower());
                        writer.WriteValue(pi.PropertyType.Name);
                        writer.WriteEndElement(); //command:parameterValue

                        WriteDevType(pi.PropertyType.Name, null);

                        writer.WriteEndElement(); //command:parameter
                    }
                    writer.WriteEndElement(); //command:parameters

                    //TODO: Find out what is supposed to go here
                    writer.WriteStartElement("command", "inputTypes", null);
                    writer.WriteStartElement("command", "inputType", null);
                    WriteDevType(null, null);
                    writer.WriteEndElement(); //command:inputType
                    writer.WriteEndElement(); //command:inputTypes

                    writer.WriteStartElement("command", "returnValues", null);
                    writer.WriteStartElement("command", "returnValue", null);
                    WriteDevType(null, null);
                    writer.WriteEndElement(); //command:returnValue
                    writer.WriteEndElement(); //command:returnValues

                    writer.WriteElementString("command", "terminatingErrors", null, null);
                    writer.WriteElementString("command", "nonTerminatingErrors", null, null);

                    writer.WriteStartElement("maml", "alertSet", null);
                    writer.WriteElementString("maml", "title", null, null);
                    writer.WriteStartElement("maml", "alert", null);
                    WritePara(string.Format("For more information, type \"Get-Help {0}-{1} -detailed\". For technical information, type \"Get-Help {0}-{1} -full\".", 
                        ca.VerbName, ca.NounName));
                    writer.WriteEndElement(); //maml:alert
                    writer.WriteEndElement(); //maml:alertSet

                    WriteExamples(type);
                    WriteRelatedLinks(type);

                    writer.WriteEndElement(); //command:command

                    if (!oneFile)
                    {
                        writer.WriteEndElement(); //helpItems
                        writer.Flush();
                        File.WriteAllText(Path.Combine(outputPath, string.Format("{0}.dll-help.xml", type.Name)), sb.ToString());
                        sb = new StringBuilder();
                        writer = new XmlTextWriter(new StringWriter(sb));
                        writer.Formatting = Formatting.Indented;
                    }
                }
            }

            if (oneFile)
            {
                writer.WriteEndElement(); //helpItems
                writer.Flush();
                File.WriteAllText(Path.Combine(outputPath, string.Format("{0}.dll-help.xml", asm.GetName().Name)), sb.ToString());
            }
        }

        const string DEFAULT_PARAMETER_SET_NAME = "__AllParameterSets";

        private static void WriteSyntax(CmdletAttribute ca, Type type)
        {
            Dictionary<string, List<PropertyInfo>> parameterSets = new Dictionary<string, List<PropertyInfo>>();

            List<PropertyInfo> defaultSet = null;
            foreach (PropertyInfo pi in type.GetProperties())
            {
                List<ParameterAttribute> pas = GetAttribute<ParameterAttribute>(pi);
                if (pas == null)
                    continue;

                foreach (ParameterAttribute temp in pas)
                {
                    string set = temp.ParameterSetName + "";
                    List<PropertyInfo> piList = null;
                    if (!parameterSets.ContainsKey(set))
                    {
                        piList = new List<PropertyInfo>();
                        parameterSets.Add(set, piList);
                    }
                    else
                        piList = parameterSets[set];
                    parameterSets[set].Add(pi);
                }
            }

            if (parameterSets.Count > 0)
            {
                try
                {
                    defaultSet = parameterSets[DEFAULT_PARAMETER_SET_NAME];

                    if (parameterSets.Count > 1 && parameterSets.ContainsKey(DEFAULT_PARAMETER_SET_NAME))
                        parameterSets.Remove(DEFAULT_PARAMETER_SET_NAME);
                    else
                        defaultSet = null;
                }
                catch
                {
                    defaultSet = null;
                }

                writer.WriteStartElement("command", "syntax", null);
                foreach (string parameterSetName in parameterSets.Keys)
                {
                    WriteSyntaxItem(ca, parameterSets, parameterSetName, defaultSet);
                }
                writer.WriteEndElement(); //command:syntax
            }
        }

       
        private static void WriteSyntaxItem(CmdletAttribute ca, Dictionary<string, List<PropertyInfo>> parameterSets, string parameterSetName, List<PropertyInfo> defaultSet)
        {
            writer.WriteStartElement("command", "syntaxItem", null);
            writer.WriteElementString("maml", "name", null, string.Format("{0}-{1}", ca.VerbName, ca.NounName));
            foreach (PropertyInfo pi in parameterSets[parameterSetName])
            {
                ParameterAttribute pa = GetParameterAttribute(pi, parameterSetName);
                if (pa == null)
                    continue;

                WriteParameter(pi, pa);
            }
            if (defaultSet != null)
            {
                foreach (PropertyInfo pi in defaultSet)
                {
                    List<ParameterAttribute> pas = GetAttribute<ParameterAttribute>(pi);
                    if (pas == null)
                        continue;
                    WriteParameter(pi, pas[0]);
                }
            }
            writer.WriteEndElement(); //command:syntaxItem
        }
 
        private static ParameterAttribute GetParameterAttribute(PropertyInfo pi, string parameterSetName)
        {
            List<ParameterAttribute> pas = GetAttribute<ParameterAttribute>(pi);
            if (pas == null)
                return null;
            ParameterAttribute pa = null;
            foreach (ParameterAttribute temp in pas)
            {
                if (temp.ParameterSetName.ToLower() == parameterSetName.ToLower())
                {
                    pa = temp;
                    break;
                }
            }
            return pa;
        }

        private static void WriteParameter(PropertyInfo pi, ParameterAttribute pa)
        {
            writer.WriteStartElement("command", "parameter", null);
            writer.WriteAttributeString("required", pa.Mandatory.ToString().ToLower());
            //writer.WriteAttributeString("parameterSetName", pa.ParameterSetName);
            if (pa.Position < 0)
                writer.WriteAttributeString("position", "named");
            else
                writer.WriteAttributeString("position", (pa.Position + 1).ToString());

            writer.WriteElementString("maml", "name", null, pi.Name);
            writer.WriteStartElement("command", "parameterValue", null);

            if (pi.DeclaringType == typeof(PSCmdlet))
                writer.WriteAttributeString("required", "false");
            else
                writer.WriteAttributeString("required", "true");

            if (pi.PropertyType.Name == "Nullable`1")
            {
                Type coreType = pi.PropertyType.GetGenericArguments()[0];
                if (coreType.IsEnum)
                    writer.WriteValue(string.Join(" | ", Enum.GetNames(coreType)));
                else
                    writer.WriteValue(coreType.Name);
            }
            else
            {
                if (pi.PropertyType.IsEnum)
                    writer.WriteValue(string.Join(" | ", Enum.GetNames(pi.PropertyType)));
                else
                    writer.WriteValue(pi.PropertyType.Name);
            }

            writer.WriteEndElement(); //command:parameterValue
            writer.WriteEndElement(); //command:parameter
        }

        private static void WriteDevType(string name, string description)
        {
            writer.WriteStartElement("dev", "type", null);
            writer.WriteElementString("maml", "name", null, name);
            writer.WriteElementString("maml", "uri", null, null);
            WriteDescription(description, false);
            writer.WriteEndElement(); //dev:type
        }

        private static void WriteSPAssignmentCollectionDescription()
        {
            WriteDescription("Manages objects for the purpose of proper disposal. Use of objects, such as SPWeb or SPSite, can use large amounts of memory and use of these objects in Windows PowerShell scripts requires proper memory management. Using the SPAssignment object, you can assign objects to a variable and dispose of the objects after they are needed to free up memory. When SPWeb, SPSite, or SPSiteAdministration objects are used, the objects are automatically disposed of if an assignment collection or the Global parameter is not used.\r\n\r\nWhen the Global parameter is used, all objects are contained in the global store. If objects are not immediately used, or disposed of by using the Stop-SPAssignment command, an out-of-memory scenario can occur.", false);
        }

        private static void WriteDescription(Type type, bool synopsis, bool addCopyright)
        {
            writer.WriteStartElement("maml", "description", null);
            CmdletDescriptionAttribute da = GetAttribute<CmdletDescriptionAttribute>(type);
            string desc = string.Empty;
            if (synopsis)
            {
                if (da != null && !string.IsNullOrEmpty(da.Synopsis))
                {
                    desc = da.Synopsis;
                }
            }
            else
            {
                if (da != null && !string.IsNullOrEmpty(da.Description))
                {
                    desc = da.Description;
                }
            }

            WritePara(desc);
            if (addCopyright)
            {
                WritePara(null);
                WritePara(copyright);
            }
            
            writer.WriteEndElement(); //maml:description
        }

        private static void WriteDescription(string desc, bool addCopyright)
        {
            writer.WriteStartElement("maml", "description", null);
            WritePara(desc);
            if (addCopyright)
            {
                WritePara(null);
                WritePara(copyright);
            }
            writer.WriteEndElement(); //maml:description
        }

        private static void WriteExamples(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(ExampleAttribute), true);
            if (attrs == null || attrs.Length == 0)
            {
                writer.WriteElementString("command", "examples", null, null);
            }
            else
            {
                writer.WriteStartElement("command", "examples", null);

                for (int i = 0; i < attrs.Length; i++)
                {
                    ExampleAttribute ex = (ExampleAttribute)attrs[i];
                    writer.WriteStartElement("command", "example", null);
                    if (attrs.Length == 1)
                        writer.WriteElementString("maml", "title", null, "------------------EXAMPLE------------------");
                    else
                        writer.WriteElementString("maml", "title", null, string.Format("------------------EXAMPLE {0}-----------------------", i + 1));

                    writer.WriteElementString("dev", "code", null, ex.Code);
                    writer.WriteStartElement("dev", "remarks", null);
                    WritePara(ex.Remarks);
                    writer.WriteEndElement(); //dev:remarks
                    writer.WriteEndElement(); //command:example
                }
                writer.WriteEndElement(); //command:examples
            }
        }

        private static void WriteRelatedLinks(Type type)
        {
            RelatedCmdletsAttribute attr = GetAttribute<RelatedCmdletsAttribute>(type);
            
            if (attr == null)
            {
                writer.WriteElementString("maml", "relatedLinks", null, null);
            }
            else
            {
                writer.WriteStartElement("maml", "relatedLinks", null);

                foreach (Type t in attr.RelatedCmdlets)
                {
                    CmdletAttribute ca = GetAttribute<CmdletAttribute>(t);
                    if (ca == null)
                        continue;

                    writer.WriteStartElement("maml", "navigationLink", null);
                    writer.WriteElementString("maml", "linkText", null, ca.VerbName + "-" + ca.NounName);
                    writer.WriteElementString("maml", "uri", null, null);
                    writer.WriteEndElement(); //maml:navigationLink
                }
                if (attr.ExternalCmdlets != null)
                {
                    foreach (string s in attr.ExternalCmdlets)
                    {
                        writer.WriteStartElement("maml", "navigationLink", null);
                        writer.WriteElementString("maml", "linkText", null, s);
                        writer.WriteElementString("maml", "uri", null, null);
                        writer.WriteEndElement(); //maml:navigationLink
                    }
                }
                writer.WriteEndElement(); //maml:relatedLinks
            }
        }

        private static T GetAttribute<T>(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(T), true);
            if (attrs == null || attrs.Length == 0)
                return default(T);
            return (T)attrs[0];
        }
        private static List<T> GetAttribute<T>(PropertyInfo pi)
        {
            object[] attrs = pi.GetCustomAttributes(typeof(T), true);
            List<T> attributes = new List<T>();
            if (attrs == null || attrs.Length == 0)
                return null;

            foreach (T t in attrs)
            {
                attributes.Add(t);
            }
            return attributes;
        }

        private static void WriteCopyright()
        {
            writer.WriteStartElement("maml", "copyright", null);
            WritePara(copyright);
            writer.WriteEndElement(); //maml:copyright
        }

        private static void WritePara(string para)
        {
            if (string.IsNullOrEmpty(para))
            {
                writer.WriteElementString("maml", "para", null, null);
                return;
            }
            string[] paragraphs = para.Split(new[] {"\r\n"}, StringSplitOptions.None);
            foreach (string p in paragraphs)
                writer.WriteElementString("maml", "para", null, p);
        }
    }
}
