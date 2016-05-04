using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Daliboris.Pomucky.Xml;
using DPXT = Daliboris.Pomucky.Xml.Transformace;
//using Daliboris.Texty.Export.Rozhrani;
//using System.Web;
//using Daliboris.Pomucky.Xml.XsltTransformation;
//using System.Collections.Specialized;
using Ujc.Ovj.Xml.Info;

namespace Daliboris.Slovniky
{
	public abstract class Slovnik : IUpravy
	{

		private string mstrChyby;
		private string mstrVstupniSoubor;
		private string mstrVystupniSoubor;

		private static char[] mchCarkaTeckaMezeraZavorky = new Char[] { ',', '.', ' ', ')', '(' };
		private static char[] mchCarkaTeckaMezeraStrednik = new Char[] { ',', '.', ' ', ';' };
		private static char[] mchSeparatory = new Char[] { ',', ':', ';', '?', '+', '=', '…', '$', '×', '†', '“', '„', '\t' };
		private static char[] mchIndexyCislic = new Char[] { '\u00B9', '\u00B2', '\u00B3' };
		private static char[] mchKrizekHvezdicka = new Char[] { '*', 'ˣ' };

		public static char[] CarkaTeckaMezeraZavorky { get { return mchCarkaTeckaMezeraZavorky; } }
		public static char[] CarkaTeckaMezeraStrednik { get { return mchCarkaTeckaMezeraStrednik; } }
		public static char[] Separatory { get { return mchSeparatory; } }
		public static char[] IndexyCislic { get { return mchIndexyCislic; } }
		public static char[] KrizekHvezdicka { get { return mchKrizekHvezdicka; } }

		public string DocasnaSlozka { get; set; }

		#region IUpravy Members

		public string VstupniSoubor
		{
			get { return mstrVstupniSoubor; }
			set { mstrVstupniSoubor = value; }
		}

		public string VystupniSoubor
		{
			get { return mstrVystupniSoubor; }
			set { mstrVystupniSoubor = value; }
		}

		public string Chyby
		{
			get { return mstrChyby; }
		}

		#endregion

		#region Konstruktory

		public Slovnik()
		{
		}
		public Slovnik(string strVstupniSoubor)
		{
			mstrVstupniSoubor = strVstupniSoubor;
		}
		public Slovnik(string strVstupniSoubor, string strVystupniSoubor)
		{
			mstrVstupniSoubor = strVstupniSoubor;
			mstrVystupniSoubor = strVystupniSoubor;
		}

		#endregion

		

		public static string HesloBezKrizkuAHvezdicky(string sHeslo)
		{
			if (sHeslo.IndexOfAny(KrizekHvezdicka) <= -1) return sHeslo;
			string sVychozi = sHeslo;
			foreach (char c in KrizekHvezdicka)
			{
				sVychozi = sVychozi.Replace(c.ToString(), "");
			}
			return sVychozi;
		}

		#region Základní funkce pro úpravu exportovaného textu slovníku z Wordu
		
		/// <summary>
		/// Ohraničí heslové stati náležející k jednomu písmenu značkou &lt;div1&gt;.
		/// Původní element <remarks>milestone</remarks> přemění na element <remarks>div1</remarks>.
		/// </summary>
		public virtual void SeskupitHeslaPismene(string inputFile, string outputFile)
		{
			Dictionary<string, string> gdcPismena = Heslar.IDPismenAbecedy();

			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{
					xw.WriteStartDocument(true);
					bool bJinaNezPrvni = false;

					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element)
						{
							switch (r.Name)
							{
								case "milestone":
									if (bJinaNezPrvni)
									{
										//poku nejde o první značku v souboru,
										//uzavře předchozí značku <div1>
										xw.WriteEndElement();
										xw.WriteWhitespace("\r\n");
										xw.Flush();
									}
									bJinaNezPrvni = true;
									xw.WriteStartElement("div1");
									//xw.WriteWhitespace("\r\n");
									//zjišťuje obsah značky, na jehož základě přiřadí písmenu identifikátor;
									string sID = null; // = r.GetAttribute("id"); - atribut už se nepoužívá

									string sText = r.ReadString();
									if (string.IsNullOrEmpty(sText))
										//chyba
										sText = "XX";
									else
										sID = sText.Replace(".", "").Replace(",", "").ToUpper();
									if (gdcPismena.ContainsKey(sID))
										xw.WriteAttributeString("id", gdcPismena[sID]);
									else
										xw.WriteAttributeString("id", "lt" + sText);

									xw.WriteAttributeString("type", "letter");

									xw.WriteAttributeString("text", sText);
									xw.WriteWhitespace("\r\n");
									xw.WriteStartElement("text");
									xw.WriteString(sText);
									//while (r.Name != "text")
									//   r.Read();
									xw.WriteEndElement(); //uzavře element <print>
									break;
								default:
									DPXT.SerializeNode(r, xw);
									break;

							}
						}
						else if (r.NodeType == XmlNodeType.EndElement)
						{
							switch (r.Name)
							{
								case "milestone":
									break;
								default:
									DPXT.SerializeNode(r, xw);
									break;
							}
						}
						else if (r.NodeType == XmlNodeType.XmlDeclaration && r.Name == "xml")
						{
							//nedělat nic, jde o začátek dokumentu; genruje cyhbu: nelze zapsat deklaraci, dokument již byl započat
						}
						else { DPXT.SerializeNode(r, xw); }

					}

				}
			}

		}

		public abstract void UpravitHraniceHesloveStati(string inputFile, string outputFile);


		//jak zajistit, aby dědící třídy tuto metodu přepsaly, i když není abstraktní?
		public abstract void KonsolidovatHeslovouStat(string inputFile, string outputFile);
		#endregion

		/// <summary>
		/// Zpracuje odkazy &lt;xref&gt;, aby byly co nejjednoznačněji určitelné.
		/// <remarks>Pokud odkaz ve značce &lt;xref&gt; neobsahuje jednoznačnou identifikaci odkazu pomocí source a target,
		/// extrahuje informace o heslovém slově (oddělí čísla homonym, morfologii, hvězdičku na začátku ap.)
		/// a vytvoří odkaz na samotné heslové slovo.
		/// Na základě předcházející informace o zdroji se pokus odkázat na heslo v konkrétním zdroji.
		/// </remarks>
		/// </summary>
		public virtual void UpravitOdkazy(string inputFile, string outputFile)
		{
			string strPredchoziZdroj = null;

			Dictionary<string, string> gdcSlovniky = new Dictionary<string, string>(Heslar.AkronymySlovniku);
			foreach (KeyValuePair<string, int> kvp in Heslar.Zdroj)
			{
				if (!gdcSlovniky.ContainsKey(kvp.Key))
					gdcSlovniky.Add(kvp.Key, kvp.Key);
			}
			gdcSlovniky.Add("SSL", "SSL"); //slovník středověké latiny - tyto odkazy by se neměly kontrolovat; nebo jo?


			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{
					xw.WriteStartDocument(true);
					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element)
						{
							switch (r.Name)
							{
								case "entry":
								case "senses":
								case "senseGrp":
								case "note":
									strPredchoziZdroj = null;
									goto default;
								case "refsource":
									string sType = r.GetAttribute("type");
									if (sType == "hidden")
										goto default;
									strPredchoziZdroj = r.GetAttribute("abbr");
									goto default;
								case "xref":
									string sSource = r.GetAttribute("source");
									if (sSource != null)
										goto default;
									string sText = r.ReadString();
									UpravitOdkazoveHeslo(strPredchoziZdroj, xw, sText, gdcSlovniky);
									break;
								default:
									DPXT.SerializeNode(r, xw);
									break;
							}

						}
						else if (r.NodeType == XmlNodeType.Text)
						{
							string sText = r.Value;
							if (strPredchoziZdroj != null && sText.Contains("."))
							{
								strPredchoziZdroj = null;
							}
							DPXT.SerializeNode(r, xw);
						}
						else
						{
							DPXT.SerializeNode(r, xw);
						}

					}
				}

			}
		}

		/// <summary>
		/// Upraví text odkazu tak, aby obsahoval pouze heslové slovo bez dalších údajů (čísla významu ap.). Nektěré součásti uloží jako atributy (prefix, homonymum).
		/// </summary>
		/// <param name="strPredchoziZdroj"></param>
		/// <param name="xw"></param>
		/// <param name="sText"></param>
		/// <param name="gdcSlovniky">Seznam slovníků, které jsou pro zpracování odkazů relevantní</param>
		private static void UpravitOdkazoveHeslo(string strPredchoziZdroj, XmlWriter xw, string sText,
			Dictionary<string, string> gdcSlovniky)
		{

			/*
			//TODO Převést proceduru rozdělení textu na odkaz do knihovny OdkazInfo
			StringBuilder sbHeslo = new StringBuilder(sText);
			string[] asKonce = new string[] { " jen pf.", " adj.", " pf.", " adv.", " ipf.", " f.", " n.", " m.", " pron.", " I", " interj." };
			//TODO Využít rozdělení pomocí regulárních výrazů
			foreach (string item in asKonce) {
				int i = sText.IndexOf(item, StringComparison.CurrentCulture);
				if (i == -1) continue;
				sbHeslo.Remove(i, item.Length);
				break;
			}
			*/
			/*
			int iKonec = sbHeslo.ToString().IndexOfAny(",(1234567890".ToCharArray());
			if (iKonec > -1)
				sbHeslo.Remove(iKonec, sbHeslo.Length - iKonec);
			 */
			/*
			if (iKonec > -1)
				sbHeslo.Append(sText.Substring(0, iKonec));
			else
				sbHeslo.Append(sText);
			*/
			/*
			if(sbHeslo[0] == '*')
				sbHeslo.Remove(0,1);
			*/
			//který z těchto algoritmů bude rychlejší?

			//string sHeslo = new string(' ', sText.Length);
			/*
			for (int i = 0; i < sText.Length; i++) {
				if (sText[i] != '*')
					if (",(¹²³1234567890".IndexOf(sText[i]) == -1)
						sbHeslo.Append(sText[i]);
					else //co ale v případě zvratných podob?; indexy, čísla významu ap. by měla být až po zvratném sě...
						break;
			}
			*/

			OdkazInfo oi = OdkazInfo.ZpracujOdkaz(sText, strPredchoziZdroj);
			xw.WriteStartElement("xref");
			if (strPredchoziZdroj != null && gdcSlovniky.ContainsKey(strPredchoziZdroj))
				xw.WriteAttributeString("source", gdcSlovniky[strPredchoziZdroj]);
			if (oi.Prefix != null)
				xw.WriteAttributeString("prefix", oi.Prefix);
			xw.WriteAttributeString("hw", oi.Heslo);
			if (oi.Homonymum != null)
				xw.WriteAttributeString("hom", oi.Homonymum);
			xw.WriteString(sText);
			xw.WriteEndElement(); //xref
			/*
						xw.WriteStartElement("xref");
						string sHomonymum = null;
						if(strPredchoziZdroj != null) {
							if(gdcSlovniky.ContainsKey(strPredchoziZdroj))
							xw.WriteAttributeString("source", gdcSlovniky[strPredchoziZdroj]);
						}
						foreach (char item in IndexyCislic) {
							if (sbHeslo[sbHeslo.Length - 1] != item) continue;
							sHomonymum = item.ToString();
							sbHeslo.Remove(sbHeslo.Length - 1, 1);
							break;
						}
						xw.WriteAttributeString("hw", sbHeslo.ToString().Trim());
						if(sHomonymum != null)
							xw.WriteAttributeString("hom", sHomonymum);
						xw.WriteString(sText);
						xw.WriteEndElement(); //xref
			 * 
			 */
		}

		public static void VypsatOdkazy(string strVstupniSoubor, string strVystupniSoubor)
		{
			using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(strVystupniSoubor))
				{
					xw.WriteStartElement("xrefs");
					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element && r.Name == "xref")
						{
							xw.WriteNode(r, false);
						}
					}
					xw.WriteEndElement(); //xrefs
				}
			}
		}

		#region Identifikace zkratek

		public virtual void IdentifikovatZkratky(string inputFile, string outputFile, string strAdresarZkratek)
		{
			string strDictionary = null;
			string strPredchoziKapitalky = null;
			string strPredchoziZkratka = null;


			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{
					xw.WriteStartDocument(true);
					Zkratky zkrZkratky = new Zkratky();

					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element)
						{
							switch (r.Name)
							{
								case "dictionary":
									strDictionary = r.GetAttribute("name");
									zkrZkratky = Zkratky.NactiZkratky(Path.Combine(strAdresarZkratek, strDictionary + "_zkratky.txt"));
									//Načíst seznamy zkratek v souboru strDictionary + "_zkrakty.txt"
									//rozparsovat zkratky
									goto default;
								case "entry":
								case "senses":
								case "senseGrp":
									strPredchoziKapitalky = null;
									strPredchoziZkratka = null;
									goto default;
								//////case "resp":
								//////  if (r.IsEmptyElement)
								//////    break;
								//////  string sTg = r.Name;
								//////  string sObsah = r.ReadElementString();
								//////  string sTyp = "resp";
								//////  Zkratka zk = new Zkratka(strDictionary, "resp", null, null, null);
								//////  Atributy xats = new Atributy();
								//////  xw.WriteStartElement("resp");
								//////  if (sObsah.IndexOf(',') > -1) {
								//////    ZapisZkratku(xw, xats, sTyp, zk, sTg);
								//////  }
								//////  else {
								//////    zk.Text = sObsah;
								//////    zk.Rozepsani = "Milostava Vajdlová";
								//////    ZapisZkratku(xw, xats, sTyp, zk, sTg);
								//////  }
								//////  xw.WriteEndElement();
								//////  break;
								case "refsource":
								case "pos":
								case "abbr":
								case "gloss":
								//case "text":
								case "morph":
									//case "location":
									if (r.IsEmptyElement)
										break;
									string sType = r.GetAttribute("type");
									if (sType == "signature" || sType == "hidden")
										goto default;
									string sRend = r.GetAttribute("rend");
									string sTag = r.Name;
									//načte do paměti všechny atributy
									int iPocetAtr = r.AttributeCount;
									AttributeInfos ats = new AttributeInfos();
									if (r.MoveToFirstAttribute())
									{
										ats.Add(new AttributeInfo(r.Name, r.Value));
										while (r.MoveToNextAttribute())
										{
											ats.Add(new AttributeInfo(r.Name, r.Value));
										}
									}
									if (sTag == "pos")
										sType = "pos";
									if (sTag == "refsource")
									{
										sType = "pram";
									}

									//DPXT.SerializeNode(r,xw);
									string strObsah = r.ReadElementString();
									//if (strObsah == "Pass. ")
									// strObsah = "Pass. ";
									bool bKonciMezerou = strObsah.EndsWith(" ");

									Zkratka zkr = null;
									//Dodatek 2008-12-08
									if (strObsah.StartsWith("~") && strPredchoziKapitalky != null)
									{
										//if (sRend == "cap")
										//{

										for (int i = strPredchoziKapitalky.Length - 1; i >= 1; i--)
										{
											if (Char.IsUpper(strPredchoziKapitalky, i))
											{
												string sRekonstrZkratka = strPredchoziKapitalky.Substring(0, i) + strObsah.Substring(1);
												zkr = zkrZkratky.NajdiZkratku(sRekonstrZkratka, sType);
												if (zkr != null)
												{
													ZapisZkratku(xw, ats, sType, zkr, sTag);
													xw.WriteString(strObsah);
													xw.WriteEndElement();
													break;
												}
											}
										}
										if (zkr != null)
											break;
										//}
									}
									//Dodatek 2008-12-08
									else
										//Zkratka zkr = zkrZkratky.NajdiZkratku(strObsah.Trim());
										zkr = zkrZkratky.NajdiZkratku(strObsah.Trim(), sType);
									if (zkr != null)
									{
										ZapisZkratku(xw, ats, sType, zkr, sTag);
										if (sType == "bible" && zkr.Delka < strObsah.Length)
										{
											int iZacatek = zkr.Delka;
											xw.WriteString(strObsah.Substring(0, iZacatek));
											if (strObsah.Substring(iZacatek, 1) == " ")
											{
												iZacatek = iZacatek + 1;
												xw.WriteString(" ");
											}
											xw.WriteEndElement();
											xw.WriteStartElement("location");
											xw.WriteAttributeString("type", "bible");
											xw.WriteString(strObsah.Substring(iZacatek));
										}
										else
										{
											if (sType == "pram" || sType == "pam")
											{
												if (sRend == "cap")
													strPredchoziKapitalky = strObsah.Trim();
												else if (!strObsah.StartsWith("~"))
												{
													//strPredchoziZkratka = strObsah.Trim();
													strPredchoziKapitalky = strObsah.Trim();
												}
											}
											xw.WriteString(strObsah);
										}
										xw.WriteEndElement();
									}
									else
									{
										Zkratky zkrt = zkrZkratky.NajdiZkratky(strObsah.Trim(), sType);
										if (zkrt == null)
											zkrt = zkrZkratky.NajdiZkratky(strObsah.Trim(), null);
										if (zkrt != null)
											sType = null;

										if (zkrt != null)
										{
											//je potřeba jemnější rozdělení zkratek v původním textu
											//ne všechna slova se musí identifikovat jako zkratky, 
											//za poslední zkratkou nemusí být mezera
											//může jít o jinou velikost písmen Sr. × sr.
											string[] astrText = strObsah.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
											for (int i = 0; i < astrText.Length; i++)
											{
												if (zkrt.ObsahujeZkratu(astrText[i]))
												{
													ZapisZkratku(xw, ats, sType, zkrt[astrText[i]], sTag);
													if (!bKonciMezerou && i == astrText.Length - 1)
														xw.WriteString(astrText[i]);
													else
														xw.WriteString(astrText[i] + " ");
													xw.WriteEndElement();
												}
												else
												{
													ZapisZkratku(xw, ats, sType, null, sTag);
													if (!bKonciMezerou && i == astrText.Length - 1)
														xw.WriteString(astrText[i]);
													else
														xw.WriteString(astrText[i] + " ");
													xw.WriteEndElement();
												}
											}
										}
										else
										{
											ZapisZkratku(xw, ats, sType, null, sTag);
											xw.WriteString(strObsah);
											xw.WriteEndElement();
										}
									}
									break;
								default:
									DPXT.SerializeNode(r, xw);
									break;

							}
						}
						else if (r.NodeType == XmlNodeType.EndElement)
						{
							switch (r.Name)
							{
								case "milestone":
									break;
								default:
									DPXT.SerializeNode(r, xw);
									break;
							}
						}
						else { DPXT.SerializeNode(r, xw); }

					}

				}
			}
		}

		private void ZapisZkratku(XmlWriter xw, AttributeInfos ats, string sType, Zkratka zkr, string sTag)
		{
			xw.WriteStartElement(sTag);
			foreach (AttributeInfo at in ats)
			{
				if (at.LocalName == "type" && sType == null)
				{
					if (zkr == null)
						xw.WriteAttributeString(at.LocalName, at.Value);
				}
				else
				{
					if (at.LocalName == "source")
					{
						if (zkr == null)
							xw.WriteAttributeString(at.LocalName, at.Value);
					}
					else
					{
						xw.WriteAttributeString(at.LocalName, at.Value);
					}
				}

			}
			if (zkr != null)
			{
				if (sType == null)
					xw.WriteAttributeString("type", zkr.Typ);
				if (sType != null && ats.Count == 0)
					xw.WriteAttributeString("type", zkr.Typ);
				xw.WriteAttributeString("id", zkr.Id);
				xw.WriteAttributeString("descr", zkr.Rozepsani);
				xw.WriteAttributeString("source", zkr.Zdroj);
				//}
			}
		}

		#endregion

		#region Rozdělení slovníků do souborů
		public virtual void RozdelitEntryDoSouboru(string[] strTagy,
			string strAtributID,
            string inputFile,
			string sVystupniAdresar,
			string sVystupniSoubor,
			String strXPathPopis)
		{

			bool bZpracovatObsah = (sVystupniSoubor != null);
			XmlTextReader reader = null;
			XmlTextWriter xmlw = null;
			mstrChyby = null;
			string strHesla = null;
			string strZpracovavanyElement = null;
			string strAktualniOddilDiv = null;
			string strPredchoziOddilDiv = null;
			int intPocetZpracovanychHesel = 0;
			bool bZapisovat = false;
			if (!sVystupniAdresar.EndsWith("\\"))
				sVystupniAdresar += "\\";
			XmlTextWriter xObsah = null;

			string strAktualniVystupniSoubor = null;
			if (bZpracovatObsah)
			{

				xObsah = new XmlTextWriter(sVystupniAdresar + sVystupniSoubor, System.Text.Encoding.UTF8);
				xObsah.Indentation = 2;
				xObsah.Formatting = Formatting.Indented;

				xObsah.WriteStartDocument();
				xObsah.WriteStartElement("obsah");
				xObsah.WriteAttributeString("soubor", inputFile);
				xObsah.WriteAttributeString("zpracovano", DateTime.Now.ToString());
			}

			//string strPrevID = null;
			//string strPrevText = null;

			string strID = "";
			try
			{
				reader = new XmlTextReader(inputFile);
				//string strHesla = null;

				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						if (reader.Name == "div1")
						{
							intPocetZpracovanychHesel = 0;
							strPredchoziOddilDiv = strAktualniOddilDiv;
							strAktualniOddilDiv = reader.GetAttribute("id");
							////if (strPredchoziOddilDiv == null)
							////    strPredchoziOddilDiv = strAktualniOddilDiv + "\\";
							System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(sVystupniAdresar + strAktualniOddilDiv);
							if (!di.Exists)
								di.Create();
							//if (di.GetFiles().Length > 0)
							//   di.Delete(true);

							////strAktualniOddilDiv = strAktualniOddilDiv + "\\";
						}

						foreach (string tg in strTagy)
						{
							if (tg == reader.Name)
							{
								bZapisovat = true;
								intPocetZpracovanychHesel++;
								////if (intPocetZpracovanychHesel == 2)
								////    strPredchoziOddilDiv = strAktualniOddilDiv;
								strZpracovavanyElement = tg;
								strID = reader.GetAttribute(strAtributID);
								if (strID != null)
								{
									strAktualniVystupniSoubor = sVystupniAdresar + strAktualniOddilDiv + "\\" + strID + ".xml";
									xmlw = new XmlTextWriter(strAktualniVystupniSoubor, System.Text.Encoding.UTF8);
									xmlw.Formatting = Formatting.Indented;
									xmlw.Indentation = 1;
									xmlw.IndentChar = ' ';
									xmlw.WriteStartDocument();
								}
							}
						}
						if (bZapisovat)
						{
							if (reader.Name == strZpracovavanyElement)
							{
								DPXT.SerializeNode(reader, xmlw);
								xmlw.WriteAttributeString("posledniZmena", DateTime.Now.ToString("o"));
							}
							else
							{
								DPXT.SerializeNode(reader, xmlw);
							}
						}
					}
					else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == strZpracovavanyElement)
					{
						if (bZapisovat)
						{
							xmlw.WriteEndDocument();
							xmlw.Close();

							bZapisovat = false;
							/*
							strHesla = ZjistitHeslaVSouboru(strXPathPopis, strAktualniVystupniSoubor, mchSeparatory);
							string strPredchoziVystupniSoubor = null;
							if (bZpracovatObsah) {
								ZapsatInformaceOSouboruDoObsahu(strHesla, strAktualniOddilDiv, xObsah, strID);
							}
							if (strPrevID != null) {
								if (intPocetZpracovanychHesel > 1)
									strPredchoziVystupniSoubor = sVystupniAdresar + strAktualniOddilDiv + "\\" + strPrevID + ".xml";
								else {
									if (strPredchoziOddilDiv != null)
										strPredchoziVystupniSoubor = sVystupniAdresar + strPredchoziOddilDiv + "\\" + strPrevID + ".xml";
								}
								if (strPredchoziVystupniSoubor != null)
									ZapsatInformaceOOkolnichHeslech(strPredchoziVystupniSoubor, "next", strID, strHesla, strAktualniOddilDiv);
								if (strPredchoziOddilDiv == null)
									ZapsatInformaceOOkolnichHeslech(strAktualniVystupniSoubor, "prev", strPrevID, strPrevText, strAktualniOddilDiv);
								else
									ZapsatInformaceOOkolnichHeslech(strAktualniVystupniSoubor, "prev", strPrevID, strPrevText, strPredchoziOddilDiv);

							}
							strPrevID = strID;
							strPrevText = strHesla;
							*/

						}
					}
					else if (reader.NodeType == XmlNodeType.Comment)
					{
						strHesla = reader.Value;
						strHesla = strHesla.Replace("*", " ").Trim();
					}
					else
					{
						if (bZapisovat)
							if (reader.NodeType != XmlNodeType.Comment)
								DPXT.SerializeNode(reader, xmlw);
					}
				}
			}
			catch (Exception ev)
			{
				mstrChyby = ev.ToString();
			}
			finally
			{
				if (reader != null)
					reader.Close();
				reader = null;
				if (xmlw != null)
					xmlw.Close();
				xmlw = null;
				if (bZpracovatObsah)
				{
					if (xObsah != null)
					{
						xObsah.WriteEndElement();
						xObsah.WriteEndDocument();
						xObsah.Close();
					}
					xObsah = null;
				}
			}

			if (mstrChyby != null)
				throw new Exception(mstrChyby);


		}

		private static string ZjistitHeslaVSouboru(String strXPathPopis, string strAktualniVystupniSoubor, char[] mchSeparatory)
		{
			XPathDocument xpD = new XPathDocument(strAktualniVystupniSoubor);
			XPathNavigator xpN = xpD.CreateNavigator();
			XPathNodeIterator xpI = xpN.Select(strXPathPopis);
			string strHeslo = null;

			StringBuilder sb = new StringBuilder();

			while (xpI.MoveNext())
			{
				string sHeslo = xpI.Current.Value.TrimEnd(mchSeparatory);
				sb.Append(sHeslo.Trim().TrimEnd(mchSeparatory) + ", ");
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 2, 2);
			sb.Replace(", ,", ", ");
			xpI = null;
			xpN = null;
			xpD = null;

			strHeslo = sb.ToString();
			strHeslo = strHeslo.Trim().TrimEnd(mchSeparatory).TrimEnd();
			//strHeslo = strHeslo.TrimEnd(mchSeparatory);
			//strHeslo = strHeslo.TrimEnd();
			return strHeslo;
		}

		private void ZapsatInformaceOOkolnichHeslech(string strSoubor, string sSmer, string sIDSouboru,
			string sHesla, string sPismeno)
		{

			XmlDocument xdPrev = new XmlDocument();
			xdPrev.Load(strSoubor);
			XmlNode xnd = xdPrev.SelectSingleNode("processing-instruction('vw')");
			////string sNextLt = null;
			////if (intPocetZpracovanychHesel == 2)
			////    sNextLt = strAktualniOddilDiv.TrimEnd(mchSeparatory);
			////else
			////    sNextLt = strPredchoziOddilDiv.TrimEnd(mchSeparatory);
			string sText = sSmer + "Lt=" + sPismeno + "| " + sSmer + "Id=" + sIDSouboru + "| "
				 + sSmer + "Text=" + sHesla + "| ";
			if (xnd != null)
				xnd.Value = xnd.Value + sSmer + sText;
			else
			{
				xnd = xdPrev.CreateProcessingInstruction("vw", sText);

			}
			xdPrev.AppendChild(xnd);
			xdPrev.Save(strSoubor);

			/*
			xnd = xdPrev.DocumentElement;
			XmlAttribute at = xdPrev.CreateAttribute("nextId");
			at.Value = strID;
			xnd.Attributes.Append(at);
			at = xdPrev.CreateAttribute("nextText");
			at.Value = strHeslo;
			xnd.Attributes.Append(at);
			 */

			/*
			xnd = xdPrev.DocumentElement;
			at = xdPrev.CreateAttribute("prevId");
			at.Value = strPrevID;
			xnd.Attributes.Append(at);
			at = xdPrev.CreateAttribute("prevText");
			at.Value = strPrevText;
			xnd.Attributes.Append(at);
			 */
		}

		private void ZapsatInformaceOSouboruDoObsahu(string strHesla, string strPredchoziOddilDiv, XmlTextWriter xObsah, string strID)
		{
			xObsah.WriteStartElement("soubor");
			xObsah.WriteAttributeString("id", strID);
			xObsah.WriteAttributeString("adresar", strPredchoziOddilDiv);
			xObsah.WriteAttributeString("nazev", strID + ".xml");
			xObsah.WriteAttributeString("posledniZmena", DateTime.Now.ToString());
			xObsah.WriteAttributeString("popis", strHesla.Trim());
			//xObsah.WriteAttributeString("oznaceno", "false");
			string[] aHesla = strHesla.Split(mchSeparatory);
			foreach (string s in aHesla)
			{
				string sText = s.Trim();
				if (s.Length > 0)
				{
					xObsah.WriteStartElement("hw");
					xObsah.WriteString(sText);
					xObsah.WriteEndElement();
				}
			}
			xObsah.WriteEndElement();
		}
		#endregion

		#region Vytvoření hesláře
		public virtual void VytvoritHeslarXml()
		{
			Heslar.HeslarXml(mstrVstupniSoubor, mstrVystupniSoubor);
		}
		public virtual void VytvoritHeslarText(bool blnSeradit)
		{
			Heslar.HeslarText(mstrVstupniSoubor, mstrVystupniSoubor);
			if (blnSeradit)
				Heslar.SeraditHeslarText(mstrVystupniSoubor);
		}

		public virtual void VytvoritHeslarText()
		{
			VytvoritHeslarText(false);

		}
		public virtual void SeraditHeslarText()
		{
			Heslar.SeraditHeslarText(mstrVstupniSoubor);
		}
		#endregion

		#region Export pro fulltextové prohledávání
		public void UpravitProXmlFulltext(string strVstupniAdresar, string strVystupniAdresar, bool bVcetnePodadresaru)
		{
			DirectoryInfo d = new DirectoryInfo(strVstupniAdresar);
			foreach (DirectoryInfo dr in d.GetDirectories())
			{
				DirectoryInfo dn = new DirectoryInfo(strVystupniAdresar + "\\" + dr.Name);
				UpravitProXmlFulltext(dr.FullName, dn.FullName);
			}

		}
		public void UpravitProXmlFulltext(string strVstupniAdresar, string strVystupniAdresar)
		{
			DirectoryInfo d = new DirectoryInfo(strVstupniAdresar);
			DirectoryInfo dn = new DirectoryInfo(strVystupniAdresar);
			if (!dn.Exists)
				dn.Create();
			FileInfo[] fi = d.GetFiles();
			foreach (FileInfo f in fi)
			{
				XmlDocument xd = new XmlDocument();
				xd.Load(f.FullName);
				string sUse = xd.DocumentElement.GetAttribute("use");
				string sType = xd.DocumentElement.GetAttribute("type");
				//if (!(sUse == "internal" || sType == "excl")) {
				if (sUse != "internal")
				{ //některá vyloučená hesla (např. propria) nejsou interní
					XmlWriter xw = Objekty.VytvorXmlWriter(strVystupniAdresar + "\\" + f.Name);
					xw.WriteStartDocument();
					xw.WriteStartElement(xd.DocumentElement.Name);
					foreach (XmlAttribute at in xd.DocumentElement.Attributes)
					{
						xw.WriteAttributeString(at.Name, at.Value);
					}
					//xnd.WriteContentTo(xw);
					foreach (XmlNode xn in xd.DocumentElement)
					{
						switch (xn.NodeType)
						{
							case XmlNodeType.Comment:
							case XmlNodeType.ProcessingInstruction:
							case XmlNodeType.CDATA:
								break;
							case XmlNodeType.Element:
								if (xn.Attributes["type"] != null && xn.Attributes["type"].Value == "hidden")
								{
									break;
								}
								else
									goto default;
							//break;
							default:
								switch (xn.ChildNodes.Count)
								{
									case 0:
										RozdelitTextElementuPoPismenech(xw, 30, xn);
										break;
									case 1:
										if (xn.FirstChild.NodeType == XmlNodeType.Text)
											RozdelitTextElementuPoPismenech(xw, 30, xn);
										else
											VypisObsahovePrvky(xn, xw, 30);
										break;
									default:
										VypisObsahovePrvky(xn, xw, 30);
										break;
								}
								if (xd.DocumentElement.NextSibling != null)
								{
									//jde o vypsání processing instruction
									//xd.DocumentElement.NextSibling.WriteTo(xw);
								}
								break;
						}

					}

					xw.WriteEndDocument();
					xw.Close();
				}
			}


		}

		private void VypisObsahovePrvky(XmlNode xnd, XmlWriter xw)
		{
			foreach (XmlNode xn in xnd.ChildNodes)
			{
				if (xn.FirstChild.NodeType == XmlNodeType.Text)
				{
					xn.WriteTo(xw);
				}
				else
					VypisObsahovePrvky(xn, xw);
			}
		}


		private void VypisObsahovePrvky(XmlNode xnd, XmlWriter xw, int iPocetPismenVTextu)
		{
			string sNazevPrvku = xnd.Name;
			if (xnd.Attributes["type"] != null && xnd.Attributes["type"].Value == "hidden")
			{

			}
			else
			{
				foreach (XmlNode xn in xnd.ChildNodes)
				{
					RozdelitTextElementuPoPismenech(xw, iPocetPismenVTextu, xn);
				}
			}
		}

		private void RozdelitTextElementuPoPismenech(XmlWriter xw, int iPocetPismenVTextu, XmlNode xn)
		{
			if (xn.FirstChild == null)
			{
				return;
			}
			if (xn.FirstChild.NodeType == XmlNodeType.Text)
			{
				bool bPrvni;
				if (xn.Attributes["type"] != null && xn.Attributes["type"].Value == "hidden") { }
				else
				{
					string sText = xn.FirstChild.Value;
					//sText = sText.Trim();
					if (sText.Length > iPocetPismenVTextu)
					{
						string sSubString = sText.Trim();
						if (sSubString.Length <= iPocetPismenVTextu)
						{
							xw.WriteStartElement(xn.Name);
							foreach (XmlAttribute at in xn.Attributes)
							{
								xw.WriteAttributeString(at.LocalName, at.Value);
							}
							xw.WriteString(sSubString);
							xw.WriteEndElement();
						}
						else
						{
							int iMezera = sSubString.LastIndexOf(" ", iPocetPismenVTextu);
							bPrvni = true;
							while (iMezera > -1 & sSubString.Length > iPocetPismenVTextu)
							{
								xw.WriteStartElement(xn.Name);
								foreach (XmlAttribute at in xn.Attributes)
								{
									xw.WriteAttributeString(at.LocalName, at.Value);
								}
								if (bPrvni)
								{
									xw.WriteAttributeString("next", "true");
									bPrvni = false;
								}
								else
									xw.WriteAttributeString("prev", "true");
								xw.WriteString(sSubString.Substring(0, iMezera));
								xw.WriteEndElement();
								sSubString = sSubString.Substring(iMezera, sSubString.Length - iMezera).Trim();
								if (sSubString.Length > iPocetPismenVTextu)
								{
									iMezera = sSubString.LastIndexOf(" ", iPocetPismenVTextu);
								}
								else
								{
									iMezera = -1;
								}

							}
							if (sSubString.Length > 0)
							{
								xw.WriteStartElement(xn.Name);
								foreach (XmlAttribute at in xn.Attributes)
								{
									xw.WriteAttributeString(at.LocalName, at.Value);
								}
								xw.WriteAttributeString("prev", "true");
								xw.WriteString(sSubString);
							}
							xw.WriteEndElement();
						}
					}
					else
						xn.WriteTo(xw);
				}
			}
			else
				VypisObsahovePrvky(xn, xw, iPocetPismenVTextu);
		}
		#endregion

		public static string PismenoZakladniAbecedy(string pismeno)
		{
			const string pismena = "ABCČDEFGHIJKLMNOPQRŘSŠTUVWXYZŽ" + "Ĝ";
			if (pismeno.ToUpperInvariant() == "CH")
				return pismeno;
			if (pismeno.Length != 1) throw new ArgumentException("Písmeno je příliš dlouhé.");
			if (pismena.Contains(pismeno.ToUpperInvariant()))
				return pismeno;
			return RemoveDiacritics(pismeno);
		}

		public static string RemoveNonLetters(string text)
		{
			StringBuilder sb = new StringBuilder(text.Length);
			foreach (char c in text)
			{
				if (Char.IsLetterOrDigit(c))
					sb.Append(c);
			}
			return sb.ToString();
		}

		private static string RemoveDiacritics(string text)
		{
			if (text == null) throw new ArgumentNullException("text");
			if (text.Length > 0)
			{
				char[] chars = new char[text.Length];
				int charIndex = 0;
				text = text.Normalize(NormalizationForm.FormD);
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
						chars[charIndex++] = c;
				}
				return new string(chars, 0, charIndex).Normalize(NormalizationForm.FormC);
			}
			return text;
		}

		public static string DejPocatecniPismeno(string pocatecniPismeno)
		{
			return DejPocatecniPismeno(pocatecniPismeno, false);
		}

		/// <summary>
		/// Vrátí počáteční písmeno v předaném parametru; písmeno změní na velké.
		/// </summary>
		/// <param name="pocatecniPismeno"></param>
		/// <param name="zakladniAbeceda"></param>
		/// <returns></returns>
		public static string DejPocatecniPismeno(string pocatecniPismeno, bool zakladniAbeceda)
		{
			if (pocatecniPismeno.Length > 2)
			{
				if ((pocatecniPismeno[0].CompareTo('C') == 0 ||
						 pocatecniPismeno[0].CompareTo('c') == 0) &&
						(pocatecniPismeno[1].CompareTo('h') == 0 ||
						 pocatecniPismeno[1].CompareTo('H') == 0))
					pocatecniPismeno = "Ch";
				else
				{
					pocatecniPismeno = pocatecniPismeno[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
				}
			}
			else
			{
				pocatecniPismeno = pocatecniPismeno[0].ToString(CultureInfo.InvariantCulture).ToUpperInvariant();
			}
			pocatecniPismeno = PismenoZakladniAbecedy(pocatecniPismeno);
			return pocatecniPismeno;
		}

		public static string UppercaseFirst(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			char[] a = text.ToCharArray();
			for (int i = 0; i < a.Length; i++)
			{
				if (char.IsLetter(a[i]))
				{
					a[i] = char.ToUpper(a[i]);
					return new string(a);
				}
			}

			return new string(a);
		}
	}
}
