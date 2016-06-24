using System;
using System.Xml;
using Daliboris.Pomucky.Xml;
using DPXT = Daliboris.Pomucky.Xml.Transformace;


namespace Daliboris.Slovniky
{
	public class ESSC : Slovnik
	{
		private char[] chInterpunkce = CarkaTeckaMezeraZavorky;
		private char[] chInterpunkceBezZavorek = CarkaTeckaMezeraStrednik;
		private char[] chSeparatory = Separatory;

		public ESSC()
		{
		}
		/// <summary>
		/// Konstruktor vytvářející instanci třídy
		/// </summary>
		/// <param name="strVstupniSoubor">Vstupní soubor obsahující slovníkový text</param>
		public ESSC(string strVstupniSoubor)
			: base(strVstupniSoubor)
		{
		}
		/// <summary>
		/// Konstruktor vytvářející instanci třídy
		/// </summary>
		/// <param name="strVstupniSoubor">Vstupní soubor obsahující slovníkový text</param>
		/// <param name="strVystupniSoubor">Výstupní soubor, do nejž se uloží slovníkový text po úpravách</param>
		public ESSC(string strVstupniSoubor, string strVystupniSoubor)
			: base(strVstupniSoubor, strVystupniSoubor)
		{
		}

		/// <summary>
		/// Upraví hranice heslové stati, seskupí všechny prvky heslové stati do elementu &lt;entry&gt;. Využívá při tom značku &lt;entryend&gt;
		/// </summary>
		/// <exception cref="ArgumentNullException">Vyvolá výjimku, pokud nejsou zadány vstupní nebo výstupní soubor.</exception>
		public override void UpravitHraniceHesloveStati(string inputFile, string outputFile)
		{
			//výchozí imnplementace se hodí pro ESSČ
			string sChyba = null;
			if (inputFile == null || outputFile == null)
			{
				throw new ArgumentNullException("Nebyly zadány vhodné názvy vstupního nebo výstupního souboru.");
			}
			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{

					/*
					string strTypSenseGrp = null;
					Stack<string> gstTypSenseGrp = new Stack<string>();
					int intPocetOtevrenychSenseGrp = 0;
					bool blnVyskytlySeVyznamy = false;
					*/
					bool blnJeSenseGrp = false;
					bool blnJeSense = false;

					xw.WriteStartDocument(true);
					bool blnPrvniEntryhead = true;
					try
					{
						while (r.Read())
						{
							if (r.NodeType == XmlNodeType.Element)
							{
								switch (r.Name)
								{
									case "entryhead":
										if (blnPrvniEntryhead)
										{
											xw.WriteStartElement("entry");
											xw.WriteWhitespace("\r\n");
											blnPrvniEntryhead = false;
										}
										DPXT.SerializeNode(r, xw);
										blnJeSenseGrp = blnJeSense = false;

										break;
									case "entryend":
										xw.WriteEndElement(); //entry
										xw.WriteWhitespace("\r\n");
										blnPrvniEntryhead = true;

										break;
									case "senseGrp":
										if (blnJeSense)
											xw.WriteEndElement(); //senses

										if (blnJeSenseGrp)
											xw.WriteEndElement(); //senseGrp

										blnJeSenseGrp = true;
										blnJeSense = false;

										goto default;
									case "sense":
										if (!blnJeSense)
											xw.WriteStartElement("senses");
										blnJeSense = true;

										goto default;
									default:
										if (blnJeSense && r.Depth == 2 && !(r.Name == "sense"))
										{
											xw.WriteEndElement(); //senses
											blnJeSense = false;
										}
										if (blnJeSenseGrp && r.Depth == 2 && !r.Name.StartsWith("sense"))
										{
											blnJeSenseGrp = false;
											xw.WriteEndElement(); //senseGrp
										}
										DPXT.SerializeNode(r, xw);
										break;
								}
							}
							else if (r.NodeType == XmlNodeType.EndElement)
							{
								if (r.Depth == 2)
									switch (r.Name)
									{
										case "senseGrp":
											/*
											if (blnJeSense)
											{
												xw.WriteEndElement(); //senseGrp
												blnJeSenseGrp = false;
											}
											 */
											//DPXT.SerializeNode(r, xw);
											break;
										case "sense":
											DPXT.SerializeNode(r, xw);
											break;
										default:
											if (blnJeSense)
											{
												xw.WriteEndElement(); //senses
												blnJeSense = false;
											}
											DPXT.SerializeNode(r, xw);
											break;
									}
								else { DPXT.SerializeNode(r, xw); }
							}
							else { DPXT.SerializeNode(r, xw); }

						}
					}
					catch (Exception ex)
					{
						sChyba = ex.Message;
						sChyba = r.ReadInnerXml();
						while (sChyba.Trim().Length == 0)
						{
							if (r.Read())
								sChyba = r.ReadInnerXml();
							else
							{
								sChyba = "XmlReader je na konci souboru.";
							}
						}
						Console.WriteLine(sChyba);
					}
					finally
					{
						if (xw.WriteState != WriteState.Error && xw.WriteState != WriteState.Closed)
							xw.WriteEndDocument();
						xw.Flush();
						xw.Close();
					}
				}
			}
		}

		/// <summary>
		/// Konsoliduje heslovou stať. Přidá informace, seskupí významy.
		/// </summary>
		public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
		{
			KonsolidovatHeslovouStat(inputFile, outputFile, 1);
		}

		/// <summary>
		/// Konsoliduje heslovou stať. Přidá informace, seskupí významy.
		/// 1) Přiřadí identifikátor heslové stati (prvku &lt;entry&gt;)
		/// 2) Přiřadí heslové stati zdroj
		/// 3) Určí typ heslové stati (full, ref)
		/// 4) Určí výchozí heslové slovo (pod nímž se bude heslová stať zobrazovat v seznamech)
		/// 5) Přiřadí identifikátor jednotlivým heslovým slovům (prvků &lt;hw&gt;)
		/// 6) Pokud záhlaví obsahuje odkaz na zpracované heslo, přiřadí tomuto odkazu zdroj (na základě údaje v rámci akce ["zpracování v MSS/ESSČ"])
		/// 7) Seskupí významy do skupiny &gt;senses&lt;
		/// 8) Přiřadí heslové stati oblast použití (public, internal) na základě údajů v rámci akce
		/// </summary>
		/// <param name="iVychoziID">Výchozí pořadové číslo heslové stati.</param>
		public void KonsolidovatHeslovouStat(string inputFile, string outputFile, int iVychoziID)
		{

			int iEntry = iVychoziID - 1;
			string sSource = null;
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
									XmlDocument xd = new XmlDocument();
									XmlNode xn = xd.ReadNode(r);
									if (xn != null)
										xd.AppendChild(xn);
									if (xd.DocumentElement != null)
										if (!xd.DocumentElement.IsEmpty)
										{
											ZkonsolidujEntry(ref xd, sSource, ++iEntry);
											xd.WriteContentTo(xw);
											xw.WriteWhitespace("\r\n");
										}

									break;
								case "dictionary":
									sSource = r.GetAttribute("name");
									Transformace.SerializeNode(r, xw);
									break;
								default:
									Transformace.SerializeNode(r, xw);
									break;

							}
						}
						else if (r.NodeType == XmlNodeType.EndElement)
						{
							switch (r.Name)
							{
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

		/// <summary>
		/// Konsolidace heslové stati (elementu &lt;entry&gt;)
		/// </summary>
		/// <param name="xd">Dokument XML obssahující jednu kompletní heslovou stať</param>
		/// <param name="sSlovnikId">Identifikátor slovníku, v jehož rámci se heslová stať nachází</param>
		/// <param name="iEntry">Pořadové číslo heslové stati v rámci slovníku</param>
		private void ZkonsolidujEntry(ref XmlDocument xd, string sSlovnikId, int iEntry)
		{

			if (xd.DocumentElement == null)
				return;

			// 1) Přiřadí identifikátor heslové stati (prvku &lt;entry&gt;)
			XmlAttribute xa = xd.CreateAttribute("id");
			xa.Value = "en" + iEntry.ToString("000000");
			if (xd.DocumentElement != null)
				xd.DocumentElement.Attributes.Append(xa);

			// 2) Přiřadí heslové stati zdroj
			xa = xd.CreateAttribute("source");
			xa.Value = sSlovnikId;
			if (xd.DocumentElement != null)
				xd.DocumentElement.Attributes.Append(xa);

			// 3) Určí typ heslové stati (full, ref)
			XmlNode xn = xd.SelectSingleNode("//entryhead");
			if (xn != null)
			{
				if (xn.Attributes.Count == 1)
				{
					xa = (XmlAttribute)xn.Attributes[0].Clone();
					if (xd.DocumentElement != null)
						xd.DocumentElement.Attributes.Append(xa);
					xn.Attributes.Remove(xn.Attributes[0]);
				}
				else
				{
					xa = xd.CreateAttribute("type");
					xa.Value = "full";
					if (xd.DocumentElement != null)
						xd.DocumentElement.Attributes.Append(xa);
				}

			}

			// 4) Určí výchozí heslové slovo (pod nímž se bude heslová stať zobrazovat v seznamech)
			xn = xd.SelectSingleNode("//hw");
			xa = xd.CreateAttribute("defaulthw");
			string sVychoziHeslo = xn.InnerText.TrimEnd(chInterpunkceBezZavorek).TrimEnd();
			xa.Value = sVychoziHeslo;
			xd.DocumentElement.Attributes.Append(xa);

			xa = xd.CreateAttribute("defaulthwsort");
			xa.Value = HesloBezKrizkuAHvezdicky(sVychoziHeslo);
			xd.DocumentElement.Attributes.Append(xa);


			/* TESTOVÁNÍ */
			/*
			if (sVychozHeslo == "zanykl")
			{
				sVychozHeslo = sVychozHeslo;
			}
			*/

			if (sVychoziHeslo.Contains("(?)"))
				sVychoziHeslo = sVychoziHeslo.Replace("(?)", "").TrimEnd();
			xa.Value = sVychoziHeslo;
			//string sVychoziHeslo = xa.Value;
			if (xd.DocumentElement != null)
				xd.DocumentElement.Attributes.Append(xa);

			// 5) Přiřadí identifikátor jednotlivým heslovým slovům (prvků &lt;hw&gt;)
			XmlNodeList xnl = xd.SelectNodes("//hw");
			for (int i = 0; i < xnl.Count; i++)
			{
				string sIdHw = (i + 1).ToString();
				xa = xd.CreateAttribute("id");
				xa.Value = "en" + iEntry.ToString("000000") + ".hw" + sIdHw;
				xnl[i].Attributes.Append(xa);
			}

			xnl = xd.SelectNodes("//action");
			string sAkce = "";
			for (int i = 0; i < xnl.Count; i++)
			{
				if (i < xnl.Count - 1)
					sAkce += xnl[i].InnerText + ",";
				else
					sAkce += xnl[i].InnerText;
			}


			// 6) Pokud záhlaví obsahuje odkaz na zpracované heslo, přiřadí tomuto odkazu zdroj (na základě údaje v rámci akce ["zpracování v MSS/ESSČ"])

			xnl = xd.SelectNodes("//identity");
			foreach (XmlNode x in xnl)
			{
				string sText = x.InnerText.TrimEnd(chInterpunkce).TrimEnd(chSeparatory);
				string sSource = null;
				XmlNode xnd = x.NextSibling;
				while (xnd.NodeType != XmlNodeType.Element)
				{
					xnd = xnd.NextSibling;
				}

				if (sText.StartsWith("h"))
					//sSource = "StcSMat";
					sSource = "HesStcS";
				else if (sText.StartsWith("en"))
				{
					//nemusí jít vždy o MSS, nyní už se odkazuje i na StčS, ESSČ aj.
					//je třeba zkontrolovat nejbližší předchozí údaj o zdroji

					//kontrola zdroja na základě informací v prvku akce (<action>)
					if (sAkce.Contains("ESSČ"))
						sSource = "ESSC";
					else if (sAkce.Contains("MSS"))
						sSource = "MSS";
					else
					{
						sSource = "MSS";
					}
				}
				if (sSource != null)
				{
					xa = xd.CreateAttribute("source");
					xa.Value = sSource;
					xnd.Attributes.Append(xa);
				}

				xa = xd.CreateAttribute("target");
				xa.Value = sText;
				xnd.Attributes.Append(xa);
				x.ParentNode.RemoveChild(x);
			}

			//nahrazeno transformační šablonou
			// 7) Seskupí významy do skupiny &gt;senses&lt;
			/*
			XmlNode xnPredchozi = null;
			foreach (XmlNode nsense in xd.DocumentElement.ChildNodes)
			{
				if (nsense.Name != "sense") continue;
				xnPredchozi = nsense.PreviousSibling;
				break;
			}

			if (xnPredchozi != null)
			{
				XmlNode xnSenses = null;
				foreach (XmlNode nsense in xd.DocumentElement.ChildNodes)
				{
					if (nsense.Name == "sense")
					{
					if(xnSenses == null)
						xnSenses = xd.CreateElement("senses");
					xnSenses.AppendChild(nsense);
					}
					else
					{
					if (xnSenses != null)
					{
						xd.DocumentElement.InsertAfter(xnSenses, xnPredchozi);
						xnSenses = null;
					}
					}
				}
				if (xnSenses != null)
					xd.DocumentElement.InsertAfter(xnSenses, xnPredchozi);
			}
			*/
			/*
			xnl = xd.SelectNodes("//sense");
			if (xnl.Count > 0) {
				XmlElement xe = xd.CreateElement("senses");
				foreach (XmlNode x in xnl) {
					xe.AppendChild(x);
				}
				xd.DocumentElement.InsertAfter(xe, xnPredchozi);

			}
		 */

			// 8) Přiřadí heslové stati oblast použití (public, internal) na základě údajů v rámci akce
			xa = xd.CreateAttribute("use");
			string[] aAkce = sAkce.Split(chSeparatory);
			bool bInterni = false;
			for (int i = 0; i < aAkce.Length; i++)
			{
				switch (aAkce[i].Trim())
				{
					case "proprium":
					case "depropriální výraz":
					case "chybějící doklady":
						if (xa.Value == "")
							xa.Value = "public";
						break;
					case "zpracováno v MSS":
					case "zpracováno v ESSČ":
						if (xd.SelectNodes("//note").Count == 0)
						{
							//pokud se heslo liší od odkazovaného v MSS (svolenie, svoľenie)
							//string sOdkaz = null;
							//if (xd.SelectSingleNode("//xref") != null)
							//   sOdkaz = xd.SelectSingleNode("//xref").InnerText;
							//else
							//   System.Diagnostics.Debug.Print(sVychoziHeslo);
							//System.Diagnostics.Debug.Listeners.Clear();
							//System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(Console.Out));
							//System.Diagnostics.Debug.AutoFlush = true;

							//if (sOdkaz == null || sOdkaz.TrimEnd(chInterpunkceBezZavorek).TrimEnd() != sVychoziHeslo) {
							//   xa.Value = "public";
							//}
							//else {
							bInterni = true;
							xa.Value = "internal";
							//}
						}
						else
							xa.Value = "public";
						break;
					case "pouhé násloví":
					case "přeřazeno sub":
					case "nenáležitá podoba v HesStčS":
					case "citátové slovo":
					case "po roce 1500":
					case "pomocné heslo": //pro potřeby poznámek u písmene Y
						bInterni = true;
						xa.Value = "internal";
						break;
					default:
						break;
				}
			}
			if (bInterni)
				xa.Value = "internal";
			if (xa.Value == "")
				xa.Value = "public";
			xd.DocumentElement.Attributes.Append(xa);
		}

		public void VypisCharakteristiku()
		{
			if (base.VstupniSoubor == null || base.VystupniSoubor == null)
			{
				throw new ArgumentNullException("Nebyly zadány vhodné názvy vstupního nebo výstupního souboru.");
			}
			using (XmlReader r = Objekty.VytvorXmlReader(base.VstupniSoubor))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(base.VystupniSoubor))
				{

					xw.WriteStartDocument(true);
					xw.WriteStartElement("entries");
					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element)
						{
							switch (r.Name)
							{
								case "entryhead":
									XmlDocument xd = new XmlDocument();
									xd.LoadXml(r.ReadOuterXml());
									if (xd.DocumentElement.InnerXml.Contains("<pos>"))
									{
										xw.WriteStartElement("entryhead");
										foreach (XmlNode xn in xd.DocumentElement.ChildNodes)
										{
											if (xn.Name == "pos")
											{
												xn.WriteTo(xw);
												xw.WriteEndElement();
												break;
											}
											xn.WriteTo(xw);
										}
									}
									break;

								default:
									break;
							}
						}
					}
					xw.WriteEndElement();
				}
			}
		}

		public void VypisHeslaTypeUse()
		{
			if (base.VstupniSoubor == null || base.VystupniSoubor == null)
			{
				throw new ArgumentNullException("Nebyly zadány vhodné názvy vstupního nebo výstupního souboru.");
			}
			using (XmlReader r = Objekty.VytvorXmlReader(base.VstupniSoubor))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(base.VystupniSoubor))
				{

					xw.WriteStartDocument(true);
					xw.WriteStartElement("entries");
					while (r.Read())
					{
						if (r.NodeType == XmlNodeType.Element)
						{
							switch (r.Name)
							{
								case "entry":
									string sUse = r.GetAttribute("use");
									string sType = r.GetAttribute("type");
									if (sUse == "public" && sType == "excl")
									{
										DPXT.SerializeNode(r, xw);
										xw.WriteEndElement();
									}
									break;
								default:
									break;
							}
						}
					}
					xw.WriteEndElement();
				}
			}
		}
	}
}
