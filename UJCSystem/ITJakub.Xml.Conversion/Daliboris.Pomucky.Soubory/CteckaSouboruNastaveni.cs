using System.Text;

namespace Daliboris.Pomucky.Soubory {
	public class CteckaSouboruNastaveni {
		private Encoding mkdKodovani = Encoding.UTF8;

		/// <summary>
		/// Kódování souboru. Výchozí kódování je UTF8.
		/// </summary>
		public Encoding Kodovani {
			get {
				return mkdKodovani;
			}
			set {
				mkdKodovani = value;
			}
		}

		/// <summary>
		/// Určuje, jestli se mají (false) nebo nemají (true) načítat prázdné řádky.
		/// </summary>
		public bool VynechatPrazdneRadky { get; set; }
	}
}
