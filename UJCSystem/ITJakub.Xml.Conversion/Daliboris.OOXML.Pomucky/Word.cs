using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Daliboris.Statistiky;
using System.Xml;

namespace Daliboris.OOXML.Pomucky {

	public enum TypStatistiky { 
		OdstavcoveStyly,
		ZnakoveStyly,
		Pisma,
		Znaky
	}

	public class Word {
        /*
		private const string csStandardniPismoOdstavce = "Standardní písmo odstavce";

		private Jevy mjvOdstavcoveStyly = null;
		private Jevy mjvZnakoveStyly = null;
		private Jevy mjvPisma = null;
		private Jevy mjvZnaky = null;

		public string SouborXml { get; set; }
		public Jevy OdstavcoveStyly {
			get { return mjvOdstavcoveStyly; }
			set { mjvOdstavcoveStyly = value; }
		}
		public Jevy ZnakoveStyly {
			get { return mjvZnakoveStyly; }
			set { mjvZnakoveStyly = value; }
		}
		public Jevy Pisma {
			get { return mjvPisma; }
			set { mjvPisma = value; }
		}

		public Jevy Znaky {
			get { return mjvZnaky; }
			set { mjvZnaky = value; }
		}

		public Jevy PredejStatistiku(TypStatistiky typ) {
			switch (typ) {
				case TypStatistiky.OdstavcoveStyly:
					return mjvOdstavcoveStyly;
					break;
				case TypStatistiky.ZnakoveStyly:
					return mjvZnakoveStyly;
					break;
				case TypStatistiky.Pisma:
					return mjvPisma;
					break;
				case TypStatistiky.Znaky:
					return mjvZnaky;
					break;
				default:
					return null;
					break;
			}
         
		}
        
		public void ZpracujStatistiky() {
            /*
			if (mjvOdstavcoveStyly == null)
				mjvOdstavcoveStyly = new Jevy();
			else
				mjvOdstavcoveStyly.Clear();
         mjvOdstavcoveStyly.Zdroj = SouborXml;

			if (mjvZnakoveStyly == null)
				mjvZnakoveStyly = new Jevy();
			else
				mjvZnakoveStyly.Clear();
         mjvZnakoveStyly.Zdroj = SouborXml;

			if (mjvPisma == null)
				mjvPisma = new Jevy();
			else
				mjvPisma.Clear();
         mjvPisma.Zdroj = SouborXml;

			if (mjvZnaky == null)
				mjvZnaky = new Jevy();
			else
				mjvZnaky.Clear();
         mjvZnaky.Zdroj = SouborXml;


            string strText = null;

			string sAktualniZnakovyStyl = null;

			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(this.SouborXml, xrs);

			while (xr.Read()) {
				if (xr.NodeType == XmlNodeType.Element) {
					switch (xr.Name) {
						case "w:pStyle":
							mjvOdstavcoveStyly.Append(xr.GetAttribute("w:val"));
							break;
						case "w:rStyle":
                            string sAktStyl = xr.GetAttribute("w:val");
                            if (sAktualniZnakovyStyl != sAktStyl)
                            {
                                mjvZnakoveStyly.Append(sAktualniZnakovyStyl ?? csStandardniPismoOdstavce);
                                sAktualniZnakovyStyl = sAktStyl;
                            }
							break;
						case "w:rFonts":
							mjvPisma.Append(xr.GetAttribute(0));
							break;
						case "w:t":
							string sAktText = xr.ReadElementContentAsString();
                            
                            for (int i = 0; i < sAktText.Length; i++)
                            {
                                mjvZnaky.Append(sAktText[i].ToString());
							}
                            strText += sAktText;
							break;
						default:
							break;
					}
				}
                    
				else if (xr.NodeType == XmlNodeType.EndElement) {
                    switch (xr.Name)
                    {
								//case "w:rPr":
								//    mjvZnakoveStyly.Append(sAktualniZnakovyStyl ?? csStandardniPismoOdstavce);
								//    sAktualniZnakovyStyl = null;
								//    break;
                        case "w:p":
									  mjvZnakoveStyly.Append(sAktualniZnakovyStyl ?? csStandardniPismoOdstavce);
									  sAktualniZnakovyStyl = null;
									  break;
                        default:
                            break;
                    }
				}
                     
			}

			xr.Close();
            mjvOdstavcoveStyly.PosledniZmena = DateTime.Now;
            mjvZnakoveStyly.PosledniZmena = DateTime.Now;
            mjvPisma.PosledniZmena = DateTime.Now;
            mjvZnaky.PosledniZmena = DateTime.Now;
            */
		}

        public Jevy ObsahStylu(string strNazevStylu) {
            return ObsahStylu(strNazevStylu, false);
        }

		public Jevy ObsahStylu(string strNazevStylu, bool blnIgnorovatMezery) {
			string strAktualniStyl = null;
			Jevy jvObsahStylu = new Jevy(strNazevStylu);
            string strCelyText = null;
            bool blnNovyRun = true;
            /*

			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.CloseInput = true;
			xrs.ConformanceLevel = ConformanceLevel.Document;
			XmlReader xr = XmlReader.Create(this.SouborXml, xrs);

			while (xr.Read()) {
				if (xr.NodeType == XmlNodeType.Element) {
					switch (xr.Name) {
                        case "w:r":
                            blnNovyRun = true;
                            break;
                        case "w:p":
                            if (strCelyText != null & strAktualniStyl == strNazevStylu)
                            {
                                if (blnIgnorovatMezery)
                                    strCelyText = strCelyText.TrimEnd(new char[] { ' ', ' ' });
                                jvObsahStylu.Add(new Jev(strCelyText));
                            }
                            strCelyText = null;
                            strAktualniStyl = null;
                            break;
						//case "w:rPr":
						//	strAktualniStyl = null;
						//	break;
						case "w:rStyle":
                            blnNovyRun = false;
                            string sAktStyl = xr.GetAttribute("w:val");
                            if (strAktualniStyl != sAktStyl & strAktualniStyl == strNazevStylu)
                            {
                                if (blnIgnorovatMezery)
                                    strCelyText = strCelyText.TrimEnd(new char[] { ' ', ' ' });
                                jvObsahStylu.Add(new Jev(strCelyText));
                                strCelyText = null;
                            }
                            strAktualniStyl = sAktStyl;
							break;
						case "w:t":
                            if (blnNovyRun)
                            {
                                if (strAktualniStyl == strNazevStylu & strAktualniStyl != csStandardniPismoOdstavce)
                                {
                                    if (blnIgnorovatMezery && !string.IsNullOrEmpty(strCelyText))
                                        strCelyText = strCelyText.TrimEnd(new char[] { ' ', ' ' });
											  if(!string.IsNullOrEmpty(strCelyText))
                                    jvObsahStylu.Add(new Jev(strCelyText));
                                    strCelyText = null;
                                }
                                    strAktualniStyl = csStandardniPismoOdstavce;
                            }
							if (strAktualniStyl == strNazevStylu) {
                                string sText = xr.ReadElementContentAsString();
                                strCelyText += sText;
							}
							break;
						default:
							break;
					}
				}
			}
			xr.Close();
            jvObsahStylu.PosledniZmena = DateTime.Now;
            */
			return (jvObsahStylu);
		}

	}
}
