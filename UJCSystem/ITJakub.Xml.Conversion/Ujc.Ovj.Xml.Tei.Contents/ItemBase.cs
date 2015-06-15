using System.Collections.Generic;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public abstract class ItemBase
	{
		private PageBreakInfo _pageBreakInfo;
		private HeadInfo _headInfo;

		public ItemBase()
		{
			_headInfo = new HeadInfo();
			_pageBreakInfo = new PageBreakInfo();
			Sections = new List<ItemBase>();
		}

		public ItemBase(ItemBase parent)
			: this()
		{
			Parent = parent;
		}

		public ItemBase Parent { get; set; }

		public List<ItemBase> Sections { get; set; }


		public PageBreakInfo PageBreakInfo
		{
			get { return _pageBreakInfo; }
		}

		public HeadInfo HeadInfo
		{
			get { return _headInfo; }
		}

		public int Level { get; set; }
		public string DivXmlId { get; set; }

	}
}