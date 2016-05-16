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
	public class ContentInfoBuilder
	{
		public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
		public const string TeiNamespace = "http://www.tei-c.org/ns/1.0";
		public const string ItJakubTeiNamespace = "http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0";
		public static readonly XslCompiledTransform xsltTransform;

		static ContentInfoBuilder()
		{
		    xsltTransform = LoadCommonTeiXslt();
		}

		#region Vlastnosti
        
		/// <summary>
		/// Složka, do níž se uloží jednotlivé části rozděleného souboru
		/// </summary>
		public string OutputDirectory { get; set; }
        
		#endregion

		#region Metody

		public TableOfContentResult MakeTableOfContent(string XmlFile, string StartingElement)
		{
			TableOfContentResult result = new TableOfContentResult();

			result.HeadwordsList = GetHeadwordsListItems(XmlFile, StartingElement);
			result.Sections = GetTableOfContentItems(XmlFile, StartingElement);
			return result;

		#endregion
		}

		private List<TableOfContentItem> GetTableOfContentItems(string XmlFile, string StartingElement)
		{
			List<TableOfContentItem> tocItems = new List<TableOfContentItem>();
			using (XmlReader reader = XmlReader.Create(XmlFile))
			{
				reader.MoveToContent();

				bool splittingStarted = (StartingElement == null);

				string elementName = null;
				string actualPageBreak = null;
				string actualPageBreakXmlId = null;

				string actualDivId = null;
				string actualEntryType = null;
				TableOfContentItem currentHwItem = null;
				ItemBase currentTocItem = null;

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
						switch (elementName)
						{
							case "pb":
								actualPageBreak = reader.GetAttribute("n");
								actualPageBreakXmlId = reader.GetAttribute("id", XmlNamespace);
								break;
							case "div":
								actualDivId = reader.GetAttribute("id", XmlNamespace);
								break;
							case "head":
								ItemBase parent = currentHwItem;
								if (parent != null && (parent.Level == reader.Depth || parent.DivXmlId == actualDivId))
									parent = currentHwItem.Parent;
								TableOfContentItem tocItem = new TableOfContentItem(parent);
								tocItem.Level = reader.Depth;
								GetElementHead(reader, tocItem);
								tocItem.PageBreakInfo.PageBreak = actualPageBreak;
								tocItem.PageBreakInfo.PageBreakXmlId = actualPageBreakXmlId;
								tocItem.DivXmlId = actualDivId;

								if (currentHwItem == null)
								{
									tocItems.Add(tocItem);
								}
								else
								{
									
									if (currentHwItem.Parent == null)
									{
										if (currentHwItem.Level < tocItem.Level || currentHwItem.DivXmlId == tocItem.DivXmlId)
											currentHwItem.Sections.Add(tocItem);
										else
											tocItems.Add(tocItem);
									}
									else
									{
										if (currentHwItem.DivXmlId == tocItem.DivXmlId && currentHwItem.Parent != null &&
												currentHwItem.Parent.DivXmlId == tocItem.DivXmlId)
											currentHwItem.Parent.Sections.Add(tocItem);
										else
										{
											if (currentHwItem.Level == tocItem.Level)
												currentHwItem.Parent.Sections.Add(tocItem);
											else
												currentHwItem.Sections.Add(tocItem);
										}
									}
								}
									currentHwItem = tocItem;
								break;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (elementName == "div")
						{
							actualDivId = null;
							if (currentHwItem != null)
								currentHwItem = currentHwItem.Parent as TableOfContentItem;
						}
					}
				}
			}
			return tocItems;
		}

		protected List<HeadwordsListItem> GetHeadwordsListItems(string XmlFile, string StartingElement)
		{
			List<HeadwordsListItem> headwords = new List<HeadwordsListItem>();
			bool isFormUsed = false;
			using (XmlReader reader = XmlReader.Create(XmlFile))
			{
				reader.MoveToContent();

				bool splittingStarted = (StartingElement == null);

				string elementName = null;
				string actualPageBreak = null;
				string actualPageBreakXmlId = null;

				string actualDivId = null;
				string actualEntryType = null;
				HeadwordsListItem currentHwItem = null;
				ItemBase currentTocItem = null;

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						elementName = reader.Name;

						if (!splittingStarted)
							if (reader.Name == StartingElement)
							{
								splittingStarted = true;
							}
						if (!splittingStarted)
							continue;

						switch (elementName)
						{
							case "pb":
								actualPageBreak = reader.GetAttribute("n");
								actualPageBreakXmlId = reader.GetAttribute("id", XmlNamespace);
								break;
							case "entryFree":
								actualEntryType = reader.GetAttribute("type");
								goto case "div";
							case "div":
								actualDivId = reader.GetAttribute("id", XmlNamespace);
								break;
							case "form":
								isFormUsed = true;
								goto case "head";
							case "head":
								HeadwordsListItem parent = currentHwItem;
								if (parent != null && (parent.Level == reader.Depth || parent.DivXmlId == actualDivId))
									parent = currentHwItem.Parent as HeadwordsListItem;
								HeadwordsListItem tocItem = new HeadwordsListItem(parent);
								tocItem.HeadwordInfo.FormXmlId = reader.GetAttribute("id", XmlNamespace);
								tocItem.HeadwordInfo.Type = reader.GetAttribute("type");
								tocItem.Level = reader.Depth;
								GetElementHead(reader, tocItem);
								tocItem.PageBreakInfo.PageBreak = actualPageBreak;
								tocItem.PageBreakInfo.PageBreakXmlId = actualPageBreakXmlId;
								tocItem.DivXmlId = actualDivId;

								if (currentHwItem == null)
								{
									headwords.Add(tocItem);
								}
								else
								{
									if (elementName == "form" && currentHwItem.DivXmlId != tocItem.DivXmlId)
									{
										HeadwordsListItem tocEntry = new HeadwordsListItem(parent);
										tocEntry.DivXmlId = actualDivId;
										tocEntry.HeadInfo.HeadText = tocItem.HeadInfo.HeadText;
										tocEntry.Level = tocItem.Level;
										tocEntry.HeadwordInfo.Type = actualEntryType;
										parent.Sections.Add(tocEntry);
										tocItem.Parent = tocEntry;
										currentHwItem = tocEntry;
									}
									if (currentHwItem.Parent == null)
									{
										if (currentHwItem.Level < tocItem.Level || currentHwItem.DivXmlId == tocItem.DivXmlId)
											currentHwItem.Sections.Add(tocItem);
										else
											headwords.Add(tocItem);
									}
									else
									{
										if (currentHwItem.DivXmlId == tocItem.DivXmlId && currentHwItem.Parent != null &&
										    currentHwItem.Parent.DivXmlId == tocItem.DivXmlId)
											currentHwItem.Parent.Sections.Add(tocItem);
										else
										{
											if (elementName == "head" && currentHwItem.Level == tocItem.Level)
												currentHwItem.Parent.Sections.Add(tocItem);
											else
												currentHwItem.Sections.Add(tocItem);
										}
									}
								}
								if (elementName != "form")
									currentHwItem = tocItem;

								break;
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement)
					{
						if (elementName == "div")
						{
							actualDivId = null;
							if (currentHwItem != null)
								currentHwItem = currentHwItem.Parent as HeadwordsListItem;
						}
					}
				}
			}
			if (!isFormUsed)
				headwords = new List<HeadwordsListItem>();
			return headwords;
		}



		private static void GetElementHead(XmlReader reader, ItemBase tocItem)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(reader.ReadOuterXml());
			tocItem.HeadInfo.HeadXml = doc.DocumentElement;
			StringWriter sw = new StringWriter();
			xsltTransform.Transform(doc.DocumentElement.CreateNavigator(), new XsltArgumentList(), sw);
			tocItem.HeadInfo.HeadText = sw.ToString().Trim();
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

        private static XslCompiledTransform LoadCommonTeiXslt()
	    {
           Assembly _assembly = Assembly.GetExecutingAssembly();
           StreamReader _textStreamReader = new StreamReader(_assembly.GetManifestResourceStream("Ujc.Ovj.Xml.Tei.Contents.Data.CommonTEI.xsl"));
           XslCompiledTransform xslt = new XslCompiledTransform();
           xslt.Load(XmlReader.Create(_textStreamReader));
            return xslt;
	    }
	}
}
