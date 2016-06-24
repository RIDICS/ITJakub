using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Daliboris.Statistiky.Frekvence;

namespace Daliboris.Slovniky.Svoboda {

	public class Index : Slovnik {
		private const string csZkratkaSlovniku = "IndexSvob";

		internal class HesloveSlovo {

			public string Original { get; set; }
			public string Text { get; set; }
			public string Heslo { get; set; }
			public string Zbytek { get; set; }
			public string Rod { get; set; }
			public bool Odkaz { get; set; }
			public bool Transliterace { get; set; }
			public bool NejisteZneni { get; set; }
			public bool NejistyPramen { get; set; }
			public bool Varianta { get; set; }
			public string TypVarianty { get; set; }
			public bool RuznaPodoba { get; set; }
			public bool RuznaPodobaVDokladech { get; set; }
			public bool ZenskaPodoba { get; set; }
			public bool VychoziPodoba { get; set; }
			public bool ZmenenaPdodoba { get; set; }
			public bool RozepsanaForma { get; set; }
			public bool ZkracenaForma { get; set; }
			public bool JineCteni { get; set; }
			public bool RekonstruovanoZMistnihoJmena { get; set; }
			private HeslovaSlova mglsRozepsanePodoby = new HeslovaSlova();
			public HeslovaSlova RozepsanePodoby {
				get {
					return mglsRozepsanePodoby;
				}
				set {
					mglsRozepsanePodoby = value;
				}
			}


			public HesloveSlovo() { }
			public HesloveSlovo(string strText) {
				this.Text = strText;
				this.Original = strText;
			}

			public HesloveSlovo(string strText, bool blnVychoziPodoba, bool blnZmenenaPodoba)
				: this(strText) {
				this.VychoziPodoba = blnVychoziPodoba;
				this.ZmenenaPdodoba = blnZmenenaPodoba;
			}

			public HesloveSlovo(bool blnVarianta, string strText)
				: this(strText) {
				this.Varianta = blnVarianta;
			}

			public HesloveSlovo(bool blnVarianta, string strText, string strOddelovacVariant) : this(blnVarianta,strText) {
				if (strOddelovacVariant == "/")
					TypVarianty = "uncertain";
				else
					TypVarianty = "both";
			}

			public void Analyzuj() {
				char[] chZnaky = new char[] { ' ', '-', '!', '(', ')', '*', ',', '.', '/', ':', '?', '[', ']', '_', '>' };
				int i = 0;
				do {
					FrekvenceZnaku frq = new FrekvenceZnaku(chZnaky, this.Text, true);
					frq.SpocitejFrekvenci();

					switch (frq.PoradiZnaku) {
						case "":
							this.Heslo = this.Text;
							break;
						case "*":
							this.Heslo = Text.Remove(0, 1);
							this.RekonstruovanoZMistnihoJmena = true;
							this.Text = this.Heslo;
							break;
						case ":":
							this.Heslo = Text.Remove(0, 1);
							this.Odkaz = true;
							this.Text = this.Heslo;
							break;
						case "_":
							this.Heslo = Text.Remove(0, 1);
							this.Transliterace = true;
							this.Text = this.Heslo;
							break;
						case "()()":
						case "()":
							if (Text.StartsWith("(") && Text.EndsWith(")")) {
								if (Text.Contains("ová"))
									this.ZenskaPodoba = true;
								this.Heslo = Text.Substring(1, Text.Length - 2);
								this.Text = this.Heslo;
							}
							else {
								this.ZkracenaForma = true;
								string[] asRozpis = Daliboris.UJC.OVJ.StcS.Slovnik.Pomucky.RozepsatVsechnyZavorky(this.Text);
								foreach (String item in asRozpis) {
									HesloveSlovo hs = CloneHesloveSlovo(item);
									hs.ZkracenaForma = false;
									hs.RozepsanaForma = true;
									this.RozepsanePodoby.Add(hs);
								}
								if (this.Heslo == null) {
									this.Heslo = this.RozepsanePodoby[0].Original;
									this.Text = this.RozepsanePodoby[0].Original;
									this.ZkracenaForma = this.RozepsanePodoby[0].ZkracenaForma;
									this.RozepsanaForma = this.RozepsanePodoby[0].RozepsanaForma;
									this.RozepsanePodoby.RemoveAt(0);
								}
								else {
									this.Text = string.Empty;
								}

							}
							break;
						case " (?)":
							if (this.Text.EndsWith(frq.PoradiZnaku)) {
								this.NejisteZneni = true;
								this.Heslo = this.Text.Remove(this.Text.Length - frq.PoradiZnaku.Length);
								this.Text = this.Heslo;
							}
							break;
						case " (!)":
							if (this.Text.EndsWith(frq.PoradiZnaku)) {
								this.NejistyPramen = true;
								this.Heslo = this.Text.Remove(this.Text.Length - frq.PoradiZnaku.Length);
								this.Text = this.Heslo;
							}
							break;
						case "(.?)":
						case "() (.)":
						case " (.  .)":
						case " (.)":
							this.Rod = this.Text.Substring(this.Text.IndexOf(' ') + 1);
							this.Heslo = this.Text.Remove(this.Text.Length - this.Rod.Length - 1);
							this.Text = this.Heslo;
							break;
						case "[]":
							if (Text.StartsWith("[") && Text.EndsWith("]")) {
								this.JineCteni = true;
								this.Heslo = Text.Substring(1, Text.Length - 2);
								this.Text = this.Heslo;
							}
							break;
						case "* (?)":
						case "*()()":
						case "*()":
							goto case "*";
						//break;
						case " ":
							this.Heslo = this.Text;
							break;

						default:
							if (this.Text.EndsWith(" (!?)")) {
								this.NejistyPramen = true;
								this.NejisteZneni = true;
								this.Text = this.Text.Substring(0, this.Text.Length - " (!?)".Length);
							}
							/*
							if (this.Text.EndsWith(" (ž.)") || this.Text.EndsWith(" (m.)")) {
								this.Rod = this.Text.Substring(this.Text.Length - " (m.)".Length).Trim();
								this.Text = this.Text.Substring(0, this.Text.Length - " (m.)".Length);
							}
							else if (this.Text.EndsWith(" (m. nebo ž. ?)")) {
								this.Rod = this.Text.Substring(this.Text.Length - " (m. nebo ž. ?)".Length).Trim();
								this.Text = this.Text.Substring(0, this.Text.Length - " (m. nebo ž. ?)".Length);
							}
							else if (this.Text.EndsWith(" (m. nebo ž.)")) {
								this.Rod = this.Text.Substring(this.Text.Length - " (m. nebo ž.)".Length).Trim();
								this.Text = this.Text.Substring(0, this.Text.Length - " (m. nebo ž.)".Length);
							}
							else if (this.Text.EndsWith(" (m. a ž.)")) {
								this.Rod = this.Text.Substring(this.Text.Length - " (m. a ž.)".Length).Trim();
								this.Text = this.Text.Substring(0, this.Text.Length - " (m. a ž.)".Length);
							}
								*/
							if (this.Text.Contains('.')) {
								string[] asRody = new string[] { " (ž.)", " (m.)", " (m.?)", " (m. ?)", " (ž.?)", "(ž. ?)", " (m., ž.?)", " (m., ž. ?)", " (m. nebo ž. ?)", " (m. nebo ž.)", " (m. a ž.)" };
								foreach (string sRod in asRody) {
									if (this.Text.EndsWith(sRod)) {
										this.Rod = this.Text.Substring(this.Text.Length - sRod.Length).Trim();
										this.Text = this.Text.Substring(0, this.Text.Length - sRod.Length);
									}
								}
							}
							if (this.Text.EndsWith(" (?)")) {
								this.NejisteZneni = true;
								this.Text = this.Text.Substring(0, this.Text.Length - " (?)".Length);
							}
							if (this.Text.EndsWith(" (!)")) {
								this.NejistyPramen = true;
								this.Text = this.Text.Substring(0, this.Text.Length - " (!)".Length);
							}
							if (this.Text.StartsWith("[*")) {
								this.Text = this.Text.Remove(1, 1);
								this.RekonstruovanoZMistnihoJmena = true;
							}
							if (this.Text[0] == '*')
								goto case "*";
							if (this.Text[0] == '_')
								goto case "_";
							if (this.Text[0] == ':')
								goto case ":";
							break;
					}
					i++;
				}
				while (this.Text.IndexOfAny(chZnaky) > -1 && i++ < 5);
				if (this.Text.IndexOfAny(chZnaky) == -1 && this.Heslo == null)
					this.Heslo = this.Text;
			}

			internal HesloveSlovo CloneHesloveSlovo(String item) {
				HesloveSlovo hs = new HesloveSlovo(item);
				hs.Odkaz = this.Odkaz;
				hs.JineCteni = this.JineCteni;
				hs.NejisteZneni = this.NejisteZneni;
				hs.NejistyPramen = this.NejistyPramen;
				hs.Odkaz = this.Odkaz;
				hs.RekonstruovanoZMistnihoJmena = this.RekonstruovanoZMistnihoJmena;
				hs.RuznaPodoba = this.RuznaPodoba;
				hs.RuznaPodobaVDokladech = this.RuznaPodobaVDokladech;
				hs.Transliterace = this.Transliterace;
				hs.Varianta = this.Varianta;
				hs.VychoziPodoba = this.VychoziPodoba;
				hs.ZenskaPodoba = this.ZenskaPodoba;
				hs.ZmenenaPdodoba = this.ZmenenaPdodoba;
				hs.ZkracenaForma = this.ZkracenaForma;
				hs.RozepsanaForma = this.RozepsanaForma;
				hs.Rod = this.Rod;
				return hs;
			}
		}
		internal class HeslovaSlova : List<HesloveSlovo> {
			private List<HesloveSlovo> mglsHeslovaSlova = new List<HesloveSlovo>();
			private string mstrOriginalniPodoba;

			public HeslovaSlova() { }
			public HeslovaSlova(string strOriginalniPodoba) {
				mstrOriginalniPodoba = strOriginalniPodoba;
			}

			public string OriginalniPodoba {
				get { return mstrOriginalniPodoba; }
				set { mstrOriginalniPodoba = value; }
			}

			public void KonsolidujHesla() {
				bool blnZkonsolidovano = false;
				while (!blnZkonsolidovano) {
					blnZkonsolidovano = true;
					foreach (HesloveSlovo hsl in this) {
						hsl.Analyzuj();
						if (hsl.RozepsanePodoby.Count > 0) {
							foreach (HesloveSlovo item in hsl.RozepsanePodoby) {
								this.Add(item);
							}
							hsl.RozepsanePodoby.Clear();
							blnZkonsolidovano = false;
							break;
						}

						if (!String.IsNullOrEmpty(hsl.Zbytek)) {
							this.Add(new HesloveSlovo(hsl.Zbytek));
							hsl.Zbytek = null;
							blnZkonsolidovano = false;
							break;
						}
					}
				}
			}
		}

		public Index() {
		}
		public Index(string strVstupniSoubor) {
			base.VstupniSoubor = strVstupniSoubor;
		}
		public Index(string strVstupniSoubor, string strVystupniSoubor) {
			base.VstupniSoubor = strVstupniSoubor;
			base.VystupniSoubor = strVystupniSoubor;
		}
		public void PrevestTxtNaXml() {
			List<string> glsVsechnaHesla = new List<string>(18000);
			Dictionary<string, List<HeslovaSlova>> gdcHeslovaSlova = new Dictionary<string, List<HeslovaSlova>>();
			XmlWriterSettings xws = new XmlWriterSettings();
			xws.Indent = true;
			xws.IndentChars = " ";
			Dictionary<string, HeslovaSlova> glsDuplicitniHesla = new Dictionary<string, HeslovaSlova>();
			using (StreamReader sr = new StreamReader(base.VstupniSoubor)) {
				using (XmlWriter xw = XmlWriter.Create(base.VystupniSoubor, xws)) {
					xw.WriteStartDocument(true);
					xw.WriteStartElement("dictionary");
					xw.WriteAttributeString("name", csZkratkaSlovniku);
					string sRadek;
					int i = 0;
					while ((sRadek = sr.ReadLine()) != null) {
						if (sRadek[sRadek.Length - 1] != '-') {
							HeslovaSlova hss = AnalyzujHeslo(xw, sRadek, ref i);
							foreach (HesloveSlovo hs in hss) {
								if (!String.IsNullOrEmpty(hs.Heslo)) {
									string sIdHeslo = hs.Heslo + " " + hs.Rod;
									if (gdcHeslovaSlova.ContainsKey(sIdHeslo)) {
										gdcHeslovaSlova[sIdHeslo].Add(hss);
									}
									else {
										gdcHeslovaSlova.Add(sIdHeslo, new List<HeslovaSlova>(new HeslovaSlova[] { hss }));
									}
									if (glsVsechnaHesla.Contains(sIdHeslo))
										glsDuplicitniHesla.Add(sIdHeslo, hss);
									else
										glsVsechnaHesla.Add(sIdHeslo);
								}
							}
						}
					}
					xw.WriteEndElement(); //dictionary
					xw.WriteEndDocument();
				}
			}
			Console.WriteLine("Duplicitní hesla:");

			using (XmlWriter xw = XmlWriter.Create(base.VystupniSoubor.Replace(".xml", "_dupl.xml"), xws)) {
				xw.WriteStartDocument(true);
				xw.WriteStartElement("dictionary");
				xw.WriteAttributeString("name", csZkratkaSlovniku);
				xw.WriteAttributeString("duplicity", "true");

				foreach (KeyValuePair<string, List<HeslovaSlova>> kvp in gdcHeslovaSlova) {
					if (kvp.Value.Count > 1) {
						xw.WriteStartElement("duplicita");
						xw.WriteAttributeString("heslo", kvp.Key);
						foreach (HeslovaSlova hs in kvp.Value) {
							VypsatKompletniEntry(xw, hs.OriginalniPodoba, hs, 0);
						}

						xw.WriteEndElement();
					}
				}

				//foreach (KeyValuePair<string, HeslovaSlova> kvp in glsDuplicitniHesla) {
				//  xw.WriteStartElement("duplicita");
				//  xw.WriteAttributeString("heslo", kvp.Key);
				//  VypsatKompletniEntry(xw, kvp.Key, kvp.Value);
				//  xw.WriteEndElement();
				//}
				xw.WriteFullEndElement();
				xw.WriteEndDocument();
			}
		}

		internal HeslovaSlova AnalyzujHeslo(XmlDocument xd, string strRadek, ref int iPoradi) {
			char[] cchOddelovaceHesel = new char[] { '/', '>', ',' };
			//1. rozdělit řádek na hesla
			//2. analyzovat jednotlivá hesla
			HeslovaSlova glsHeslovaSlova = new HeslovaSlova(strRadek);
			char[] chZnaky = new char[] { ' ', '-', '!', '(', ')', '*', ',', '.', '/', ':', '?', '[', ']', '_', '>' };
			if (strRadek.IndexOfAny(cchOddelovaceHesel) > -1) {
				FrekvenceZnaku fz = new FrekvenceZnaku(chZnaky, strRadek.Trim(), true);
				fz.SpocitejFrekvenci();
				if (fz.PoradiZnaku.Contains('>')) {
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					glsHeslovaSlova.Add((new HesloveSlovo(asHesla[0].Trim(), true, false)));
					glsHeslovaSlova.Add((new HesloveSlovo(asHesla[1].Trim(), false, true)));
				}
				else if (fz.PoradiZnaku == "(/)") {
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					foreach (string sHeslo in asHesla) {
						if (sHeslo.StartsWith("("))
							glsHeslovaSlova.Add((new HesloveSlovo(true, sHeslo.Trim() + ")", "/")));
						else
							glsHeslovaSlova.Add((new HesloveSlovo(true, "(" + sHeslo.Trim(), "/")));
					}
				}
				else if (fz.PoradiZnaku == " (., . ?)") {
					glsHeslovaSlova.Add((new HesloveSlovo(true, strRadek.Trim())));
				}
				else {
					string strOddelovac = ",";
					if (fz.PoradiZnaku.IndexOf('/') > -1)
						strOddelovac = "/";
					else if (fz.PoradiZnaku.IndexOf(',') > -1)
						strOddelovac = ",";
					else {
						strOddelovac = "";
					}
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					foreach (string sHeslo in asHesla) {
						glsHeslovaSlova.Add((new HesloveSlovo(true, sHeslo.Trim(), strOddelovac)));
					}
				}
			}
			else {
				glsHeslovaSlova.Add(new HesloveSlovo(strRadek));
			}
			glsHeslovaSlova.KonsolidujHesla();

			if (strRadek.IndexOfAny(new char[] { '_', ':' }) == -1) {
				VytvoritJednotlivaHeslovaSlova(xd, glsHeslovaSlova, ++iPoradi);
				return glsHeslovaSlova;
			}
			else {
				return new HeslovaSlova(strRadek);
			}
		}
		internal HeslovaSlova AnalyzujHeslo(XmlWriter xw, string strRadek, ref int iPoradi) {
			char[] cchOddelovaceHesel = new char[] { '/', '>', ',' };
			//1. rozdělit řádek na hesla
			//2. analyzovat jednotlivá hesla
			HeslovaSlova glsHeslovaSlova = new HeslovaSlova(strRadek);
			char[] chZnaky = new char[] { ' ', '-', '!', '(', ')', '*', ',', '.', '/', ':', '?', '[', ']', '_', '>' };
			if (strRadek.IndexOfAny(cchOddelovaceHesel) > -1) {
				FrekvenceZnaku fz = new FrekvenceZnaku(chZnaky, strRadek.Trim(), true);
				fz.SpocitejFrekvenci();
				if (fz.PoradiZnaku.Contains('>')) {
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					glsHeslovaSlova.Add((new HesloveSlovo(asHesla[0].Trim(), true, false)));
					glsHeslovaSlova.Add((new HesloveSlovo(asHesla[1].Trim(), false, true)));
				}
				else if (fz.PoradiZnaku == "(/)") {
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					foreach (string sHeslo in asHesla) {
						if (sHeslo.StartsWith("("))
							glsHeslovaSlova.Add((new HesloveSlovo(true, sHeslo.Trim() + ")", "/")));
						else
							glsHeslovaSlova.Add((new HesloveSlovo(true, "(" + sHeslo.Trim(), "/")));
					}
				}
				else if (fz.PoradiZnaku == " (., . ?)") {
					glsHeslovaSlova.Add((new HesloveSlovo(true, strRadek.Trim())));
				}
				else {
					string strOddelovac = ",";
					if (fz.PoradiZnaku.IndexOf('/') > -1)
						strOddelovac = "/";
					else if (fz.PoradiZnaku.IndexOf(',') > -1)
						strOddelovac = ",";
					else {
						strOddelovac = "";
					}
					string[] asHesla = strRadek.Split(cchOddelovaceHesel, StringSplitOptions.RemoveEmptyEntries);
					foreach (string sHeslo in asHesla) {
						glsHeslovaSlova.Add((new HesloveSlovo(true, sHeslo.Trim(), strOddelovac)));
					}
				}
			}
			else {
				glsHeslovaSlova.Add(new HesloveSlovo(strRadek));
			}
			glsHeslovaSlova.KonsolidujHesla();

			if (strRadek.IndexOfAny(new char[] { '_', ':' }) == -1) {
				VypsatKompletniEntry(xw, strRadek, glsHeslovaSlova, ++iPoradi);
				return glsHeslovaSlova;
			}
			else {
				return new HeslovaSlova(strRadek);
			}
		}

		private static void VypsatKompletniEntry(XmlWriter xw, string strRadek, HeslovaSlova glsHeslovaSlova, int iPoradi) {
			xw.WriteStartElement("entry");
			xw.WriteAttributeString("source", csZkratkaSlovniku);
			xw.WriteAttributeString("id", "h" + iPoradi.ToString("000000"));
			xw.WriteElementString("original", strRadek);
			VypsatJednotlivaHeslovaSlova(xw, glsHeslovaSlova, iPoradi);
			xw.WriteEndElement(); //entry
		}

		private static void VytvoritJednotlivaHeslovaSlova(XmlDocument xd, HeslovaSlova glsHeslovaSlova, int iPoradi) {
			int iPodporadi = 0;
			//xd.DocumentElement.RemoveChild(xd.DocumentElement.SelectSingleNode("//hw"));
			XmlNode xe = xd.DocumentElement.SelectSingleNode("//hw");
			xe = Index.RenameNode(xe, "", "hwo"); //hwo = headword original
			bool bInterni = (glsHeslovaSlova.Count == 1 && glsHeslovaSlova[0].Heslo == xe.InnerText);
			//if (glsHeslovaSlova.Count == 1 && glsHeslovaSlova[0].Heslo == xe.InnerText)
			//  return;
			XmlNode xhwg = xd.CreateElement("hwGrp");
			foreach (HesloveSlovo hs in glsHeslovaSlova) {
				if (!hs.Odkaz && !hs.Transliterace && !String.IsNullOrEmpty(hs.Heslo)) {
					iPodporadi++;
					XmlNode xnhw = xd.CreateElement("hw");
					PripojitAtribut(xd, xnhw, "id", String.Format("h{0:000000}.{1}", iPoradi, iPodporadi));
					//PripojitAtribut(xd, xnhw, "source", csZkratkaSlovniku)
					if (hs.Varianta) {
						//PripojitAtribut(xd, xnhw, "variant", "true");
						PripojitAtribut(xd, xnhw, "variant", hs.TypVarianty);
					}
					if (hs.Odkaz)
						PripojitAtribut(xd, xnhw, "xref", "true");
					if (hs.ZenskaPodoba)
						PripojitAtribut(xd, xnhw, "fem", "true");
					if (hs.RekonstruovanoZMistnihoJmena)
						PripojitAtribut(xd, xnhw, "type", "reconstr");
					if (hs.Transliterace)
						PripojitAtribut(xd, xnhw, "form", "trsl");
					if (hs.NejisteZneni)
						PripojitAtribut(xd, xnhw, "cert", "uncertain");
					if (hs.NejistyPramen)
						PripojitAtribut(xd, xnhw, "sourceCert", "uncertain");
					if (hs.JineCteni)
						PripojitAtribut(xd, xnhw, "reading", "other");
					if (hs.VychoziPodoba)
						PripojitAtribut(xd, xnhw, "sourceForm", "true");
					if (hs.ZmenenaPdodoba)
						PripojitAtribut(xd, xnhw, "changedForm", "true");
					if (hs.Rod != null)
						PripojitAtribut(xd, xnhw, "gender", hs.Rod);
					if (hs.RozepsanaForma)
						PripojitAtribut(xd, xnhw, "form", "restored");
					if (hs.ZkracenaForma)
						PripojitAtribut(xd, xnhw, "form", "short");
					if (bInterni)
						PripojitAtribut(xd, xnhw, "use", "internal");
					xnhw.InnerText = hs.Heslo;
					xhwg.AppendChild(xnhw);
				}
				xd.DocumentElement.AppendChild(xhwg);
			}
		}

		private static void PripojitAtribut(XmlDocument xd, XmlNode xnhw, string strNazev, string strHodnota) {
			XmlAttribute xa = xd.CreateAttribute(strNazev);
			xa.Value = strHodnota;
			xnhw.Attributes.SetNamedItem(xa);
		}

		private static void VypsatJednotlivaHeslovaSlova(XmlWriter xw, HeslovaSlova glsHeslovaSlova, int iPoradi) {
			int iPodporadi = 0;
			foreach (HesloveSlovo hs in glsHeslovaSlova) {
				if (!hs.Odkaz && !hs.Transliterace && !String.IsNullOrEmpty(hs.Heslo)) {
					iPodporadi++;
					xw.WriteStartElement("hw");
					xw.WriteAttributeString("id", String.Format("h{0:000000}.{1}", iPoradi, iPodporadi));
					xw.WriteAttributeString("source", csZkratkaSlovniku);
					if (hs.Varianta)
						xw.WriteAttributeString("var", "true");
					if (hs.Odkaz)
						xw.WriteAttributeString("xref", "true");
					if (hs.ZenskaPodoba)
						xw.WriteAttributeString("fem", "true");
					if (hs.RekonstruovanoZMistnihoJmena)
						xw.WriteAttributeString("type", "reconstr");
					if (hs.Transliterace)
						xw.WriteAttributeString("form", "trsl");
					if (hs.NejisteZneni)
						xw.WriteAttributeString("cert", "uncertain");
					if (hs.NejistyPramen)
						xw.WriteAttributeString("sourceCert", "uncertain");
					if (hs.JineCteni)
						xw.WriteAttributeString("reading", "other");
					if (hs.VychoziPodoba)
						xw.WriteAttributeString("sourceForm", "true");
					if (hs.ZmenenaPdodoba)
						xw.WriteAttributeString("changedForm", "true");
					if (hs.Rod != null)
						xw.WriteAttributeString("gender", hs.Rod);

					xw.WriteString(hs.Heslo);
					xw.WriteEndElement();//hw
				}
			}
		}

		public static XmlNode RenameNode(XmlNode node, string namespaceURI, string qualifiedName) {
			if (node.NodeType == XmlNodeType.Element) {
				XmlElement oldElement = (XmlElement)node;
				XmlElement newElement = node.OwnerDocument.CreateElement(qualifiedName, namespaceURI);
				while (oldElement.HasAttributes) {
					newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
				}
				while (oldElement.HasChildNodes) {
					newElement.AppendChild(oldElement.FirstChild);
				}
				if (oldElement.ParentNode != null) {
					oldElement.ParentNode.ReplaceChild(newElement, oldElement);
				}
				return newElement;
			}
			else {
				return null;
			}
		}

	    public override void UpravitHraniceHesloveStati(string inputFile, string outputFile)
	    {
	        throw new NotImplementedException();
	    }

	    public override void KonsolidovatHeslovouStat(string inputFile, string outputFile)
	    {
	        throw new NotImplementedException();
	    }
	}
}
