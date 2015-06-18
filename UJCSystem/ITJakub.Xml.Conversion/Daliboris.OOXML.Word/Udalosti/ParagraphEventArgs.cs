using System;

namespace Daliboris.OOXML.Word
{
	/// <summary>
	/// Informace o odstavci (jeho pořadí a názvu)
	/// </summary>
	public class ParagraphEventArgs : EventArgs {
		public ParagraphEventArgs() { }
		public ParagraphEventArgs(Style stStyle) {
			this.Style = stStyle;
		}
		public ParagraphEventArgs(Style stStyle, int intSequence) {
			this.Style = stStyle;
			this.Sequence = intSequence;
		}
		public Style Style { get; set; }
		public int Sequence { get; set; }
	}
}