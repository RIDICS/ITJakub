using System;
using System.Collections.Generic;
using System.Xml;
using Daliboris.Pomucky.Xml;
using DPXT = Daliboris.Pomucky.Xml.Transformace;


namespace Daliboris.Slovniky
{
	public class StcS : Slovnik
	{
		private char[] chInterpunkceBezZavorek = Slovnik.CarkaTeckaMezeraStrednik;

		public StcS()
		{
		}
		public StcS(string strVstupniSoubor)
		{
			base.VstupniSoubor = strVstupniSoubor;
		}
		public StcS(string strVstupniSoubor, string strVystupniSoubor)
		{
			base.VstupniSoubor = strVstupniSoubor;
			base.VystupniSoubor = strVystupniSoubor;
		}

		/// <summary>
		/// Upraví hranice heslové stati. Ohraničí jednotlivé části značkou &lt;entry&gt; a v rámci heslové stati seskupí významy podřazené značce &lt;senseGrp&gt;.
		/// </summary>
		public override void UpravitHraniceHesloveStati(string inputFile, string outputFile)
		{
			if (inputFile == null || outputFile == null)
			{
				throw new ArgumentNullException("Nebyly zadány vhodné názvy vstupního nebo výstupního souboru.");
			}
			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{


					xw.WriteStartDocument(true);
					bool blnPrvniEntryhead = true;
					bool blnVyskytlySeVyznamy = false;
					string strTypSenseGrp = null;
					Stack<string> gstTypSenseGrp = new Stack<string>();
					int intPocetOtevrenychSenseGrp = 0;
					bool blnPrvniVyznam = false;
					int xmlReaderDepp = 0;
					int xmlWriterDeep = 0;

					while (r.Read())
					{
						//Console.WriteLine("Reader: {0}, Writer: {1}", xmlReaderDepp, xmlWriterDeep);
						if (r.NodeType == XmlNodeType.Element)
						{
							xmlReaderDepp++;
							if (r.IsEmptyElement)
							{
								xmlReaderDepp--;
							}
							if (r.Name == "div1")
							{
								string name = r.Name;
							}
							switch (r.Name)
							{
								case "entryhead":
									if (blnPrvniVyznam)
									{
										WriteEndElement(xw, ref xmlWriterDeep);  //senses
										xw.WriteWhitespace("\r\n");
									}
									blnPrvniVyznam = false;
									blnVyskytlySeVyznamy = false;
									int iPocetSenseGrp = gstTypSenseGrp.Count;
									if (iPocetSenseGrp > 0)
									{
										for (int i = 1; i < iPocetSenseGrp; i++)
										{
											intPocetOtevrenychSenseGrp--;
											WriteEndElement(xw, ref xmlWriterDeep);  //senseGrp
											xw.WriteWhitespace("\r\n");
										}
										gstTypSenseGrp.Clear();
									}
									while (intPocetOtevrenychSenseGrp > 0)
									{
										WriteEndElement(xw, ref xmlWriterDeep);
										xw.WriteWhitespace("\r\n");
										intPocetOtevrenychSenseGrp--;
									}
									if (!blnPrvniEntryhead)
									{
										WriteEndElement(xw, ref xmlWriterDeep); //entry
										xw.WriteWhitespace("\r\n");
									}
									else
									{
										blnPrvniEntryhead = false;
									}
									WriteStartElement(xw, "entry", ref xmlWriterDeep);

									xw.WriteWhitespace("\r\n");
									goto default;
								case "senseGrp":
									strTypSenseGrp = r.GetAttribute("type");
									intPocetOtevrenychSenseGrp++;
									if (gstTypSenseGrp.Count > 0)
									{
										if (strTypSenseGrp == gstTypSenseGrp.Peek())
										{
											WriteEndElement(xw, ref xmlWriterDeep);  //senseGrp
											xw.WriteWhitespace("\r\n");
											intPocetOtevrenychSenseGrp--;
										}
										else
										{
											if (blnVyskytlySeVyznamy)
											{
												WriteEndElement(xw, ref xmlWriterDeep);
												xw.WriteWhitespace("\r\n");
												intPocetOtevrenychSenseGrp--;
											}
											else
											{
												gstTypSenseGrp.Push(strTypSenseGrp);
											}
										}
									}
									else
									{
										gstTypSenseGrp.Push(strTypSenseGrp);
									}
									blnVyskytlySeVyznamy = false;
									goto default;
								case "sense":
									if (!blnPrvniVyznam)
									{
										WriteStartElement(xw, "senses", ref xmlWriterDeep); //xw.WriteStartElement("senses");
										xw.WriteWhitespace("\r\n");
										blnPrvniVyznam = true;
									}
									blnVyskytlySeVyznamy = true;
									goto default;
								case "note":
								case "appendix":
									if (r.IsEmptyElement)
										goto default;
									while (gstTypSenseGrp.Count > 0)
									{
										//xw.WriteEndElement();
										WriteEndElement(xw, ref xmlWriterDeep);
										xw.WriteWhitespace("\r\n");
										intPocetOtevrenychSenseGrp--;
										gstTypSenseGrp.Pop();
									}
									if (blnPrvniVyznam)
									{
										//xw.WriteEndElement(); //senses
										WriteEndElement(xw, ref xmlWriterDeep);
										xw.WriteWhitespace("\r\n");
										blnPrvniVyznam = false;
									}
									goto default;
								default:
									if (!r.IsEmptyElement)
										xmlWriterDeep++;
									DPXT.SerializeNode(r, xw);
									break;
							}
						}
						else
						{
							if (r.NodeType == XmlNodeType.EndElement)
							{
								xmlReaderDepp--;
								switch (r.Name)
								{
									case "senseGrp":
										if (blnPrvniVyznam)
										{
											//xw.WriteEndElement(); //senses
											WriteEndElement(xw, ref xmlWriterDeep);
											xw.WriteWhitespace("\r\n");
											blnPrvniVyznam = false;
										}
										break;
									case "div1":
										//xw.WriteEndElement(); //entry
										while (xmlWriterDeep > xmlReaderDepp + 1)
										{
											WriteEndElement(xw, ref xmlWriterDeep);
										}

										blnPrvniEntryhead = true;
										blnPrvniVyznam = false;
										goto default;
									default:
										xmlWriterDeep--;
										DPXT.SerializeNode(r, xw);
										break;
								}
							}
							//else if (r.NodeType == XmlNodeType.XmlDeclaration && r.Name == "xml") {
							//  //nedělat nic, jde o začátek dokumentu; genruje cyhbu: nelze zapsat deklaraci, dokument již byl započat
							//}

							else
							{
								DPXT.SerializeNode(r, xw);
							}
						}
					}
				}
			}
		}

		private void WriteStartElement(XmlWriter xw, string elementName, ref int xmlWriterDeep)
		{
			xw.WriteStartElement(elementName);
			xmlWriterDeep++;
		}

		private void WriteEndElement(XmlWriter xw, ref int xmlWriterDeep)
		{
			xw.WriteEndElement();
			xmlWriterDeep--;
		}


		public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
		{
			KonsolidovatHeslovouStat(inputFile, outputFile, 1);
		}
		public void KonsolidovatHeslovouStat(string inputFile, string outputFile, int iVychoziID)
		{

			int iEntry = iVychoziID - 1;
			string sSource = null;
			using (XmlReader r = Objekty.VytvorXmlReader(inputFile))
			{
				using (XmlWriter xw = Objekty.VytvorXmlWriter(outputFile))
				{
					xw.WriteStartDocument(true);

				    bool skipReading = false;

					while (skipReading || r.Read())
					{
					    skipReading = false;

					    switch (r.NodeType)
					    {
					        case XmlNodeType.Element:
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
					                        }

					                    if (r.NodeType == XmlNodeType.Element || r.NodeType == XmlNodeType.EndElement)
					                        skipReading = true;

					                    break;
					                case "dictionary":
					                    sSource = r.GetAttribute("name");
					                    goto default;
					                default:
					                    Transformace.SerializeNode(r, xw);
					                    break;

					            }

					            break;

					        case XmlNodeType.EndElement:
					            switch (r.Name)
					            {
					                case "entry":
					                    break;
					                default:
					                    Transformace.SerializeNode(r, xw);
					                    break;
					            }

					            break;

					        default:
					            Transformace.SerializeNode(r, xw);
					            break;
					    }
					}
				}
			}

		}

		private void ZkonsolidujEntry(ref XmlDocument xd, string sSlovnikID, int iEntry)
		{

			if (xd.DocumentElement == null)
				return;
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
			if (xn != null)
			{
				//Pokud existuje atibut type existuje, jeho hodnota se přiřadí ke značce entry
				if (xn.Attributes.Count == 1)
				{
					xa = (XmlAttribute)xn.Attributes[0].Clone();
					xd.DocumentElement.Attributes.Append(xa);
					//nakonec se atribut type u značky entryhead odstraní (byl tam jenom kvůli generování z Wordu)
					xn.Attributes.Remove(xn.Attributes[0]);
				}
				else
				{
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
			xa.Value = xn.InnerText.TrimEnd(chInterpunkceBezZavorek).TrimEnd();
			xd.DocumentElement.Attributes.Append(xa);
			string sVychoziHeslo = xa.Value;

			xa = xd.CreateAttribute("defaulthwsort");
			xa.Value = Slovnik.HesloBezKrizkuAHvezdicky(sVychoziHeslo);
			xd.DocumentElement.Attributes.Append(xa);

			//každému heslovému slovu se přiřadí identifikátor
			XmlNodeList xnl = xd.SelectNodes("//hw");
			for (int i = 0; i < xnl.Count; i++)
			{
				string sIdHw = (i + 1).ToString();
				xa = xd.CreateAttribute("id");
				xa.Value = "en" + iEntry.ToString("000000") + ".hw" + sIdHw;
				xnl[i].Attributes.Append(xa);
			}
			/*
			XmlNode xnPredchozi = null;
			XmlNodeList xnlSenseGrp = xd.SelectNodes(".//senseGrp");
			if (xnlSenseGrp != null && xnlSenseGrp.Count > 0) {

				foreach (XmlNode xnSenseGrp in xnlSenseGrp) {

					XmlNodeList xnlSubSenseGrp = xnSenseGrp.SelectNodes(".//senseGrp");
					if (xnlSubSenseGrp != null && xnlSubSenseGrp.Count > 0)
					{
						foreach (XmlNode node in xnlSubSenseGrp)
						{
							SeskupitSensesVRamciSenseGrp(xd, node, xnPredchozi);
						}
					}
					else {
						SeskupitSensesVRamciSenseGrp(xd, xnSenseGrp, xnPredchozi);
					}
				}
			}
			else {
				//seskupí více významových odstavců do jednoho elementu
				foreach (XmlNode nsense in xd.DocumentElement.ChildNodes) {
					if (nsense.Name == "sense") {
						xnPredchozi = nsense.PreviousSibling;
						break;
					}
				}

				xnl = xd.SelectNodes("//sense");
				if (xnl.Count > 0) {
					XmlElement xe = xd.CreateElement("senses");
					foreach (XmlNode x in xnl) {
						xe.AppendChild(x);
					}
					xd.DocumentElement.InsertAfter(xe, xnPredchozi);
				}
			}
		 */

		}

		private void SeskupitSensesVRamciSenseGrp(XmlDocument xd, XmlNode xnSenseGrp, XmlNode xnPredchozi)
		{
			XmlNodeList xnl;
			foreach (XmlNode nsense in xnSenseGrp.ChildNodes)
			{
				if (nsense.Name == "sense")
				{
					xnPredchozi = nsense.PreviousSibling;
					break;
				}
			}

			xnl = xnSenseGrp.SelectNodes(".//sense");
			if (xnl.Count > 0)
			{
				XmlElement xe = xd.CreateElement("senses");
				foreach (XmlNode x in xnl)
				{
					xe.AppendChild(x);
				}
				xnSenseGrp.InsertAfter(xe, xnPredchozi);
			}
		}
	}
}
