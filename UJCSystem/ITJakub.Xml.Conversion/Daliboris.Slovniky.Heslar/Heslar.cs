using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Daliboris.Pomucky.Texty;
using Daliboris.Pomucky.Xml;
using System.Reflection;
using System.Collections;
using Daliboris.Pomucky.Soubory;

namespace Daliboris.Slovniky {

  public delegate void ZpracovatTagProHeslarXml(XmlReader xrVstup, XmlWriter xwVystup,
	  PismenoInfo piPismeno, HeslovaStatInfo hsiHeslovaStat, HesloInfo hiHeslo);

  public class Heslar {
	private const string cstrHeslarXml = "Heslar.xml";
	private static Dictionary<string, string> sgdcPismenaAbecedy = new Dictionary<string, string>();
	private static Dictionary<string, int> sgdcZdroje = new Dictionary<string, int>();
	private static Dictionary<string, int> sgdcTypHesloveStati = new Dictionary<string, int>();
	private static Dictionary<string, int> sgdcTypHeslovehoSlova = new Dictionary<string, int>();
	private static Dictionary<string, int> sgdcZpusobVyuziti = new Dictionary<string, int>();
	private static Dictionary<string, string> sgdcAkronymySlovniku = new Dictionary<string, string>();
	private static ZdrojeInfo zdrojeInfo  = new ZdrojeInfo();

	private static List<HesloInfo> sglsHesloInfo;

	private static char[] schIndexy;
	private static char[] schSeparatory;

	static Heslar() {


	  Assembly asm = Assembly.GetExecutingAssembly();
		string jmennyProstor = asm.FullName.Substring(0, asm.FullName.IndexOf(',') - 1).Trim();
		//string[] names = asm.GetManifestResourceNames();
		jmennyProstor = "Daliboris.Slovniky";

	  Stream xmlStream = asm.GetManifestResourceStream(jmennyProstor + ".Data.Heslar.xml");


	  /*
		FileInfo fi = new FileInfo(asm.Location);
		if (fi.DirectoryName == null) return;
		String sSouborDat = Path.Combine(fi.DirectoryName, cstrHeslarXml);

		if (!File.Exists(sSouborDat))
			throw new FileNotFoundException("Soubor " + cstrHeslarXml + " nebyl nalezen.");
	  */
	  XmlDocument xd = new XmlDocument();
	  try {
		if (xmlStream != null)
		  xd.Load(xmlStream);
		//xd.Load(sSouborDat);
	  }
	  catch (Exception ex) {
		throw ex;
	  }

	  NactiDataDoDictionary(xd.SelectNodes("/data/pismena/pismeno"), ref sgdcPismenaAbecedy);
	  NactiDataDoDictionary(xd.SelectNodes("/data/akronymy/akronym"), ref sgdcAkronymySlovniku);

	  NactiDataDoDictionary(xd.SelectNodes("/data/zdroje/zdroj"), ref sgdcZdroje);
	  NactiDataDoDictionary(xd.SelectNodes("/data/typyHeslovehoSlova/typHeslovehoSlova"), ref sgdcTypHeslovehoSlova);
	  NactiDataDoDictionary(xd.SelectNodes("/data/typyHesloveStati/typHesloveStati"), ref sgdcTypHesloveStati);
	  NactiDataDoDictionary(xd.SelectNodes("/data/zpusobyVyuziti/zpusobVyuziti"), ref sgdcZpusobVyuziti);

	  NactiDataZDictionary(xd.SelectNodes("/data/indexy/index"), ref schIndexy);
	  NactiDataZDictionary(xd.SelectNodes("/data/separatory/separator"), ref schSeparatory);
	  NactiZdrojeInfo(xd.SelectNodes("/data/zdroje/zdroj"), ref zdrojeInfo);
	}

	public Heslar() { }
	public Heslar(char[] chIndexy, char[] chSeparatory) {
	  schIndexy = chIndexy;
	  schSeparatory = chSeparatory;
	}

	private static void NactiZdrojeInfo(XmlNodeList xnl, ref ZdrojeInfo zdroje)
	{
	  foreach (XmlNode xn in xnl)
	  {
		ZdrojInfo zdrojInfo = new ZdrojInfo();
	  	zdrojInfo.Identifikator = int.Parse(xn.Attributes["id"].Value);
	  	zdrojInfo.Akronym = xn.Attributes["akronym"].Value;
		zdrojInfo.Zkratka = xn.Attributes["zkratka"].Value;
		if (xn.Attributes["pomocny"] != null)
		{
			zdrojInfo.Pomocny = Boolean.Parse(xn.Attributes["pomocny"].Value);
		}
		zdroje.Add(zdrojInfo);
	  }

	}

	private static void NactiDataDoDictionary(XmlNodeList xnl, ref Dictionary<string, string> gdc) {
	  foreach (XmlNode xn in xnl) {
		gdc.Add(xn.Attributes["id"].Value, xn.Attributes["text"].Value);
	  }
	}

	private static void NactiDataDoDictionary(XmlNodeList xnl, ref Dictionary<string, int> gdc) {
	  foreach (XmlNode xn in xnl) {
		gdc.Add(xn.Attributes["text"].Value, Int32.Parse(xn.Attributes["id"].Value));
	  }

	}

	private static void NactiDataZDictionary(XmlNodeList xnl, ref char[] chZnaky) {
	  chZnaky = new char[xnl.Count];
	  for (int i = 0; i < xnl.Count; i++) {
		chZnaky[i] = xnl[i].Attributes["text"].Value[0];
	  }
	}

	/// <summary>
	/// Akronymy slovníku
	/// </summary>
	/// <example>Key = StčS, Value = StcS</example>
	public static Dictionary<string, string> AkronymySlovniku {
	  get {
		return sgdcAkronymySlovniku;
	  }
	}

	public static Dictionary<string, string> PismenaAbecedy {
	  get {
		return sgdcPismenaAbecedy;
	  }
	}

	public static Dictionary<string, int> TypHeslovehoSlova {
	  get {
		return sgdcTypHeslovehoSlova;
	  }
	}

	public static Dictionary<string, int> Zdroj {
	  get {
		return sgdcZdroje;
	  }
	}

  	public static ZdrojeInfo Zdroje
  	{
	  get { return zdrojeInfo; }
  	}

	public static Dictionary<string, int> TypHesloveStati {
	  get {
		return sgdcTypHesloveStati;
	  }
	}

	public static Dictionary<string, int> ZpusobVyuziti {
	  get {
		return sgdcZpusobVyuziti;
	  }
	}

	public static Dictionary<string, string> IDPismenAbecedy() {

	  return sgdcPismenaAbecedy;

	}

	public static void HeslarXml(string strVstupniSoubor, string strVystupniSoubor, Dictionary<string, ZpracovatTagProHeslarXml> gztTagyZpracovani) {
	  char[] chIndexy = schIndexy;
	  char[] chSeparatory = schSeparatory;

	  using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor)) {
		using (XmlWriter xwHeslar = Objekty.VytvorXmlWriter(strVystupniSoubor)) {
		  xwHeslar.WriteStartDocument(true);
		  xwHeslar.WriteStartElement("heslar");
		  string strNazevTagu = null;
		  HesloInfo hiHeslo = null;
		  PismenoInfo piPismeno = null;
		  HeslovaStatInfo hsiHeslovaStat = null;

		  #region WHILE
		  while (r.Read()) {
			strNazevTagu = r.Name;
			if (r.NodeType == XmlNodeType.Element) {

			  if (gztTagyZpracovani != null && gztTagyZpracovani.ContainsKey(strNazevTagu)) {
				gztTagyZpracovani[strNazevTagu](r, xwHeslar, piPismeno, hsiHeslovaStat, hiHeslo);
			  }
			  else {
				#region SWITCH
				switch (r.Name) {
				  case "dictionary":
					string sSource = r.GetAttribute("name");
					xwHeslar.WriteAttributeString("dictionary", sSource);
					break;
				  case "div1":
					piPismeno = new PismenoInfo();
					piPismeno.Id = r.GetAttribute("id");
					piPismeno.Text = r.GetAttribute("text");

					VypisZacatekPismene(xwHeslar, piPismeno);
					break;
				  /*
				  case "entryref":
					   iHw = 0;
					   xwHeslar.WriteStartElement("heslovaStat");
					   sIdEntry = r.GetAttribute("id");
					   xwHeslar.WriteAttributeString("id", sIdEntry);
					   xwHeslar.WriteAttributeString("type", "ref");
					   break;
				  */
				  case "entry":
					//iHw = 0;
					hsiHeslovaStat = new HeslovaStatInfo();
					hsiHeslovaStat.Id = r.GetAttribute("id");
					hsiHeslovaStat.Typ = r.GetAttribute("type");
					xwHeslar.WriteStartElement("heslovaStat");
					Transformace.SerializeAttributes(r, xwHeslar, false);
					//sIdEntry = r.GetAttribute("id");
					//xwHeslar.WriteAttributeString("id", sIdEntry);
					//string sTypEntry = r.GetAttribute("type");
					//if (null != sTypEntry)
					//    xwHeslar.WriteAttributeString("type", sTypEntry);
					break;
				  case "hw":
					//zkontrolovat, jestli odstavec obsahuje "nenáležitá podoba" - a pak heslo vyřadit/označit jako interní
					//jenže akce následuje až za heslovým slovem
					string sForma = r.GetAttribute("form");
					Transformace.SerializeNode(r, xwHeslar);
					string strHeslo = r.ReadString();
					strHeslo = strHeslo.Trim();
					for (int i = 0; i < chIndexy.Length; i++) {
					  if (strHeslo.Contains(chIndexy[i].ToString())) {
						strHeslo = strHeslo.Remove(strHeslo.IndexOf(chIndexy[i]), 1);
						//xwHeslar.WriteAttributeString("hom", chIndexy[i].ToString());
						break;
					  }

					}
					if (strHeslo.IndexOf('-') == strHeslo.Length - 1 || strHeslo.IndexOf('-') == 0) {
					  if (sForma == null) {
						xwHeslar.WriteAttributeString("form", "short");
					  }
					}
					if (strHeslo.Contains("(?)"))
					  strHeslo = strHeslo.Replace("(?)", ""); //otazník v závorce za heslem
					strHeslo = strHeslo.TrimEnd(chSeparatory);
					strHeslo = strHeslo.TrimEnd();
					//strHeslo = strHeslo.TrimEnd(chIndexy);
					if (strHeslo.Length > 0) {
					  char chPismeno = strHeslo[0];
					  if (chPismeno == '*' || chPismeno == '\u02E3') {  //  || chPismeno == '\u02DF')  - zobrazovalo se špatně v IE
						//nemělo by se takové heslo upravit tak, že se odstraní první znak?
						xwHeslar.WriteAttributeString("pref", strHeslo.Substring(0, 1));
					  }

					  xwHeslar.WriteString(strHeslo);
					}
					break;
				}
				#endregion
			  }
			}
			#region IF2
			if (r.NodeType == XmlNodeType.EndElement) {
			  if (gztTagyZpracovani != null && gztTagyZpracovani.ContainsKey(strNazevTagu)) {
				gztTagyZpracovani[strNazevTagu](r, xwHeslar, piPismeno, hsiHeslovaStat, hiHeslo);
			  }
			  else {
				switch (r.Name) {
				  case "div1":
					xwHeslar.WriteEndElement();
					break;
				  case "entry":
					xwHeslar.WriteEndElement();
					break;
				  case "hw":
					xwHeslar.WriteEndElement();
					break;
				}
			  }
			}
			#endregion
		  }
		  #endregion
		}
	  }

	}

	private static void VypisZacatekPismene(XmlWriter xwHeslar, PismenoInfo pi) {
	  xwHeslar.WriteStartElement("pismeno");
	  xwHeslar.WriteAttributeString("id", pi.Id);
	  xwHeslar.WriteAttributeString("text", pi.Text);
	}

	public static void HeslarXml(string strVstupniSoubor, string strVystupniSoubor) {

	  char[] chIndexy = schIndexy;
	  char[] chSeparatory = schSeparatory;

	  using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor)) {
		using (XmlWriter xwHeslar = Objekty.VytvorXmlWriter(strVystupniSoubor)) {
		  xwHeslar.WriteStartDocument(true);
		  xwHeslar.WriteStartElement("heslar");

		  #region WHILE
		  while (r.Read()) {
			if (r.NodeType == XmlNodeType.Element) {
			  #region SWITCH
			  switch (r.Name) {
				case "dictionary":
				  string sSource = r.GetAttribute("name");
				  xwHeslar.WriteAttributeString("dictionary", sSource);
				  break;
				case "div1":
				  xwHeslar.WriteStartElement("pismeno");
				  xwHeslar.WriteAttributeString("id", r.GetAttribute("id"));
				  xwHeslar.WriteAttributeString("text", r.GetAttribute("text"));
				  break;
				/*
				case "entryref":
					 iHw = 0;
					 xwHeslar.WriteStartElement("heslovaStat");
					 sIdEntry = r.GetAttribute("id");
					 xwHeslar.WriteAttributeString("id", sIdEntry);
					 xwHeslar.WriteAttributeString("type", "ref");
					 break;
				*/
				case "entry":
				  //iHw = 0;
				  xwHeslar.WriteStartElement("heslovaStat");
				  Transformace.SerializeAttributes(r, xwHeslar, false);
				  //sIdEntry = r.GetAttribute("id");
				  //xwHeslar.WriteAttributeString("id", sIdEntry);
				  //string sTypEntry = r.GetAttribute("type");
				  //if (null != sTypEntry)
				  //    xwHeslar.WriteAttributeString("type", sTypEntry);
				  break;
				case "hwo":
				case "hw":
				  //zkontrolovat, jestli odstavec obsahuje "nenáležitá podoba" - a pak heslo vyřadit/označit jako interní
				  //jenže akce následuje až za heslovým slovem
				  string sForma = r.GetAttribute("form");
				  string sHom = r.GetAttribute("hom");
				  Transformace.SerializeNode(r, xwHeslar);
				  string strHeslo = r.ReadString();
				  strHeslo = strHeslo.Trim();
				  for (int i = 0; i < chIndexy.Length; i++) {
					if (strHeslo.Contains(chIndexy[i].ToString())) {
					  strHeslo = strHeslo.Remove(strHeslo.IndexOf(chIndexy[i]), 1);
					  if (sHom == null)
						xwHeslar.WriteAttributeString("hom", chIndexy[i].ToString()); //je potřeba to zapisovat, nebo ne?
					  break;
					}

				  }
				  if (strHeslo.IndexOf('-') == strHeslo.Length - 1 || strHeslo.IndexOf('-') == 0) {
					if (sForma == null) {
					  xwHeslar.WriteAttributeString("form", "short");
					}
				  }
				  if (strHeslo.Contains("(?)"))
					strHeslo = strHeslo.Replace("(?)", ""); //otazník v závorce za heslem
				  strHeslo = strHeslo.TrimEnd(chSeparatory);
				  strHeslo = strHeslo.TrimEnd();
				  //strHeslo = strHeslo.TrimEnd(chIndexy);
				  if (strHeslo.Length > 0) {
					char chPismeno = strHeslo[0];
					if (chPismeno == '*' || chPismeno == '\u02E3') {  //  || chPismeno == '\u02DF')  - zobrazovalo se špatně v IE
					  //nemělo by se takové heslo upravit tak, že se odstraní první znak?
					  xwHeslar.WriteAttributeString("pref", strHeslo.Substring(0, 1));
					}

					xwHeslar.WriteString(strHeslo);
				  }

				  /*
				  while (r.Name != "")
				  {
					   r.Read();
				  }
					   SerializeNode(r, xwHeslar);
				   */
				  /*
				  string sTyp = r.GetAttribute("type");
				  string strHeslo = r.ReadString();
				  strHeslo = strHeslo.Trim();
				  strHeslo = strHeslo.TrimEnd(chSeparatory);
				  strHeslo = strHeslo.TrimEnd();
				  string[] aHesla = strHeslo.Split(chSeparatory);
				  foreach (string s in aHesla) {
					   string sText = s.Trim();
					   if (s.Length > 0) {
							xwHeslar.WriteStartElement("hw");
							if (sTyp != null)
								  xwHeslar.WriteAttributeString("type",sTyp);
							++iHw;
							xwHeslar.WriteAttributeString("id", sIdEntry + ".hw" + iHw.ToString());

							xwHeslar.WriteString(sText);
							xwHeslar.WriteEndElement();
					   }
				  }
				   */
				  break;
			  }
			  #endregion
			}
			#region IF2
			if (r.NodeType == XmlNodeType.EndElement) {
			  switch (r.Name) {
				case "div1":
				  xwHeslar.WriteEndElement();
				  break;
				case "entry":
				  xwHeslar.WriteEndElement();
				  break;
				case "hwo":
				case "hw":
				  xwHeslar.WriteEndElement();
				  break;
			  }
			}
			#endregion
		  }
		  #endregion
		}
	  }
	}

	public static void SeraditHeslarText(string strVstupniSoubor) {

	  CteckaSouboru csb = new CteckaSouboru(strVstupniSoubor);
	  csb.ZacatekNacitani += new CteckaSouboru.Zacatek(csb_ZacatekNacitani);
	  csb.NactenyRadek += new CteckaSouboru.Radek(csb_NactenyRadek);
	  csb.KonecNacitani += new CteckaSouboru.Konec(csb_KonecNacitani);
	  csb.NactiSouborPoRadcich();

	  /*
	  List<HesloInfo> glsHeslar = new List<HesloInfo>();
	  using (StreamReader sr = new StreamReader(strVstupniSoubor)) {
		  string sRadek = null;
		  while ((sRadek = sr.ReadLine()) != null) {

			  HesloInfo hi = new HesloInfo(sRadek);
			  glsHeslar.Add(hi);
		  }

	  }
	  glsHeslar.Sort(delegate(HesloInfo hi1, HesloInfo hi2) {
		  int i = String.Compare(hi1.HesloveSlovo, hi2.HesloveSlovo, false);
		  if (i != 0)
			  return i;
		  i = string.Compare(hi1.Homonymum, hi2.Homonymum);
		  if (i != 0)
			  return i;
		  i = hi1.HeslovaStatTypId.CompareTo(hi2.HeslovaStatTypId);
		  if (i != 0)
			  return i;
		  i = String.Compare(hi1.TypHeslovehoSlova, hi2.TypHeslovehoSlova);
		  return i;

	  });
	  */
	  using (StreamWriter sw = new StreamWriter(strVstupniSoubor, false)) {
		foreach (HesloInfo hi in sglsHesloInfo) {
		  sw.WriteLine(hi.Zaznam());
		}
	  }
	}

	static void csb_KonecNacitani(object sender) {
	  sglsHesloInfo.Sort();
	}

	static void csb_NactenyRadek(object sender, RadekEventArgs ev) {
	  HesloInfo hi = new HesloInfo(ev.Text);
	  sglsHesloInfo.Add(hi);
	}

	static void csb_ZacatekNacitani(object sender) {
	  sglsHesloInfo = new List<HesloInfo>();
	}


	public static void HeslarText(string strVstupniSoubor, string sVystupniSoubor) {
	  string sSubVoce = null;
	  string sSubVoceId = null;
	  //string sHeslo = null;
	  //string sId = null;
	  //string sType = null;
	  string sHeslovaStatTyp = null;
	  string sHeslovaStatID = null;
	  //string sHomonymum = null;
	  //string sRetrograd = null;
	  string sPismeno = null;
	  string sZdroj = null;
	  string sPouziti = null;

	  using (XmlReader r = Objekty.VytvorXmlReader(strVstupniSoubor)) {
		using (StreamWriter sw = new StreamWriter(sVystupniSoubor, false, System.Text.Encoding.Unicode)) {
		  while (r.Read()) {
			if (r.NodeType == XmlNodeType.Element) {
			  switch (r.Name) {
				case "hw":
				  string[] asVlastnosti = new string[20];
				  HesloInfo hi = new HesloInfo();

				  string sRef = r.GetAttribute("ref");
				  string sZdrojHW = r.GetAttribute("source");
				  string sZdrojID = r.GetAttribute("target");
				  string sPrefix = r.GetAttribute("pref");
				  //if(r.GetAttribute("id") == "en000040.hw3")
				  // sPrefix = r.GetAttribute("pref");
				  string strTyp = r.GetAttribute("type");

				  //asVlastnosti[0] = ""; //prefix
				  //asVlastnosti[1] = ""; //hesloveSlovo
				  //asVlastnosti[2] = ""; //postfix
				  /*
				  asVlastnosti[3] = r.GetAttribute("hom");
				  asVlastnosti[4] = ""; //slovní druh
				  asVlastnosti[5] = ""; //další informace
				  asVlastnosti[6] = ""; //varianta

				  asVlastnosti[7] = r.GetAttribute("id");
				  asVlastnosti[8] = r.GetAttribute("form");
				  asVlastnosti[9] = strTyp; //substandard
				  asVlastnosti[10] = r.GetAttribute("xml:lang");
				  asVlastnosti[11] = sHeslovaStatID;
				  asVlastnosti[12] = ""; //heslová stať
				  asVlastnosti[13] = sHeslovaStatTyp;
				  asVlastnosti[14] = sPismeno;
				  asVlastnosti[15] = sZdroj;
				  */


				  hi.Homonymum = r.GetAttribute("hom");
				  hi.SlovniDruh = "";
				  hi.DalsiInformace = "";
				  hi.Varianta = "";
				  hi.Id = r.GetAttribute("id");
				  hi.Forma = r.GetAttribute("form");
				  hi.TypHeslovehoSlova = strTyp; //substandard
				  hi.Lang = r.GetAttribute("xml:lang");
				  hi.HeslovaStatId = sHeslovaStatID;
				  hi.HeslovaStat = ""; //heslová stať
				  hi.HeslovaStatTyp = sHeslovaStatTyp;
				  hi.Pismeno = sPismeno;
				  hi.ZkratkaZdroje = sZdroj;
				  hi.JeRef = sRef;


				  if (strTyp == "substandard" && sZdroj == "ESSC") // hi.TypHeslovehoSlova == "1"
					break;

				  /*
				  if (asVlastnosti[9] == "substandard" && sZdroj == "ESSC") {
					  break;
				  }
				  */


				  if (sZdrojHW == "HesStcS")
					hi.HesStcSIdRef = sZdrojID;

				  /*
				  if (sZdrojHW == "HesStcS") {
					  asVlastnosti[19] = sZdrojID;
				  }
				  */

				  //if (asVlastnosti[3] == null)
				  //   asVlastnosti[3] = "";

				  hi.ZpusobVyuziti = sPouziti;

				  //if (sPouziti == "internal")
				  // hi.FormId = 1;

				  /*
				  if (sPouziti == "internal")
					  asVlastnosti[18] = "1";
				  */


				  //logika součást get
				  /*
				  switch (asVlastnosti[8]) {
					  case "short":
						  asVlastnosti[8] = "4";
						  break;
					  case "restored":
						  asVlastnosti[8] = "2";
						  break;
					  default:
						  asVlastnosti[8] = "1";
						  break;
				  }

				  if (asVlastnosti[9] == null)
					  asVlastnosti[9] = "0";
				  else
					  asVlastnosti[9] = "1";

				  if (asVlastnosti[10] == null) {
					  asVlastnosti[10] = "1";
				  }

				  switch (sHeslovaStatTyp) {
					  case "ref":
						  asVlastnosti[13] = "4";
						  break;
					  case "full":
						  asVlastnosti[13] = "2";
						  break;
					  case "excl": //v ESSČ 
						  asVlastnosti[13] = "8";
						  break;
					  default:
						  asVlastnosti[13] = "1";
						  break;
				  }
				  if (sRef == "true") {
					  asVlastnosti[13] = "4";
				  }


				  switch (sZdroj) {
					  case "MSS":
						  asVlastnosti[15] = "2";
						  break;
					  case "ESSC":
						  asVlastnosti[15] = "4";
						  break;
					  case "GbSlov":
						  asVlastnosti[15] = "8";
						  break;
					  case "StcS":
					  case "StcSSlov":
						  asVlastnosti[15] = "16";
						  break;
					  case "SimekSlov":
						  asVlastnosti[15] = "32";
						  break;
					  case "HesStcS":
					  case "StcSMat":
						  asVlastnosti[15] = "1";
						  break;
					  default:
						  break;
				  }
				  */

				  string sObsahElementu = r.ReadElementString();

				  if (!hi.ZpracujHesloveSlovo(sObsahElementu, sPrefix))
					break;

				  /*
				  if (hi.HesloveSlovo == "žižň")
				  {
					  string d = hi.HesloveSlovo;
				  }
				  */
				  /*
				  asVlastnosti[1] = sObsahElementu;
				  if (!String.IsNullOrEmpty(sPrefix)) {
					  asVlastnosti[0] = sPrefix;
					  asVlastnosti[1] = asVlastnosti[1].Substring(sPrefix.Length);
				  }


				  if (asVlastnosti[1].Contains("(?)")) {
					  asVlastnosti[1] = asVlastnosti[1].Replace("(?)", "").TrimEnd();
				  }
				  int i = 0;
				  //while (!Char.IsLetter(asVlastnosti[1], i))
				  //{
				  //   asVlastnosti[0] += asVlastnosti[1][i].ToString();
				  //   asVlastnosti[1] = asVlastnosti[1].Substring(1);
				  //   //i++;
				  //}

				  i = asVlastnosti[1].Length - 1;
				  if (i == -1) {
					  break;
				  }
				  while (!Char.IsLetter(asVlastnosti[1], i)) {
					  if (asVlastnosti[1][i] == ')' || asVlastnosti[1][i] == '-')
						  break;
					  asVlastnosti[2] += asVlastnosti[1][i].ToString();
					  asVlastnosti[1] = asVlastnosti[1].Substring(0, i);
					  i--;
					  if (i == -1) {
						  break;
					  }
				  }
				  if (i == -1) {
					  break;
				  }
				  sRetrograd = Text.Retrograd(asVlastnosti[1], true);

				  if (sRetrograd.IndexOf(" ") > 0 && !(sRetrograd.Contains(" – ") || sRetrograd.Contains(" - "))) {
					  asVlastnosti[16] = sRetrograd.Substring(sRetrograd.LastIndexOf(" ") + 1);
					  asVlastnosti[17] = sRetrograd.Substring(0, sRetrograd.LastIndexOf(" "));
				  }
				  else
					  asVlastnosti[16] = sRetrograd;
				  */

				  if (sSubVoceId == null) {
					//sSubVoce = sHeslo;
					sSubVoceId = hi.Id;
				  }

				  /*
				  if (sSubVoceId == null) {
					  //sSubVoce = sHeslo;
					  sSubVoceId = asVlastnosti[7];
				  }
				  */

				  //if (sType == null)
				  //    sType = "";

				  //sw.WriteLine(String.Join("|", asVlastnosti));

				  /*
				  if (hi.HesloveSlovo == "Žižka") {
					  string sT = hi.HesloveSlovo;
					  bool b = hi.JeInterni;
				  }
				   * */

				  sw.WriteLine(hi.Zaznam());


				  break;
				case "heslovaStat":
				  sHeslovaStatTyp = r.GetAttribute("type");
				  sHeslovaStatID = r.GetAttribute("id");
				  sSubVoce = r.GetAttribute("defaulthw");
				  sPouziti = r.GetAttribute("use"); //internal, public

				  sSubVoceId = null;
				  break;
				case "pismeno":
				  sPismeno = r.GetAttribute("id");
				  break;
				case "heslar":
				  sZdroj = r.GetAttribute("dictionary");
				  break;
			  }
			}
		  }
		}
	  }
	}

	/// <summary>
	/// Rozdělí heslář podle písmen abecedy. Jedno písmeno = jeden soubor.
	/// </summary>
	/// <param name="strVstupniSoubor">Vstupní soubor (kompletní heslář ve formátu XML.</param>
	/// <param name="sVystupniAdresar">Výstoní adresář, do něhož se uloží vygenerované soubory (pro každé písmeno jeden soubor).</param>
	internal static void RozdelitPodlePismen(string strVstupniSoubor, string sVystupniAdresar) {
	  XmlTextReader treader = new XmlTextReader(strVstupniSoubor);
	  XmlReaderSettings xrs = new XmlReaderSettings();
	  xrs.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
	  XmlReader r = XmlReader.Create(treader, xrs);
	  XmlTextWriter xwPismeno = null;
	  StringBuilder sb = new System.Text.StringBuilder();
	  bool bPismeno = false;

	  /*
	  using (XmlReader r = Objekty.VytvorXmlReader(mstrVstupniSoubor)) {
		  using (XmlWriter xw = Objekty.VytvorXmlWriter(mstrVystupniSoubor)) {
			  xw.WriteStartDocument(true);

			  while (r.Read()) {
				  if (r.NodeType == XmlNodeType.Element) {
					  switch (r.Name) {
						  case "milestone":
							  break;
						  default:
							  Transformace.SerializeNode(r, xw);
							  break;

					  }
				  }
				  else if (r.NodeType == XmlNodeType.EndElement) {
					  switch (r.Name) {
						  case "milestone":
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
	  */

	  #region WHILE
	  while (r.Read()) {
		if (r.NodeType == XmlNodeType.Element) {
		  #region SWITCH
		  switch (r.Name) {
			case "pismeno":
			  if (bPismeno) {
				xwPismeno.WriteEndDocument();
				xwPismeno.Flush();
				xwPismeno.Close();
			  }
			  bPismeno = true;

			  string sID = r.GetAttribute("id");

			  xwPismeno = new XmlTextWriter(sVystupniAdresar + sID + ".xml", System.Text.Encoding.UTF8);
			  xwPismeno.Formatting = Formatting.Indented;
			  xwPismeno.Indentation = 2;
			  xwPismeno.WriteStartDocument(true);
			  Transformace.SerializeNode(r, xwPismeno);
			  break;

			case "heslovaStat":
			  Transformace.SerializeNode(r, xwPismeno);
			  sb = new System.Text.StringBuilder();
			  break;
			case "hw":
			  sb.Append(r.ReadString() + ", ");
			  break;
		  }
		  #endregion
		}
		#region IF2
		if (r.NodeType == XmlNodeType.EndElement) {
		  switch (r.Name) {
			case "heslovaStat":
			  sb.Remove(sb.Length - 2, 2);
			  xwPismeno.WriteStartElement("hw");
			  xwPismeno.WriteString(sb.ToString());
			  xwPismeno.WriteEndElement(); // hw
			  xwPismeno.WriteEndElement(); // heslovaStat
			  break;
			case "pismeno":
			  Transformace.SerializeNode(r, xwPismeno);
			  break;
		  }
		}
		#endregion
	  }
	  #endregion
	  xwPismeno.WriteEndDocument();
	  xwPismeno.Close();
	  xwPismeno = null;
	}

  }


}
