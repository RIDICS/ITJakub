// -----------------------------------------------------------------------
// <copyright file="XsltTransformer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Schema;
using Microsoft.CSharp;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
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

						if (reader.LocalName == "output")
						{
							string method = reader.GetAttribute("method");
							xsltInfo.Method = method ?? "xml";
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
				//goto AltovaExe;

			if (settings.PreferredProvider == "Altova")
				goto Altova;
			if (settings.PreferredProvider == "AltovaExe")
				goto AltovaExe;

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
		AltovaExe:
			transformer = new AltovaXsltExeTransformer();
			transformer.Parameters = xsltInformation.Parameters;
			transformer.Create(xsltInformation, settings);
			return transformer;

		}

		/// <summary>
		/// Vrací seznam transformačních objektů (XSLT) pro daný identifikátor.
		/// </summary>
		/// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
		/// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
		/// <returns></returns>
		public static IList<IXsltTransformer> GetXsltTransformers(string transformationFile, string sectionId)
		{
			return GetXsltTransformers(transformationFile, sectionId, null);
		}

		/// <summary>
		/// Vrací seznam transformačních objektů (XSLT) pro daný identifikátor.
		/// </summary>
		/// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
		/// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
		/// <param name="transformationFilesDirectory"></param>
		/// <returns></returns>
		public static IList<IXsltTransformer> GetXsltTransformers(string transformationFile, string sectionId, string transformationFilesDirectory)
		{
			return GetXsltTransformers(transformationFile, sectionId, transformationFilesDirectory, new XsltTransformerSettings());
		}

		/// <summary>
		/// Vrací seznam transformačních souborů (XSLT) pro daný identifikátor.
		/// </summary>
		/// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
		/// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
		/// <returns></returns>
		public static List<string> GetTransformationFilePaths(string transformationFile, string sectionId)
		{
			return GetTransformationFilePaths(transformationFile, sectionId, null);
		}

		/// <summary>
		/// Vrací seznam transformačních souborů (XSLT) pro daný identifikátor.
		/// </summary>
		/// <param name="transformationFile">Transformační soubor, který obsahuje jednotlivé kroky při převodu XML.</param>
		/// <param name="sectionId">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
		/// <param name="transformationFilesDirectory"></param>
		/// <returns></returns>
		public static List<string> GetTransformationFilePaths(string transformationFile, string sectionId, string transformationFilesDirectory)
		{
			List<string> kroky = TransformationFilePathsImplementationXmlDocument(transformationFile, sectionId, transformationFilesDirectory);
			return kroky;
		}


		private static List<string> TransformationFilePathsImplementationXmlDocument(string transformationFile, string sectionId)
		{
			return TransformationFilePathsImplementationXmlDocument(transformationFile, sectionId, null);
		}

		private static List<string> TransformationFilePathsImplementationXmlDocument(string transformationFile, 
			string sectionId, 
			string transformationFilesDirectory)
		{
			const string xmlNamespace = "http://www.w3.org/XML/1998/namespace";
			const string transformationsNamespace = "http://vokabular.ujc.cas.cz/ns/xslt-transformation/1.0";
			const string transformationsPrefix = "xt";
			const string transformationsElement = transformationsPrefix + ":" + "transformations";
			const string transformationElement = transformationsPrefix + ":" + "transformation";
			const string stepElement = transformationsPrefix + ":" + "step";
			const string directoryAttribute = "directory";
			const string fileAttribute = "file";
			const string repeatAttribute = "repeat";

			if (transformationFile == null) return null;
			List<string> kroky = new List<string>();


			XmlDocument document = new XmlDocument();
			XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);

			manager.AddNamespace("xml", xmlNamespace);
			manager.AddNamespace(transformationsPrefix, transformationsNamespace);

			document.Load(transformationFile);
			XmlNode transformations = document.SelectSingleNode("/" + transformationsElement, manager);
			if (transformations == null) return kroky;

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
				mainDirectory = (transformations.Attributes[directoryAttribute] == null) ? null : transformations.Attributes[directoryAttribute].Value;

			if (transformationFilesDirectory != null && mainDirectory != null)
			{
				if (Path.IsPathRooted(mainDirectory))
					mainDirectory = transformationFilesDirectory;
				else
				{
					mainDirectory = Path.GetFullPath(Path.Combine(transformationFilesDirectory, mainDirectory));
				}
			}

			XmlNode transformation = document.SelectSingleNode("//" + transformationElement + "[@xml:id='" + sectionId + "']", manager);
			if (transformation == null) return kroky;
			if (transformation.Attributes != null)
			{
				string transformationDirectory = null;
				if (transformation.Attributes[directoryAttribute] != null)
					transformationDirectory = transformation.Attributes[directoryAttribute].Value;
				transformationDirectory = CombineDiractories(mainDirectory, transformationDirectory);

				XmlNodeList nodeList = transformation.SelectNodes(".//" + stepElement, manager);
				if (nodeList != null)
					foreach (XmlNode node in nodeList)
					{
						if (node.Attributes != null)
						{
							string file = node.Attributes[fileAttribute].Value;
							string stepDirectory = (node.Attributes[directoryAttribute] == null) ? null : node.Attributes[directoryAttribute].Value;
							stepDirectory = CombineDiractories(transformationDirectory, stepDirectory);
							string fileName = Path.Combine(stepDirectory, file);
							int count = 1;
							if (node.Attributes[repeatAttribute] != null)
							{
								count = int.Parse(node.Attributes[repeatAttribute].Value);
							}
							for (int i = 0; i < count; i++)
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
			string transformationFilesDirectory, XsltTransformerSettings xsltTransformerSettings)
		{
			List<string> steps = GetTransformationFilePaths(transformationFile, sectionId, transformationFilesDirectory);
			IList<IXsltTransformer> transformers = new List<IXsltTransformer>();
			foreach (string step in steps)
			{
				transformers.Add(XsltTransformerFactory.GetXsltTransformer(step, xsltTransformerSettings));
			}

			return transformers;
				
		}
	}
}
