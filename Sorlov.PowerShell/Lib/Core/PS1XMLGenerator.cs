using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using Sorlov.PowerShell.Lib.Core.Attributes;

namespace Sorlov.PowerShell.Lib.Core
{
    /// <summary>
    /// Copyright (C) 2008 TeX HeX of Xteq System
    /// http://texhex.blogspot.com/
    /// http://www.texhex.info/
    /// </summary>
    public static class PS1XMLGenerator
    {
       #region Types XML constanst
private const string CTypeHeaderStart=""+
"<Type>\r\n"+
"  <Name>{0}</Name>\r\n"+
"  <Members>\r\n"+
"    <MemberSet>\r\n"+
"      <Name>PsStandardMembers</Name>\r\n"+
"      <Members>\r\n";

private const string CTypeDefaultProperty=""+
"       <NoteProperty>\r\n"+
"          <Name>DefaultDisplayProperty</Name>\r\n"+
"          <Value>{0}</Value>\r\n"+
"       </NoteProperty>\r\n";


private const string CTypePropertySetHeader=""+
"       <PropertySet>\r\n"+
"          <Name>{0}</Name>\r\n"+
"          <ReferencedProperties>\r\n";

private const string CTypePropertySetItem=""+
"             <Name>{0}</Name>\r\n";

private const string CTypePropertySetEnd=""+
"         </ReferencedProperties>\r\n"+
"       </PropertySet>\r\n";

private const string CTypeAliasProperty=""+
"    <AliasProperty>\r\n"+
"       <Name>{0}</Name>\r\n"+
"       <ReferencedMemberName>{1}</ReferencedMemberName>\r\n"+
"    </AliasProperty>\r\n";

private const string CTypePsStandardMemberEnd = ""+
"      </Members>\r\n"+
"    </MemberSet>\r\n";

private const string CTypeHeaderEnd=""+
"  </Members>\r\n" +
"</Type>";

#endregion

       #region Format XML const
private const string CFormatHeaderStart=""+
"<View>\r\n"+
"  <Name>{0}</Name>\r\n"+
"  <ViewSelectedBy>\r\n"+
"    <TypeName>{1}</TypeName>\r\n"+
"  </ViewSelectedBy>\r\n"+
"  <TableControl>\r\n";

private const string CFormatHeaderEnd=""+
"  </TableControl>\r\n"+
"</View>";

private const string CFormatTableHeaderStart=""+
"    <TableHeaders>\r\n";

private const string CFormatTableHeaderItemStart=""+
"       <TableColumnHeader>\r\n";

private const string CFormatTableHeaderItemLabel=""+
"          <Label>{0}</Label>\r\n";

private const string CFormatTableHeaderItemAlignment=""+
"          <Alignment>{0}</Alignment>\r\n";

private const string CFormatTableHeaderItemWidth=""+
"          <Width>{0}</Width>\r\n";


private const string CFormatTableHeaderItemEnd=""+
"       </TableColumnHeader>\r\n";

private const string CFormatTableHeaderEnd = "" +
"    </TableHeaders>\r\n";


private const string CFormatRowEntriesStart=""+
"    <TableRowEntries>\r\n"+
"       <TableRowEntry>\r\n"+
"          <TableColumnItems>\r\n";

private const string CFormatRowEntriesEnd = "" +
"          </TableColumnItems>\r\n"+
"       </TableRowEntry>\r\n"+
"    </TableRowEntries>\r\n";


private const string CFormatRowEntryItemProp = ""+
"          <TableColumnItem>\r\n"+
"              <PropertyName>{0}</PropertyName>\r\n"+
"          </TableColumnItem>\r\n";

private const string CFormatRowEntryItemScript = "" +
"          <TableColumnItem>\r\n" +
"              <ScriptBlock>{0}</ScriptBlock>\r\n" +
"          </TableColumnItem>\r\n";
        #endregion

        public static void GeneratePS1XML(string sourceDir, string outputPath, string filePrefix)
        {
            List<Assembly> assList = new List<Assembly>();

            foreach (string file in Directory.GetFiles(sourceDir, "Sorlov.PowerShell.*.dll"))
                assList.Add(Assembly.LoadFrom(file));

            GeneratePS1XML(assList, outputPath, filePrefix);
        }

        public static void GeneratePS1XML(List<Assembly> asmList, string outputPath, string filePrefix)
        {
            string typeXML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                             "<Types>\r\n";

            string formatXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                               "<Configuration>\r\n" +
                               "  <ViewDefinitions>\r\n";

            foreach (Assembly asm in asmList) {
                foreach (Type type in asm.GetExportedTypes())
                {
                    PSReturnableObjectAttribute ca = GetAttribute<PSReturnableObjectAttribute>(type);
                    if (ca != null) {

                        PS1XMLOutput output = GetPS1XML(type);
                        typeXML += output.TypeXML;
                        formatXML += output.FormatXML;

                    }
            }

            }

            formatXML += Resources.General.Format;
            typeXML += Resources.General.Types;


            formatXML += "  </ViewDefinitions>\r\n"+
                         "</Configuration>";

            typeXML += "</Types>";

                string formatXMLName = string.Format("{0}\\{1}.Format.ps1xml", outputPath, filePrefix);
                string typeXMLName = string.Format("{0}\\{1}.Types.ps1xml", outputPath, filePrefix);

                if (File.Exists(formatXMLName)) File.Delete(formatXMLName);
                File.WriteAllText(formatXMLName, formatXML);
                if (File.Exists(typeXMLName)) File.Delete(typeXMLName);
                File.WriteAllText(typeXMLName, typeXML);
    }

        private static T GetAttribute<T>(Type type)
        {
            object[] attrs = type.GetCustomAttributes(typeof(T), true);
            if (attrs == null || attrs.Length == 0)
                return default(T);
            return (T)attrs[0];
        }


        private static
                    PS1XMLOutput GetPS1XML(Type t)
        {
            #region Get type data using reflection

            string sTypeName = t.ToString();
            string[] sTypeNameArray = sTypeName.Split('.');
            string sTypeNameSimple = sTypeNameArray[sTypeNameArray.Length-1];
            string sDefault = "";

            
            List<PropertyAttributeTuple> _listProps = new List<PropertyAttributeTuple>();
            List<PropertyAttributeTuple> _listSortProps = new List<PropertyAttributeTuple>();
            NameValueCollection _nvcAlias = new NameValueCollection();
            

            //enumerate over all Propertiers in this class
            foreach (PropertyInfo property in t.GetProperties())
            {
                //Get all custom attributes
                foreach (Attribute attr in property.GetCustomAttributes(true))
                {
                    // Normal property
                    if (attr is PSPropertyViewAttribute)
                    {
                        PSPropertyViewAttribute prop_attr = (PSPropertyViewAttribute)attr;

                        if (prop_attr.Default)
                        {
                            if (sDefault.Length > 0)
                            {
                                throw new ArgumentException(
                                      string.Format(
                                        "Can not set property '{0}' as default since property '{1}' is already default",
                                        property.Name, sDefault
                                       )
                                );
                            }
                            //If an alias has been specified, use it 
                            sDefault = prop_attr.Alias.Length > 0 ? prop_attr.Alias : property.Name;
                        }

                        PropertyAttributeTuple tuple = new PropertyAttributeTuple();
                        tuple.PropertyName = property.Name;
                        //If an alias has been specified, use it 
                        tuple.Name = prop_attr.Alias.Length > 0 ? prop_attr.Alias : property.Name;
                        tuple.SortIndicator = prop_attr.Sequence;
                        tuple.Attr = prop_attr;

                        _listProps.Add(tuple);


                        //Check if this attribute defines an alias. If so, add it to the alias list!
                        if (prop_attr.Alias.Length > 0)
                        {
                            string sExisting=_nvcAlias.Get(prop_attr.Alias);
                            if (sExisting == null)
                            {
                                _nvcAlias.Add(prop_attr.Alias, property.Name);
                            }
                            else
                            {
                                throw new ArgumentException(
                                      string.Format(
                                        "The alias '{0}' for '{2}' can not be defined, since it is already an alias for '{1}'!",
                                        prop_attr.Alias, sExisting, property.Name
                                       )
                                );

                            }
                        }

                    }
                    else
                    {
                        //Sort properties
                        if (attr is PSPropertySortAttribute)
                        {
                            PSPropertySortAttribute sortprop_attr = (PSPropertySortAttribute)attr;

                            PropertyAttributeTuple tuple = new PropertyAttributeTuple();
                            tuple.Name = property.Name;
                            tuple.SortIndicator = sortprop_attr.SortID;

                            _listSortProps.Add(tuple);
                        }
                    }                                         
                }                
            }

            //Do we have a default property?
            if (sDefault.Length == 0)
            {
                throw new ArgumentException("No default property set, please provide one using [xPSProperty(Default=true)]");
            }

            //sort list of properties based on SortIndicator or OrderID
            _listProps.Sort(new PropertyAttributeTupleComparer());
            _listSortProps.Sort(new PropertyAttributeTupleComparer());

            #endregion


            //Start types XML generation
            #region Typ XML generation
            StringBuilder sbTypes = new StringBuilder();
            
            //Header with type name
            sbTypes.AppendFormat(CTypeHeaderStart,sTypeName);

            //default property 
            sbTypes.AppendFormat(CTypeDefaultProperty, sDefault);

            //Display properties
            if (_listProps.Count > 0)
            {
                sbTypes.AppendFormat(CTypePropertySetHeader, "DefaultDisplayPropertySet");
                foreach (PropertyAttributeTuple dispTuple in _listProps)
                {
                    sbTypes.AppendFormat(CTypePropertySetItem, dispTuple.Name);
                }
                sbTypes.Append(CTypePropertySetEnd);
            }

            //Sort properties
            if (_listSortProps.Count > 0)
            {
                sbTypes.AppendFormat(CTypePropertySetHeader, "DefaultKeyPropertySet");
                foreach (PropertyAttributeTuple keyTuple in _listSortProps)
                {
                    sbTypes.AppendFormat(CTypePropertySetItem, keyTuple.Name);
                }
                sbTypes.Append(CTypePropertySetEnd);
            }

            //Standart properties done
            sbTypes.Append(CTypePsStandardMemberEnd);

            //Alias properties
            if (_nvcAlias.Count > 0)
            {
                foreach (string sKey in _nvcAlias.AllKeys)
                {
                    string sValue = _nvcAlias[sKey];
                    sbTypes.AppendFormat(CTypeAliasProperty, sKey, sValue);

                }
            }

            //Types done
            sbTypes.Append(CTypeHeaderEnd);
            #endregion


            //Start format XML generation
            #region Format XML generation
            StringBuilder sbFormats= new StringBuilder();
            sbFormats.AppendFormat(CFormatHeaderStart, sTypeNameSimple, sTypeName);


            //Table headers
            sbFormats.Append(CFormatTableHeaderStart);  
            foreach (PropertyAttributeTuple dispTuple in _listProps)
            {
                if (dispTuple.Attr.ColumnOutput == true)
                {
                    sbFormats.Append(CFormatTableHeaderItemStart);

                    //Name needed?
                    string sLabel = dispTuple.Attr.ColumnName.Length > 0 ? dispTuple.Attr.ColumnName : dispTuple.Attr.Alias;
                    if (sLabel.Length > 0)
                    {
                        sbFormats.AppendFormat(CFormatTableHeaderItemLabel, sLabel);
                    }

                    if (dispTuple.Attr.ColumnRightAligned)
                    {
                        sbFormats.AppendFormat(CFormatTableHeaderItemAlignment, "Right");
                    }
                    
                    
                    if (dispTuple.Attr.ColumnWidth > 0)
                    {
                        sbFormats.AppendFormat(CFormatTableHeaderItemWidth, dispTuple.Attr.ColumnWidth);
                    }


                    sbFormats.Append(CFormatTableHeaderItemEnd);
                }
            }
            sbFormats.Append(CFormatTableHeaderEnd);
            

            //Row entries
            sbFormats.Append(CFormatRowEntriesStart);
            foreach (PropertyAttributeTuple dispTuple in _listProps)
            {
                if (dispTuple.Attr.ColumnOutput == true)
                {
                    if (dispTuple.Attr.ColumnScript.Length > 0)
                    {
                        sbFormats.AppendFormat(CFormatRowEntryItemScript, dispTuple.Attr.ColumnScript);
                    }
                    else
                    {
                        //DO NOT use the alias if one is defined, use the property name directly!
                        sbFormats.AppendFormat(CFormatRowEntryItemProp, dispTuple.PropertyName); 
                    }
                }
            }
            sbFormats.Append(CFormatRowEntriesEnd);

            //All done
            sbFormats.Append(CFormatHeaderEnd);

            #endregion


            PS1XMLOutput output = new PS1XMLOutput();
            output.TypeXML = sbTypes.ToString();
            output.FormatXML = sbFormats.ToString();
            return output;
        }
    }

    internal class PropertyAttributeTuple
    {
        public string Name = "";
        public string PropertyName = "";
        public int SortIndicator = 1;

        public PSPropertyViewAttribute Attr;
    }

    internal class PropertyAttributeTupleComparer : IComparer<PropertyAttributeTuple>
    {
        public int Compare(PropertyAttributeTuple x, PropertyAttributeTuple y)
        {
            return x.SortIndicator.CompareTo(y.SortIndicator);
            
        }
    }

    internal struct PS1XMLOutput
    {
        public string TypeXML;
        public string FormatXML;
    }
}
