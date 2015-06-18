using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Koren {
		private Atributy matAtributy = new Atributy();

		public Koren() { }
		public Koren(string strNamespace, string strNazev, Atributy atAtributy) {

			matAtributy = atAtributy;
			this.Namespace = strNamespace;
			this.Nazev = strNazev;
		}

		public Atributy Atributy {
			get { return matAtributy; }
			set { matAtributy = value; }
		}
		public string Namespace { get; set; }
		public string Nazev { get; set; }

	}
}
