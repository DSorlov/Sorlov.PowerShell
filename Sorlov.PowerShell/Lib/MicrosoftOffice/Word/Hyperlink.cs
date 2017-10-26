﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO.Packaging;

namespace Sorlov.PowerShell.Lib.MicrosoftOffice.Word
{
    /// <summary>
    /// Represents a Hyperlink in a document.
    /// </summary>
    public class Hyperlink: DocXElement
    {
        internal Uri uri;
        internal String text;

        internal Dictionary<PackagePart, PackageRelationship> hyperlink_rels;
        public PackagePart mainPart;
        internal int type;
        internal String id;
        internal XElement instrText;
        internal List<XElement> runs;

        /// <summary>
        /// Remove a Hyperlink from this Paragraph only.
        /// </summary>
        /// <example>
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Create(@"Test.docx"))
        /// {
        ///    // Add a hyperlink to this document.
        ///    Hyperlink h = document.AddHyperlink("link", new Uri("http://www.google.com"));
        ///
        ///    // Add a Paragraph to this document and insert the hyperlink
        ///    Paragraph p1 = document.InsertParagraph();
        ///    p1.Append("This is a cool ").AppendHyperlink(h).Append(" .");
        ///
        ///    /* 
        ///     * Remove the hyperlink from this Paragraph only. 
        ///     * Note a reference to the hyperlink will still exist in the document and it can thus be reused.
        ///     */
        ///    p1.Hyperlinks[0].Remove();
        ///
        ///    // Add a new Paragraph to this document and reuse the hyperlink h.
        ///    Paragraph p2 = document.InsertParagraph();
        ///    p2.Append("This is the same cool ").AppendHyperlink(h).Append(" .");
        ///
        ///    document.Save();
        /// }// Release this document from memory.
        /// </code>
        /// </example>
        public void Remove()
        {
            Xml.Remove();
        }

        /// <summary>
        /// Change the Text of a Hyperlink.
        /// </summary>
        /// <example>
        /// Change the Text of a Hyperlink.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///    // Get all of the hyperlinks in this document
        ///    List<Hyperlink> hyperlinks = document.Hyperlinks;
        ///    
        ///    // Change the first hyperlinks text and Uri
        ///    Hyperlink h0 = hyperlinks[0];
        ///    h0.Text = "DocX";
        ///    h0.Uri = new Uri("http://docx.codeplex.com");
        ///
        ///    // Save this document.
        ///    document.Save();
        /// }
        /// </code>
        /// </example>
        public string Text 
        { 
            get
            {
                return this.text;
            } 

            set
            {
                XElement rPr =
                    new XElement
                    (
                        DocX.w + "rPr",
                        new XElement
                        (
                            DocX.w + "rStyle",
                            new XAttribute(DocX.w + "val", "Hyperlink")
                        )
                    );

                // Format and add the new text.
                List<XElement> newRuns = HelperFunctions.FormatInput(value, rPr);

                if (type == 0)
                {
                    // Get all the runs in this Text.
                    var runs = from r in Xml.Elements()
                               where r.Name.LocalName == "r"
                               select r;

                    // Remove each run.
                    for (int i = 0; i < runs.Count(); i++)
                        runs.Remove();

                    Xml.Add(newRuns);
                }

                else
                {
                    XElement separate = XElement.Parse(@"
                    <w:r xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main'>
                        <w:fldChar w:fldCharType='separate'/> 
                    </w:r>");

                    XElement end = XElement.Parse(@"
                    <w:r xmlns:w='http://schemas.openxmlformats.org/wordprocessingml/2006/main'>
                        <w:fldChar w:fldCharType='end' /> 
                    </w:r>");

                    runs.Last().AddAfterSelf(separate, newRuns, end);
                    runs.ForEach(r => r.Remove());
                }

                this.text = value;
            } 
        }

        /// <summary>
        /// Change the Uri of a Hyperlink.
        /// </summary>
        /// <example>
        /// Change the Uri of a Hyperlink.
        /// <code>
        /// // Create a document.
        /// using (DocX document = DocX.Load(@"Test.docx"))
        /// {
        ///    // Get all of the hyperlinks in this document
        ///    List<Hyperlink> hyperlinks = document.Hyperlinks;
        ///    
        ///    // Change the first hyperlinks text and Uri
        ///    Hyperlink h0 = hyperlinks[0];
        ///    h0.Text = "DocX";
        ///    h0.Uri = new Uri("http://docx.codeplex.com");
        ///
        ///    // Save this document.
        ///    document.Save();
        /// }
        /// </code>
        /// </example>
        public Uri Uri 
        { 
            get
            {
                if (type == 0 && id != String.Empty)
                {
                    PackageRelationship r = mainPart.GetRelationship(id);
                    return r.TargetUri;
                }

                return this.uri;
            } 

            set
            {
                if (type == 0)
                {
                    PackageRelationship r = mainPart.GetRelationship(id);
                    
                    // Get all of the information about this relationship.
                    TargetMode r_tm = r.TargetMode;
                    string r_rt = r.RelationshipType;
                    string r_id = r.Id;

                    // Delete the relationship
                    mainPart.DeleteRelationship(r_id);
                    mainPart.CreateRelationship(value, r_tm, r_rt, r_id);
                }

                else
                {
                    instrText.Value = "HYPERLINK " + "\"" + value + "\"";
                }

                this.uri = value;
            } 
        }

        internal Hyperlink(DocX document, PackagePart mainPart, XElement i): base(document, i)
        {
            this.type = 0;
            this.id = i.Attribute(XName.Get("id", DocX.r.NamespaceName)).Value;

            StringBuilder sb = new StringBuilder();
            HelperFunctions.GetTextRecursive(i, ref sb);
            this.text = sb.ToString();
        }

        internal Hyperlink(DocX document, XElement instrText, List<XElement> runs) : base(document, null)
        {
            this.type = 1;
            this.instrText = instrText;
            this.runs = runs;

            try
            {
                int start = instrText.Value.IndexOf("HYPERLINK \"") + "HYPERLINK \"".Length;
                int end = instrText.Value.IndexOf("\"", start);
                if (start != -1 && end != -1)
                {
                    this.uri = new Uri(instrText.Value.Substring(start, end - start), UriKind.Absolute);

                    StringBuilder sb = new StringBuilder();
                    HelperFunctions.GetTextRecursive(new XElement(XName.Get("temp", DocX.w.NamespaceName), runs), ref sb);
                    this.text = sb.ToString();
                }
            }

            catch (Exception e){throw e;}
        }
    }
}
