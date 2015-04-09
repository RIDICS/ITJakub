using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Citac {


		public Citac() { }
		public Citac(string strFormat, int intHodnota, int intInkrement, string strInkrementator,
			string strNazev, string strPostfix, string strPrefix, string strResetator,
			int intVychoziHodnota) {
			this.Format = strFormat;
			this.Hodnota = intHodnota;
			this.Inkrement = intInkrement;
			this.Inkrementator = strInkrementator;
			this.Nazev = strNazev;
			this.Postfix = strPostfix;
			this.Prefix = strPrefix;
			this.Resetator = strResetator;
			this.VychoziHodnota = intVychoziHodnota;
		}

		public void Inkrementovat() {
			this.Hodnota += this.Inkrement;
		}
		public void Resetovat() {
			this.Hodnota = this.VychoziHodnota;
		}


		public string Format { get; set; }
		public int Hodnota { get; set; }
		public int Inkrement { get; set; }
		public string Inkrementator { get; set; }
		public string Nazev { get; set; }
		public string Postfix { get; set; }
		public string Prefix { get; set; }
		public string Resetator { get; set; }
		public int VychoziHodnota { get; set; }

		public override string ToString() {
			return String.Format("{0}{1}{2}", this.Prefix, String.Format(this.Format, this.Hodnota), this.Postfix);
			//return base.ToString();
		}

	}
}
