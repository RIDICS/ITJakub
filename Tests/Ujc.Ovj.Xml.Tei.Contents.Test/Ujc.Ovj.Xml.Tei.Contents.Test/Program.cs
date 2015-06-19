using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ujc.Ovj.Xml.Tei.Contents.Test
{
	class Program
	{
		static XNamespace nsTei = ContentInfoBuilder.TeiNamespace;
		static XNamespace nsItj = ContentInfoBuilder.ItJakubTeiNamespace;
		static void Main(string[] args)
		{
			TestDocument();
			Console.WriteLine("Hotovo");
			Console.ReadLine();
		}
		private static void TestDocument()
		{
			DirectoryInfo directory = new DirectoryInfo(@"V:\Projekty\BitBucket\itjakub\Tests\Ujc.Ovj.Xml.Tei.Contents.Test\Ujc.Ovj.Xml.Tei.Contents.Test\Data");
			FileInfo[] files = directory.GetFiles("*.xml");
			foreach (FileInfo file in files)
			{
				Console.WriteLine("{0}", file.Name);
				TestDocument(file.FullName);
				Console.WriteLine();
			}
		}
		private static void TestDocument(string file)
		{
			ContentInfoBuilder builder = new ContentInfoBuilder();
			builder.XmlFile = file;
			builder.StartingElement = "body";
			TableOfContentResult result = builder.MakeTableOfContent();

			//GenerateTocXml(result);
			SaveToc(result, Path.Combine(@"V:\Projekty\BitBucket\itjakub\UJCSystem\ITJakub.Xml.Conversion\Ujc.Ovj.Ooxml.Conversion.Test\Data\Metadata", 
				file.Substring(file.LastIndexOf('\\') + 1).Replace(".xml", ".cnt")));

			//GenerateTocText(result);
		}

		private static void GenerateTocXml(TableOfContentResult result)
		{
			XElement toc = GenerateToc(result);
			Console.WriteLine(toc);
			Console.WriteLine();
		}

		private static void GenerateTocText(TableOfContentResult result)
		{
			Console.WriteLine("<tableOfContent>");
			Console.WriteLine("<list>");
			foreach (TableOfContentItem item in result.Sections)
			{
				//WriteTocInfo(item);
				WriteTocInfoXml(item, 1);
			}
			Console.WriteLine("</list>");
			Console.WriteLine("</tableOfContent>");
		}

		private static XElement GenerateToc(TableOfContentResult result)
		{
			XDocument doc = new XDocument( new XElement(nsItj + "tableOfContent"));
			doc.Root.Add(GenerateList(result.Sections));
			return doc.Root;
		}

		private static XElement GenerateHwList(TableOfContentResult result)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "headwordsList"));
			doc.Root.Add(GenerateList(result.HeadwordsList));
			return doc.Root;
		}

		private static void SaveToc(TableOfContentResult result, string filepath)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "metadata"));
			XElement toc = new XElement(nsItj + "tableOfContent");
			toc.Add(GenerateList(result.Sections));

			XElement hw = new XElement(nsItj + "headwordsList");
			hw.Add(GenerateList(result.HeadwordsList));


			doc.Root.Add(toc);
			doc.Root.Add(hw);
			doc.Save(filepath);
		}

		private static XElement GenerateList(List<TableOfContentItem> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (TableOfContentItem item in items)
				{
					list.Add(GenerateList(item));
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private static XElement GenerateList(List<HeadwordsListItem> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (HeadwordsListItem item in items)
				{
					list.Add(GenerateList(item));
				}
				return list;
			}
			return null;
		}

		private static XElement GenerateList(HeadwordsListItem item)
		{
			//if (item == null) return null;
			//string corresp = item.DivXmlId;
			//XElement it =
			//	new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp),
			//		new XElement(nsTei + "head", new XText(item.HeadInfo.HeadText)),
			//		item.PageBreakInfo.PageBreak == null
			//			? null
			//			: new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakInfo.PageBreakXmlId),
			//				new XText(item.PageBreakInfo.PageBreak)),
			//		GenerateList(item.Sections)
			//		);
			//return it;

			if (item == null) return null;
			string corresp = item.HeadwordInfo.FormXmlId ?? item.DivXmlId;
			XAttribute type = null;
			if (item.HeadwordInfo.Type != null)
				type = new XAttribute("type", item.HeadwordInfo.Type);

			XElement it =
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp),
					type,
					new XElement(nsTei + "head", new XText(item.HeadInfo.HeadText)),
					item.PageBreakInfo.PageBreak == null
						? null
						: new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakInfo.PageBreakXmlId),
							new XText(item.PageBreakInfo.PageBreak)),
					GenerateList(item.Sections)
					);
			return it;
		}

		private static XElement GenerateList(List<ItemBase> items)
		{
			if (items.Count > 0)
			{
				XElement list = new XElement(nsTei + "list");
				foreach (ItemBase item in items)
				{
					if (item is TableOfContentItem)
					{
						list.Add(GenerateList(item as TableOfContentItem));
					}
					if (item is HeadwordsListItem)
					{
						list.Add(GenerateList(item as HeadwordsListItem));
					}
				}
				if (list.IsEmpty)
					return null;
				return list;
			}
			return null;
		}

		private static XElement GenerateList(TableOfContentItem item)
		{
			if (item == null || item.PageBreakInfo.PageBreakXmlId == null) return null;
			string corresp = item.DivXmlId;
			XElement it = 
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp), 
						new XElement(nsTei + "head", new XText(item.HeadInfo.HeadText)),
						item.PageBreakInfo.PageBreak == null ? null : new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakInfo.PageBreakXmlId), new XText(item.PageBreakInfo.PageBreak)),
						GenerateList(item.Sections)	
					);
			return it;
		}

		

		private static void WriteTocInfo(TableOfContentItem item)
		{
			Console.WriteLine("{0}: [{1}] {2}", item.Level, item.PageBreakInfo.PageBreak, item.HeadInfo.HeadText);
			foreach (TableOfContentItem section in item.Sections)
			{
				WriteTocInfo(section);
			}
		}

		private static void WriteTocInfoXml(TableOfContentItem item, int indent)
		{
			Console.WriteLine("<item corresp=\"#{0}\"><head>{1}</head><ref target=\"#{2}\">{3}</xref><item>", item.DivXmlId, item.HeadInfo.HeadText, item.PageBreakInfo.PageBreakXmlId, item.PageBreakInfo.PageBreak);
			if (item.Sections.Count > 0)
			{
				Console.WriteLine();
				indent++;
			string textIndent = new string(' ', indent);
			Console.WriteLine("{0}<list>", textIndent);

				foreach (TableOfContentItem section in item.Sections)
				{
					WriteTocInfoXml(section, indent);
				}
			Console.WriteLine("</list>");

			}

		}
	}
}
