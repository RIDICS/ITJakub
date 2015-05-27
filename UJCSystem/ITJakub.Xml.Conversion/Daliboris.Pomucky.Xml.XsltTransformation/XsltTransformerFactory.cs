// -----------------------------------------------------------------------
// <copyright file="XsltTransformer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Schema;

namespace Daliboris.Pomucky.Xml.XsltTransformation
{
    using System.Xml;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class XsltTransformerFactory
    {

        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        const string xslUri = "http://www.w3.org/1999/XSL/Transform";


        public static IXsltInformation GetXsltInformation(string xmlFile)
        {

            XsltInformation xsltInfo = new XsltInformation(xmlFile);
            FileInfo fileInfo = new FileInfo(xmlFile);

            if (!fileInfo.Exists)
                return xsltInfo;



            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.CloseInput = true;
            readerSettings.ConformanceLevel = ConformanceLevel.Document;
            readerSettings.IgnoreComments = false;
            readerSettings.IgnoreProcessingInstructions = false;
            readerSettings.IgnoreWhitespace = false;
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.XmlResolver = new XmlUrlResolver();
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;


            readerSettings.ValidationEventHandler += settings_ValidationEventHandler;

            XmlNamespaceManager manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace("xml", XmlNamespace);


            using (XmlReader reader = XmlReader.Create(xmlFile, readerSettings))
            {
                //reader.MoveToContent();
                while (reader.Read())
                {

                    if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == xslUri)
                    {
                        bool isEmptyElement = reader.IsEmptyElement;

                        if (reader.LocalName == "stylesheet")
                        {
                            string version = reader.GetAttribute("version");
                            xsltInfo.Version = version;
                        }

                        if (reader.LocalName == "include")
                        {
                            string path = reader.GetAttribute("href");
                            if (path != null && !path.Contains(":"))
                            {
                                path = Path.Combine(fileInfo.DirectoryName, path);
                            }

                            IXsltInformation information = GetXsltInformation(path);
                            xsltInfo.IncludedXslt.Add(information);
                        }

                        if (reader.LocalName == "param" && reader.Depth == 1) //pouze parametry nejvyšší úrovně, tj. na úrovní šablony
                        {
                            string name = reader.GetAttribute("name");
                            string value = reader.GetAttribute("select");

                            if (value == null && !isEmptyElement)
                            {
                                value = reader.ReadElementContentAsString();
                            }
                            xsltInfo.Parameters.Add(name, value);
                        }

                    }
                }
            }

            xsltInfo.IsValid = true;

            return xsltInfo;
        }

        static void settings_ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            //může se vyskytnou v případě, že je deklarován atribut xml:id
            //<fileDesc n="{$guid}" xml:id="{$zkratka}">
            /*
        <fileDesc n="{$guid}">
            <xsl:attribute name="id" namespace="http://www.w3.org/XML/1998/namespace">
                <xsl:value-of select="$zkratka"/>
            </xsl:attribute>
             */

            /*
             * xml:id ap. vyřešno pomocí readerSettings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
             * viz
             * http://social.msdn.microsoft.com/Forums/en-US/xmlandnetfx/thread/0ce7c48a-634f-44b4-ab03-70d73ae5062f/
             * http://msdn.microsoft.com/en-us/library/System.Xml.XmlReaderSettings.ValidationFlags.aspx
             * 
             */
            XmlReader reader = sender as XmlReader;
            if (reader != null)
                if (reader.NodeType == XmlNodeType.Attribute && reader.Name == "xml:id")
                {

                }
                else
                    throw new System.NotImplementedException();

        }

        public static IXsltTransformer GetXsltTransformer(string xsltFile)
        {
            XsltTransformerSettings settings = new XsltTransformerSettings();
            return GetXsltTransformer(xsltFile, settings);
        }

        public static IXsltTransformer GetXsltTransformer(string xsltFile, XsltTransformerSettings settings)
        {
            IXsltInformation information = GetXsltInformation(xsltFile);
            return GetXsltTransformer(information, settings);
        }

        public static IXsltTransformer GetXsltTransformer(IXsltInformation xsltInformation, XsltTransformerSettings settings)
        {

            if (settings == null)
                settings = new XsltTransformerSettings();
            IXsltTransformer transformer;

            if (settings.PreferredVersion == "2.0" || xsltInformation.Version == "2.0")
                goto Altova;

            if (settings.PreferredProvider == "Altova")
                goto Altova;

            if (settings.PreferredProvider == "Microsoft")
                goto Microsoft;


        Microsoft:
            transformer = new MicrosoftXsltTransformer();
            transformer.Parameters = xsltInformation.Parameters;
            transformer.Create(xsltInformation, settings);
            return transformer;

        Altova:
            transformer = new AltovaXsltTransformer();
            transformer.Parameters = xsltInformation.Parameters;
            transformer.Create(xsltInformation, settings);
            return transformer;
        }


    }
}
