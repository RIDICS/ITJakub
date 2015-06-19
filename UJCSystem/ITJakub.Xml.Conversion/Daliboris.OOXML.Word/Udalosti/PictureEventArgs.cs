using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.OOXML.Word {
 public class PictureEventArgs : EventArgs {
	public string Identifikator { get; set; }
		public int Sequence { get; set; }
		public string Umisteni { get; set; }
		public PictureEventArgs() { }
		public PictureEventArgs(int intSequence, string strIdentifikator) {
			Identifikator = strIdentifikator;
			Sequence = intSequence;

		}
  }
}
