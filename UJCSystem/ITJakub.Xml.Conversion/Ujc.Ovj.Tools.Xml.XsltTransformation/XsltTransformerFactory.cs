// -----------------------------------------------------------------------
// <copyright file="XsltTransformer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
{
    /// <summary>
    ///     TODO: Update summary.
    /// </summary>
    public class XsltTransformerFactory
    {
        private const string m_xmlNamespace = "http://www.w3.org/XML/1998/namespace";

        private const string m_xslUri = "http://www.w3.org/1999/XSL/Transform";

        private const string m_transformationsPrefix = "xt";
        private const string m_transformationElement = m_transformationsPrefix + ":" + "transformation";


        public static IXsltInformation GetXsltInformation(string xmlFile)
        {
            var xsltInfo = new XsltInformation(xmlFile);
            var fileInfo = new FileInfo(xmlFile);

            if (!fileInfo.Exists)
                return xsltInfo;

            var readerSettings = new XmlReaderSettings();
            readerSettings.CloseInput = true;
            readerSettings.ConformanceLevel = ConformanceLevel.Document;
            readerSettings.IgnoreComments = false;
            readerSettings.IgnoreProcessingInstructions = false;
            readerSettings.IgnoreWhitespace = false;
            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.XmlResolver = new XmlUrlResolver();
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;


            readerSettings.ValidationEventHandler += settings_ValidationEventHandler;

            var manager = new XmlNamespaceManager(new NameTable());
            manager.AddNamespace("xml", m_xmlNamespace);

            using (var reader = XmlReader.Create(xmlFile, readerSettings))
            {
                //reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == m_xslUri)
                    {
                        var isEmptyElement = reader.IsEmptyElement;

                        if (reader.LocalName == "stylesheet")
                        {
                            var version = reader.GetAttribute("version");
                            xsltInfo.Version = version;
                        }

                        if (reader.LocalName == "output")
                        {
                            var method = reader.GetAttribute("method");
                            xsltInfo.Method = method ?? "xml";
                        }

                        if (reader.LocalName == "include")
                        {
                            var path = reader.GetAttribute("href");
                            if (path != null && !path.Contains(":"))
                            {
                                path = Path.Combine(fileInfo.DirectoryName, path);
                            }

                            var information = GetXsltInformation(path);
                            xsltInfo.IncludedXslt.Add(information);
                        }

                        if (reader.LocalName == "param" && reader.Depth == 1)
                            //pouze parametry nejvyšší úrovně, tj. na úrovní šablony
                        {
                            var name = reader.GetAttribute("name");
                            var value = reader.GetAttribute("select");

                            if (value == null && !isEmptyElement)
                            {
                                value = reader.ReadElementContentAsString();
                            }
                            xsltInfo.Parameters.Add(name, value);
                        }

                        if (reader.LocalName == "result-document")
                        {
                            xsltInfo.GeneratesResultDocument = true;
                        }
                    }
                }
            }


            xsltInfo.IsValid = true;

            return xsltInfo;
        }

        private static void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
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
            var reader = sender as XmlReader;
            if (reader != null)
                if (reader.NodeType == XmlNodeType.Attribute && reader.Name.StartsWith("xml:"))
                {
                }
                else
                    throw new NotImplementedException();
        }

        public static IXsltTransformer GetXsltTransformer(string xsltFile)
        {
            var settings = new XsltTransformerSettings();
            return GetXsltTransformer(xsltFile, settings);
        }

        public static IXsltTransformer GetXsltTransformer(string xsltFile, XsltTransformerSettings settings)
        {
            var information = GetXsltInformation(xsltFile);
            return GetXsltTransformer(information, settings);
        }

        public static IXsltTransformer GetXsltTransformer(IXsltInformation xsltInformation,
            XsltTransformerSettings settings)
        {
            if (settings == null)
                settings = new XsltTransformerSettings();
            IXsltTransformer transformer;

            if (settings.PreferredVersion == "2.0" || xsltInformation.Version == "2.0")
                goto AltovaExe; //Altova originally
            //goto AltovaExe;

            if (settings.PreferredProvider == "Altova")
                goto AltovaExe; //Altova originally
            if (settings.PreferredProvider == "AltovaExe")
                goto AltovaExe;

            if (settings.PreferredProvider == "Microsoft")
                goto Microsoft;


            Microsoft:
            transformer = new MicrosoftXsltTransformer();
            transformer.Parameters = xsltInformation.Parameters;
            transformer.Create(xsltInformation, settings);
            return transformer;

            transformer = new AltovaXsltTransformer();
            transformer.Parameters = xsltInformation.Parameters;
            transformer.Create(xsltInformation, settings);
            return transformer;
            AltovaExe:
            transformer = new AltovaXsltExeTransformer();
            transformer.Parameters = xsltInformation.Parameters;
            transformer.Create(xsltInformation, settings);
            return transformer;
        }

        /// <summary>
        ///     Vrací seznam transformačních objektů (XSLT) pro daný identifikátor.
        /// </summary>
        /// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
        /// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
        /// <returns></returns>
        public static IList<IXsltTransformer> GetXsltTransformers(string transformationFile, string sectionId)
        {
            return GetXsltTransformers(transformationFile, sectionId, null);
        }

        /// <summary>
        ///     Vrací seznam transformačních objektů (XSLT) pro daný identifikátor.
        /// </summary>
        /// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
        /// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
        /// <param name="transformationFilesDirectory"></param>
        /// <returns></returns>
        public static IList<IXsltTransformer> GetXsltTransformers(string transformationFile, string sectionId,
            string transformationFilesDirectory, bool throwException = false)
        {
            return GetXsltTransformers(transformationFile, sectionId, transformationFilesDirectory,
                new XsltTransformerSettings(), throwException);
        }

        /// <summary>
        ///     Vrací seznam transformačních souborů (XSLT) pro daný identifikátor.
        /// </summary>
        /// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
        /// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
        /// <returns></returns>
        public static List<string> GetTransformationFilePaths(string transformationFile, string sectionId)
        {
            return GetTransformationFilePaths(transformationFile, sectionId, null);
        }

        /// <summary>
        ///     Vrací seznam transformačních souborů (XSLT) pro daný identifikátor.
        /// </summary>
        /// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
        /// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
        /// <param name="transformationFilesDirectory"></param>
        /// <returns></returns>
        public static List<string> GetTransformationFilePaths(string transformationFile, string sectionId,
            string transformationFilesDirectory, bool throwException = false)
        {
            var kroky = TransformationFilePathsImplementationXmlDocument(transformationFile, sectionId,
                transformationFilesDirectory, throwException);
            return kroky;
        }


        private static List<string> TransformationFilePathsImplementationXmlDocument(string transformationFile,
            string sectionId)
        {
            return TransformationFilePathsImplementationXmlDocument(transformationFile, sectionId, null);
        }

        private static XmlDocument GetTransformationsFileXml(string transformationFile, ref XmlNamespaceManager manager,
            ref XmlNode transformations)
        {
            const string transformationsNamespace = "http://vokabular.ujc.cas.cz/ns/xslt-transformation/1.0";

            const string transformationsElement = m_transformationsPrefix + ":" + "transformations";

            if (transformationFile == null) return null;

            var document = new XmlDocument();
            manager = new XmlNamespaceManager(document.NameTable);

            manager.AddNamespace("xml", m_xmlNamespace);
            manager.AddNamespace(m_transformationsPrefix, transformationsNamespace);

            document.Load(transformationFile);
            transformations = document.SelectSingleNode("/" + transformationsElement, manager);
            if (transformations == null)
            {
                throw new XsltTransformatinException(string.Format("Transformations element '{0}' not found in '{1}'",
                    transformationsElement, transformationFile));
            }

            return document;
        }

        public static List<string> GetTransformationFromTransformationsFile(string transformationFile,
            string sectionPrefix)
        {
            var transformation = new List<string>();

            XmlNamespaceManager manager = null;
            XmlNode transformations = null;
            var document = GetTransformationsFileXml(transformationFile, ref manager, ref transformations);

            var xmlNodeList =
                document.SelectNodes(
                    string.Format("//{0}[starts-with(@xml:id,'{1}')]", m_transformationElement, sectionPrefix), manager);
            if (xmlNodeList != null)
            {
                for (var i = 0; i < xmlNodeList.Count; i++)
                {
                    var node = xmlNodeList.Item(i);
                    if (node?.Attributes != null)
                    {
                        transformation.Add(node.Attributes["xml:id"].Value);
                    }
                }
            }

            return transformation;
        }

        private static List<string> TransformationFilePathsImplementationXmlDocument(string transformationFile,
            string sectionId,
            string transformationFilesDirectory,
            bool throwException = false)
        {
            const string directoryAttribute = "directory";

            const string stepElement = m_transformationsPrefix + ":" + "step";

            const string fileAttribute = "file";
            const string repeatAttribute = "repeat";

            var kroky = new List<string>();
            XmlDocument document;
            XmlNamespaceManager manager = null;
            XmlNode transformations = null;

            try
            {
                document = GetTransformationsFileXml(transformationFile, ref manager, ref transformations);
            }
            catch (XsltTransformatinException)
            {
                if (throwException)
                {
                    throw;
                }

                return kroky;
            }

            //
            // Pravidla pro kombinvání cest:
            // Největší váhu má externě předaná cesta.
            // Dále se kombinují cesta v externím nebo nadřezeném elementu s podřazeným:
            // pokud je cesta v podřazeném elementu relativní (nezačíná kořenovou složkou),
            // zkombinují se cesta externího/nadřízeného a podřízeného elementu;
            // v opačném případě se použije absolutní cesta podřízeného elementu.
            //
            string mainDirectory = null;

            if (transformations.Attributes != null)
            {
                mainDirectory = transformations.Attributes[directoryAttribute]?.Value;
            }

            if (transformationFilesDirectory != null && mainDirectory != null)
            {
                mainDirectory = Path.IsPathRooted(mainDirectory)
                    ? transformationFilesDirectory
                    : Path.GetFullPath(Path.Combine(transformationFilesDirectory, mainDirectory));
            }

            var transformation =
                document.SelectSingleNode("//" + m_transformationElement + "[@xml:id='" + sectionId + "']", manager);
            if (transformation == null)
            {
                if (throwException)
                {
                    throw new XsltTransformatinNotFoundSectionException(string.Format(
                        "Section '{0}' not found in '{1}'", sectionId, transformationFile));
                }

                return kroky;
            }
            if (transformation.Attributes != null)
            {
                string transformationDirectory = null;
                if (transformation.Attributes[directoryAttribute] != null)
                    transformationDirectory = transformation.Attributes[directoryAttribute].Value;
                transformationDirectory = CombineDiractories(mainDirectory, transformationDirectory);

                var nodeList = transformation.SelectNodes(".//" + stepElement, manager);
                if (nodeList != null)
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes != null)
                        {
                            var file = node.Attributes[fileAttribute].Value;
                            var stepDirectory = node.Attributes[directoryAttribute] == null
                                ? null
                                : node.Attributes[directoryAttribute].Value;
                            stepDirectory = CombineDiractories(transformationDirectory, stepDirectory);
                            var fileName = Path.Combine(stepDirectory, file);
                            var count = 1;
                            if (node.Attributes[repeatAttribute] != null)
                            {
                                count = int.Parse(node.Attributes[repeatAttribute].Value);
                            }
                            for (var i = 0; i < count; i++)
                            {
                                kroky.Add(fileName);
                            }
                        }
                    }
            }
            return kroky;
        }

        private static string CombineDiractories(string mainDirectory, string subDirectory)
        {
            if (mainDirectory == null && subDirectory == null) return null;
            if (mainDirectory == null)
                return subDirectory;
            if (subDirectory == null)
                return mainDirectory;
            if (Path.IsPathRooted(subDirectory)) return subDirectory;
            return Path.GetFullPath(Path.Combine(mainDirectory, subDirectory));
        }

        public static IList<IXsltTransformer> GetXsltTransformers(string transformationFile, string sectionId,
            string transformationFilesDirectory, XsltTransformerSettings xsltTransformerSettings,
            bool throwException = false)
        {
            var steps = GetTransformationFilePaths(transformationFile, sectionId, transformationFilesDirectory,
                throwException);
            IList<IXsltTransformer> transformers = new List<IXsltTransformer>();
            foreach (var step in steps)
            {
                transformers.Add(GetXsltTransformer(step, xsltTransformerSettings));
            }

            return transformers;
        }
    }
}
