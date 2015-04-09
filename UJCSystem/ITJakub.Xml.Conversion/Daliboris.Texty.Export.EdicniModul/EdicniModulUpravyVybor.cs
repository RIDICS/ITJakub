using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Daliboris.Texty.Evidence.Rozhrani;
using System.Xml.Xsl;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export {
	internal class EdicniModulUpravyVybor : Upravy {


		public EdicniModulUpravyVybor() {
		}

		public EdicniModulUpravyVybor(IExportNastaveni emnNastaveni)
		  : base(emnNastaveni) {
		}

		public override void Uprav(IPrepis prepis) {



			string strDocasnaSlozka = Nastaveni.DocasnaSlozka ?? Path.GetTempPath();

			Dictionary<string, XslCompiledTransform> gdxc = NactiTransformacniKroky();
			foreach (IPrepis prp in Nastaveni.Prepisy) {

				string sCesta = Path.Combine(Nastaveni.VstupniSlozka, prp.Soubor.Nazev);
				sCesta = sCesta.Replace(".docx", ".xml");
				FileInfo fi = new FileInfo(sCesta);
				if (!fi.Exists) {
					throw new FileNotFoundException("Zadaný soubor '" + sCesta + "' neexistuje.");
				}

				List<string> glsVystupy = new List<string>(Nastaveni.TransformacniKroky.Count);
				string strNazev = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

				int iKrok = 0;
				string strVstup = sCesta;
				foreach (TransformacniKrok krok in Nastaveni.TransformacniKroky) {
					iKrok++;

					string strVystup = Path.Combine(strDocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iKrok));
					glsVystupy.Add(strVystup);

					if (krok.Parametry != null) {
						//foreach (KeyValuePair<string, string> kvp in krok.Parametry)
						//{
						//  switch (kvp.Key)
						//  {
						//    case "soubor":

						//  }
						//}
					}
					gdxc[krok.Sablona].Transform(strVstup, strVystup);
					strVstup = strVystup;

				}
			}
		}

		/// <summary>
		/// Načte jednotlivé transformační kroky (šablony XSLT)
		/// </summary>
		/// <returns>Vrací slovník se šablonami, které jsou přístupné prostřednictvím textového identifikátoru</returns>
		private Dictionary<string, XslCompiledTransform> NactiTransformacniKroky() {
			Dictionary<string, XslCompiledTransform> gdcx =
				new Dictionary<string, XslCompiledTransform>(Nastaveni.TransformacniKroky.Count);

			foreach (TransformacniKrok krok in Nastaveni.TransformacniKroky) {
				XsltSettings xsltSettings = new XsltSettings(true, false);
				XslCompiledTransform xct = new XslCompiledTransform();
				xct.Load(krok.Soubor, xsltSettings, null);
				gdcx.Add(krok.Sablona, xct);
			}
			return gdcx;
		}


		/// <summary>
		/// Upraví jeden přepis, transformuje ho do podoby pro ediční modul
		/// </summary>
		/// <param name="prp">Přepis (informace o něm)</param>
		/// <param name="emnNastaveni">Nastavení exportu pro ediční modul</param>
		/// <param name="glsTransformacniSablony">Seznam transformačních šablon XSLT, které se při úpravách použijí</param>
		public static void Uprav(IPrepis prp, IExportNastaveni emnNastaveni, List<string> glsTransformacniSablony)
		{

			//
			string sCesta = Path.Combine(emnNastaveni.VstupniSlozka, prp.Soubor.Nazev);
			sCesta = sCesta.Replace(".docx", ".xml").Replace(".doc", ".xml");
			FileInfo fi = new FileInfo(sCesta);
			if (!fi.Exists) {
				throw new FileNotFoundException("Zadaný soubor '" + sCesta + "' neexistuje.");
			}

			if (emnNastaveni.DocasnaSlozka == null)
				emnNastaveni.DocasnaSlozka = Path.GetTempPath();

			//Zpracovat hlavičku a front
			string strNazev = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

			Console.WriteLine("Zpracovávám soubor '{0}'", strNazev);


			string sHlavicka = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Vytvorit_hlavicku_TEI.xsl");
			//string sHlavicka = Path.Combine(emnNastaveni.SlozkaXslt, "EB_Vytvorit_hlavicku_TEI.xsl");

			string sHlavickaVystup = Path.Combine(emnNastaveni.DocasnaSlozka, strNazev + "_hlavicka.xml");

			string sFront = Path.Combine(emnNastaveni.SlozkaXslt, "Vytvorit_front_TEI.xsl");
			string sFrontVystup = Path.Combine(emnNastaveni.DocasnaSlozka, strNazev + "_front.xml");

			List<string> glsVystupy = new List<string>(glsTransformacniSablony.Count + 2);

			XslCompiledTransform xctHlavick = new XslCompiledTransform();
			xctHlavick.Load(sHlavicka);
			XsltArgumentList xal = new XsltArgumentList();
			xal.AddParam("soubor", "", prp.NazevSouboru);
			XmlWriter xw = XmlWriter.Create(sHlavickaVystup);
			xctHlavick.Transform(emnNastaveni.SouborMetadat, xal, xw);
			xw.Close();

			glsVystupy.Add(sHlavickaVystup);

			XslCompiledTransform xctFront = new XslCompiledTransform();
			xctFront.Load(sFront);
			xw = XmlWriter.Create(sFrontVystup);
			xctFront.Transform(emnNastaveni.SouborMetadat, xal, xw);
			xw.Close();
			glsVystupy.Add(sFrontVystup);

			string strVstup = sCesta;
			string strVystup = null;
			for (int i = 0; i < glsTransformacniSablony.Count; i++) {
				
				strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, i));
				glsVystupy.Add(strVystup);
			 
				XslCompiledTransform xslt = new XslCompiledTransform();
				Console.WriteLine("{0} = {1}", String.Format("{0}_{1:00}.xml", strNazev, i), glsTransformacniSablony[i]);

				xslt.Load(glsTransformacniSablony[i]);
				xslt.Transform(strVstup, strVystup);
				strVstup = strVystup;
			}


		 //EBV
		 /*
			if (prp.LiterarniZanr == "biblický text") {
				strVstup = ZpracovatBiblickyText(emnNastaveni, strNazev, glsTransformacniSablony, glsVystupy, strVystup);
			}
			else
			{
				strVstup = ZpracovatOdkazyNaBiblickaMista(emnNastaveni, strNazev, glsTransformacniSablony, glsVystupy, strVystup);
			}
		 */


			//string sSlouceni = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Spojit_s_hlavickou_a_front.xsl");//EBV

			string sSlouceni = Path.Combine(emnNastaveni.SlozkaXslt, "EB_Vybor_Spojit_s_hlavickou_a_front.xsl");//EBV
			//string sSlouceni = Path.Combine(emnNastaveni.SlozkaXslt, "EB_Spojit_s_hlavickou_a_front.xsl");
			
		 strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, glsVystupy.Count));
			
			XsltSettings xsltSettings = new XsltSettings(true, false);
			XslCompiledTransform xctSlouceni = new XslCompiledTransform();

			xctSlouceni.Load(sSlouceni, xsltSettings, null);
			xal = new XsltArgumentList();
			xal.AddParam("hlavicka", "", sHlavickaVystup);
			xal.AddParam("zacatek", "", sFrontVystup);
			xw = XmlWriter.Create(strVystup);
			xctSlouceni.Transform(strVstup, xal, xw);
			xw.Close();
			strVstup = strVystup;
			glsVystupy.Add(strVystup);

			/*
			strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, glsVystupy.Count));
			PresunoutMezeryVneTagu(strVstup, strVystup);
			glsVystupy.Add(strVystup);
			strVstup = strVystup;
		*/
			string sEdicniKomentar = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Presunout_edicni_komentar.xsl");
			strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, glsVystupy.Count));

			XmlUrlResolver xrl = new XmlUrlResolver();
			XslCompiledTransform xctEdicniKomentare = new XslCompiledTransform();
			xctEdicniKomentare.Load(sEdicniKomentar, xsltSettings, xrl);
			xw = XmlWriter.Create(strVystup);
			xctEdicniKomentare.Transform(strVstup, xal, xw);
			xw.Close();
			glsVystupy.Add(strVystup);

			strVstup = strVystup;
			strVystup = Path.Combine(emnNastaveni.VystupniSlozka, strNazev + ".xml");
			PresunoutMezeryVneTagu(strVstup, strVystup);


			//File.Copy(strVystup, Path.Combine(emnNastaveni.VystupniSlozka, strNazev + ".xml"),true);

		}

		/// <summary>
		/// Zpracuje odkazy na biblická místa v nebiblických textech; označí je jako text přidaný editorem
		/// </summary>
		/// <param name="emnNastaveni">Nastavení exportu pro ediční modul</param>
		/// <param name="strNazev">Název pramene (bez přípon dokumentu)</param>
		/// <param name="glsTransformacniSablony">Seznam transformačních šalbon (používá se pro identifikaci pořadí generovaného souboru)</param>
		/// <param name="glsVystupy"></param>
		/// <param name="strVystup"></param>
		/// <returns></returns>
		internal static string ZpracovatOdkazyNaBiblickaMista(IExportNastaveni emnNastaveni, string strNazev, List<string> glsTransformacniSablony, List<string> glsVystupy, string strVystup)
		{
			string strVstup;
			int i = glsTransformacniSablony.Count;
			strVystup = OdkazyNaBiblickyText(strVystup, emnNastaveni, strNazev, i);
			glsVystupy.Add(strVystup);
			strVstup = strVystup;
			return strVstup;
		}


		internal static string ZpracovatBiblickyText(IExportNastaveni emnNastaveni, string strNazev, List<string> glsTransformacniSablony, List<string> glsVystupy, string strVstup)
		{
			string strVystup;

			int i = 0;
		 if(glsTransformacniSablony != null)
			i = glsTransformacniSablony.Count;

			strVstup = RozdelitBibliNaKnihyAKapitoly(strVstup, emnNastaveni, strNazev, i);
			glsVystupy.Add(strVstup);
			strVystup = strVstup;

			strVstup = RozdelitBibliNaVerse(strVystup, emnNastaveni, strNazev, ++i);
			glsVystupy.Add(strVstup);
			strVystup = strVstup;

			return strVystup;
		}

		private static string RozdelitBibliNaVerse(string strVstup, IExportNastaveni emnNastaveni, string strNazev, int iPoradi) {
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

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
						else if (xr.NodeType == XmlNodeType.EndElement)
						{
							if (xr.Name == "p" && sbBible.Kapitola != null)
							{
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

			return strVystup;
		}

		/// <summary>
		/// Vytvoří biblických míst odkazy na biblická místa.
		/// </summary>
		/// <param name="strVstup">Název výstupního souboru</param>
		/// <param name="emnNastaveni">Nastavení pro ediční modul</param>
		/// <param name="strNazev">Název pramene (bez přípon dokumentu)</param>
		/// <param name="i">Pořadí dokumentu ve zpracování</param>
		/// <returns>Vrací název vytvořeného souboru</returns>
		private static string OdkazyNaBiblickyText(string strVstup, IExportNastaveni emnNastaveni, string strNazev, int iPoradi)
		{
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

			using (XmlReader xr = XmlReader.Create(strVstup))
			{
				using (XmlWriter xw = XmlWriter.Create(strVystup))
				{
					while (xr.Read())
					{
						if (xr.NodeType == XmlNodeType.Element)
						{
							string sNazev = xr.Name;
							if (sNazev == "anchor")
							{
								string sType;
								string sSubtype;
								string sN;
								NactiAtributy(xr, out sType, out sSubtype, out sN);
								if (sType == "bible" && sSubtype == "book")
								{
									StrukturaBible sb = new StrukturaBible();
									sb.Kniha = sN;

									xr.Read();
									NactiAtributy(xr, out sType, out sSubtype, out sN);
									if (sType == "bible" && sSubtype == "chapter")
										sb.Kapitola = sN;
									else
									{
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

		private static void ZapsatOdkazNaBiblickeMisto(StrukturaBible sb, XmlWriter xw)
		{
			//TODO Dodat ještě @reason?
			xw.WriteStartElement("supplied");
			xw.WriteStartElement("ref");
			xw.WriteAttributeString("type", "bible");
			xw.WriteAttributeString("n", sb.IdentifikatorXml());
			xw.WriteString(sb.OznaceniBiblickehoMista());
			xw.WriteEndElement(); //ref
			xw.WriteEndElement(); //supplied

		}

		private static void NactiAtributy(XmlReader xr, out string sType, out string sSubtype, out string sN)
		{
			sType = xr.GetAttribute("type");
			sSubtype = xr.GetAttribute("subtype");
			sN = xr.GetAttribute("n");
		}

		private static string RozdelitBibliNaKnihyAKapitoly(string strVstup, IExportNastaveni emnNastaveni,
																												string strNazev, int iPoradi) {
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iPoradi));

			using (XmlReader xr = XmlReader.Create(strVstup)) {
				using (XmlWriter xw = XmlWriter.Create(strVystup)) {

					StrukturaBible sbBible = new StrukturaBible();

					string sKonecKapitoly = null;
					while (xr.Read()) {
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
								case "div":
									XmlDocument xdc = Pomucky.Xml.Objekty.ReadNodeAsXmlDocument(xr);

									XmlNodeList xnlp = xdc.SelectNodes("//p");
									Dictionary<XmlNode, KrajniStrukturyBible> dcOdstavecMisto = new Dictionary<XmlNode, KrajniStrukturyBible>();
									if (xnlp != null) { 
										if (xnlp.Count > 1)
										{

											
											List<XmlNodeList> xnlpb = new List<XmlNodeList>();
											List<XmlNodeList> xnlpc = new List<XmlNodeList>();
											List<XmlNodeList> xnlpv = new	List<XmlNodeList>();

											foreach (XmlNode node in xnlp)
											{
												XmlNodeList nlpb = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'book']");
												xnlpb.Add(nlpb);
												XmlNodeList nlpc = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'chapter']");
												xnlpc.Add(nlpc);
												XmlNodeList nlpv = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'verse']");
												xnlpv.Add(nlpv);

												StrukturaBible sb1 = new StrukturaBible();
												if (nlpb != null) sb1.Kniha = StrukturaBible.ZiskejUdajZAtributu(nlpb[0].Attributes["xml:id"].Value);
												if (nlpc != null) sb1.Kapitola = StrukturaBible.ZiskejUdajZAtributu(nlpc[0].Attributes["xml:id"].Value);
												if (nlpv != null) sb1.Vers = StrukturaBible.ZiskejUdajZAtributu(nlpv[0].Attributes["xml:id"].Value);

												StrukturaBible sbp = new StrukturaBible();
												if (nlpb != null)
													sbp.Kniha = StrukturaBible.ZiskejUdajZAtributu(nlpb[nlpb.Count - 1].Attributes["xml:id"].Value);
												if (nlpc != null)
													sbp.Kapitola = StrukturaBible.ZiskejUdajZAtributu(nlpc[nlpc.Count - 1].Attributes["xml:id"].Value);
												if (nlpv != null)
													sbp.Vers = StrukturaBible.ZiskejUdajZAtributu(nlpv[nlpc.Count - 1].Attributes["xml:id"].Value);
												KrajniStrukturyBible ksb = new KrajniStrukturyBible(sb1, sbp);

												dcOdstavecMisto.Add(node, ksb);
											}

											bool bCoOdstavecToKapitola = false;
											foreach (KeyValuePair<XmlNode, KrajniStrukturyBible> kvp in dcOdstavecMisto)
											{
												if (kvp.Value.Zacatek.Kapitola == kvp.Value.Konec.Kapitola) {
													bCoOdstavecToKapitola = true;
												}
												else
												{
													bCoOdstavecToKapitola = false;
													break;
												}
											}

											if (bCoOdstavecToKapitola)
											{
												foreach (KeyValuePair<XmlNode, KrajniStrukturyBible> kvp in dcOdstavecMisto)
												{
													xw.WriteStartElement("div");
													xw.WriteAttributeString("type", "bible");
													xw.WriteAttributeString("subtype", "chapter");
													xw.WriteAttributeString("n", kvp.Value.Zacatek.Kapitola.ToString());
													xw.WriteAttributeString("xml", "id", "http://www.w3.org/XML/1998/namespace", 
														new StrukturaBible(kvp.Value.Zacatek.Kniha, kvp.Value.Zacatek.Kapitola).IdentifikatorXml(VypisStruktury.Kapitola)
														);
													
													kvp.Key.WriteTo(xw);
													xw.WriteEndElement(); //div
												}
												break;
											}

										}
									}

									XmlNodeList xnlb = xdc.SelectNodes("//anchor[@type='bible' and @subtype = 'book']");
									XmlNodeList xnlc = xdc.SelectNodes("//anchor[@type='bible' and @subtype = 'chapter']");
									XmlNodeList xnlv = xdc.SelectNodes("//anchor[@type='bible' and @subtype = 'verse']");

									if (xnlb != null && xnlb.Count > 0) {
										string sKniha = null;
										XmlNode xnb = xnlb[0];
										if (xnb.Attributes != null) sKniha = xnb.Attributes.GetNamedItem("xml:id").Value;
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
											
											/*
											XmlAttribute xa = xdc.CreateAttribute("type");
											xa.Value = "book";
											XmlAttribute xan = xdc.CreateAttribute("n");
											xan.Value = sKniha;
											XmlAttribute xaid = xdc.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
											xaid.Value = StrukturaBible.IdentifikatorXml(sKniha);

											
											xdc.DocumentElement.Attributes.Append(xa);
											xdc.DocumentElement.Attributes.Append(xan);
											xdc.DocumentElement.Attributes.Append(xaid);
											*/

										}
										foreach (XmlNode node in xnlb) {
											if (node.ParentNode != null) node.ParentNode.RemoveChild(node);
										}

									}


									if (xnlv != null)
										if (xnlc != null && xnlv.Count > 0) {
											string sKonec = null;
											XmlNode xnck = xnlc[xnlc.Count - 1];

											if (xnck.Attributes != null) sKonec = xnck.Attributes.GetNamedItem("xml:id").Value;
											sKonec = StrukturaBible.ZiskejUdajZAtributu(sKonec);
											XmlNode xncz = xnlc[0];
											if (xncz.Attributes != null)
											{
												string sZacatekKapitoly = xncz.Attributes.GetNamedItem("xml:id").Value;
												sZacatekKapitoly = StrukturaBible.ZiskejUdajZAtributu(sZacatekKapitoly);
												sbBible.Kapitola = sZacatekKapitoly;
												if (sZacatekKapitoly == sKonec) {
													if (sKonec != sKonecKapitoly) {
														/*
												XmlAttribute xat = xdc.CreateAttribute("type");
												xat.Value = "chapter";
												xdc.DocumentElement.Attributes.Append(xat);
												*/

														XmlAttribute xat = xdc.CreateAttribute("type");
														xat.Value = "bible";
														XmlNode xde = xdc.DocumentElement;
														if (xde != null)
														{
															if (xde.Attributes != null) xde.Attributes.Append(xat);

															xat = xdc.CreateAttribute("subtype");
															xat.Value = "chapter";
															if (xde.Attributes != null) xde.Attributes.Append(xat);

															XmlAttribute xan = xdc.CreateAttribute("n");
															xan.Value = sZacatekKapitoly;
															if (xde.Attributes != null) xde.Attributes.Append(xan);

															XmlAttribute xaid = xdc.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
															xaid.Value = sbBible.IdentifikatorXml(VypisStruktury.Kapitola);;
															if (xde.Attributes != null) xde.Attributes.Append(xaid);
														}
													}
													foreach (XmlNode node in xnlc) {
														if (node.ParentNode != null) node.ParentNode.RemoveChild(node);
													}

												}
												else {
													XmlElement xe = VytvoritAnchorChapterStart(xdc, sbBible, sZacatekKapitoly);
													if (xdc.DocumentElement != null) xdc.DocumentElement.InsertBefore(xe, xdc.DocumentElement.FirstChild);

													bool bBylKonec = false;
													foreach (XmlNode node in xnlc)
													{
														if (node.Attributes != null)
														{
															string sk = StrukturaBible.ZiskejUdajZAtributu(node.Attributes.GetNamedItem("xml:id").Value);
															if (!bBylKonec && sk == sKonec) {

																bBylKonec = true;
																XmlElement xche = VytvoritAnchorChapterEnd(xdc, sbBible, sZacatekKapitoly);
																XmlElement xchs = VytvoritAnchorChapterStart(xdc, sbBible, sKonec);
																if (node.ParentNode != null)
																{
																	node.ParentNode.InsertBefore(xche, node);
																	node.ParentNode.InsertBefore(xchs, node);
																}
															}
														}
														if (node.ParentNode != null) node.ParentNode.RemoveChild(node);
													}
												}
											}
											sKonecKapitoly = sKonec;
										}

									xdc.WriteTo(xw);
									/*
									if (xnlc != null && xnlv.Count > 0) {
										xw.WriteEndElement(); //<div type='chapter'>
									}
									*/
									break;
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

			return strVystup;
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

		private static void PresunoutMezeryVneTagu(string strVstupniSoubor, string strVystupniSoubor) {
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.ProhibitDtd = false;
			List<string> l = new List<string>();
			l.Add("supplied");
			l.Add("add");
			l.Add("foreign");
			l.Add("hi");
			l.Add("rdg");

			using (XmlReader xr = XmlReader.Create(strVstupniSoubor, xrs)) {
				using (XmlWriter xw = XmlWriter.Create(strVystupniSoubor)) {
					Pomucky.Xml.Transformace.PresunoutMezeryVneTagu(xr, xw, l, null);
				}

			}
		}


		public override string UpravZaver(IPrepis prepis, List<ITransformacniKrok> kroky)
		{
		  throw new NotImplementedException();
		}

		/// <summary>
		/// Zkombinuje jednotlivé části textu.
		/// </summary>
		/// <param name="strHlavicka">Soubor s textem hlavičky (&lt;teiHeader&gt;)</param><param name="strUvod">Soubor s textem úvodu (&lt;front&gt;)</param><param name="strTelo">Soubor s hlavním textem (&lt;body&gt;)</param><param name="strZaver">Soubor s textem na závěr (&lt;back&gt;)</param><param name="kroky">Trnasformační kroky, které se mají na slučované části aplikovat.</param>
		/// <returns>
		/// &gt;Pokud vrátí null, znamená to, že zkombinovaný soubor nebyl vytvořen.
		/// </returns>
		public override string ZkombinujCastiTextu(IPrepis prepis, string strHlavicka, string strUvod, string strTelo, string strZaver, IEnumerable<ITransformacniKrok> kroky)
		{
			throw new NotImplementedException();
		}

		public override string UpravPoSpojeni(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Přiřadí identifikátory důležitým prvkům
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
		/// <returns>Vrací celou cestu k přetransformovanému souboru.</returns>
		public override string PriradId(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			throw new NotImplementedException();
		}

		public override string ProvedUpravy(IPrepis prepis, string strVstup, IEnumerable<UpravaSouboruXml> upravy)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Vrací název výstupu pro zadaný přepis.
		/// </summary>
		/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
		/// <returns>Název výstupu pro zadaný přepis</returns>
		/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
		public override string DejNazevVystupu(IPrepis prepis)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Vrací název předchozího výstupu pro zadaný přepis.
		/// </summary>
		/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
		/// <returns>Název předchozího výstupu pro zadaný přepis</returns>
		/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
		public override string DejNazevPredchozihoVystupu(IPrepis prepis)
		{
			throw new NotImplementedException();
		}

		public override int PocetKroku
		{
			get { throw new NotImplementedException(); }
		}

		public override void SmazDocasneSoubory()
		{
			throw new NotImplementedException();
		}

		public override void NastavVychoziHodnoty()
		{
			throw new NotImplementedException();
		}

		public override string DocasnaSlozka
		{
			get { throw new NotImplementedException(); }
		}

		public override string UpravTelo(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
		  throw new NotImplementedException();
		}

		public override string UpravUvod(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
		  throw new NotImplementedException();
		}

		public override string UpravHlavicku(IPrepis prepis, List<ITransformacniKrok> kroky)
		{
		  throw new NotImplementedException();
		}
	}
}
