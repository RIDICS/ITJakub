using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Daliboris.OOXML.Word.Transform {
	public class Tabulky {
		public Tabulky() { }
		public Tabulky(string strTabulka, string strRadek, string strBunka,
			string strObsahPrazdneBunky, string strTextMistoTabulky, bool blnCislovatElementy) {
			Tabulka = strTabulka;
			Radek = strRadek;
			Bunka = strBunka;
			ObsahPrazdneBunky = strObsahPrazdneBunky;
			TextMistoTabulky = strTextMistoTabulky;
			CislovatElementy = blnCislovatElementy;
		}
		public string Tabulka { get; set; }
		public string Radek { get; set; }
		public string Bunka { get; set; }
		public string ObsahPrazdneBunky { get; set; }
		public string TextMistoTabulky { get; set; }
		public bool CislovatElementy { get; set; }

	}
}
