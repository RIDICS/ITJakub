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
		static XNamespace nsTei = TableOfContentBuilder.TeiNamespace;
		static XNamespace nsItj = TableOfContentBuilder.ItJakubTeiNamespace;
		static void Main(string[] args)
		{
			TestDocument();
			Console.WriteLine("Hotovo");
			Console.ReadLine();
		}
		private static void TestDocument()
		{
			DirectoryInfo directory = new DirectoryInfo(@"V:\Projekty\BitBucket\itjakub\Tests\Ujc.Ovj.Xml.Tei.Contents.Test\Ujc.Ovj.Xml.Tei.Contents.Test\Data");
			FileInfo[] files = directory.GetFiles("ESS*.xml");
			foreach (FileInfo file in files)
			{
				Console.WriteLine("{0}", file.Name);
				TestDocument(file.FullName);
				Console.WriteLine();
			}
		}
		private static void TestDocument(string file)
		{
			TableOfContentBuilder builder = new TableOfContentBuilder();
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

		private static void SaveToc(TableOfContentResult result, string filepath)
		{
			XDocument doc = new XDocument(new XElement(nsItj + "tableOfContent"));
			doc.Root.Add(GenerateList(result.Sections));
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
				return list;
			}
			return null;
		}

		private static XElement GenerateList(TableOfContentItem item)
		{
			if (item == null) return null;
			string corresp = item.FormXmlId ?? item.DivXmlId;
			XElement it = 
				new XElement(nsTei + "item", new XAttribute("corresp", "#" + corresp), (item.Type == null) ? null : new XAttribute("type", item.Type),
						new XElement(nsTei + "head", new XText(item.Head)),
						item.PageBreak == null ? null : new XElement(nsTei + "ref", new XAttribute("target", "#" + item.PageBreakXmlId), new XText(item.PageBreak)),
						GenerateList(item.Sections)	
					);
			/*
			if (item.Sections.Count > 0)
			{
				List<XElement> its = new List<XElement>();
				foreach (TableOfContentItem section in item.Sections)
				{
					its.Add(GenerateList(section));
				}
			}
			 */
			return it;
		}

		

		private static void WriteTocInfo(TableOfContentItem item)
		{
			Console.WriteLine("{0}: [{1}] {2}", item.Level, item.PageBreak, item.Head);
			foreach (TableOfContentItem section in item.Sections)
			{
				WriteTocInfo(section);
			}
		}

		private static void WriteTocInfoXml(TableOfContentItem item, int indent)
		{
			Console.WriteLine("<item corresp=\"#{0}\"><head>{1}</head><ref target=\"#{2}\">{3}</xref><item>", item.DivXmlId, item.Head, item.PageBreakXmlId, item.PageBreak);
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
