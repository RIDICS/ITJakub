using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Atribut {
		public Atribut() { }
		public Atribut(string strNazev, string strHodnota) {
			this.Nazev = strNazev;
			this.Hodnota = strHodnota;
		}
		public string Nazev { get; set; }
		public string Hodnota { get; set; }
	}
}
