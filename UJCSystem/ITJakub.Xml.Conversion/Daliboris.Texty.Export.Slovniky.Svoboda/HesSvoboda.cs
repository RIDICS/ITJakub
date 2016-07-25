﻿using System.IO;
using System.Xml;
using Daliboris.Pomucky.Xml;


namespace Daliboris.Slovniky {
	public class IndexSvob : Slovnik {
		public IndexSvob() {
		}
		public IndexSvob(string strVstupniSoubor) {
			base.VstupniSoubor = strVstupniSoubor;
		}
		public IndexSvob(string strVstupniSoubor, string strVystupniSoubor) {
			base.VstupniSoubor = strVstupniSoubor;
			base.VystupniSoubor = strVystupniSoubor;
		}

		public override void UpravitHraniceHesloveStati(string inputFile, string outputFile) {
            File.Copy(inputFile, outputFile);

            //u IndexSvob není potřeba nic upracovat, protože heslová stať zabírá vždy jeden odstavec
        }

		public override void KonsolidovatHeslovouStat(string inputFile, string outputFile) {
			int iEntry = 0;
			string sSource = null;
			using (XmlReader r = Objekty.VytvorXmlReader(inputFile)) {
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile)) {
					
					xw.WriteStartDocument(true);

					while (r.Read()) {
						if (r.NodeType == XmlNodeType.Element) {
							switch (r.Name) {
								case "entry":
									XmlDocument xd = new XmlDocument();
									XmlNode xn = xd.ReadNode(r);
									if (xn != null)
										xd.AppendChild(xn);
									if (xd.DocumentElement != null)
										if (!xd.DocumentElement.IsEmpty) {
											if (ZkonsolidujEntry(ref xd, sSource, ++iEntry))
												xd.WriteContentTo(xw);
										}

									break;
								case "dictionary":
									sSource = r.GetAttribute("name");
									goto default;
								default:
									Transformace.SerializeNode(r, xw);
									break;

							}
						}
						else if (r.NodeType == XmlNodeType.EndElement) {
							switch (r.Name) {
								case "entry":
									break;
								default:
									Transformace.SerializeNode(r, xw);
									break;
							}
						}
						else { Transformace.SerializeNode(r, xw); }

					}

				}
			}
		}

		private bool ZkonsolidujEntry(ref XmlDocument xd, string sSlovnikID, int iEntry) {

			if (xd.DocumentElement == null)
				return false;
			//Přiřadí heslové stati identifikátor
			XmlAttribute xa = xd.CreateAttribute("id");
			xa.Value = "en" + iEntry.ToString("000000");
			xd.DocumentElement.Attributes.Append(xa);

			//Přiřadí heslové stati zdroj (z jakého slovníku pochází)
			xa = xd.CreateAttribute("source");
			xa.Value = sSlovnikID;
			xd.DocumentElement.Attributes.Append(xa);

			//Zjišťuje typ heslové stati (uvedený v atributu type u značky entryhead)

			XmlNode xn = xd.SelectSingleNode("//entryhead");
			if (xn != null) {
				//Pokud existuje atibut type existuje, jeho hodnota se přiřadí ke značce entry
				if (xn.Attributes.Count == 1) {
					xa = (XmlAttribute)xn.Attributes[0].Clone();
					xd.DocumentElement.Attributes.Append(xa);
					//nakonec se atribut type u značky entryhead odstraní (byl tam jenom kvůli generování z Wordu)
					xn.Attributes.Remove(xn.Attributes[0]);
				}
				else {
					//pokud entryhead atribut nemá, jde o plnohodnotnou heslovou stať
					xa = xd.CreateAttribute("type");
					xa.Value = "full";
					xd.DocumentElement.Attributes.Append(xa);
				}
			}
			//heslové stati se přiřadí výchozí heslové slovo
			//TODO mělo by se dávat pozor na zkrácené podoby?
			xn = xd.SelectSingleNode("//hw");
			xa = xd.CreateAttribute("defaulthw");
			xa.Value = xn.InnerText.TrimEnd(Slovnik.CarkaTeckaMezeraStrednik).TrimEnd();
			string sVychoziHeslo = xa.Value;
			xd.DocumentElement.Attributes.Append(xa);

			xa = xd.CreateAttribute("defaulthwsort");
			xa.Value = Slovnik.HesloBezKrizkuAHvezdicky(sVychoziHeslo);
			xd.DocumentElement.Attributes.Append(xa);


			Daliboris.Slovniky.Svoboda.Index hsSvoboda = new Daliboris.Slovniky.Svoboda.Index();
			Daliboris.Slovniky.Svoboda.Index.HeslovaSlova hss = hsSvoboda.AnalyzujHeslo(xd, sVychoziHeslo, ref iEntry);
				
			if (hss.Count == 0 || hss[0].Odkaz || hss[0].Transliterace)
				return false;
			else
				return true;
		}

	}
}