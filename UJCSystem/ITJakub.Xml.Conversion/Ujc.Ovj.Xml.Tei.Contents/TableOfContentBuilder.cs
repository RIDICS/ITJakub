using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public class TableOfContentBuilder
	{
		public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
		public const string TeiNamespace = "http://www.tei-c.org/ns/1.0";
		public const string ItJakubTeiNamespace = "http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0";
		public static readonly XslCompiledTransform xsltTransform;

		static TableOfContentBuilder()
		{
			string filePath = Path.Combine(GetDataDirectory(), "CommonTEI.xsl");
			xsltTransform = new XslCompiledTransform();
			xsltTransform.Load(XmlReader.Create(filePath));
		}

		#region Vlastnosti

		/// <summary>
		/// Soubor XML, pro nějž se má vytvořit obsah
		/// </summary>
		public string XmlFile { get; set; }

		/// <summary>
		/// Složka, do níž se uloží jednotlivé části rozděleného souboru
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// Počáteční element, od něhož začne rozdělování souboru na menší části
		/// </summary>
		public string StartingElement { get; set; }

		#endregion

		#region Metody

		public TableOfContentResult MakeTableOfContent()
		{

			TableOfContentResult result = new TableOfContentResult();

			bool splittingStarted = (StartingElement == null);

			FileInfo xmlFileInfo = new FileInfo(XmlFile);

			string elementName = null;
			string actualPageBreak = null;
			string actualPageBreakXmlId = null;

			string actualDivId = null;
			TableOfContentItem currentTocItem = null;

			using (XmlReader reader = XmlReader.Create(XmlFile))
			{
				reader.MoveToContent();
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						elementName = reader.Name;
						bool isEmpty = reader.IsEmptyElement;

						if (!splittingStarted)
							if (reader.Name == StartingElement)
							{
								splittingStarted = true;
							}
						if (!splittingStarted)
							continue;
						if (elementName == "pb")
						{
							actualPageBreak = reader.GetAttribute("n");
							actualPageBreakXmlId = reader.GetAttribute("id", XmlNamespace);
						}
						else if (elementName == "div")
						{
							actualDivId = reader.GetAttribute("id", XmlNamespace);

						}
						else if (elementName == "head")
						{
							TableOfContentItem parent = currentTocItem;
							if (parent != null && parent.Level == reader.Depth)
								parent = currentTocItem.Parent;
							TableOfContentItem tocItem = new TableOfContentItem(parent);
							tocItem.Level = reader.Depth;
							GetElementHead(reader, tocItem);
							tocItem.PageBreak = actualPageBreak;
							tocItem.PageBreakXmlId = actualPageBreakXmlId;
							tocItem.DivXmlId = actualDivId;
							if (currentTocItem == null)
							{
								result.Sections.Add(tocItem);
							}
							else
							{
								if (currentTocItem.Parent == null)
									if(currentTocItem.Level < tocItem.Level)
										currentTocItem.Sections.Add(tocItem);
								else
									result.Sections.Add(tocItem);
								else
									currentTocItem.Parent.Sections.Add(tocItem);
							}
							currentTocItem = tocItem;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (elementName == "div")
						{
							actualDivId = null;
							if (currentTocItem != null)
								currentTocItem = currentTocItem.Parent;
						}
					}
				}

				return result;
			}

			#endregion

		}

		private static void GetElementHead(XmlReader reader, TableOfContentItem tocItem)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(reader.ReadOuterXml());
			tocItem.HeadXml = doc.DocumentElement;
			StringWriter sw = new StringWriter();
			xsltTransform.Transform(doc.DocumentElement.CreateNavigator(), new XsltArgumentList(), sw);
			tocItem.Head = sw.ToString();
		}

		private static string GetDataDirectory()
		{
			string dataDirectory = AssemblyDirectory;
			//dataDirectory = dataDirectory.Substring(0, dataDirectory.LastIndexOf(String.Format("{0}bin{0}", Path.DirectorySeparatorChar)));

			dataDirectory = Path.Combine(dataDirectory, "Data");
			return dataDirectory;
		}

		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}
}
