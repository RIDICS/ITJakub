using System.Collections.Generic;
using Daliboris.Pomucky.Databaze.Zaznamy;

namespace Daliboris.Pomucky.Soubory {
	public class CteckaZaznamu<T> : CteckaSouboru where T : IZaznam<T>, new() {
		private List<T> glstZaznamy;

		public CteckaZaznamu() { }
		public CteckaZaznamu(string strSoubor) : base(strSoubor) { }
		public CteckaZaznamu(Soubor sbSoubor) : base(sbSoubor) { }
		public CteckaZaznamu(Soubor sbSoubor, CteckaSouboruNastaveni csbNastaveni) : base(sbSoubor, csbNastaveni) { }

		public List<T> Zaznamy {
			get { return glstZaznamy; }
		}

		public new void NactiSouborPoRadcich() {
			base.NactenyRadek += new Radek(CteckaZaznamu_NactenyRadek);
			base.ZacatekNacitani += new Zacatek(CteckaZaznamu_ZacatekNacitani);
			base.NactiSouborPoRadcich();
		}

		void CteckaZaznamu_ZacatekNacitani(object sender) {
			glstZaznamy = new List<T>(base.Soubor.PocetRadku);
		}

		void CteckaZaznamu_NactenyRadek(object sender, RadekEventArgs ev) {
			T t = new T();
			t.NactiZaznam(ev.Text);
			glstZaznamy.Add(t);
		}
	}
}
