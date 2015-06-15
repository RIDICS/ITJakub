using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	[DebuggerDisplay("HeadText = {HeadInfo.HeadText}, DivXmlId = {DivXmlId}, Level = {Level}, Type = {HeadwordInfo.Type}")]
	public class HeadwordsListItem : ItemBase
	{
		private readonly HeadwordInfo _headwordInfo;

		public HeadwordsListItem() : base()
		{
			_headwordInfo = new HeadwordInfo();
		}

		public HeadwordsListItem(HeadwordsListItem parent)
			: base(parent)
		{
			_headwordInfo = new HeadwordInfo();
		}


		public HeadwordInfo HeadwordInfo
		{
			get { return _headwordInfo; }
		}
	}
}