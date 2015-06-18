using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Reflection;

namespace Daliboris.OOXML.Word.Transform {
	public class Transformace {
		private string mstrSoubor;
		private Koren mkrKoren;
		private Tagy mtgTagy;
		private Citace mctCitace;
		private Nahrady mnhNahrady;
		private DateTime mdtPosledniZmena;
		private Tabulky mtbTabulky;

		public Transformace() {
			mkrKoren = new Koren();
			mtgTagy = new Tagy();
			mctCitace = new Citace();
			mnhNahrady = new Nahrady();
			mtbTabulky = new Tabulky();
			mdtPosledniZmena = DateTime.Now;
		}
		public Transformace(string strSoubor) {
			mstrSoubor = strSoubor;
		}


		public DateTime PosledniZmena {
			get { return mdtPosledniZmena; }
			set { mdtPosledniZmena = value; }
		}

		public Koren Koren {
			get { return mkrKoren; }
			set { mkrKoren = value; }
		}

		public Tagy Tagy {
			get { return mtgTagy; }
			set { mtgTagy = value; }
		}

		public Citace Citace {
			get { return mctCitace; }
			set { mctCitace = value; }

		}

		public Nahrady Nahrady {
			get { return mnhNahrady; }
			set { mnhNahrady = value; }
		}

		public Tabulky Tabulky {
			get { return mtbTabulky; }
			set { mtbTabulky = value; }
		}

		public string Soubor {
			get { return mstrSoubor; }
			set { mstrSoubor = value; }
		}

		#region Zpracování v souboru
		private transformace NacistTransformaceXml(string sSoubor)
		{
		    MemoryStream ms;
            Stream fs;
            transformace trs = null;
			XmlSerializer ser = new XmlSerializer(typeof(transformace));
		    if (sSoubor == null)
		    {
		        Assembly _assembly;
		        _assembly = Assembly.GetExecutingAssembly();
		        fs = _assembly.GetManifestResourceStream(_assembly.GetName().Name + ".Resources.AllStyles.2xml");
		        if (fs != null)
		        {
		            trs = (transformace) ser.Deserialize(fs);
		            fs.Close();
		        }
		    }
		    else
		    {
		        fs = new FileStream(sSoubor, FileMode.Open, FileAccess.Read);
		        trs = (transformace)ser.Deserialize(fs);
		        fs.Close();
		    }


		    // If the XML document has been altered with unknown 
			// nodes or attributes, handles them with the 
			// UnknownNode and UnknownAttribute events.

			//ser.UnknownNode+= new XmlNodeEventHandler(serializer_UnknownNode);
			//ser.UnknownAttribute+= new XmlAttributeEventHandler(serializer_UnknownAttribute);

		    

		    return trs;

		}

		private void UlozitTransformaceDoXml(transformace trs, string sSoubor) {

			FileStream fs = new FileStream(sSoubor, FileMode.Create);
			XmlSerializer ser = new XmlSerializer(typeof(transformace));
			ser.Serialize(fs, trs);
			fs.Close();

		}

		public void NactiZeSouboru(string strSoubor) {
			this.Soubor = strSoubor;
			NactiZeSouboru();
		}

		public void NactiZeSouboru() {
            //if (mstrSoubor == null)
            //    throw new NullReferenceException("Není určen soubor pro načtení transformací.");

			mkrKoren = new Koren();
			mtgTagy = new Tagy();
			mctCitace = new Citace();
			mnhNahrady = new Nahrady();

			transformace tr = NacistTransformaceXml(mstrSoubor);

			Atributy atr;
			mkrKoren.Nazev = tr.koren.nazev;
			mkrKoren.Namespace = tr.koren.@namespace;
			mkrKoren.Atributy = PrevedAtributy(tr.koren.atribut);

			foreach (tag tg in tr.tagy) {
				atr = PrevedAtributy(tg.atribut);
				Nahrady nhr = PrevedNahrady(tg.nahrada);
				Tag t = new Tag(tg.bezZnacky, tg.ignorovat, tg.@namespace, tg.nazev,
					tg.prazdnyElement, tg.predchoziStyl, tg.sloucitSPredchozim, 
					tg.nasledujiciStyl, tg.sloucitSNasledujicim, tg.styl, atr, nhr);
				mtgTagy.Add(t);
			}

			Citace ctc = new Citace();
			if (tr.citace != null) {
				foreach (citac ct in tr.citace) {
					ctc.Add(new Citac(ct.format, ct.hodnota, ct.inkrement, ct.inkrementator,
						ct.nazev, ct.postfix, ct.prefix, ct.resetator, ct.vychoziHodnota));
				}
			}
			mctCitace = ctc;

			Nahrady nhrd = PrevedNahrady(tr.nahrady);
			mnhNahrady = nhrd;

			mtbTabulky = new Tabulky(tr.tabulky.tabulka, tr.tabulky.radek, tr.tabulky.bunka,
				tr.tabulky.textMistoTabulky, tr.tabulky.obsahPrazdneBunky, tr.tabulky.cislovatElementy);

			mdtPosledniZmena = tr.posledniZmena;
		}


		private void UlozDoSouboru() {
			UlozDoSouboru(mstrSoubor);
		}


		public void UlozDoSouboru(string sSouborXml) {
			if (sSouborXml == null)
				throw new NullReferenceException("Není určen soubor pro načtení transformací.");
			transformace tr = new transformace();
			tr.posledniZmena = this.PosledniZmena;
			if (mkrKoren != null) {
				koren kr = new koren();
				kr.@namespace = mkrKoren.Namespace;
				kr.nazev = mkrKoren.Nazev;
				kr.atribut = PrevedAtributy(mkrKoren.Atributy);
				tr.koren = kr;
			}
			if (this.Citace.Count > 0) {
				citac[] ct = new citac[this.Citace.Count];
				//TODO: rozepsat citace
				int i = -1;
				foreach (Citac c in mctCitace) {
					i++;
					ct[i] = new citac();
					ct[i].format = c.Format;
					ct[i].hodnota = c.Hodnota;
					ct[i].inkrement = c.Inkrement;
					ct[i].inkrementator = c.Inkrementator;
					ct[i].nazev = c.Nazev;
					ct[i].postfix = c.Postfix;
					ct[i].prefix = c.Prefix;
					ct[i].resetator = c.Resetator;
					ct[i].vychoziHodnota = c.VychoziHodnota;
				}
				//tr.citace = ct;
				tr.citace = ct.ToList();
			}
			tr.nahrady = PrevedNahrady(this.Nahrady);
			if (this.Tagy.Count > 0) {
				tag[] tg = new tag[this.Tagy.Count];
				int i = -1;
				foreach (Tag t in this.Tagy) {
					i++;
					tg[i] = new tag();

					tg[i].atribut = PrevedAtributy(t.Atributy);
					tg[i].nahrada = PrevedNahrady(t.Nahrady);

					tg[i].bezZnacky = t.BezZnacky;
					tg[i].bezZnackySpecified = true;
					tg[i].ignorovat = t.Ignorovat;
					tg[i].ignorovatSpecified = true;
					tg[i].@namespace = t.Namespace;
					tg[i].nazev = t.Nazev;
					tg[i].prazdnyElement = t.PrazdnyElement;
					tg[i].predchoziStyl = t.PredchoziStyl;
					tg[i].sloucitSPredchozim = t.SloucitSPredchozim;
					tg[i].styl = t.Styl;

				}
				//tr.tagy = tg;
				tr.tagy = tg.ToList();
			}
			UlozitTransformaceDoXml(tr, sSouborXml);

		}

		private Nahrady PrevedNahrady(List<nahrada> nhrs) {
			Nahrady nhr = new Nahrady();
			if (nhrs == null)
				return nhr;
			foreach (nahrada nh in nhrs) {
				nhr.Add(new Nahrada(nh.co, nh.cim));
			}
			return nhr;
		}

		[Obsolete("Starší kód pro transofrmaci z Xml")]
		private Nahrady PrevedNahrady(nahrada[] nhrs) {
			Nahrady nhr = new Nahrady();
			if (nhrs == null)
				return nhr;
			foreach (nahrada nh in nhrs) {
				nhr.Add(new Nahrada(nh.co, nh.cim));
			}
			return nhr;
		}

		private Atributy PrevedAtributy(List<atribut> atrs) {
			Atributy atr = new Atributy();
			if (atrs == null)
				return atr;
			foreach (atribut at in atrs) {
				atr.Add(new Atribut(at.nazev, at.hodnota));
			}
			return atr;
		}


		[Obsolete("Starší kód pro transofrmaci z Xml")]
		private Atributy PrevedAtributy(atribut[] atrs) {
			Atributy atr = new Atributy();
			if (atrs == null)
				return atr;
			foreach (atribut at in atrs) {
				atr.Add(new Atribut(at.nazev, at.hodnota));
			}
			return atr;
		}

		private List<nahrada> PrevedNahrady(Nahrady nhNahrady) {
			if (nhNahrady == null || nhNahrady.Count == 0)
				return null;
			List<nahrada> nhr = new List<nahrada>(nhNahrady.Count);
			foreach (Nahrada nh in nhNahrady) {
				nahrada n = new nahrada();
				n.co = nh.Co;
				n.cim = nh.Cim;
				nhr.Add(n);
			}
			//int j = -1;
			//foreach (Nahrada nh in nhNahrady) {
			// j++;
			// nhr[j] = new nahrada();
			// nhr[j].co = nh.Co;
			// nhr[j].cim = nh.Cim;
			//}
			return nhr;
		}

		//private nahrada[] PrevedNahrady(Nahrady nhNahrady) {
		// if (nhNahrady == null || nhNahrady.Count == 0)
		//  return null;
		// nahrada[] nhr = new nahrada[nhNahrady.Count];
		// int j = -1;
		// foreach (Nahrada nh in nhNahrady) {
		//  j++;
		//  nhr[j] = new nahrada();
		//  nhr[j].co = nh.Co;
		//  nhr[j].cim = nh.Cim;
		// }
		// return nhr;
		//}

		private List<atribut> PrevedAtributy(Atributy atAtributy) {
			if (atAtributy == null || atAtributy.Count == 0)
				return null;
			List<atribut> atr = new List<atribut>(atAtributy.Count);
			foreach (Atribut at in atAtributy) {
				atribut a = new atribut();
				a.nazev = at.Nazev;
				a.hodnota = at.Hodnota;
				atr.Add(a);
			}

			return atr;
		}

		////private atribut[] PrevedAtributy(Atributy atAtributy) {
		//// if (atAtributy == null || atAtributy.Count == 0)
		////  return null;
		//// atribut[] atrs = new atribut[atAtributy.Count];
		//// int j = -1;
		//// foreach (Atribut at in atAtributy) {
		////  j++;
		////  atrs[j] = new atribut();
		////  atrs[j].hodnota = at.Hodnota;
		////  atrs[j].nazev = at.Nazev;
		//// }
		//// return atrs;
		////}

		#endregion
	}
}
