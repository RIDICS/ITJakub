using System;

namespace Daliboris.OOXML.Word
{
	public class PrimeFormatovaniEventArgs : EventArgs {
		public PrimeFormatovaniEventArgs () {}
		public PrimeFormatovaniEventArgs(string strFormat, string strText, string strStyl, int intSequence) {
			PrimeFormatovani = strFormat;
			Text = strText;
			Styl = strStyl;
			Sequence = intSequence;
		}
		public string PrimeFormatovani { get; set; }
		public string Text { get; set; }
		public string Styl { get; set; }
		public int Sequence { get; set; }
	}
}