using System.Collections.Generic;

namespace Ujc.Ovj.Xml.Tei.Contents
{

	public class TableOfContentResult
	{
		public TableOfContentResult()
		{
			Sections = new List<TableOfContentItem>();
			HeadwordsList = new List<HeadwordsListItem>();
		}

		public string Errors { get; set; }

		public List<TableOfContentItem> Sections { get; set; }

		public List<HeadwordsListItem> HeadwordsList { get; set; }
	}
}