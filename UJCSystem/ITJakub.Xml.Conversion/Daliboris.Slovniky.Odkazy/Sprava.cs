using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Daliboris.Pomucky.Xml;
using System.IO;
using Daliboris.Pomucky.Soubory;

namespace Daliboris.Slovniky.Odkazy {
	public class Sprava {

		public static void VytvoritSeznamText(string strVstupniSoubor, string strVystupniSoubor, bool blnSeradit, bool blnBezDuplicit) {
			List<OdkazInfo> glsOdkazy = new List<OdkazInfo>();
			using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor)) {
				string sZdroj = null;
				while (r.Read()) {

					if (r.NodeType == XmlNodeType.Element && r.Name == "xrefs") {
						sZdroj = r.GetAttribute("source");
					}
					if (r.NodeType == XmlNodeType.Element && r.Name == "xref") {
						OdkazInfo o = new OdkazInfo(r.GetAttribute("prefix"), r.GetAttribute("hw"), r.GetAttribute("hom"), sZdroj, r.GetAttribute("source"), r.GetAttribute("target"), r.GetAttribute("defaulthw"), r.ReadString().Trim());
						glsOdkazy.Add(o);
					}
				}
			}

			if (blnSeradit)
				glsOdkazy.Sort();

			using (StreamWriter sw = new StreamWriter(strVystupniSoubor, false, System.Text.Encoding.Unicode)) {
				foreach (OdkazInfo o in glsOdkazy) {
					sw.WriteLine(o.Zaznam());
				}
			}


			if (blnBezDuplicit) {
				CteckaSouboru csb = new CteckaSouboru(strVystupniSoubor);
				csb.OdstranDuplicitniRadky(true);
				csb = null;
			}


		}

		public static void ExtrahovatSeznamXml(string strVstupniSoubor, string strVystupniSoubor, string strZdroj) {
			using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor)) {
				using (XmlWriter xw = Objekty.VytvorXmlWriter(strVystupniSoubor)) {
					string sHeslo = null;
					xw.WriteStartElement("xrefs");
					xw.WriteAttributeString("source", strZdroj);
					while (r.Read()) {
						if (r.NodeType == XmlNodeType.Element) {
							switch (r.Name) {
								case "entry":
									sHeslo = r.GetAttribute("defaulthw");
									break;
								case "xref":
									XmlDocument xd = new XmlDocument();
									XmlNode xn = xd.ReadNode(r);
									if (xn != null) {
										xd.AppendChild(xn);
										XmlAttribute xa = xd.CreateAttribute("defaulthw");
										xa.Value = sHeslo;
										xd.DocumentElement.Attributes.Append(xa);
									}

									if (xd.DocumentElement != null)
										xd.WriteContentTo(xw);
									break;
								default:
									break;
							}
							
						}
					}
					xw.WriteEndElement(); //xrefs
				}
			}
		}

	}

}
