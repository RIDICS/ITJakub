using System;

namespace Daliboris.OOXML.Word
{
	public class FontEventArgs : EventArgs {
		public string FontName { get; set; }
		public int Sequence { get; set; }
	}
}