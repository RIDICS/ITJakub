using System;
using System.Globalization;
using System.Xml;

namespace Daliboris.OOXML.Word.Transform
{
	public class XmlGenerator : IXmlGenerator
	{

		string obrazekElementName = "picture";
		string poznamkaPodCarouElementName = "poznamka_pod_carou";

		private const string csRoot = "WDoc2XMLRoot";
		private const string csObsah = "\\o";
		private const string csDelimitator = "^";
		private const string csHvezdicka = "*";

		public delegate void Progress(object sender, ProgressEventArgs ev);


		public event Progress Prubeh;

		private string mstrSouborDocx;
		private string mstrSouborDoc2Xml;
		private string mstrSouborXml;
		private DocxReader mdxrReader;
		private Transformace mtrTransformace;
		//private transformace mtrTransformace;
		private XmlWriter mxwWriter;
		//private Styles mstStyly;

		//pokud se soubor změnil, je potřeba znovu nahrát transformace
		private bool mblnZmenaSouboruDocx2Xml;

		private string mstrIgnorovanyOdstavcovyStyl = null;
		private bool mblnIgnorovat = false;
		private string mstrPredchoziOdstavcovyStyl = null;
		private string mstrAktualniOdstavcovyStyl = null;
		private string mstrPredchoziZnakovyStyl = null;
		private Tag mtgAktualniOdstavcovyTag = null;
		private Tag mtgAktualniZnakovyTag = null;
		private Tag mtgPredchoziZnakovyTag = null;
		private TextRunEventArgs mtreaAktualniTextRun = null;
		private Settings mnstSettings = new Settings();

		public XmlGenerator() { }

		public XmlGenerator(string souborDocx, string souborXml) : this(souborDocx, null, souborXml) { }

		public XmlGenerator(string souborDocx, string souborXml, Settings nastaveni) : this(souborDocx, null, souborXml, nastaveni) { }

		public XmlGenerator(string strSouborDocx, string strSouborDoc2Xml, string strSouborXml)
		{
			mstrSouborDocx = strSouborDocx;
			SouborDoc2Xml = strSouborDoc2Xml;
			mstrSouborXml = strSouborXml;
		}

		public XmlGenerator(string strSouborDocx, string strSouborDoc2Xml, string strSouborXml, Settings nstNastaveni)
			: this(strSouborDocx, strSouborDoc2Xml, strSouborXml)
		{
			mnstSettings = nstNastaveni;

		}

		public Settings Nastaveni
		{
			get { return mnstSettings; }
			set { mnstSettings = value; }
		}

		#region IXmlGenerator Members

		public string SouborDocx
		{
			get { return mstrSouborDocx; }
			set { mstrSouborDocx = value; }
		}

		public string SouborDoc2Xml
		{
			get { return mstrSouborDoc2Xml; }
			set
			{
				if (mstrSouborDoc2Xml != value)
				{
					mblnZmenaSouboruDocx2Xml = true;
					mstrSouborDoc2Xml = value;
				}

			}
		}

		public string SouborXml
		{
			get { return mstrSouborXml; }
			set { mstrSouborXml = value; }
		}

		public void Read()
		{
			if (mtrTransformace == null)
			{
				mtrTransformace = new Transformace(mstrSouborDoc2Xml);
				mtrTransformace.NactiZeSouboru();
				mblnZmenaSouboruDocx2Xml = false;

			}
			else
			{
				if (mblnZmenaSouboruDocx2Xml)
				{
					mtrTransformace.Soubor = mstrSouborDoc2Xml;
					mtrTransformace.NactiZeSouboru();
					mblnZmenaSouboruDocx2Xml = false;
				}
			}

			mdxrReader = new DocxReader(mstrSouborDocx);
			mdxrReader.ReadUsedStyles();
			Styles stsUsed = mdxrReader.UsedStyles;
			Styles stNedefinovane = new Styles();
			string sStyly = null;
			foreach (Style st in stsUsed)
			{
				if (!mtrTransformace.Tagy.ContainsID(st.UIName))
				{
					stNedefinovane.Add(st);
					sStyly += st.UIName + "; ";
				}
			}
			if (stNedefinovane.Count > 0)
			{
				if (!mtrTransformace.Tagy.ContainsID("*"))
					throw new ArgumentException("Transofrmační soubor neobsahuje všechny styly použité v dokumentu: " + sStyly);
			}


			//mdxrReader.ReadStyles();
			mdxrReader.CountStructures();
			//mstStyly = mdxrReader.Styles;


			//provést kontrolu, jestli Xml2Docx obsahuje pravidla pro všechny styly


			mdxrReader.ZacatekDokumentu += new DocxReader.Document(mdxrReader_ZacatekDokumentu);
			mdxrReader.KonecDokumentu += new DocxReader.Document(mdxrReader_KonecDokumentu);

			mdxrReader.ZacatekTelaDokumentu += new DocxReader.Body(mdxrReader_ZacatekTelaDokumentu);
			mdxrReader.KonecTelaDokumentu += new DocxReader.Body(mdxrReader_KonecTelaDokumentu);

			mdxrReader.ZacatekOdstavce += new DocxReader.Paragraph(mdxrReader_ZacatekOdstavce);
			mdxrReader.KonecOdstavce += new DocxReader.Paragraph(mdxrReader_KonecOdstavce);
			mdxrReader.ZnakovyStyl += new DocxReader.TextRun(mdxrReader_ZnakovyStyl);

			mdxrReader.ZacatekTabulky += new DocxReader.Table(mdxrReader_ZacatekTabulky);
			mdxrReader.ZacatekRadku += new DocxReader.TableRow(mdxrReader_ZacatekRadku);
			mdxrReader.ZacatekBunky += new DocxReader.TableCell(mdxrReader_ZacatekBunky);
			mdxrReader.KonecBunky += new DocxReader.TableCell(mdxrReader_KonecBunky);
			mdxrReader.KonecRadku += new DocxReader.TableRow(mdxrReader_KonecRadku);
			mdxrReader.KonecTabulky += new DocxReader.Table(mdxrReader_KonecTabulky);

			mdxrReader.Pismo += new DocxReader.Font(mdxrReader_Pismo);

			mdxrReader.Prubeh += new DocxReader.Progress(mdxrReader_Prubeh);

			mdxrReader.Obrazek += new DocxReader.Picture(mdxrReader_Obrazek);

			mdxrReader.ZacatekPoznamkyPodCarou += new DocxReader.Footnote(mdxrReader_ZacatekPoznamkyPodCarou);
			mdxrReader.KonecPoznamkyPodCarou += new DocxReader.Footnote(mdxrReader_KonecPoznamkyPodCarou);

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.CloseOutput = true;
			xws.Encoding = mnstSettings.OutputEncoding;
			xws.Indent = mnstSettings.OutputIndent;
			xws.IndentChars = mnstSettings.OutputIndentChars;

			mxwWriter = XmlWriter.Create(mstrSouborXml, xws);

			mdxrReader.Read();

			mdxrReader = null;
			mxwWriter.Flush();
			mxwWriter.Close();
		}

		void mdxrReader_KonecPoznamkyPodCarou(object sender, FootnoteEventArgs ev)
		{
			//poznámka skončila, text poznámky již byl vypsán a není aktuální
			mtreaAktualniTextRun = null;
			mxwWriter.WriteEndElement();//poznamka
		}

		void mdxrReader_ZacatekPoznamkyPodCarou(object sender, FootnoteEventArgs ev)
		{
			mxwWriter.WriteStartElement(poznamkaPodCarouElementName);
			mxwWriter.WriteAttributeString("n", ev.Sequence.ToString(CultureInfo.InvariantCulture));
			mxwWriter.WriteAttributeString("id", ev.Identifikator);
		}

		void mdxrReader_Obrazek(object sender, PictureEventArgs ev)
		{

			VypsatTagProAktualniZnakovyStyl();
			if (mtreaAktualniTextRun != null)
				mtreaAktualniTextRun.Text = String.Empty;

			mxwWriter.WriteStartElement(obrazekElementName);
			mxwWriter.WriteAttributeString("id", ev.Identifikator);
			mxwWriter.WriteAttributeString("n", ev.Sequence.ToString(CultureInfo.InvariantCulture));
			mxwWriter.WriteAttributeString("soubor", ev.Umisteni);
			mxwWriter.WriteEndElement(); //obrazek
		}


		void mdxrReader_Prubeh(object sender, ProgressEventArgs ev)
		{
			if (Prubeh != null)
				Prubeh(this, ev);
		}

		void mdxrReader_ZacatekDokumentu(object sender, EventArgs ev)
		{
			mxwWriter.WriteStartDocument(true);
		}

		void mdxrReader_ZacatekTelaDokumentu(object sender, EventArgs ev)
		{
			if (!String.IsNullOrEmpty(mtrTransformace.Koren.Namespace))
				mxwWriter.WriteStartElement(String.Empty, mtrTransformace.Koren.Nazev, mtrTransformace.Koren.Namespace);
			else
				mxwWriter.WriteStartElement(mtrTransformace.Koren.Nazev);

			GenerujAtributy(mtrTransformace.Koren.Atributy, null);
		}

		void mdxrReader_KonecTelaDokumentu(object sender, EventArgs ev)
		{
			mxwWriter.WriteEndElement();
		}

		void mdxrReader_KonecDokumentu(object sender, EventArgs ev)
		{
			mxwWriter.WriteEndDocument();
		}

		#endregion

		void mdxrReader_ZacatekTabulky(object sender, TableEventArgs ev)
		{
			mxwWriter.WriteStartElement(mtrTransformace.Tabulky.Tabulka);
			if (mtrTransformace.Tabulky.CislovatElementy)
				mxwWriter.WriteAttributeString("n", ev.Sequence.ToString());
		}

		void mdxrReader_KonecTabulky(object sender, TableEventArgs ev)
		{
			mxwWriter.WriteEndElement();
		}

		void mdxrReader_ZacatekRadku(object sender, TableRowEventArgs ev)
		{
			mxwWriter.WriteStartElement(mtrTransformace.Tabulky.Radek);
			if (mtrTransformace.Tabulky.CislovatElementy)
				mxwWriter.WriteAttributeString("n", ev.Sequence.ToString());
		}

		void mdxrReader_KonecRadku(object sender, TableRowEventArgs ev)
		{
			mxwWriter.WriteEndElement();
		}

		void mdxrReader_ZacatekBunky(object sender, TableCellEventArgs ev)
		{
			mxwWriter.WriteStartElement(mtrTransformace.Tabulky.Bunka);
			if (mtrTransformace.Tabulky.CislovatElementy)
				mxwWriter.WriteAttributeString("n", ev.Sequence.ToString());
		}

		void mdxrReader_KonecBunky(object sender, TableCellEventArgs ev)
		{
			//zkontrolovat, jestli buňka obsahuje nějaký text
			//nejspíš pomocí StringBuilderu, který bude shomažďovat text podřízených elementů v buňce
			//stačí boolean? (null == nejsme v buňce)
			mxwWriter.WriteEndElement();
		}




		void mdxrReader_Pismo(object sender, FontEventArgs ev)
		{
			//throw new NotImplementedException();
		}


		void mdxrReader_ZnakovyStyl(object sender, TextRunEventArgs ev)
		{
			if (mtgAktualniOdstavcovyTag != null)

				if (mblnIgnorovat || mtgAktualniOdstavcovyTag.PrazdnyElement)
					return;
			//Tag tg = null;

			//Zjišťuje se identifikátor tagu na základě jména stylu
			string si = Tag.GetIdentifikator(ev.Style.UIName, null);
			if (!mtrTransformace.Tagy.ContainsID(si))
			{
				//pokud identifikace pomocí jména stylu není možná, zkouší se, jestli je definován tag pomocí hvězdičky
				si = Tag.GetIdentifikator(csHvezdicka, null);
			}

			if (mtreaAktualniTextRun == null)
			{
				if (mtrTransformace.Tagy.ContainsID(si))
				{
					mtgAktualniZnakovyTag = mtrTransformace.Tagy.GetTagByID(si);
				}
				mtreaAktualniTextRun = ev;
				return;
			}

			//úpravy - začátek
			if (mtrTransformace.Tagy.ContainsID(si))
			{
				Tag tg = mtrTransformace.Tagy.GetTagByID(si);
				if (tg.SloucitSPredchozim)
				{
					if (tg.BezZnacky)
					{
						mtreaAktualniTextRun.Text += ev.Text;
					}
					else
					{
						//GenerujTag nefunguje
						//GenerujTag(tg, true, ev.Text);
					}
					return;
				}
			}
			//if (mtgAktualniZnakovyTag.SloucitSPredchozim) {
			// mtreaAktualniTextRun.Text += ev.Text;
			//}
			//úpravy - konec
			if (mtreaAktualniTextRun.Style == ev.Style || mtgAktualniZnakovyTag.SloucitSNasledujicim)
			{
				if (mtreaAktualniTextRun.Style.UIName != ev.Style.UIName)
				{
					string sId = Tag.GetIdentifikator(ev.Style.UIName, null);

					if (!mtrTransformace.Tagy.ContainsID(sId))
						sId = Tag.GetIdentifikator(csHvezdicka, null);

					if (mtrTransformace.Tagy.ContainsID(sId))
						mtgAktualniZnakovyTag = mtrTransformace.Tagy.GetTagByID(sId);
				}

				mtreaAktualniTextRun.Text += ev.Text;
			}
			else
			{
				VypsatTagProAktualniZnakovyStyl();

				string sId = Tag.GetIdentifikator(ev.Style.UIName, null);
				if (!mtrTransformace.Tagy.ContainsID(sId))
					sId = Tag.GetIdentifikator(csHvezdicka, null);

				if (mtrTransformace.Tagy.ContainsID(sId))
					mtgAktualniZnakovyTag = mtrTransformace.Tagy.GetTagByID(sId);

				mtreaAktualniTextRun = ev;
				mstrPredchoziZnakovyStyl = mtreaAktualniTextRun.Style.UIName;
			}
			//mxwWriter.WriteStartElement("text");
			/*
			if(ev != null)
				mxwWriter.WriteString(ev.Text);
			mxwWriter.WriteEndElement();
			*/
		}

		private void VypsatTagProAktualniZnakovyStyl()
		{
			if (mtreaAktualniTextRun == null)
				return;
			Tag tg;
			string sId = Tag.GetIdentifikator(mtreaAktualniTextRun.Style.UIName, mstrAktualniOdstavcovyStyl);
			if (mtrTransformace.Tagy.ContainsID(sId))
			{
				tg = mtrTransformace.Tagy.GetTagByID(sId);
			}
			else
			{
				sId = Tag.GetIdentifikator(csHvezdicka, mstrPredchoziZnakovyStyl);
				if (sId != csHvezdicka && mtrTransformace.Tagy.ContainsID(sId))
				{
					tg = mtrTransformace.Tagy.GetTagByID(sId);
				}
				else
				{
					sId = Tag.GetIdentifikator(mtreaAktualniTextRun.Style.UIName, mstrPredchoziZnakovyStyl);
					if (mtrTransformace.Tagy.ContainsID(sId))
					{
						tg = mtrTransformace.Tagy.GetTagByID(sId);
					}
					else
					{
						sId = Tag.GetIdentifikator(csHvezdicka, mstrPredchoziZnakovyStyl);
						if (mtrTransformace.Tagy.ContainsID(sId))
						{
							tg = mtrTransformace.Tagy.GetTagByID(sId);
						}
						else
						{
							sId = Tag.GetIdentifikator(mtreaAktualniTextRun.Style.UIName, null);
							if (!mtrTransformace.Tagy.ContainsID(sId))
							{
								sId = Tag.GetIdentifikator(csHvezdicka, null);
							}
							tg = mtrTransformace.Tagy.GetTagByID(sId);
						}
					}
				}
			}

			tg.TypTagu = TypTagu.Znak;
			GenerujTag(tg, mtreaAktualniTextRun.Style.UIName, true, mtreaAktualniTextRun.Text);
		}

		void mdxrReader_KonecOdstavce(object sender, ParagraphEventArgs ev)
		{
			if (mblnIgnorovat)
			{
				if (ev.Style.UIName == mstrIgnorovanyOdstavcovyStyl)
				{
					mstrIgnorovanyOdstavcovyStyl = null;
					mblnIgnorovat = false;
				}
			}
			else
			{
				//mstrPredchoziOdstavcovyStyl = ev.StyleID;
				//jak zjistit, který tag se používá?
				VypsatTagProAktualniZnakovyStyl();
				//TODO Proč je někdy mtgAktualniOdstavcovyTag null?
				if (mtgAktualniOdstavcovyTag != null)
					if (!mtgAktualniOdstavcovyTag.BezZnacky)
						mxwWriter.WriteEndElement();
			}
			//TODO Zjistit, proč někdy konec odstavce neobsahuje žádný styl; v textu predcházel prvek w:pict
			if (ev.Style != null)
			{
				mstrPredchoziOdstavcovyStyl = ev.Style.UIName;
			}
			mtgPredchoziZnakovyTag = null;
		}

		void mdxrReader_ZacatekOdstavce(object sender, ParagraphEventArgs ev)
		{
			//mtrTransformace.tagy[2];
			//je potřeba vygenerovat tag: zjistit IDStylu, na jeho základě název,
			// najít název v Transformacích
			mstrPredchoziZnakovyStyl = null;
			mtreaAktualniTextRun = null;
			mtgAktualniZnakovyTag = null;

			string sID = Tag.GetIdentifikator(ev.Style.UIName, mstrPredchoziOdstavcovyStyl);
			if (mtrTransformace.Tagy.ContainsID(sID))
				mtgAktualniOdstavcovyTag = mtrTransformace.Tagy.GetTagByID(sID);
			else
			{
				sID = Tag.GetIdentifikator(ev.Style.UIName, null);
				mtgAktualniOdstavcovyTag = mtrTransformace.Tagy.GetTagByID(sID);
			}
			mstrAktualniOdstavcovyStyl = ev.Style.UIName;

			if (mtgAktualniOdstavcovyTag == null && mtrTransformace.Tagy.ContainsID(csHvezdicka))
			{
				string sId = Tag.GetIdentifikator(csHvezdicka, null);

				if (mtrTransformace.Tagy.ContainsID(sId))
				{
					mtgAktualniOdstavcovyTag = mtrTransformace.Tagy.GetTagByID(sId);
				}
			}

			if (mtgAktualniOdstavcovyTag == null) return;

			mtgAktualniOdstavcovyTag.TypTagu = TypTagu.Odstavec;
			if (mtgAktualniOdstavcovyTag.Ignorovat)
			{
				mblnIgnorovat = true;
				mstrIgnorovanyOdstavcovyStyl = ev.Style.UIName;
			}
			else
			{
				GenerujTag(mtgAktualniOdstavcovyTag, mstrAktualniOdstavcovyStyl, false);
			}
			//mxwWriter.WriteStartElement("p");
		}

		private void GenerujTag(Tag tgTag, string strNazevStylu, bool bUkoncit)
		{
			GenerujTag(tgTag, strNazevStylu, bUkoncit, null);
		}

		private void GenerujTag(Tag tgTag, string strNazevStylu, bool bUkoncit, string sText)
		{
			if (tgTag.Ignorovat)
				return;
			foreach (Nahrada nh in mtrTransformace.Nahrady)
			{
				sText = sText.Replace(nh.Co, nh.Cim);
			}
			if (tgTag.BezZnacky)
			{
				if (!String.IsNullOrEmpty(sText))

					mxwWriter.WriteString(sText);
				return;
			}
			string nazevStyluXml = strNazevStylu;

			string nazevTagu = tgTag.Nazev;
			if (Nastaveni.StyleNamesAsAttributes)
			{
				switch (tgTag.TypTagu)
				{
					case TypTagu.Neurceno:
						break;
					case TypTagu.Odstavec:
						nazevTagu = "paragraph";
						break;
					case TypTagu.Znak:
						nazevTagu = "character";
						break;
					case TypTagu.Tabulka:
						nazevTagu = "table";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				if (nazevTagu == csHvezdicka)
				{
					//TODO vygenerovat název stylu - znakového nebo odstavcového, podle toho, odkud přichází generování
					nazevStyluXml = strNazevStylu;
					string strTag = strNazevStylu.Replace(" ", "_").Replace("(", "_").Replace(")", "_");
					nazevTagu = XmlConvert.EncodeLocalName(strTag);
				}
			}

			mxwWriter.WriteStartElement(nazevTagu);



			if (!String.IsNullOrEmpty(tgTag.Namespace))
				mxwWriter.WriteAttributeString("xmlns", tgTag.Namespace);

			if (Nastaveni.StyleNamesAsAttributes)
			{
				mxwWriter.WriteAttributeString("style", nazevStyluXml);
			}
			GenerujAtributy(tgTag.Atributy, sText);

			if (!String.IsNullOrEmpty(sText) && !tgTag.PrazdnyElement)
				mxwWriter.WriteString(sText);

			if (bUkoncit)
				mxwWriter.WriteEndElement();

			// mobjCitace.AktualizujHodnoty tg.Nazev
		}

		private void GenerujAtributy(Atributy atrs, string sText)
		{
			foreach (Atribut at in atrs)
			{
				string sHodnota = at.Hodnota;
				if (sHodnota == csDelimitator + csObsah)
				{
					sHodnota = sText;
				}
				else
				{
					if (sHodnota.Contains(csDelimitator))
					{
						//sHodnota = ZpracujHodnotu(sText);
						sHodnota = ZpracujHodnotu(sHodnota);
					}
				}
				if (!String.IsNullOrEmpty(sHodnota))
					mxwWriter.WriteAttributeString(at.Nazev, sHodnota.Trim());
			}
		}


		private string ZpracujHodnotu(string sText)
		{
			string sVysledek = null;
			bool bExistuje = false;
			string[] sHodnoty = sText.Split(new char[] { csDelimitator[0] }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in sHodnoty)
			{
				bExistuje = false;
				foreach (Citac ct in mtrTransformace.Citace)
				{
					if (ct.Nazev == s)
					{
						//Text = mvarPrefix & Format$(mvarHodnota, mvarPodoba) & mvarPostfix
						sVysledek += ct.ToString(); //String.Format("{0}{1}{2}", ct.Prefix, String.Format(ct.Format, ct.Hodnota), ct.Postfix);
						bExistuje = true;
						break;
					}
				}
				if (!bExistuje)
					sVysledek += s;
			}
			return sVysledek;
		}


	}
}
