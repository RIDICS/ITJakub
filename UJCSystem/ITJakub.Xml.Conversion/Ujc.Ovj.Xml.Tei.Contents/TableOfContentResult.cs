using System.Collections.Generic;
using System.Xml;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public class TableOfContentResult
	{
		public TableOfContentResult()
		{
			Sections = new List<TableOfContentItem>();
		}

		public string Errors { get; set; }

		public List<TableOfContentItem> Sections { get; set; } 

	}

	public class TableOfContentItem
	{
		public TableOfContentItem()
		{
			Subsections = new List<TableOfContentItem>();
		}

		public string Head { get; set; }
		public XmlElement HeadXml { get; set; }
		public string PageBreakFirst { get; set; }
		public string PageBreakLast { get; set; }
		public int Level { get; set; }

		public List<TableOfContentItem> Subsections { get; set; }

	}
}