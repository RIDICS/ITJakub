using System.Collections.Generic;
using System.Diagnostics;
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

	[DebuggerDisplay("Head = {Head}, DivXmlId = {DivXmlId}, Level = {Level}, Type = {Type}")]
	public class TableOfContentItem
	{
		public TableOfContentItem Parent { get; set; }

		public TableOfContentItem()
		{
			Sections = new List<TableOfContentItem>();
		}

		public TableOfContentItem(TableOfContentItem parent)
			: this()
		{
			Parent = parent;
		}


		public string Head { get; set; }
		public XmlElement HeadXml { get; set; }
		public string PageBreak { get; set; }
		public string PageBreakXmlId { get; set; }
		public int Level { get; set; }
		public string Type { get; set; }

		public string DivXmlId { get; set; }

		public List<TableOfContentItem> Sections { get; set; }

	}
}