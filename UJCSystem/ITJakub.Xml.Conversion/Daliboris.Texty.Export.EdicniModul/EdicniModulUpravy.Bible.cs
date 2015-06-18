using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export
{
	partial class EdicniModulUpravy
	{
		private const string ZnakyInterpunkce = ".,;:?!…“";
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
			if (glsTransformacniSablony != null)
				i = glsTransformacniSablony.Count;

			strVstup = RozdelitBibliNaKnihyAKapitoly(strVstup, emnNastaveni, strNazev, i);
			glsVystupy.Add(strVstup);
			strVystup = strVstup;

			strVstup = RozdelitBibliNaVerse(strVystup, emnNastaveni, strNazev, ++i);
			glsVystupy.Add(strVstup);
			strVystup = strVstup;

			return strVystup;
		}

		private static string RozdelitBibliNaVerse(string strVstup, IExportNastaveni emnNastaveni, string strNazev, int iPoradi)
		{
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, iPoradi));

			using (XmlReader xr = XmlReader.Create(strVstup))
			{
				using (XmlWriter xw = XmlWriter.Create(strVystup))
				{
					StrukturaBible sbBible = new StrukturaBible();
					bool blnPrvniVers = true;
					while (xr.Read())
					{
						if (xr.NodeType == XmlNodeType.Element)
						{
							string sNazev = xr.Name;
							string sTyp;
							string sPodtyp;
							string sN;

							switch (sNazev)
							{
								case "div":
									sTyp = xr.GetAttribute("type");
									sPodtyp = xr.GetAttribute("subtype");
									sN = xr.GetAttribute("n");

									if (sTyp == "bible" && sPodtyp == "book")
										sbBible.Kniha = sN;
									if (sTyp == "bible" && sPodtyp == "chapter")
									{
										if (sbBible.Kapitola != sN)
										{
											blnPrvniVers = true;
											sbBible.Kapitola = sN;
										}

									}
									goto default;
								case "anchor":
									sTyp = xr.GetAttribute("type");
									sPodtyp = xr.GetAttribute("subtype");
									sN = xr.GetAttribute("n");

									if (sTyp == "delimiter")
									{
										if (sPodtyp == "chapterStart")
										{
											sbBible.Kapitola = sN;
											blnPrvniVers = true;
										}
										else if (sPodtyp == "chapterEnd")
										{
											xw.WriteEndElement(); //předchozí <seg>
										}
										goto default;
									}
									if (sTyp == "bible")
									{
										if (sPodtyp == "chapter")
										{
											sbBible.Kapitola = sN;
											goto default;
										}
										if (sPodtyp == "verse")
										{
											if (!blnPrvniVers)
											{
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
									else
									{
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
						else
						{
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
		/// <param name="iPoradi">Pořadí dokumentu ve zpracování</param>
		/// <returns>Vrací název vytvořeného souboru</returns>
		private static string OdkazyNaBiblickyText(string strVstup, IExportNastaveni emnNastaveni, string strNazev, int iPoradi)
		{
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, iPoradi));

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
									else
									{
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
																												string strNazev, int iPoradi)
		{
			string strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, iPoradi));

			using (XmlReader xr = XmlReader.Create(strVstup))
			{
				using (XmlWriter xw = XmlWriter.Create(strVystup))
				{

					StrukturaBible sbBible = new StrukturaBible();

					string sKonecKapitoly = null;
					while (xr.Read())
					{
						if (xr.NodeType == XmlNodeType.Element)
						{
							string sNazev = xr.Name;
							switch (sNazev)
							{
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
									if (xnlp != null)
									{
										if (xnlp.Count > 1)
										{


											List<XmlNodeList> xnlpb = new List<XmlNodeList>();
											List<XmlNodeList> xnlpc = new List<XmlNodeList>();
											List<XmlNodeList> xnlpv = new List<XmlNodeList>();

											foreach (XmlNode node in xnlp)
											{
												XmlNodeList nlpb = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'book']");
												xnlpb.Add(nlpb);
												XmlNodeList nlpc = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'chapter']");
												xnlpc.Add(nlpc);
												XmlNodeList nlpv = node.SelectNodes(".//anchor[@type='bible' and @subtype = 'verse']");
												xnlpv.Add(nlpv);

												StrukturaBible sb1 = new StrukturaBible();
												if (nlpb != null)
													sb1.Kniha = StrukturaBible.ZiskejUdajZAtributu(nlpb[0].Attributes["xml:id"].Value);
												if (nlpc != null)
													sb1.Kapitola = StrukturaBible.ZiskejUdajZAtributu(nlpc[0].Attributes["xml:id"].Value);
												if (nlpv != null)
													sb1.Vers = StrukturaBible.ZiskejUdajZAtributu(nlpv[0].Attributes["xml:id"].Value);

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
												if (kvp.Value.Zacatek.Kapitola == kvp.Value.Konec.Kapitola)
												{
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
													xw.WriteAttributeString("n", kvp.Value.Zacatek.Kapitola);
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

									if (xnlb != null && xnlb.Count > 0)
									{
										string sKniha = null;
										XmlNode xnb = xnlb[0];
										if (xnb.Attributes != null)
											sKniha = xnb.Attributes.GetNamedItem("xml:id").Value;
										sKniha = StrukturaBible.ZiskejUdajZAtributu(sKniha);
										if (sbBible.Kniha != sKniha)
										{

											if (sbBible.Kniha != null)
											{
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
										foreach (XmlNode node in xnlb)
										{
											if (node.ParentNode != null)
												node.ParentNode.RemoveChild(node);
										}

									}


									if (xnlv != null)
										if (xnlc != null && xnlv.Count > 0)
										{
											string sKonec = null;
											XmlNode xnck = xnlc[xnlc.Count - 1];

											if (xnck.Attributes != null)
												sKonec = xnck.Attributes.GetNamedItem("xml:id").Value;
											sKonec = StrukturaBible.ZiskejUdajZAtributu(sKonec);
											XmlNode xncz = xnlc[0];
											if (xncz.Attributes != null)
											{
												string sZacatekKapitoly = xncz.Attributes.GetNamedItem("xml:id").Value;
												sZacatekKapitoly = StrukturaBible.ZiskejUdajZAtributu(sZacatekKapitoly);
												sbBible.Kapitola = sZacatekKapitoly;
												if (sZacatekKapitoly == sKonec)
												{
													if (sKonec != sKonecKapitoly)
													{
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
															if (xde.Attributes != null)
																xde.Attributes.Append(xat);

															xat = xdc.CreateAttribute("subtype");
															xat.Value = "chapter";
															if (xde.Attributes != null)
																xde.Attributes.Append(xat);

															XmlAttribute xan = xdc.CreateAttribute("n");
															xan.Value = sZacatekKapitoly;
															if (xde.Attributes != null)
																xde.Attributes.Append(xan);

															XmlAttribute xaid = xdc.CreateAttribute("xml", "id", "http://www.w3.org/XML/1998/namespace");
															xaid.Value = sbBible.IdentifikatorXml(VypisStruktury.Kapitola);
															if (xde.Attributes != null)
																xde.Attributes.Append(xaid);
														}
													}
													foreach (XmlNode node in xnlc)
													{
														if (node.ParentNode != null)
															node.ParentNode.RemoveChild(node);
													}

												}
												else
												{
													XmlElement xe = VytvoritAnchorChapterStart(xdc, sbBible, sZacatekKapitoly);
													if (xdc.DocumentElement != null)
														xdc.DocumentElement.InsertBefore(xe, xdc.DocumentElement.FirstChild);

													bool bBylKonec = false;
													foreach (XmlNode node in xnlc)
													{
														if (node.Attributes != null)
														{
															string sk = StrukturaBible.ZiskejUdajZAtributu(node.Attributes.GetNamedItem("xml:id").Value);
															if (!bBylKonec && sk == sKonec)
															{

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
														if (node.ParentNode != null)
															node.ParentNode.RemoveChild(node);
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
						else if (xr.NodeType == XmlNodeType.EndElement)
						{
							Pomucky.Xml.Transformace.SerializeNode(xr, xw);
						}
						else
						{
							Pomucky.Xml.Transformace.SerializeNode(xr, xw);
						}
					}
				}
			}

			return strVystup;
		}

		private static XmlElement VytvoritAnchorChapterStart(XmlDocument xdc, StrukturaBible sbBible, string sCisloKapitoly)
		{
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

		private static XmlElement VytvoritAnchorChapterEnd(XmlDocument xdc, StrukturaBible sbBible, string sCisloKapitoly)
		{
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


	}
}
