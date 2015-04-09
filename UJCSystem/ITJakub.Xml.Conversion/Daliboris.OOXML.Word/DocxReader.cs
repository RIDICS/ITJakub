using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
//using System.Linq;
using System.Xml;
using Daliboris.OOXML.Pomucky;

namespace Daliboris.OOXML.Word
{
	/// <summary>
	/// Prochází wordovský dokument a generuje události
	/// </summary>
	public class DocxReader
	{

		private enum TypDokumentu
		{
			Dokument,
			Footnotes,
			Relace
		}


		public DocxReaderSettings Settings { get; set; }


		private int mintPribliznyPocetZnaku;

		private string mstrSouborXml;
		private string mstrSouborRelaci;
		private string mstrSouborXmlFootnotes;
		private string mstrSouborStylu;
		private string mstrSouborVlastnosti;
		Structures mstStruktury;

		private Style mstDefaultParagraphStyle;
		private Style mstDefaulCharacterStyle;


		//private Dictionary<string, Style> mgdcStyly = null;
		private Styles mstDefinovaneStyly;
		private Styles mstPouziteStyly;

		public delegate void Progress(object sender, ProgressEventArgs ev);
		public event Progress Prubeh;

		public delegate void Paragraph(object sender, ParagraphEventArgs ev);
		public event Paragraph ZacatekOdstavce;
		public event Paragraph KonecOdstavce;

		public delegate void TextRun(object sender, TextRunEventArgs ev);
		public event TextRun ZnakovyStyl;

		public delegate void Font(object sender, FontEventArgs ev);
		public event Font Pismo;

		public delegate void PrimeFormatovani(object sender, PrimeFormatovaniEventArgs ev);
		public event PrimeFormatovani PrimyFormat;

		public delegate void Document(object sender, EventArgs ev);
		public event Document ZacatekDokumentu;
		public event Document KonecDokumentu;

		public delegate void Body(object sender, EventArgs ev);
		public event Body ZacatekTelaDokumentu;
		public event Body KonecTelaDokumentu;

		public delegate void Table(object sender, TableEventArgs ev);
		public event Table ZacatekTabulky;
		public event Table KonecTabulky;

		public delegate void TableRow(object sender, TableRowEventArgs ev);
		public event TableRow ZacatekRadku;
		public event TableRow KonecRadku;

		public delegate void TableCell(object sender, TableCellEventArgs ev);
		public event TableCell ZacatekBunky;
		public event TableCell KonecBunky;


		public delegate void Footnote(object sender, FootnoteEventArgs ev);
		public event Footnote ZacatekPoznamkyPodCarou;
		public event Footnote KonecPoznamkyPodCarou;

		public delegate void Picture(object sender, PictureEventArgs ev);
		public event Picture Obrazek;

		public DocxReader()
		{
			Settings = new DocxReaderSettings();
		}
		public DocxReader(string souborDocx)
			: this()
		{
			SouborDocx = souborDocx;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public DocxReader(string souborDocx, DocxReaderSettings settings)
		{
			Settings = settings;
			SouborDocx = souborDocx;
		}

		~DocxReader()
		{
			OdstranitSoubor(mstrSouborXml);
			OdstranitSoubor(mstrSouborXmlFootnotes);
			OdstranitSoubor(mstrSouborStylu);
			OdstranitSoubor(mstrSouborVlastnosti);
			OdstranitSoubor(mstrSouborRelaci);
		}

		private void OdstranitSoubor(string sSoubor)
		{
			if (!String.IsNullOrEmpty(sSoubor))
			{
				if (File.Exists(sSoubor))
					try
					{
						File.Delete(sSoubor);
					}
					catch (Exception e)
					{
						string sText = e.Message;
					}
			}
		}

		public string SouborDocx { get; set; }

		public Structures StrukturniPrvky
		{
			get { return mstStruktury; }
		}


		public int PocetZnaku { get { return mstStruktury.Characters; } } //mintPocetZnaku; } }

		//public Dictionary<string, Style> Styly { get { return mgdcStyly; } }
		/// <summary>
		/// Vracé seznam definovaných stylů
		/// </summary>
		public Styles Styles { get { return mstDefinovaneStyly; } }

		/// <summary>
		/// Vrací seznam stylů použitých v dokumentu
		/// </summary>
		public Styles UsedStyles { get { return mstPouziteStyly; } }

		public void Read()
		{
			ExtrahovatDokumentXml(TypDokumentu.Dokument);
			ExtrahovatDokumentXml(TypDokumentu.Footnotes);
			ExtrahovatDokumentXml(TypDokumentu.Relace);
			//CountCharacters();
			ReadDocxDocument();
		}

		private void ExtrahovatDokumentXml(TypDokumentu typDokumentu)
		{
			switch (typDokumentu)
			{
				case TypDokumentu.Dokument:
					if (mstrSouborXml == null)
					{
						mstrSouborXml = GetTempFileName();
						Pomucky.Dokument.ExtrahovatDoSouboru(SouborDocx, mstrSouborXml, true);
					}
					break;
				case TypDokumentu.Footnotes:
					if (mstrSouborXmlFootnotes == null)
					{
						mstrSouborXmlFootnotes = GetTempFileName();

						if (!Pomucky.Dokument.ExtrahovatPoznamkyPodCarouDoSouboru(SouborDocx, mstrSouborXmlFootnotes, true))
						{
							OdstranitSoubor(mstrSouborXmlFootnotes);
							mstrSouborXmlFootnotes = String.Empty;
						}
					}
					break;
				case TypDokumentu.Relace:
					if (mstrSouborRelaci == null)
					{
						mstrSouborRelaci = GetTempFileName();
						Pomucky.Dokument.ExtrahovatRelaceDokumentuDoSouboru(SouborDocx, mstrSouborRelaci, true);
					}
					break;

				default:
					break;
			}
		}

		private static string GetTempFileName()
		{
			return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
		}

		private string SouborXml
		{
			get
			{
				if (mstrSouborXml == null)
					ExtrahovatDokumentXml(TypDokumentu.Dokument);
				return mstrSouborXml;
			}
		}

		private string SouborXmlFootnotes
		{
			get
			{
				if (mstrSouborXmlFootnotes == null)
					ExtrahovatDokumentXml(TypDokumentu.Footnotes);
				return mstrSouborXmlFootnotes;
			}

		}

		public void ApproximateCharactersCount()
		{
			mintPribliznyPocetZnaku = 0;
			mstrSouborVlastnosti = GetTempFileName();
			Pomucky.Dokument.ExtrahovatRozsireneVlastnostiDoSouboru(SouborDocx, mstrSouborVlastnosti);

			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			if (!File.Exists(mstrSouborVlastnosti))
			{
				mintPribliznyPocetZnaku = 0;
				return;
			}
			XmlReader xr = XmlReader.Create(mstrSouborVlastnosti, xrs);
			try
			{
				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element && xr.Name == "Characters")
					{
						mintPribliznyPocetZnaku = xr.ReadElementContentAsInt();
						break;
					}
				}
			}
			catch (Exception)
			{

				mintPribliznyPocetZnaku = 0;
			}
			finally
			{
				xr.Close();
			}
		}

		private void CountCharacters()
		{

			//nespočítat raději znaky z vlastností dokumentu?
			int mintPocetZnaku = 0;

			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(SouborXml, xrs);

			while (xr.Read())
			{
				if (xr.NodeType == XmlNodeType.Element && xr.Name == "w:t")
				{
					string sAktText = xr.ReadElementContentAsString();
					mintPocetZnaku += sAktText.Length;
				}
			}
		}
		public void CountStructures()
		{
			ApproximateCharactersCount();
			ReadUsedStyles();

			Structures str = new Structures();
			str.UsedStyles = mstPouziteStyly;

			SpocitejStruktury(SouborXml, ref str);
			SpocitejStruktury(SouborXmlFootnotes, ref str);

			mintPribliznyPocetZnaku = str.Characters;
			mstStruktury = str;
		}

		private void SpocitejStruktury(string strDokumentXml, ref Structures str)
		{
			if (strDokumentXml == string.Empty)
				return;
			ProgressEventArgs peaProgress = new ProgressEventArgs(mintPribliznyPocetZnaku);
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			using (XmlReader xr = XmlReader.Create(strDokumentXml, xrs))
			{

				#region Procházení dokumentem
				while (xr.Read())
				{
					if (xr.NodeType == XmlNodeType.Element)
					{
						switch (xr.Name)
						{
							case "w:t":
								string sAktText = xr.ReadElementContentAsString();
								str.TextRuns++;
								//mintPocetZnaku += sAktText.Length;
								str.Characters += sAktText.Length;
								FirePrubeh(str, peaProgress);
								break;
							case "w:bookmarkStart":
								str.Bookmarks++;
								break;
							case "w:commentRangeStart":
								str.Comments++;
								break;
							case "w:fldChar":
							case "w:fldSimple":
								//"w:instrText" - představbuje konkrétní realizaci složeného pole
								str.Fields++;
								break;
							case "w:rFonts":
								str.Fonts++;
								break;
							case "w:footerReference":
								str.Footer++;
								break;
							case "w:endnoteReference":
							case "w:footnoteReference":
								str.Footnotes++;
								break;
							case "w:pic":
							case "w:drawing":
								str.Graphics++;
								break;
							case "w:headerReference":
								str.Headers++;
								break;
							case "w:p":
								str.Paragraphs++;
								break;
							case "w:ins":
							case "w:del":
								str.Revisions++;
								break;
							case "w:sectPr":
								str.Sections++;
								break;
							case "w:br":
								//<w:br w:type=”column”/>: column, page, textWrapping (= Line Break)
								string sTyp = xr.GetAttribute("w:type");
								switch (sTyp)
								{
									case "column":
										str.ColumnBreaks++;
										break;
									case "page":
										str.PageBreaks++;
										break;
									case "textWrapping":
										str.SoftLineBreaks++;
										break;
									default:
										str.SoftLineBreaks++;
										break;
								}
								break;
							case "w:tab":
								//asi bude třeba ještě vychytat
								if (xr.AttributeCount == 0)
									str.Tabs++;
								break;
							case "w:tbl":
								str.Tables++;
								break;
							case "w:lang":
								str.Languages++;
								break;
							case "w:noProof":
								string sAtribut = xr.GetAttribute("w:val");
								if (JeAtributValTrue(sAtribut, false))
									str.NoProofs++;
								break;
							case "w:lastRenderedPageBreak":
								str.PrintedPages++;
								break;
							default:
								break;
						}
					}
				}
				#endregion
			}
			FirePrubeh(str, peaProgress);

		}

		private void FirePrubeh(Structures str, ProgressEventArgs peaProgress)
		{
			if (Prubeh != null)
			{
				peaProgress.CurrentChar = str.Characters;
				Prubeh(this, peaProgress);
			}
		}

		public void ReadUsedStyles()
		{
			ReadStyles();
			Styles stNoveStyly = new Styles();

			this.ZacatekOdstavce += delegate(object sender, ParagraphEventArgs ev)
			{
				// if (ev.Style.ID == "Normln")
				//	ev.Style.ID= "Normln";
				if (ev == null)
					ev = new ParagraphEventArgs(new Style() { ID = "Normln" });
				if (mstDefinovaneStyly.ContainsStyleID(ev.Style.ID))
				{
					mstDefinovaneStyly.StyleByID(ev.Style.ID).IsUsed = true;
				}
				else
				{
					if (!stNoveStyly.ContainsStyleID(ev.Style.ID))
					{
						stNoveStyly.Add(ev.Style);
					}
				}
				mstDefinovaneStyly.StyleByID(ev.Style.ID).Count += 1;

				/*
				Style st = mstDefinovaneStyly.StyleByID(ev.StyleID);
				if (!mstPouziteStyly.Contains(st))
						mstPouziteStyly.Add(st);
				*/
			};
			this.ZnakovyStyl += delegate(object sender, TextRunEventArgs ev)
			{
				if (mstDefinovaneStyly.ContainsStyleID(ev.Style.ID))
					mstDefinovaneStyly.StyleByID(ev.Style.ID).IsUsed = true;
				else
				{
					if (!stNoveStyly.ContainsStyleID(ev.Style.ID))
					{
						stNoveStyly.Add(ev.Style);
					}
				}
				mstDefinovaneStyly.StyleByID(ev.Style.ID).Count += 1;
				/*
				Style st = mstDefinovaneStyly.StyleByID(ev.StyleID);
				if (!mstPouziteStyly.Contains(st))
						mstPouziteStyly.Add(st);
				*/
			};
			ReadDocxDocument();
			mstPouziteStyly = new Styles();
			foreach (Style st in mstDefinovaneStyly)
			{
				if (st.IsUsed)
					mstPouziteStyly.Add(st);
			}
			foreach (Style st in stNoveStyly)
			{
				mstPouziteStyly.Add(st);
			}
		}

		public static Styles ReadStylesDefinitionFromFile(string strStylesXml)
		{
			bool blnVychoziStyl = false;
			Styles stDefinovaneStyly = new Styles();
			Dictionary<string, string> gdcPrevodnikStylu = GetPrevodnikStylu();

			Style stStyl = null;
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(strStylesXml, xrs);
			IRunFormatting formatting = null;


			while (xr.Read())
			{
			Zacatek:
				if (xr.NodeType == XmlNodeType.Element)
				{
					switch (xr.Name)
					{
						case "w:style":
							stStyl = new Style();
							string sTyp = xr.GetAttribute("w:type");
							string sId = xr.GetAttribute("w:styleId");
							string sCustom = xr.GetAttribute("w:customStyle");
							string sDefault = xr.GetAttribute("w:default");

							stStyl.IsDefault = JeAtributValTrue(sDefault, true);
							stStyl.IsCustom = JeAtributValTrue(sCustom, true);
							stStyl.ID = sId;

							switch (sTyp)
							{
								case "character":
									stStyl.Type = StyleType.ZnakovyStyl;
									break;
								case "table":
									stStyl.Type = StyleType.TabulkovyStyl;
									break;
								case "numbering":
									stStyl.Type = StyleType.CislovaciStyl;
									break;
								case "paragraph":
								case null:
									stStyl.Type = StyleType.OdstavcovyStyl;
									break;
							}

							break;
						case "w:name":
							if (stStyl != null)
							{
								string sNazev = xr.GetAttribute("w:val");
								if (gdcPrevodnikStylu.ContainsKey(sNazev))
								{
									stStyl.Name = sNazev;
									stStyl.UIName = gdcPrevodnikStylu[sNazev];
								}
								else
								{
									stStyl.UIName = sNazev;
									stStyl.Name = sNazev;
								}
							}
							break;
						case "w:rPrDefault":
							blnVychoziStyl = true;
							break;
						case "w:rPr":
							XmlDocument format = Daliboris.Pomucky.Xml.Objekty.ReadNodeAsXmlDocument(xr);
							formatting = GetRunFormatting(format);
							if (blnVychoziStyl)
							{
								stDefinovaneStyly.DefaultLanguage = formatting.Lang;
							}
							if (stStyl != null)
							{
								stStyl.RunFormatting = formatting;
								stStyl.Language = formatting.Lang;
								stStyl.NoProof = formatting.NoProof;
							}
							goto Zacatek;
						case "w:lang":
							string sJazyk = xr.GetAttribute("w:val");
							if (blnVychoziStyl)
								stDefinovaneStyly.DefaultLanguage = sJazyk;
							else if (stStyl != null) stStyl.Language = sJazyk;
							break;
						case "w:noProof":
							string sVal = xr.GetAttribute("w:val");

							if (stStyl != null) stStyl.NoProof = JeAtributValTrue(sVal, true);
							break;
						case "w:basedOn":
							if (stStyl != null) stStyl.BasenOnStyleID = xr.GetAttribute("w:val");
							break;
						default:
							break;
					}
				}
				else if (xr.NodeType == XmlNodeType.EndElement)
				{
					switch (xr.Name)
					{
						case "w:style":
							if (stStyl != null)
							{
								//mgdcStyly.Add(stStyl.Name, stStyl);
								if (stStyl.Name == null)
									stStyl.Name = "XXXXXX";
								if (stStyl.UIName == null)
									stStyl.UIName = "YYYYYYY";
								stDefinovaneStyly.Add(stStyl);
							}
							stStyl = null;
							break;
						case "w:rPrDefault":
							blnVychoziStyl = false;
							break;
						default:
							break;
					}

				}
			}
			xr.Close();

			foreach (Style st in stDefinovaneStyly)
			{
				if (st.BasenOnStyleID != null)
				{
					st.BasedOnStyle = stDefinovaneStyly.StyleByID(st.BasenOnStyleID);
					if (st.NoProof == null && st.BasedOnStyle.NoProof == true)
						st.NoProof = true;
					if (st.Language == null)
					{
						st.Language = st.BasedOnStyle.Language;
					}
				}
			}

			return stDefinovaneStyly;
		}

		public static IRunFormatting GetRunFormatting(XmlDocument format)
		{
			IRunFormatting formatting = new RunFormatting();
			foreach (XmlNode node in format.DocumentElement.ChildNodes)
			{
				switch (node.Name)
				{
					case "w:b":
						formatting.Bold = JeAtributValTrue(true, node);
						break;
					case "w:color":
						formatting.Color = node.Attributes["val", Dokument.RelWordprocessingRelationshipTypeW].Value;
						break;
					case "w:caps":
						formatting.Caps = JeAtributValTrue(true, node);
						break;

					case "w:rFonts":
						if (node.Attributes["hAnsi", Dokument.RelWordprocessingRelationshipTypeW] != null)
							formatting.FontName = node.Attributes["hAnsi", Dokument.RelWordprocessingRelationshipTypeW].Value;
						break;

					case "w:sz":
						formatting.FontSize = Int32.Parse(node.Attributes["val", Dokument.RelWordprocessingRelationshipTypeW].Value);
						break;
					case "w:i":
						formatting.Italic = JeAtributValTrue(true, node);
						break;

					case "w:lang":
						formatting.Lang = GetAtributeValue(node, "name");
						//if(node.Attributes["val", Daliboris.OOXML.Pomucky.Dokument.RelWordprocessingRelationshipTypeW] != null)
						//	formatting.Lang = node.Attributes["val", Daliboris.OOXML.Pomucky.Dokument.RelWordprocessingRelationshipTypeW].Value;
						break;
					case "w:noProof":
						formatting.NoProof = JeAtributValTrue(false, node);
						//formatting.NoProof = JeAtributValTrue(node.Attributes["val", Daliboris.OOXML.Pomucky.Dokument.RelWordprocessingRelationshipTypeW].Value, true);
						break;

					case "w:smallCaps":
						formatting.SmallCaps = JeAtributValTrue(true, node);
						break;
					case "w:u":
						formatting.Underline = node.Attributes["val", Daliboris.OOXML.Pomucky.Dokument.RelWordprocessingRelationshipTypeW].Value;

						break;
					case "w:verticalAlign":
						formatting.VerticalAlign = node.Attributes["val", Daliboris.OOXML.Pomucky.Dokument.RelWordprocessingRelationshipTypeW].Value;
						break;

					default:
						break;
				}
			}
			return formatting;
		}

		private static string GetAtributeValue(XmlNode node, string name)
		{
			if (node.Attributes.Count == 0)
				return null;

			for (int i = 0; i < node.Attributes.Count; i++)
			{
				if (node.Attributes[i].Name == name)
					return node.Attributes[i].Value;
			}
			return null;
		}

		private static bool JeAtributValTrue(bool nullJeFalse, XmlNode node)
		{
			if (node.Attributes.Count == 0)
				return JeAtributValTrue(null, nullJeFalse);

			for (int i = 0; i < node.Attributes.Count; i++)
			{
				if (node.Attributes[i].Name == "val")
					return JeAtributValTrue(node.Attributes[i].Value, nullJeFalse);
			}
			return nullJeFalse;
		}

		public void ReadStyles()
		{

			// je potřeba provést přejmenování základních stylů na národní varianty
			// name | ID = český název
			// typu Normal | Normln = Normální
			// Default Paragraph Font | Standardnpsmoodstavce = Standardní písmo odstavce
			// Normal Table | Normlntabulka = Normální tabulka
			// No List | Bezseznamu = Bez seznamu
			// heading 1 | Nadpis1 = Nadpis 1
			// Title | Nzev = Název

			//Nastaveno na true, pokud se čtečka nachází v oblasti výchozího stylu


			if (mstrSouborStylu == null)
				mstrSouborStylu = GetTempFileName();

			//mgdcStyly = new Dictionary<string, Style>();

			Pomucky.Dokument.ExtrahovatStylyDoSouboru(SouborDocx, mstrSouborStylu);
			mstDefinovaneStyly = ReadStylesDefinitionFromFile(mstrSouborStylu);

		}

		private static Dictionary<string, string> GetPrevodnikStylu()
		{
			Dictionary<string, string> gdcPrevodnikStylu = new Dictionary<string, string>();
			gdcPrevodnikStylu.Add("Default Paragraph Font", "Standardní písmo odstavce");
			gdcPrevodnikStylu.Add("Normal", "Normální");
			gdcPrevodnikStylu.Add("Normal Table", "Normální tabulka");
			gdcPrevodnikStylu.Add("No List", "Bez seznamu");
			gdcPrevodnikStylu.Add("Title", "Název");
			for (int i = 1; i < 10; i++)
			{
				gdcPrevodnikStylu.Add("heading " + i, "Nadpis " + i);
				gdcPrevodnikStylu.Add("toc " + i, "Obsah " + i);
			}
			return gdcPrevodnikStylu;
		}

		/// <summary>
		/// Vrací údaj o tom, jestli je atribut val platný, nebo ne.
		/// </summary>
		/// <param name="sHodnotaAtributu">Hodnota atributu val.</param>
		/// <param name="nullJeFalse">Jestli hodnota null znamená neplatnost.</param>
		/// <returns>True, pokud hodnota odpovídá pravě, jinak vrací false.</returns>
		/// <remarks>Je-li hodnota atributu val rovna <value>1</value>, <value>on</value> nebo <value>true</value>, pak je pravdivý, jinak je nepravdivý.</remarks>
		private static bool JeAtributValTrue(string sHodnotaAtributu, bool nullJeFalse)
		{
			if (nullJeFalse && sHodnotaAtributu == null)
			{
				return false;
			}
			return (sHodnotaAtributu == null || sHodnotaAtributu == "1" || sHodnotaAtributu == "on" || sHodnotaAtributu == "true");
		}




		private void ReadDocxDocument()
		{

			//if (mgdcStyly == null)
			if (mstDefinovaneStyly == null)
				ReadStyles();


			mstDefaultParagraphStyle = mstDefinovaneStyly.DefaultStyle(StyleType.OdstavcovyStyl);
			mstDefaulCharacterStyle = mstDefinovaneStyly.DefaultStyle(StyleType.ZnakovyStyl);
			//string strDefaultParagraphStyleID = mstDefinovaneStyly.DefaultStyle(StyleType.OdstavcovyStyl).ID;
			//string strDefaulCharacterStyleID = mstDefinovaneStyly.DefaultStyle(StyleType.ZnakovyStyl).ID;


			mstStruktury = new Structures();

			if (mintPribliznyPocetZnaku == 0)
			{
				ApproximateCharactersCount();
			}

			//string sAktualniZnakovyStyl = null;
			if (ZacatekDokumentu != null)
				ZacatekDokumentu(this, new EventArgs());

			CtiDokumentXml(mstDefaultParagraphStyle, mstDefaulCharacterStyle, SouborXml, null);
			//CtiDokumentXml(mstDefaultParagraphStyle, mstDefaulCharacterStyle, SouborXmlFootnotes);

			if (KonecDokumentu != null)
				KonecDokumentu(this, new EventArgs());

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stDefaultParagraphStyle">Definice výchozího odstavcového stylu</param>
		/// <param name="stDefaulCharacterStyle">Definice výchozího znakového stylu</param>
		/// <param name="strSouborXml">Soubor XML, který se má zpracovat</param>
		/// <param name="strIdentifikator">Identifikátor poznámky pod čarou</param>
		private void CtiDokumentXml(Style stDefaultParagraphStyle, Style stDefaulCharacterStyle, string strSouborXml, string strIdentifikator)
		{
			bool blnProbehlaInformaceOOdstavcovemStylu = false;
			string sAktualniJazyk = null;

			//Seznam identifikátorů poznámek pod čarou, které se objevily v aktuálním odstavci
			List<string> poznamkyPodCarouAktualnihoOdstavce = null;


			FootnoteEventArgs feaFootnote = null;
			Queue<ParagraphEventArgs> mgqPeaParagraph = new Queue<ParagraphEventArgs>(2);
			ParagraphEventArgs peaParagraph = null;
			TextRunEventArgs traTextRun = null;
			TableEventArgs teaTable = null;
			TableRowEventArgs traTableRow = null;
			TableCellEventArgs tcaTableCell = null;
			FontEventArgs feaFont;
			ProgressEventArgs peaProgress = null;
			//PrimeFormatovaniEventArgs pfaPrimyFormat = null;
			int iAktualniTabulka = 0;
			int iAktualniPoznamkaPodCarou = 0;
			int iAktualniObrazek = 0;
			peaProgress = new ProgressEventArgs(mintPribliznyPocetZnaku);
			bool blnZpracovat = (strIdentifikator == null);
			string sAktualniText = null;
			List<string> glsFormatovani = new List<string>();

			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			using (XmlReader xr = XmlReader.Create(strSouborXml, xrs))
			{

				#region Procházení dokumentem

				while (xr.Read())
				{
				Zacatek_prochazeni:
					if (xr.NodeType == XmlNodeType.Element)
					{
						#region NazevElementu

						string sAtribut = null;
						string elementName = xr.Name;
						switch (elementName)
						{
							//case "w:pPrChange":
							//je třeba se posunout jinam! zaznamenání změny stylu => objeví se dva začátky stylu po sobě
							case "w:r":
								if (!blnZpracovat) break;
								if (!blnProbehlaInformaceOOdstavcovemStylu)
								{
									if (ZacatekOdstavce != null)
									{
										if (peaParagraph == null)
											sAktualniText = sAktualniText;
										else
										{
											ZacatekOdstavce(this, peaParagraph);
											blnProbehlaInformaceOOdstavcovemStylu = true;
										}
									}

								}
								break;
							case "w:p":
								if (!blnZpracovat) break;
								poznamkyPodCarouAktualnihoOdstavce = new List<string>();
								//co když je odstavec v odstavci, resp. v odstavci je obrázek a k němu přiřazené pole s odstavcem
								blnProbehlaInformaceOOdstavcovemStylu = false;
								peaParagraph = new ParagraphEventArgs(stDefaultParagraphStyle, ++mstStruktury.Paragraphs);

								if(xr.IsEmptyElement)
								{
									if(ZacatekOdstavce != null)
										ZacatekOdstavce(this, peaParagraph); 
									
									if (KonecOdstavce != null)
										KonecOdstavce(this, peaParagraph);
									peaParagraph = null;
								}
								break;
							case "w:pStyle":
								if (!blnZpracovat) break;
								string sStyl = xr.GetAttribute("w:val");
								if (mstDefinovaneStyly.ContainsStyleID(sStyl))
									peaParagraph.Style = mstDefinovaneStyly.StyleByID(sStyl);
								else
								{
									peaParagraph.Style = new Style { ID = sStyl, IsUsed = true, Name = sStyl, Type = StyleType.OdstavcovyStyl };
								}
								if (ZacatekOdstavce != null)
								{
									ZacatekOdstavce(this, peaParagraph);
								}
								blnProbehlaInformaceOOdstavcovemStylu = true;
								break;
							case "w:rStyle":
								if (!blnZpracovat) break;
								string sAktStyl = xr.GetAttribute("w:val");
								traTextRun = new TextRunEventArgs();
								traTextRun.Sequence = ++mstStruktury.TextRuns;
								if (mstDefinovaneStyly.ContainsStyleID(sAktStyl))
									traTextRun.Style = mstDefinovaneStyly.StyleByID(sAktStyl);
								else
								{
									traTextRun.Style = new Style { ID = sAktStyl, IsUsed = true, Name = sAktStyl, Type = StyleType.ZnakovyStyl };
								}
								traTextRun.Language = sAktualniJazyk;
								/*
								traTextRun.StyleID = sAktStyl;
								traTextRun.StyleName = mstDefinovaneStyly.StyleByID(sAktStyl).Name;
								traTextRun.UIStyleName = mstDefinovaneStyly.StyleByID(sAktStyl).UIName;
								*/

								//if (mgdcStyly.ContainsKey(sAktStyl))
								//   traTextRun.StyleName = mgdcStyly[sAktStyl].Name;
								break;
							case "w:rFonts":
								if (!blnZpracovat) break;
								//udělat lepší přehled nastavených fontů
								string sAktFont = xr.GetAttribute(0);
								if (Pismo != null)
								{
									feaFont = new FontEventArgs();
									feaFont.FontName = sAktFont;
								}
								break;

							case "w:t":
								if (!blnZpracovat) break;
								sAktualniText = xr.ReadElementContentAsString();
								//if (sAktualniText == "Klepý ")
								//	sAktualniText = sAktualniText;
								//používá se Standardní písmo odstavce
								if (traTextRun == null)
								{
									traTextRun = new TextRunEventArgs();
									traTextRun.Style = stDefaulCharacterStyle;
									//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
									//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
									//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
								}
								traTextRun.Language = sAktualniJazyk;
								traTextRun.Text += sAktualniText;
								mstStruktury.Characters += sAktualniText.Length;
								FirePrubeh(mstStruktury, peaProgress);
								if (PrimyFormat != null && glsFormatovani.Count > 0)
								{
									PrimeFormatovaniEventArgs pfaPrimyFormat = new PrimeFormatovaniEventArgs(String.Join(", ", glsFormatovani.ToArray()),
											sAktualniText, traTextRun.Style.UIName, mstStruktury.TextRuns);
									PrimyFormat(this, pfaPrimyFormat);
									glsFormatovani.Clear();
								}
								//vypočítat procento zpracování na základě délky zpracovaného textu
								break;
							case "w:tab":
								if (!blnZpracovat) break;
								//dodělat zpracování textu - rozlišit od definice/nastavení tabulátorů
								if (xr.AttributeCount == 0)
								{
									string sAktTab = "\t";
									//používá se Standardní písmo odstavce
									if (traTextRun == null)
									{
										traTextRun = new TextRunEventArgs();
										traTextRun.Style = stDefaulCharacterStyle;
										/*
										traTextRun.StyleID = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).ID;
										traTextRun.StyleName = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).Name;
										traTextRun.UIStyleName = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).UIName;
										*/
										//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
										//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
										//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
									}
									traTextRun.Language = sAktualniJazyk;
									traTextRun.Text += sAktTab;
									mstStruktury.Characters += sAktTab.Length;
									FirePrubeh(mstStruktury, peaProgress);

									//vypočítat procento zpracování na základě délky zpracovaného textu
								}
								break;
							case "w:tbl":
								if (!blnZpracovat) break;
								teaTable = new TableEventArgs();
								teaTable.Sequence = ++iAktualniTabulka; //++mstStruktury.Table;
								mstStruktury.TableRows = 0;
								mstStruktury.TableCells = 0;
								if (ZacatekTabulky != null)
									ZacatekTabulky(this, teaTable);
								break;
							case "w:tr":
								if (!blnZpracovat) break;
								traTableRow = new TableRowEventArgs();
								traTableRow.Sequence = ++mstStruktury.TableRows;
								mstStruktury.TableCells = 0;
								if (ZacatekRadku != null)
									ZacatekRadku(this, traTableRow);
								break;
							case "w:tc":
								if (!blnZpracovat) break;
								tcaTableCell = new TableCellEventArgs();
								tcaTableCell.Sequence = ++mstStruktury.TableCells;
								if (ZacatekBunky != null)
									ZacatekBunky(this, tcaTableCell);
								break;
							case "w:body":
								if (!blnZpracovat) break;
								if (ZacatekTelaDokumentu != null)
									ZacatekTelaDokumentu(this, new EventArgs());
								break;
							case "w:lang":
								if (!blnZpracovat) break;
								sAktualniJazyk = xr.GetAttribute("w:val");
								mstStruktury.Languages++;
								break;
							case "w:noProof":
								if (!blnZpracovat) break;
								sAtribut = xr.GetAttribute("w:val");
								if (JeAtributValTrue(sAtribut, true))
									mstStruktury.NoProofs++;
								break;
							case "w:footnote":
								string sIdFootnote = xr.GetAttribute("w:id");
								if (sIdFootnote == strIdentifikator)
									blnZpracovat = true;
								if (!blnZpracovat)
								{
									xr.Skip();
									goto Zacatek_prochazeni;

								}
								sAtribut = xr.GetAttribute("w:type"); //co to znamená?
								// může být typu separator, continuationSeparator, continuationNotice nebo normal, resp. null
								//pouze typ normal obsahuje text
								if (sAtribut == null || sAtribut == "normal")
								{
									if (ZacatekPoznamkyPodCarou != null && !Settings.SkipFootnotesAndEndnotesContent)
									{
										feaFootnote = new FootnoteEventArgs(++iAktualniPoznamkaPodCarou, sIdFootnote);
										ZacatekPoznamkyPodCarou(this, feaFootnote);
									}
								}

								break;

							case "w:footnoteRef": //objevuje se jako odkaz na číslo poznámky v samotné poznámce pod čarou
								if (traTextRun == null)
								{
									traTextRun = new TextRunEventArgs();
									traTextRun.Style = stDefaulCharacterStyle;
									//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
									//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
									//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
								}
								traTextRun.Language = sAktualniJazyk;
								//TODO Dodělat správné číslo poznámky pod čarou
								sAktualniText = iAktualniPoznamkaPodCarou.ToString(CultureInfo.InvariantCulture);
								traTextRun.Text += sAktualniText;
								mstStruktury.Characters += sAktualniText.Length;
								FirePrubeh(mstStruktury, peaProgress);
								break;


							case "w:endnoteReference":
							case "w:footnoteReference":
								if (!blnZpracovat) break;
								string strId = xr.GetAttribute("w:id");
								//FootnoteEventArgs feaFootnote = new FootnoteEventArgs(++iAktualniPoznamkaPodCarou, strId);

								if (!Settings.SkipFootnotesAndEndnotesContent)
									poznamkyPodCarouAktualnihoOdstavce.Add(strId);

								if (traTextRun == null)
								{
									traTextRun = new TextRunEventArgs();
									traTextRun.Style = stDefaulCharacterStyle;
									//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
									//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
									//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
								}
								traTextRun.Language = sAktualniJazyk;
								
								//Pokud je označeno atributem, obsahuje text udávající hodnotu čísla poznámky pod čarou
								if (JeAtributValTrue(xr.GetAttribute("w:customMarkFollows"), true))
									traTextRun.Text = null;
								else
								{
									sAktualniText = (++iAktualniPoznamkaPodCarou).ToString(CultureInfo.InvariantCulture);
									traTextRun.Text = sAktualniText;
								}
								mstStruktury.Characters += sAktualniText.Length;
								FirePrubeh(mstStruktury, peaProgress);
								//ZpracovatKonecTextovehoUseku(traTextRun, mstDefaulCharacterStyle);

								//if (ZacatekPoznamkyPodCarou != null)
								//{
								//	if (Settings.SkipFootnotesAndEndnotesContent)
								//	{
								//		ZacatekPoznamkyPodCarou(this, feaFootnote);
								//		break;
								//	}

								//	if (traTextRun == null)
								//	{
								//		traTextRun = new TextRunEventArgs();
								//		traTextRun.Style = stDefaulCharacterStyle;
								//		//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
								//		//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
								//		//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
								//	}
								//	traTextRun.Language = sAktualniJazyk;
								//	traTextRun.Text += sAktualniText;
								//	mstStruktury.Characters += sAktualniText.Length;
								//	if (Prubeh != null)
								//	{
								//		peaProgress.CurrentChar = mstStruktury.Characters;
								//		Prubeh(this, peaProgress);
								//	}
								//	ZpracovatKonecTextovehoUseku(traTextRun, mstDefaulCharacterStyle);

								//	ZacatekPoznamkyPodCarou(this, feaFootnote);


								//	//je potřeba ukončit předchozí textový úsek
								//	ZpracovatKonecTextovehoUseku(traTextRun, stDefaulCharacterStyle);

								//	ZpracujPoznamkuPodCarou(strId);

								//	traTextRun = null;

								//	//zpracovat poznámku pod čarou => otevřít dokument, najít poznámku podle Id
								//	if (KonecPoznamkyPodCarou != null)
								//	{
								//		KonecPoznamkyPodCarou(this, feaFootnote);
								//	}
								//}
								break;
							case "w:pPrChange":
							case "w:rPrChange":
								if (!blnZpracovat) break;
								//je třeba se posunout jinam! zaznamenání změny stylu => objeví se dva začátky stylu po sobě
								string sNazev = xr.Name;
								while (!(xr.NodeType == XmlNodeType.EndElement && xr.Name == sNazev))
								{
									xr.Read();
								}
								break;
							case "w:b": //Bold
							case "w:bCs": //Complex String Bold
							case "w:bdr": //Text Border 
							case "w:caps": //Display All Characters As Capital Letters 
							case "w:color": //Run Content Color 
							case "w:cs": //Use Complex Script Formatting on Run 
							case "w:del": //Deleted Paragraph 
							case "w:dstrike": //Double Strikethrough 
							case "w:eastAsianLayout": //East Asian Typography Settings 
							case "w:effect": //Animated Text Effect 
							case "w:em": //Emphasis Mark 
							case "w:emboss": //Embossing 
							case "w:fitText": //Manual Run Width 
							case "w:highlight": //Text Highlighting 
							case "w:i": //Italics 
							case "w:iCs": //Complex Script Italics 
							case "w:imprint": //Imprinting 
							case "w:ins": //Inserted Paragraph 
							case "w:kern": //Font Kerning 
							//case "w:lang": //Languages for Run Content 
							case "w:moveFrom": //Move Source Paragraph 
							case "w:moveTo": //Move Destination Paragraph 
							//case "w:noProof": //Do Not Check Spelling or Grammar 
							case "w:oMath": //Office Open XML Math 
							case "w:outline": //Display Character Outline 
							case "w:position": //Vertically Raised or Lowered Text 
							//case "w:rFonts": //Run Fonts 
							//case "w:rPrChange": //Revision Information for Run Properties on the Paragraph Mark 
							//case "w:rStyle": //Referenced Character Style 
							case "w:rtl": //Right To Left Text 
							case "w:shadow": //Shadow 
							case "w:shd": //Run Shading 
							case "w:smallCaps": //Small Caps 
							case "w:snapToGrid": //Use Document Grid Settings For Inter-Character Spacing 
							case "w:spacing": //Character Spacing Adjustment 
							case "w:specVanish": //Paragraph Mark Is Always Hidden 
							case "w:strike": //Single Strikethrough 
							case "w:sz": //Font Size 
							case "w:szCs": //Complex Script Font Size 
							case "w:u": //Underline 
							case "w:vanish": //Hidden Text 
							case "w:vertAlign": //Subscript/Superscript Text 
							case "w:w": //Expanded/Compressed Text 
							case "w:webHidden": //Web Hidden Text 
								if (!blnZpracovat) break;
								if (PrimyFormat != null)
								{
									string sVypnuto = xr.GetAttribute("w:val");
									if (JeAtributValTrue(sVypnuto, false))
										glsFormatovani.Add(xr.Name);
									else
										glsFormatovani.Add("non-" + xr.Name);
								}
								break;

							case "v:imagedata":
							case "w:drawing":
							case "w:pict":
							if (!blnZpracovat) break;

								PictureEventArgs pictureEventArgs = GetImageData(++iAktualniObrazek, xr);

								/*
								string strIdImg = xr.GetAttribute("r:id");
								PictureEventArgs pictureEventArgs = new PictureEventArgs(++iAktualniObrazek, strIdImg);
								 */
								if (Obrazek != null)
									Obrazek(this, pictureEventArgs);
								break;
							//TODO Vymyslet způsob zpracování obrázků (kdy je přeskočit a kdy ne)
							//case "w:drawing":
							//	if (!blnZpracovat)
							//	{
							//		xr.Skip();
							//		break;
							//	}
							//	PictureEventArgs pictureEventArgs2 = GetImageData(iAktualniObrazek, xr);
							//	break;
								/*
							case "w:pict":
								xr.Skip();
								break;
								*/
							case "w:sym":
								if (!blnZpracovat) break;
								string hexadecimal = xr.GetAttribute("w:char");
								string font = xr.GetAttribute("w:font");
								//TODO Co když bude font jiný než aktuální styl?
								sAktualniText = ConvertCharacterFromHexadecimalToString(hexadecimal);
								//if (sAktualniText == "Klepý ")
								//	sAktualniText = sAktualniText;
								//používá se Standardní písmo odstavce
								if (traTextRun == null)
								{
									traTextRun = new TextRunEventArgs();
									traTextRun.Style = stDefaulCharacterStyle;
									//nemá být traTextRun.StyleName?// peaParagraph.StyleName = mstStyly.StyleByID(traTextRun.StyleID).Name;
									//if (mgdcStyly.ContainsKey(traTextRun.StyleID))
									//   peaParagraph.StyleName = mgdcStyly[traTextRun.StyleID].Name;
								}
								traTextRun.Language = sAktualniJazyk;
								traTextRun.Text += sAktualniText;
								mstStruktury.Characters += sAktualniText.Length;
								FirePrubeh(mstStruktury, peaProgress);
								if (PrimyFormat != null && glsFormatovani.Count > 0)
								{
									PrimeFormatovaniEventArgs pfaPrimyFormat = new PrimeFormatovaniEventArgs(String.Join(", ", glsFormatovani.ToArray()),
											sAktualniText, traTextRun.Style.UIName, mstStruktury.TextRuns);
									PrimyFormat(this, pfaPrimyFormat);
									glsFormatovani.Clear();
								}
								//vypočítat procento zpracování na základě délky zpracovaného textu
								break;

							default:
								break;
						}

						#endregion
					}

					else if (xr.NodeType == XmlNodeType.EndElement)
					{
						#region NazevElementu

						switch (xr.Name)
						{
							case "w:p":
								if (!blnZpracovat) break;
								//mjvZnakoveStyly.Append(sAktualniZnakovyStyl ?? csStandardniPismoOdstavce);
								//co když je odstavec v odstavci, resp. v odstavci je obrázek a k němu přiřazené pole s odstavcem

								if (!blnProbehlaInformaceOOdstavcovemStylu)
								{
									if (ZacatekOdstavce != null)
									{
										ZacatekOdstavce(this, peaParagraph);
									}
								}
								if (KonecOdstavce != null)
									KonecOdstavce(this, peaParagraph);
								peaParagraph = null;

								foreach (string poznamkaPodCarou in poznamkyPodCarouAktualnihoOdstavce)
								{
									ZpracujPoznamkuPodCarou(poznamkaPodCarou);
								}

								break;
							case "w:r":
								if (!blnZpracovat) break;
								ZpracovatKonecTextovehoUseku(traTextRun, stDefaulCharacterStyle);
								sAktualniJazyk = null;
								traTextRun = null;
								break;
							case "w:tbl":
								if (!blnZpracovat) break;
								if (KonecTabulky != null)
									KonecTabulky(this, teaTable);
								teaTable = null;
								break;
							case "w:tr":
								if (!blnZpracovat) break;
								if (KonecRadku != null)
									KonecRadku(this, traTableRow);
								traTableRow = null;
								break;
							case "w:tc":
								if (!blnZpracovat) break;
								if (KonecBunky != null)
									KonecBunky(this, tcaTableCell);
								tcaTableCell = null;
								traTextRun = null;
								peaParagraph = null;
								break;
							case "w:footnote":
								//Console.WriteLine("End; w:footnote - {0} {1}", strIdentifikator, blnZpracovat);
								if (KonecPoznamkyPodCarou != null)
								{
									KonecPoznamkyPodCarou(this, feaFootnote);
									feaFootnote = null;
								}
								if (strIdentifikator != null && blnZpracovat)
									blnZpracovat = false;
								break;
							case "w:body":
								if (!blnZpracovat) break;
								if (KonecTelaDokumentu != null)
									KonecTelaDokumentu(this, new EventArgs());
								break;
							default:
								break;
						}

						#endregion
					}

				}
				#endregion
			}
		}

		private static string ConvertCharacterFromHexadecimalToString(string hexadecimal)
		{
			int value = Convert.ToInt32(hexadecimal, 16);
			string stringValue = Char.ConvertFromUtf32(value);
			return stringValue;
		}
		private PictureEventArgs GetImageData(int aktualniObrazek, XmlReader xr)
		{
			PictureEventArgs args = new PictureEventArgs(aktualniObrazek, null);
			string identifikator = null;

			string elementName = xr.Name;
			XmlDocument document = new XmlDocument(xr.NameTable);

			XmlNode root = document.ReadNode(xr);

			XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);

			nsmgr.AddNamespace("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
			nsmgr.AddNamespace("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
			nsmgr.AddNamespace("a14", "http://schemas.microsoft.com/office/drawing/2010/main");
			nsmgr.AddNamespace("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
			nsmgr.AddNamespace("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
			nsmgr.AddNamespace("pic", "http://schemas.openxmlformats.org/drawingml/2006/picture");
			nsmgr.AddNamespace("pr", "http://schemas.openxmlformats.org/package/2006/relationships");
			nsmgr.AddNamespace("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
			nsmgr.AddNamespace("v", "urn:schemas-microsoft-com:vml");


			if (elementName == "w:pict")
			{
				XmlNode node = root.SelectSingleNode("//v:imagedata", nsmgr);
				if(node != null )
				if (node.Attributes["r:id"] != null)
					identifikator = node.Attributes["r:id"].Value;
			}
			if (elementName == "v:imagedata")
			{
				identifikator = xr.GetAttribute("r:id");
			}
			if (elementName == "w:drawing")
			{
				
				XmlNode node = root.SelectSingleNode("//a:blip", nsmgr);
				if(node != null && node.Attributes != null)
				if (node.Attributes["r:embed"] != null)
					identifikator = node.Attributes["r:embed"].Value;
				else if ((node.Attributes["r:link"] != null))
				{
					identifikator = node.Attributes["r:link"].Value;
				}
			}

			args.Identifikator = identifikator;

			if (identifikator != null && !String.IsNullOrEmpty(mstrSouborRelaci) && File.Exists(mstrSouborRelaci))
			{
				XmlDocument relationships = new XmlDocument();
				relationships.Load(mstrSouborRelaci);
				root = relationships.DocumentElement;
				if (root != null)
				{
					XmlNode node = root.SelectSingleNode("//pr:Relationship[@Id='" + identifikator + "']", nsmgr);
					if (node != null && node.Attributes!= null &&  node.Attributes["Target"] != null)
					{
						args.Umisteni = node.Attributes["Target"].Value;
					}
				}
			}

			return args;
		}

		private void ZpracovatKonecTextovehoUseku(TextRunEventArgs traTextRun, Style stDefaulCharacterStyle)
		{
			if (ZnakovyStyl != null)
			{
				//opravdu to může nastat?; nejspíš v případě, kdy text není proznačen styly
				if (traTextRun == null)
					traTextRun = new TextRunEventArgs();
				if (traTextRun.Style == null)
				{
					traTextRun.Style = stDefaulCharacterStyle;
					/*
							traTextRun.StyleID = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).ID;
							traTextRun.StyleName = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).Name;
							traTextRun.UIStyleName = mstDefinovaneStyly.StyleByID(strDefaulCharacterStyleID).UIName;
							*/
				}
				else
					ZnakovyStyl(this, traTextRun);
			}
		}

		private void ZpracujPoznamkuPodCarou(string strId)
		{
			if (String.IsNullOrEmpty(mstrSouborXmlFootnotes))
				return;
			CtiDokumentXml(mstDefaultParagraphStyle, mstDefaulCharacterStyle, mstrSouborXmlFootnotes, strId);
		}
	}
}
