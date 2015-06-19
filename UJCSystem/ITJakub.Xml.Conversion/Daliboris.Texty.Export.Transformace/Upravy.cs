using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Daliboris.Pomucky.Funkce.Textove;
using Daliboris.Texty.Evidence.Rozhrani;
using Daliboris.Texty.Export.Rozhrani;
using Daliboris.Transkripce.Objekty;

namespace Daliboris.Texty.Export {
 public abstract class Upravy : IUpravy {

	private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

	protected Upravy() { }

	protected Upravy(IExportNastaveni ienNastaveni) {
	 Nastaveni = ienNastaveni;
	}

	#region Implementation of IUpravy

	public abstract void Uprav(IPrepis prepis);

	public IExportNastaveni Nastaveni { get; set; }

	/// <summary>
	/// Upraví závěr dokumentu (&gt;back&lt;), které obsahuje závěrečné pasáže, např. informaci a autorských právech.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru se závěrem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že závěr nic neobsahuje.</remarks>
	public abstract string UpravZaver(IPrepis prepis, List<ITransformacniKrok> kroky);

	public abstract string ZkombinujCastiTextu(IPrepis prepis, string strHlavicka, string strUvod, string strTelo, string strZaver, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// Přiřadí identifikátory důležitým prvkům
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
	/// <returns>Vrací celou cestu k přetransformovanému souboru.</returns>
	public abstract string PriradId(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	public abstract string ProvedUpravy(IPrepis prepis, string strVstup, IEnumerable<UpravaSouboruXml> upravy);

	/// <summary>
	/// Vrací název výstupu pro zadaný přepis.
	/// </summary>
	/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
	/// <returns>Název výstupu pro zadaný přepis</returns>
	/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
	public abstract string DejNazevVystupu(IPrepis prepis);

	/// <summary>
	/// Vrací název předchozího výstupu pro zadaný přepis.
	/// </summary>
	/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
	/// <returns>Název předchozího výstupu pro zadaný přepis</returns>
	/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
	public abstract string DejNazevPredchozihoVystupu(IPrepis prepis);


	public abstract int PocetKroku { get; }
	public abstract void SmazDocasneSoubory();
	public abstract void NastavVychoziHodnoty();


	public abstract string DocasnaSlozka { get; }

	/// <summary>
	/// Upraví tělo dokumentu (&gt;body&lt;), které obsahuje hlavní text.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož tělo se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na tělo aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru s tělem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
	public abstract string UpravTelo(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// Upraví úvod dokumentu (&gt;front&lt;), který obsahuje nadpis a autora.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož úvod se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na úvod aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru s úvodem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
	public abstract string UpravUvod(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// Upraví hlavičku dokumentu (&gt;teiHeader&lt;).
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož hlavička se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na hlavičku aplikovat.</param>
	/// <returns>Vrací celou cestu k vytvořenému souboru.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že hlavička nic neobsahuje.</remarks>
	public abstract string UpravHlavicku(IPrepis prepis, List<ITransformacniKrok> kroky);

	public abstract string UpravPoSpojeni(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	public static void RozdelitBibliNaVerse(string strVstup, string strVystup) {
	 
	 _logger.Debug("Metoda={0}; Vstup={1}; Vystup={2}", "RozdelitBibliNaVerse", strVstup, strVystup);
	 //Console.WriteLine("{0} => {1}, {2}", strVstup, strVystup, "RozdelitBibliNaVerse");

	 bool blnJeP = false;
	 bool blnObsahujePVers = false;
	 bool blnJeSeg = false;
	 using (XmlReader xr = XmlReader.Create(strVstup)) {
		using (XmlWriter xw = XmlWriter.Create(strVystup)) {
		 StrukturaBible sbBible = new StrukturaBible();
		 bool blnPrvniVers = true;
		 while (xr.Read()) {
			if (xr.NodeType == XmlNodeType.Element) {
			 string sNazev = xr.Name;
			 string sTyp;
			 string sPodtyp;
			 string sN;

			 switch (sNazev) {
				case "div":
				sTyp = xr.GetAttribute("type");
				sPodtyp = xr.GetAttribute("subtype");
				sN = xr.GetAttribute("n");

				if (sTyp == "bible" && sPodtyp == "book")
				 sbBible.Kniha = sN;
				if (sTyp == "bible" && sPodtyp == "chapter") {
				 if (sbBible.Kapitola != sN) {
					blnPrvniVers = true;
					sbBible.Kapitola = sN;
				 }

				}
				if (sTyp == "explicit") {
				 //sbBible.Kniha = null;
				 sbBible = new StrukturaBible();
				}
				goto default;
				case "anchor":
				sTyp = xr.GetAttribute("type");
				sPodtyp = xr.GetAttribute("subtype");
				sN = xr.GetAttribute("n");

				if (sTyp == "delimiter") {
				 switch (sPodtyp) {
					case "bookStart":
					sbBible.Kniha = sN;
					blnPrvniVers = true;
					break;
					case "chapterStart":
					sbBible.Kapitola = sN;
					blnPrvniVers = true;
					break;
					case "chapterEnd":
					case "bookEnd":
					//xw.WriteEndElement(); //předchozí <seg>
					break;
				 }
				 goto default;
				}

				if (sTyp == "bible") {
				 if (sPodtyp == "chapter") {
					sbBible.Kapitola = sN;
					goto default;
				 }
				 if (sPodtyp == "verse") {
					if (!blnPrvniVers) {
					 xw.WriteEndElement();
					}
					blnPrvniVers = false;
					if (blnJeP)
					 blnObsahujePVers = true;
					sbBible.Vers = sN;
					xw.WriteStartElement("seg");
					xw.WriteAttributeString("type", sTyp);
					xw.WriteAttributeString("subtype", sPodtyp);
					xw.WriteAttributeString("n", sN);
					xw.WriteAttributeString("xml", "id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml());
					blnJeSeg = true;
				 }
				}
				else {
				 goto default;
				}

				break;
				//na začátku odstavce je vždy "první" verš, nesmí se ukončit předchozí značka
				//v Prorocích odstavec bez verše
				case "p":
				blnPrvniVers = true;
				blnJeP = true;
				goto default;
				default:
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				break;
			 } // switch (sNazev) {
			} // if (xr.NodeType == XmlNodeType.Element)
			else if (xr.NodeType == XmlNodeType.Text) {
			 if (blnJeSeg) {
				if (xr.Value[0] == ' ') {
				 string sText = xr.Value;
				 xw.WriteString('\u00A0' + sText.Substring(1));
				}
				else {
				 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				}
			 }
			 else
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			 blnJeSeg = false;
			}
			else if (xr.NodeType == XmlNodeType.EndElement) {
			 if (xr.Name == "p" && sbBible.Kapitola != null) {
				if (blnObsahujePVers)
				 xw.WriteEndElement(); //</seg>
			 }
			 if (xr.Name == "p") {
				blnJeP = false;
				blnObsahujePVers = false;
			 }
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
			else {
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
		 } //while (xr.Read())
		} //using (XmlWriter xw = XmlWriter.Create(strVystup))
	 } //using (XmlReader xr = XmlReader.Create(strVstup))

	 //return strVystup;

	}

	/// <summary>
	/// Rozdělí bibli na biblické verše
	/// </summary>
	/// <param name="strVstup">Vstupní soubor Xml</param>
	/// <param name="strVystup">Výstupní soubor Xml</param>
	/// <returns></returns>
	public static void RozdelitBibliNaVerseOld(string strVstup, string strVystup) {
	 //string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

	 using (XmlReader xr = XmlReader.Create(strVstup)) {
		using (XmlWriter xw = XmlWriter.Create(strVystup)) {
		 StrukturaBible sbBible = new StrukturaBible();
		 bool blnPrvniVers = true;
		 while (xr.Read()) {
			if (xr.NodeType == XmlNodeType.Element) {
			 string sNazev = xr.Name;
			 string sTyp;
			 string sPodtyp;
			 string sN;

			 switch (sNazev) {
				case "div":
				sTyp = xr.GetAttribute("type");
				sPodtyp = xr.GetAttribute("subtype");
				sN = xr.GetAttribute("n");

				if (sTyp == "bible" && sPodtyp == "book")
				 sbBible.Kniha = sN;
				if (sTyp == "bible" && sPodtyp == "chapter") {
				 if (sbBible.Kapitola != sN) {
					blnPrvniVers = true;
					sbBible.Kapitola = sN;
				 }

				}
				if (sTyp == "explicit") {
				 //sbBible.Kniha = null;
				 sbBible = new StrukturaBible();
				}
				goto default;
				case "anchor":
				sTyp = xr.GetAttribute("type");
				sPodtyp = xr.GetAttribute("subtype");
				sN = xr.GetAttribute("n");

				if (sTyp == "delimiter") {
				 if (sPodtyp == "chapterStart") {
					sbBible.Kapitola = sN;
					blnPrvniVers = true;
				 }
				 else if (sPodtyp == "chapterEnd") {
					xw.WriteEndElement(); //předchozí <seg>
				 }
				 goto default;
				}
				if (sTyp == "bible") {
				 if (sPodtyp == "chapter") {
					sbBible.Kapitola = sN;
					goto default;
				 }
				 if (sPodtyp == "verse") {
					if (!blnPrvniVers) {
					 xw.WriteEndElement();
					}
					blnPrvniVers = false;
					sbBible.Vers = sN;
					xw.WriteStartElement("seg");
					xw.WriteAttributeString("type", sTyp);
					xw.WriteAttributeString("subtype", sPodtyp);
					xw.WriteAttributeString("n", sN);
					xw.WriteAttributeString("xml", "id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml());
				 }
				}
				else {
				 goto default;
				}

				break;
				//na začátku odstavce je vždy "první" verš, nesmí se ukončit předchozí značka
				case "p":
				blnPrvniVers = true;
				goto default;
				default:
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				break;
			 } // switch (sNazev) {
			} // if (xr.NodeType == XmlNodeType.Element) {
			else if (xr.NodeType == XmlNodeType.EndElement) {
			 if (xr.Name == "p" && sbBible.Kapitola != null) {
				xw.WriteEndElement(); //</seg>
			 }
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
			else {
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
		 } //while (xr.Read())
		} //using (XmlWriter xw = XmlWriter.Create(strVystup))
	 } //using (XmlReader xr = XmlReader.Create(strVstup))

	 //return strVystup;
	}

	public static void RozdelitBibliNaKnihyAKapitoly(string strVstup, string strVystup) {
	 _logger.Debug("Metoda={0}; Vstup={1}; Vystup={2}", "RozdelitBibliNaKnihyAKapitoly", strVstup, strVystup);
	 //Console.WriteLine("{0} => {1}, {2}", strVstup, strVystup, "RozdelitBibliNaKnihyAKapitoly");

	 string sKorenovyPrvek = null;
	 using (XmlWriter xw = XmlWriter.Create(strVystup)) {
		StrukturaBible sbBible = new StrukturaBible();
		using (XmlReader xr = XmlReader.Create(strVstup)) {
		 while (xr.Read()) {
			if (xr.NodeType == XmlNodeType.Element) {
			 if (sKorenovyPrvek == null)
				sKorenovyPrvek = xr.Name;
			 string sNazev = xr.Name;
			 if (sNazev == "anchor") {
				string sType = xr.GetAttribute("type");

				if (sType == "bible") {
				 string sKniha = null;
				 string sKapitola = null;

				 string sSubtype = xr.GetAttribute("subtype");
				 if (sSubtype == "book") {
					sKniha = xr.GetAttribute("n");
					if (sbBible.Kniha == sKniha)
					 continue;
					StrukturaBible sb = new StrukturaBible(sKniha);
					if (sbBible.Kniha != null)
					 ZapsatKoncovouZnacku(xw, sbBible, sSubtype);
					ZapsatPocatecniZnacku(xw, sb, sSubtype);
					sbBible = new StrukturaBible(sKniha);
				 }
				 else if (sSubtype == "chapter") {
					sKapitola = xr.GetAttribute("n");
					if (sbBible.Kapitola == sKapitola)
					 continue;
					if (sbBible.Kapitola != sKapitola) {
					 if (sbBible.Kapitola != null)
						ZapsatKoncovouZnacku(xw, sbBible, sSubtype);
					 sbBible.Kapitola = sKapitola;
					 ZapsatPocatecniZnacku(xw, sbBible, sSubtype);
					}
				 } //if (sSubtype == "book") 
				 else
					Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				} //if (sType == "bible")
				else
				 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			 } //if (sNazev == "anchor")
			 else
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			} //if (xr.NodeType == XmlNodeType.Element)
			else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == sKorenovyPrvek) {
				//v případech, kdy jde o prology a text není rozčleněn na biblické knihy
				if (sbBible.Kniha != null)
				{
					ZapsatKoncovouZnacku(xw, sbBible, "chapter");
					ZapsatKoncovouZnacku(xw, sbBible, "book");
				}
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
			else
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
		 }
		}
	 }
	}


	private static void ZapsatPocatecniZnacku(XmlWriter xw, StrukturaBible sbBible, string strTyp) {
	 xw.WriteStartElement("anchor");
	 xw.WriteAttributeString("type", "delimiter");
	 xw.WriteAttributeString("subtype", strTyp + "Start");
	 if (strTyp == "book") {
		xw.WriteAttributeString("n", sbBible.Kniha);
		xw.WriteAttributeString("id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml(VypisStruktury.Kniha));
	 }
	 else if (strTyp == "chapter") {
		xw.WriteAttributeString("n", sbBible.Kapitola);
		xw.WriteAttributeString("id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml(VypisStruktury.Kapitola));
	 }
	 xw.WriteEndElement(); //<anchor>
	}

	private static void ZapsatKoncovouZnacku(XmlWriter xw, StrukturaBible sbBible, string strTyp) {
	 xw.WriteStartElement("anchor");
	 xw.WriteAttributeString("type", "delimiter");
	 xw.WriteAttributeString("subtype", strTyp + "End");
	 if (strTyp == "book") {
		xw.WriteAttributeString("n", sbBible.Kniha);
		xw.WriteAttributeString("id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml(VypisStruktury.Kniha) + ".e");
	 }
	 else if (strTyp == "chapter") {
		xw.WriteAttributeString("n", sbBible.Kapitola);
		xw.WriteAttributeString("id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml(VypisStruktury.Kapitola) + ".e");
	 }
	 xw.WriteEndElement(); //<anchor>
	}

	public static void RozdelitBibliNaKnihyAKapitolyOld(string strVstup, string strVystup) {
	 //string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

	 using (XmlReader xr = XmlReader.Create(strVstup)) {
		using (XmlWriter xw = XmlWriter.Create(strVystup)) {

		 StrukturaBible sbBible = new StrukturaBible();

		 string sKonecKapitoly = null;
		 while (xr.Read()) {
Zacatek:
			if (xr.NodeType == XmlNodeType.Element) {
			 string sNazev = xr.Name;
			 switch (sNazev) {
				/*
			case "anchor":
				string sTyp = xr.GetAttribute("type");
				string sPodtyp = xr.GetAttribute("subtype");
				string sId = xr.GetAttribute("xml:id");
				break;
			*/
				#region case div

				case "div":
				XmlDocument xdcDiv = Pomucky.Xml.Objekty.ReadNodeAsXmlDocument(xr);  //po načtení celého prvku se kurzor přesune na začátek dalšího tagu

				//if (xdcDiv.InnerXml.Contains("Počíná sě chvála tří dietek, jenžto pána boha chválili v peci"))
				if (xdcDiv.InnerXml.Contains("Kapitola třidcátá čte sě"))
				 sKonecKapitoly = sKonecKapitoly;

				XmlNodeList xnlbBook = xdcDiv.SelectNodes("//anchor[@type='bible' and @subtype = 'book']");
				if (xnlbBook != null && xnlbBook.Count > 0) {
				 string sKniha = null;
				 XmlNode xnb = xnlbBook[0];
				 if (xnb.Attributes != null)
					sKniha = xnb.Attributes.GetNamedItem("xml:id").Value;
				 sKniha = StrukturaBible.ZiskejUdajZAtributu(sKniha);
				 if (sbBible.Kniha != sKniha) {

					if (sbBible.Kniha != null) {
					 xw.WriteEndElement(); //div
					}

					sbBible.Kniha = sKniha;

					xw.WriteStartElement("div");
					//xw.WriteAttributeString("type", "book");
					xw.WriteAttributeString("type", "bible");
					xw.WriteAttributeString("subtype", "book");
					xw.WriteAttributeString("n", sKniha);
					xw.WriteAttributeString("xml", "id", "http://www.w3.org/XML/1998/namespace", sbBible.IdentifikatorXml(VypisStruktury.Kniha));

				 }
				 foreach (XmlNode node in xnlbBook) {
					if (node.ParentNode != null)
					 node.ParentNode.RemoveChild(node);
				 }

				} //if (xnlbBook != null && xnlbBook.Count > 0)

				XmlNodeList xnlpOdstravce = xdcDiv.SelectNodes("//p");
				Dictionary<XmlNode, KrajniStrukturyBible> dcOdstavecMisto = new Dictionary<XmlNode, KrajniStrukturyBible>();
				if (xnlpOdstravce != null && xnlpOdstravce.Count > 1) {

				 //zkontrolovat, jestli taky náhodou neobsahuje začátek biblické knihy

				 List<XmlNodeList> xnlpb = new List<XmlNodeList>();
				 List<XmlNodeList> xnlpc = new List<XmlNodeList>();
				 List<XmlNodeList> xnlpv = new List<XmlNodeList>();

				 foreach (XmlNode node in xnlpOdstravce) {
					XmlNodeList nlpBook = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'book']");
					xnlpb.Add(nlpBook);
					XmlNodeList nlpChapter = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'chapter']");
					xnlpc.Add(nlpChapter);
					XmlNodeList nlpVerse = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'verse']");
					xnlpv.Add(nlpVerse);

					StrukturaBible sb1 = new StrukturaBible();
					// ReSharper disable PossibleNullReferenceException
					if (nlpBook != null && nlpBook.Count > 0)
					 sb1.Kniha = StrukturaBible.ZiskejUdajZAtributu(nlpBook[0].Attributes["xml:id"].Value);
					if (nlpChapter != null && nlpChapter.Count > 0)
					 sb1.Kapitola = StrukturaBible.ZiskejUdajZAtributu(nlpChapter[0].Attributes["xml:id"].Value);
					if (nlpVerse != null && nlpVerse.Count > 0)
					 sb1.Vers = StrukturaBible.ZiskejUdajZAtributu(nlpVerse[0].Attributes["xml:id"].Value);
					StrukturaBible sbp = new StrukturaBible();
					if (nlpBook != null && nlpBook.Count > 0)
					 sbp.Kniha = StrukturaBible.ZiskejUdajZAtributu(nlpBook[nlpBook.Count - 1].Attributes["xml:id"].Value);
					if (nlpChapter != null && nlpChapter.Count > 0)
					 sbp.Kapitola = StrukturaBible.ZiskejUdajZAtributu(nlpChapter[nlpChapter.Count - 1].Attributes["xml:id"].Value);
					if (nlpVerse != null && nlpVerse.Count > 0)
					 sbp.Vers = StrukturaBible.ZiskejUdajZAtributu(nlpVerse[nlpChapter.Count - 1].Attributes["xml:id"].Value);
					// ReSharper restore PossibleNullReferenceException

					if (sb1.Kniha == null)
					 sb1.Kniha = sbBible.Kniha;
					if (sbp.Kniha == null)
					 sbp.Kniha = sbBible.Kniha;
					if (sbp.Kapitola == null) {
					 sbp.Kapitola = sbBible.Kapitola;
					 if (sb1.Kapitola == null)
						sb1.Kapitola = sbBible.Kapitola;
					}

					KrajniStrukturyBible ksb = new KrajniStrukturyBible(sb1, sbp);

					dcOdstavecMisto.Add(node, ksb);
				 }

				 bool bCoOdstavecToKapitola = false;
				 foreach (KeyValuePair<XmlNode, KrajniStrukturyBible> kvp in dcOdstavecMisto) {
					if (kvp.Value.Zacatek.Kapitola == kvp.Value.Konec.Kapitola) {
					 bCoOdstavecToKapitola = true;
					}
					else {
					 bCoOdstavecToKapitola = false;
					 break;
					}
				 }

				 if (bCoOdstavecToKapitola) {
					foreach (KeyValuePair<XmlNode, KrajniStrukturyBible> kvp in dcOdstavecMisto) {
					 xw.WriteStartElement("div");
					 xw.WriteAttributeString("type", "bible");
					 xw.WriteAttributeString("subtype", "chapter");
					 xw.WriteAttributeString("n", kvp.Value.Zacatek.Kapitola);
					 xw.WriteAttributeString("xml", "id", "http://www.w3.org/XML/1998/namespace",
											new StrukturaBible(kvp.Value.Zacatek.Kniha, kvp.Value.Zacatek.Kapitola).IdentifikatorXml(VypisStruktury.Kniha));

					 kvp.Key.WriteTo(xw);
					 xw.WriteEndElement(); //div
					 sbBible.Kapitola = kvp.Value.Zacatek.Kapitola;
					 sKonecKapitoly = kvp.Value.Konec.Kapitola;
					}
					goto Zacatek;
					break;
				 }
				} //if (xnlpOdstravce != null && xnlpOdstravce.Count > 1)

				XmlNodeList xnlcChapter = xdcDiv.SelectNodes("//anchor[@type='bible' and @subtype = 'chapter']");
				XmlNodeList xnlvVerse = xdcDiv.SelectNodes("//anchor[@type='bible' and @subtype = 'verse']");


				//Zde

				if (xnlvVerse != null)
				 if (xnlcChapter != null && xnlvVerse.Count > 0) {
					string sKonec = null;
					XmlNode xnck = xnlcChapter[xnlcChapter.Count - 1];

					if (xnck.Attributes != null)
					 sKonec = xnck.Attributes.GetNamedItem("xml:id").Value;
					//if (sKonec == "v.ID0EUUGK.51")
					// sKonec = "v.ID0EUUGK.51";

					sKonec = StrukturaBible.ZiskejUdajZAtributu(sKonec);
					XmlNode xncz = xnlcChapter[0];
					if (xncz.Attributes != null) {
					 string sZacatekKapitoly = xncz.Attributes.GetNamedItem("xml:id").Value;
					 sZacatekKapitoly = StrukturaBible.ZiskejUdajZAtributu(sZacatekKapitoly);
					 sbBible.Kapitola = sZacatekKapitoly;
					 if (sZacatekKapitoly == sKonec) {
						if (sKonec != sKonecKapitoly) {

						 XmlAttribute xat = xdcDiv.CreateAttribute("type");
						 xat.Value = "bible";
						 XmlNode xde = xdcDiv.DocumentElement;
						 if (xde != null) {
							if (xde.Attributes != null)
							 xde.Attributes.Append(xat);

							xat = xdcDiv.CreateAttribute("subtype");
							xat.Value = "chapter";
							if (xde.Attributes != null)
							 xde.Attributes.Append(xat);

							XmlAttribute xan = xdcDiv.CreateAttribute("n");
							xan.Value = sZacatekKapitoly;
							if (xde.Attributes != null)
							 xde.Attributes.Append(xan);

							XmlAttribute xaid = xdcDiv.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
							 xaid.Value = sbBible.IdentifikatorXml(VypisStruktury.Kapitola);
							if (xde.Attributes != null)
							 xde.Attributes.Append(xaid);
						 }
						}
						foreach (XmlNode node in xnlcChapter) {
						 if (node.ParentNode != null)
							node.ParentNode.RemoveChild(node);
						}

					 }
					 else {
						XmlElement xe = VytvoritAnchorChapterStart(xdcDiv, sbBible, sZacatekKapitoly);
						if (xdcDiv.DocumentElement != null)
						 xdcDiv.DocumentElement.InsertBefore(xe, xdcDiv.DocumentElement.FirstChild);

						bool bBylKonec = false;
						foreach (XmlNode node in xnlcChapter) {
						 if (node.Attributes != null) {
							string sk = StrukturaBible.ZiskejUdajZAtributu(node.Attributes.GetNamedItem("xml:id").Value);
							if (!bBylKonec && sk == sKonec) {

							 bBylKonec = true;
							 XmlElement xche = VytvoritAnchorChapterEnd(xdcDiv, sbBible, sZacatekKapitoly);
							 XmlElement xchs = VytvoritAnchorChapterStart(xdcDiv, sbBible, sKonec);
							 if (node.ParentNode != null) {
								node.ParentNode.InsertBefore(xche, node);
								node.ParentNode.InsertBefore(xchs, node);
							 }
							}
						 }
						 if (node.ParentNode != null)
							node.ParentNode.RemoveChild(node);
						}
					 }
					}
					sKonecKapitoly = sKonec;
				 } // if (xnlcChapter != null && xnlvVerse.Count > 0)

				xdcDiv.WriteTo(xw);
				/*
				if (xnlc != null && xnlv.Count > 0) {
					xw.WriteEndElement(); //<div type='chapter'>
				}
				*/
				goto Zacatek;

				break;
				#endregion

				default:
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				break;
			 }
			}
			else if (xr.NodeType == XmlNodeType.EndElement) {
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
			else {
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
		 }
		}
	 }

	 //return strVystup;
	}


	private static XmlElement VytvoritAnchorChapterStart(XmlDocument xdc, StrukturaBible sbBible, string sCisloKapitoly) {
	 XmlElement xe = xdc.CreateElement("anchor");
	 XmlAttribute xa = xdc.CreateAttribute("type");
	 xa.Value = "delimiter";
	 xe.Attributes.Append(xa);

	 xa = xdc.CreateAttribute("subtype");
	 xa.Value = "chapterStart";
	 xe.Attributes.Append(xa);

	 xa = xdc.CreateAttribute("n");
	 xa.Value = sCisloKapitoly;
	 xe.Attributes.Append(xa);

	 XmlAttribute xaid = xdc.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
	 xaid.Value = new StrukturaBible(sbBible.Kniha, sCisloKapitoly).IdentifikatorXml(VypisStruktury.Kapitola);
	 return xe;
	}

	private static XmlElement VytvoritAnchorChapterEnd(XmlDocument xdc, StrukturaBible sbBible, string sCisloKapitoly) {
	 XmlElement xe = xdc.CreateElement("anchor");
	 XmlAttribute xa = xdc.CreateAttribute("type");
	 xa.Value = "delimiter";
	 xe.Attributes.Append(xa);

	 xa = xdc.CreateAttribute("subtype");
	 xa.Value = "chapterEnd";
	 xe.Attributes.Append(xa);

	 xa = xdc.CreateAttribute("n");
	 xa.Value = sCisloKapitoly;
	 xe.Attributes.Append(xa);

	 XmlAttribute xaid = xdc.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
	 xaid.Value = new StrukturaBible(sbBible.Kniha, sCisloKapitoly).IdentifikatorXml(VypisStruktury.Kapitola) + ".e";
	 return xe;
	}

	#region Odkazy na biblická místa
	/// <summary>
	/// Vytvoří biblických míst odkazy na biblická místa.
	/// </summary>
	/// <param name="strVstup">Název výstupního souboru</param>
	/// <param name="strVystup">Název výstupního souboru</param>
	public static string OdkazyNaBiblickyText(string strVstup, string strVystup) {
	 //string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

	 using (XmlReader xr = XmlReader.Create(strVstup)) {
		using (XmlWriter xw = XmlWriter.Create(strVystup)) {
		 while (xr.Read()) {
			if (xr.NodeType == XmlNodeType.Element) {
			 string sNazev = xr.Name;
			 if (sNazev == "anchor") {
				string sType;
				string sSubtype;
				string sN;
				NactiAtributy(xr, out sType, out sSubtype, out sN);
				if (sType == "bible" && sSubtype == "book") {
				 StrukturaBible sb = new StrukturaBible();
				 sb.Kniha = sN;

				 xr.Read();
				 NactiAtributy(xr, out sType, out sSubtype, out sN);
				 if (sType == "bible" && sSubtype == "chapter")
					sb.Kapitola = sN;
				 else {
					ZapsatOdkazNaBiblickeMisto(sb, xw);
					Pomucky.Xml.Transformace.SerializeNode(xr, xw);
					break;
				 }
				 xr.Read();
				 NactiAtributy(xr, out sType, out sSubtype, out sN);
				 if (sType == "bible" && sSubtype == "verse")
					sb.Vers = sN;
				 else {
					ZapsatOdkazNaBiblickeMisto(sb, xw);
					Pomucky.Xml.Transformace.SerializeNode(xr, xw);
					break;
				 }
				 ZapsatOdkazNaBiblickeMisto(sb, xw);
				}
				else
				 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			 }
			 else
				Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			}
			else
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
		 }
		}
		return strVystup;
	 }
	}

	private static void ZapsatOdkazNaBiblickeMisto(StrukturaBible sb, XmlWriter xw) {
	 //TODO Dodat ještě @reason?
	 xw.WriteStartElement("supplied");
	 xw.WriteStartElement("ref");
	 xw.WriteAttributeString("type", "bible");
	 xw.WriteAttributeString("n", sb.IdentifikatorXml());
	 xw.WriteString(sb.OznaceniBiblickehoMista());
	 xw.WriteEndElement(); //ref
	 xw.WriteEndElement(); //supplied

	}

	private static void NactiAtributy(XmlReader xr, out string sType, out string sSubtype, out string sN) {
	 sType = xr.GetAttribute("type");
	 sSubtype = xr.GetAttribute("subtype");
	 sN = xr.GetAttribute("n");
	}

	#endregion


	#endregion

	public static Pravidla NactiTranskripcniPravidla(string strTranskripcniSoubor) {
	 Pravidla prvs = new Pravidla();
	 if (!String.IsNullOrEmpty(strTranskripcniSoubor))
		prvs = Spravce.Deserializuj(typeof(Pravidla), strTranskripcniSoubor) as Pravidla;
	 return prvs;
	}

	public static string NahradSpecialniZnaky(string sText, Pravidla prvs) {
	 foreach (Pravidlo pr in prvs) {
		if (sText.Contains(pr.Transliterace))
		 sText = sText.Replace(pr.Transliterace, pr.Transkripce);
	 }
	 return sText;
	}
	public static void UpravitTextTypograficky(string strVstup, string strVystup) {
		Console.WriteLine("{0} => {1}, Upravit text typograficky", strVstup, strVystup);
	 using (XmlReader xr = XmlReader.Create(strVstup)) {
		using (XmlWriter xw = XmlWriter.Create(strVystup)) {
		 while (xr.Read()) {
			if (xr.NodeType != XmlNodeType.Text)
			 Pomucky.Xml.Transformace.SerializeNode(xr, xw);
			else {
			 string strText = xr.Value;
			 xw.WriteString(Retezce.NahraditPevneMezeryUJendopismennychZnaku(strText));
			}
		 }
		}
	 }
	}
 }
}
