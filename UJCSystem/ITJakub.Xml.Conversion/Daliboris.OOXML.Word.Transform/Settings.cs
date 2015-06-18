using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.OOXML.Word.Transform {
	public class Settings {
		public Encoding OutputEncoding { get; set; }
		public bool OutputIndent { get; set; }
		public string OutputIndentChars { get; set; }
		/// <summary>
		/// If true, elements have general name paragraph and character.
		/// </summary>
		public bool StyleNamesAsAttributes { get; set; }

		public bool SkipFootnotesAndEndnotesContent { get; set; }

		public Settings()
		{
			OutputEncoding = Encoding.UTF8;
			OutputIndent = true;
			OutputIndentChars = " ";
		}
	}
}
