using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Daliboris.Slovniky.Svoboda {
	public class Heslar : Slovniky.Heslar {

		public void HeslarXml(string strVstupniSoubor, string strVystupniSoubor) {
			Dictionary<string, ZpracovatTagProHeslarXml> gdZpracovaniTagu = new Dictionary<string, ZpracovatTagProHeslarXml>();
			gdZpracovaniTagu.Add("hwo", ZpracovatTagXml);
			//Daliboris.Slovniky.Heslar.HeslarXml(strVstupniSoubor, strVystupniSoubor, gdZpracovaniTagu);
			Daliboris.Slovniky.Heslar.HeslarXml(strVstupniSoubor, strVystupniSoubor, null);
		}

		internal static void ZpracovatTagXml(XmlReader xrVstup, XmlWriter xwVystup,
			PismenoInfo piPismeno, HeslovaStatInfo hsiHeslovaStat, HesloInfo hiHeslo) {
			string strNazevTagu = xrVstup.Name;
			if (xrVstup.NodeType == XmlNodeType.Element && strNazevTagu == "hwo") {
				if (hiHeslo == null)
					hiHeslo = new HesloInfo();
				hiHeslo.ZpracujHesloveSlovo(xrVstup.ReadString(), null);
				xwVystup.WriteElementString("hw", hiHeslo.HesloveSlovo);
  		}
			else if (xrVstup.NodeType == XmlNodeType.EndElement && strNazevTagu == "hwo") { 
			}
		}
	}

}
