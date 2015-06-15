using System;

namespace Daliboris.OOXML.Word
{
	public class TableEventArgs : EventArgs {
		public string StyleID { get; set; }
		public string StyleName { get; set; }
		public int Sequence { get; set; }
	}
}