using System;

namespace Daliboris.OOXML.Word
{
	public class FootnoteEventArgs : EventArgs {
		public string Identifikator { get; set; }
		public int Sequence { get; set; }
		public FootnoteEventArgs() { }
		public FootnoteEventArgs(int intSequence, string strIdentifikator) {
			Identifikator = strIdentifikator;
			Sequence = intSequence;
		}
	}
}