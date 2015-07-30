using System;
using System.Xml;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public class HeadInfo
	{
	    private string _headSort;

	    public HeadInfo()
		{
		}

		public string HeadText { get; set; }

	    /// <summary>
	    /// Form of headword userd for sorting.
	    /// Leading nonalphabetical characters (*, † etc.) are removed.
	    /// </summary>
	    public string HeadSort()
	    {
	        ComputeHeadSort();
            return _headSort;
	    }

	    private void ComputeHeadSort()
	    {
            if(_headSort == null)
	            _headSort = HeadwordCleaner.CleanForSorting(HeadText);
	        
	    }
		public XmlElement HeadXml { get; set; }
	}
}