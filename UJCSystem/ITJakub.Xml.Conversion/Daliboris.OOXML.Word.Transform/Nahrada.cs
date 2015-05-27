using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Nahrada {
		public Nahrada() { }
		public Nahrada(string strCo, string strCim) {
			this.Co = strCo;
			this.Cim = strCim;
		}
		public string Co { get; set; }
		public string Cim { get; set; }
	}
}
