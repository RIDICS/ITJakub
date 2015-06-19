using System;

namespace Daliboris.OOXML.Word
{
	public class TextRunEventArgs : EventArgs {
		private string mstrLanguage;

		public TextRunEventArgs() { }
		public TextRunEventArgs(Style stStyle) {
			this.Style = stStyle;
		}
		public TextRunEventArgs(Style stStyle, int intSequence) {
			this.Style = stStyle;
			this.Sequence = intSequence;
		}

		public Style Style { get; set; }
		public int Sequence { get; set; }
		public string Language {
			get {
				return mstrLanguage ?? Style.Language;
			}
			set {
				mstrLanguage = value;
			}
		}
		public string Text { get; set; }
	}
}