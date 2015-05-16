using System.Collections.Generic;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public class TableOfContentItem : ItemBase
	{

		public TableOfContentItem() : base()
		{
			Sections = new List<TableOfContentItem>();
		}

		public TableOfContentItem(ItemBase parent)
			: base(parent)
		{
			Sections = new List<TableOfContentItem>();
		}

		public List<TableOfContentItem> Sections { get; set; }

	}
}