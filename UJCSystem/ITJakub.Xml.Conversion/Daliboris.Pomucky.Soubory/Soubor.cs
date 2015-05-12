using System;
using System.IO;
using System.Diagnostics;

namespace Daliboris.Pomucky.Soubory {

	/// <summary>
	/// Základní informace o textovém souboru
	/// </summary>
	[DebuggerDisplay("{CelaCesta}")]
	public class Soubor {
		private string mstrNazevBezPripony;
		int mintPocetRadku = -1;

		/// <summary>
		/// Název souboru (včetně přípony)
		/// </summary>
		public string Nazev { get; set; }

		/// <summary>
		/// Název složky, v níž je soubor uložen
		/// </summary>
		public string Slozka { get; set; }

		/// <summary>
		/// Počet řádků v souboru
		/// </summary>
		public int PocetRadku {
			get {
				if (mintPocetRadku == -1)
					SpocitejRadky();
				return mintPocetRadku;
			}
		}

		/// <summary>
		/// Datum a čas poslední změny souboru
		/// </summary>
		public DateTime PosledniZmena { get; set; }

		#region Kontruktory

		public Soubor(string strSlozka, string strNazev)
			: this(Path.Combine(strSlozka, strNazev)) {
		}

		public Soubor(string strCelaCesta)
			: this(new FileInfo(strCelaCesta)) {
		}

		public Soubor(FileInfo fiInformaceOSouboru) {
			PriraditSlozkuANazev(fiInformaceOSouboru);
			PosledniZmena = fiInformaceOSouboru.LastWriteTime;
			mstrNazevBezPripony = fiInformaceOSouboru.Name.Substring(0, fiInformaceOSouboru.Name.Length - fiInformaceOSouboru.Extension.Length);
		}

		#endregion

		/// <summary>
		/// Název souboru bez přípony
		/// </summary>
		public string NazevBezPripony {
			get {
				if (mstrNazevBezPripony == null) {
					FileInfo fi = new FileInfo(CelaCesta);
					return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
				}
				else {
					return mstrNazevBezPripony;
				}
			}
		}


		private void PriraditSlozkuANazev(FileInfo fi) {
			PriraditSlozkuANazev(fi.DirectoryName, fi.Name);
		}

		private void PriraditSlozkuANazev(string strSlozka, string strNazev) {
			Slozka = strSlozka;
			Nazev = strNazev;
			SpocitejRadky();
		}

		private void SpocitejRadky() {
			mintPocetRadku = -1;
			if (!File.Exists(CelaCesta))
				return;
			using (StreamReader sr = new StreamReader(CelaCesta)) {
				while (sr.ReadLine() != null)
					mintPocetRadku++;
			}
			mintPocetRadku++;
		}

		public string CelaCesta { get { return (Path.Combine(Slozka, Nazev)); } }

		public static string NazevSouboruBezPripony(string strCelaCesta) {
			FileInfo fi = new FileInfo(strCelaCesta);
			return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
		}


	}
}
