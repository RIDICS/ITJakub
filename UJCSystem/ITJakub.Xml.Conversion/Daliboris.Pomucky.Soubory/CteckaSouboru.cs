using System;
using System.Collections.Generic;
using System.IO;

namespace Daliboris.Pomucky.Soubory {
	public class CteckaSouboru {
		public delegate void Radek(object sender, RadekEventArgs ev);
		public delegate void Zacatek(object sender);
		public delegate void Konec(object sender);
		public event Radek NactenyRadek;
		public event Zacatek ZacatekNacitani;
		public event Konec KonecNacitani;
		private Soubor msbSoubor;
		private CteckaSouboruNastaveni mcsnNastaveni = new CteckaSouboruNastaveni();

		private List<string> mglsRadky;

		public CteckaSouboru() { }
		public CteckaSouboru(Soubor sbSoubor) {
			msbSoubor = sbSoubor;
		}
		public CteckaSouboru(string strSoubor) {
			msbSoubor = new Soubor(strSoubor);
		}

		public CteckaSouboru(string strSoubor, CteckaSouboruNastaveni csnNastaveni) : this(strSoubor) {
			mcsnNastaveni = csnNastaveni;
		}

		public CteckaSouboru(Soubor sbSoubor, CteckaSouboruNastaveni csnNastaveni) : this(sbSoubor) {
			mcsnNastaveni = csnNastaveni;
		}

		public  Soubor Soubor
		{
			get { return msbSoubor; }
		}

		public string[] NactiSouborNajednou() {
			string[] asRadky = null;
			using (StreamReader sr = new StreamReader(msbSoubor.CelaCesta, mcsnNastaveni.Kodovani)) {
				//asRadky = sr.ReadToEnd().Split( new char[] { '\r', '\n' });
				if (mcsnNastaveni.VynechatPrazdneRadky)
					asRadky = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				else
					asRadky = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			}
			return asRadky;
		}



		/// <summary>
		/// Seřadí řádky textově podle abecedy.
		/// </summary>
		/// <param name="strVystupniSoubor">Soubor, do kterého se mají výsledky řazení uložit.</param>
		public void SeradRadky(string strVystupniSoubor) {
			NacistRadkyAVypsat(false, false, strVystupniSoubor);
		}
		/// <summary>
		/// Seřadí řádky textově podle abecedy. Výsledky uloži do stejného souboru.
		/// </summary>
		public void SeradRadky() {
			SeradRadky(msbSoubor.CelaCesta);
		}

		/// <summary>
		/// Odstraní duplicitní řádky ze sobuoru a uloží je do nového souboru. Ve výsledku budou řádky seřazeny textově podle abecedy.
		/// </summary>
		/// <param name="strVystupniSoubor">Soubor, do kterého se mají výsledky odstranění duplicitních řádků uložit.</param>
		public void OdstranDuplicitniRadky(string strVystupniSoubor) {
			NacistRadkyAVypsat(false, true, strVystupniSoubor);
		}

		/// <summary>
		/// Odstraní duplicitní řádky, přičemž se určuje, zda se soubor seřadí, nebo ne.
		/// <remarks>Je to vhodné u soubrů, které již jsou seřazené, nebo u nichž nelze použít pouhé textové řazení.</remarks>
		/// </summary>
		/// <param name="blnNeraditSoubor">Zda se soubor seřadí, nebo ne</param>
		public void OdstranDuplicitniRadky(bool blnNeraditSoubor) {
			NacistRadkyAVypsat(blnNeraditSoubor, true, msbSoubor.CelaCesta);
		}
		/// <summary>
		/// Odstraní duplicitní řádky ze sobuoru. Ve výsledku budou řádky seřazeny textově podle abecedy.
		/// </summary>
		/// <param name="blnNeraditSoubor">Určuje, zda se řádky mají před odstraněním duplicit seřadit.</param>
		/// <param name="strVystupniSoubor">Soubor, do kterého se mají výsledky odstranění duplicitních řádků uložit.</param>
		public void OdstranDuplicitniRadky(bool blnNeraditSoubor, string strVystupniSoubor) {
			NacistRadkyAVypsat(blnNeraditSoubor, true, strVystupniSoubor);
		}

		/// <summary>
		/// Odstraní duplicitní řádky ze souboru. Ve výsledku budou řádky seřazeny textově podle abecedy.
		/// </summary>
		public void OdstranDuplicitniRadky() {
			OdstranDuplicitniRadky(msbSoubor.CelaCesta);
		}

		private void NacistRadkyAVypsat(bool blnNeradit, bool blnVynechatDuplicitni, string strVystupniSoubor) {
			mglsRadky = new List<string>(msbSoubor.PocetRadku);

			this.NactenyRadek += new Radek(CteckaSouboru_NactenyRadek);
			NactiSouborPoRadcich();
			if(!blnNeradit)
				mglsRadky.Sort();
			VypisRadkyDoSouboru(mglsRadky, blnVynechatDuplicitni, strVystupniSoubor);

		}
		private void SeraditRadkyAVypsat(bool blnVynechatDuplicitni) {
			NacistRadkyAVypsat(true, blnVynechatDuplicitni, msbSoubor.CelaCesta);
		}

		private void VypisRadkyDoSouboru(IList<string> iglsRadky, bool blnVynechatDuplicitni, string strVystupniSoubor) {
			using (StreamWriter sw = new StreamWriter(strVystupniSoubor, false, mcsnNastaveni.Kodovani)) {
				string sPredchozi = null;
				foreach (string item in iglsRadky) {
					if (blnVynechatDuplicitni) {
						if (String.Compare(sPredchozi, item, false) != 0) {
							sw.WriteLine(item);
							sPredchozi = item;
						}
					}
					else
						sw.WriteLine(item);
				}
			}
		}


		void CteckaSouboru_NactenyRadek(object sender, RadekEventArgs ev) {
			mglsRadky.Add(ev.Text);
		}



		public void NactiSouborPoRadcich() {
			if (ZacatekNacitani != null)
				ZacatekNacitani(this);
			using (StreamReader sr = new StreamReader(msbSoubor.CelaCesta, mcsnNastaveni.Kodovani)) {
				string sRadek = null;
				int i = 0;
				while ((sRadek = sr.ReadLine()) != null) {
					RadekEventArgs rea = new RadekEventArgs(sRadek, ++i);
					if (mcsnNastaveni.VynechatPrazdneRadky) {
						if (sRadek.Length > 0)
							NactenyRadek(this, rea);
						else
							i--;
					}
					else {
							NactenyRadek(this, rea);
					}
					
				}
			}
			if (KonecNacitani != null)
				KonecNacitani(this);
		}
	}



	/*
	public class CteckaSouboru<T> where T : IZaznam, IComparable, new() {
	
		private List<T> glstSeznam;

		public void NactiSouborPoRadcich() {
			T t = new T();
			string strZaznam = "";
			t.ZpracujZaznam(strZaznam);
			glstSeznam.Sort();

		}

	}
	*/
}
