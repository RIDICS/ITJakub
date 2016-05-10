using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Daliboris.Pomucky.Rozsireni.Strings;
using Daliboris.Pomucky.Rozsireni.Chars;
using System.Linq;

namespace Daliboris.UJC.OVJ.StcS.Slovnik {

	public static class Pomucky {
		private static Regex RegexZavorky = new Regex(
	 "\\(\r\n    (?>\r\n        [^()]+ \r\n    )*\r\n    (?(number)(?!))\r\n" +
	 "\\)\r\n",
	 RegexOptions.IgnoreCase
	 | RegexOptions.CultureInvariant
	 | RegexOptions.IgnorePatternWhitespace
	 | RegexOptions.Compiled
);

		public static string[] RozepsatVsechnyZavorky(string sVyraz) {
			Dictionary<string, string> dcsRozepsane = new Dictionary<string, string>();
			Dictionary<string, string> dcsNove = new Dictionary<string, string>();
			Dictionary<string, string> dcsVynechat = new Dictionary<string, string>();
			dcsRozepsane.Add(sVyraz, sVyraz);
			bool bObsahujeZavorky = true;
			while (bObsahujeZavorky) {
				dcsNove = new Dictionary<string, string>();
				bObsahujeZavorky = false;
				foreach (string strKey in dcsRozepsane.Keys) {
					if (dcsRozepsane[strKey].Contains("(") && !dcsVynechat.ContainsKey(strKey)) {
						bObsahujeZavorky = true;
						Dictionary<string, string> dcTemp = RozepsatZavorky(dcsRozepsane[strKey]);
						foreach (string sk in dcTemp.Keys) {
							if (!dcsNove.ContainsKey(sk)) {
								dcsNove.Add(sk, sk);
							}
						}
						dcsVynechat.Add(strKey, strKey);
					}
				}
				foreach (string sk in dcsNove.Keys) {
					dcsRozepsane.Add(sk, sk);
				}
			}
			string[] astrRozepsane = new string[dcsRozepsane.Keys.Count - dcsVynechat.Keys.Count];
			int i = 0;
			foreach (string sk in dcsRozepsane.Keys) {
				if (!dcsVynechat.ContainsKey(sk)) {
					astrRozepsane[i++] = sk;
				}
			}
			return astrRozepsane;
		}

		private static Dictionary<string, string> RozepsatZavorky(string sVyraz) {
			Dictionary<string, string> dcsRozepsane = new Dictionary<string, string>();
			MatchCollection ms = RegexZavorky.Matches(sVyraz);
			if (ms.Count == 0) {
				dcsRozepsane.Add(sVyraz, sVyraz);
				return dcsRozepsane;
			}
			foreach (Match m in ms) {

				StringBuilder sbVysledek = new StringBuilder(sVyraz.Length);

				sbVysledek.Append(sVyraz.Substring(0, m.Index));
				sbVysledek.Append(sVyraz.Substring(m.Index + 1, m.Length - 2));
				sbVysledek.Append(sVyraz.Substring(m.Index + m.Length));

				if (!dcsRozepsane.ContainsKey(sbVysledek.ToString())) {
					dcsRozepsane.Add(sbVysledek.ToString(), sbVysledek.ToString());
				}

				sbVysledek = new StringBuilder(sVyraz.Length);
				sbVysledek.Append(sVyraz.Substring(0, m.Index));
				sbVysledek.Append(sVyraz.Substring(m.Index + m.Length));

				if (!dcsRozepsane.ContainsKey(sbVysledek.ToString())) {
					dcsRozepsane.Add(sbVysledek.ToString(), sbVysledek.ToString());
				}

			}
			return (dcsRozepsane);
		}
		public static string RozepisPomlcku(string sHeslo, string sZkracenaPodoba) {
			//int iDelkaShody = 0;
			string sNoveHeslo = null;
			int[] aiShoda = new int[] { 0, 0 };
			string sZbytek = null;

			if (sZkracenaPodoba.StartsWith("-")) {
				//je třeba hladat shodu na konci slova: pětmezidsietma | -mědcítma
				sZbytek = sZkracenaPodoba.Substring(1);

				aiShoda = ShodaSufixu2(sZbytek, sHeslo);
				if (aiShoda[1] == -1) {
					if (aiShoda[0] > -1)
						sNoveHeslo = sHeslo.Substring(0, aiShoda[0]) + sZbytek;
					else
						sNoveHeslo = sZkracenaPodoba;
				}
				else
					if (aiShoda[0] == -1) {
						if (aiShoda[1] >= 0)
							sNoveHeslo = sHeslo.Substring(0, sHeslo.Length - sZbytek.Length)
								+ sZbytek;
						else
							sNoveHeslo = sHeslo + " + " + sZkracenaPodoba;
					}
					else
						sNoveHeslo = sHeslo.Substring(0, aiShoda[0]) + sZbytek;

				return (sNoveHeslo);
			}
			if (sZkracenaPodoba.EndsWith("-")) {
				//je třeba hledat shodu na žačátku slova: povzdvihánie | pozdv- 
				sZbytek = sZkracenaPodoba.Substring(0, sZkracenaPodoba.Length - 1);
				aiShoda = ShodaPrefixu(sZbytek, sHeslo);
				sNoveHeslo = sZbytek + sHeslo.Substring(aiShoda[1] + 1);
				return sNoveHeslo;
			}

			string[] aText = sZkracenaPodoba.Split(new char[] { '-' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (aText.Length == 1)
				return sZkracenaPodoba;

			if (aText[1].Length == 1) {
				if (aText[1][0].JeVokal() & sHeslo[sHeslo.Length - 2].JeVokal()) {
					sNoveHeslo = aText[0] + sHeslo.Substring(aText[0].Length) + aText[1];
				}
				else
					sNoveHeslo = aText[0] + sHeslo.Substring(aText[0].Length, sHeslo.Length - 2) + aText[1];
			}
			else {
				aiShoda = ShodaSufixu(aText[1], sHeslo);
				if (aiShoda[1] == 0) {
					sNoveHeslo = sZkracenaPodoba;
				}
				else
					sNoveHeslo = aText[0] +
						 sHeslo.Substring(aText[0].Length, aiShoda[1] - 1)
						 + aText[1].Substring(aiShoda[0]);
			}
			return (sNoveHeslo);
		}

		/// <summary>
		/// Vrací pozice ve slovech, kde jsou shodná písmena
		/// </summary>
		/// <param name="sPrefix">Začátek slova, který se porovnává</param>
		/// <param name="sSlovo">Slovo, u nějž se hledá shoda</param>
		/// <returns>Pozice písmen, která jsou v prefixu a ve slově shodná</returns>
		private static int[] ShodaPrefixu(string sPrefix, string sSlovo) {
			int[] aiShodaZacatek = new int[] { 0, 0 };
			int[] aiShodaKonec = new int[] { 0, 0 };
			//povzdvihánie | pozdv- 
			//požitčenie | požič-, pojič-, pójč-
			//je potřeba najít písmeno na začátku, kde se hesla slova shodují,
			//a pak na konci, kde se opět začínají shodovat

			//porovnávají se začátky slov, písmena na stejných pozicích
			for (int i = 0; i < sPrefix.Length; i++) {
				//písmena na stejné pozici se liší, tj. shodná byla předchozí písmena
				if (sPrefix[i] != sSlovo[i]) {
					aiShodaZacatek[0] = i - 1;
					break;
				}
			}
			//slovo a sufix nemají na začátku žádnou shodu - může to nastat? asi ano: ostrý : vo-
			if (aiShodaZacatek[0] == -1) { return aiShodaZacatek; }
			for (int i = aiShodaZacatek[0] + 1; i < sSlovo.Length; i++) {
				if (sSlovo[i] == sPrefix[sPrefix.Length - 1]) {
					aiShodaZacatek[1] = i;
					//pro případ povzdvihánie | pozdv-
					if (i >= sPrefix.Length)
						break;
				}
			}
			//pro případy typu podržěti | podd-
			if (aiShodaZacatek[1] == 0) {
				aiShodaZacatek[1] = aiShodaZacatek[0];
			}

			return aiShodaZacatek;
		}

		private static int[] ShodaSufixu2(string sSufix, string sSlovo) {
			int[] aiShoda = new int[] { -1, -1 };
			//aiShoda[0] = pozice ve slově, kde se shoduje se sifixem
			// -1 = nebyla nalezena shoda sufixu na začátku sufixu

			//aiShoda[1] = pozice v sufixu od konce, kde je shoda se slovem
			//-1 = shoda nebyla nalezena
			string sSufixRev = sSufix.Reverze();
			string sSlovoRev = sSlovo.Reverze();

			//kontroluje se shoda zakončení slov (na obrácených formách výrazů)
			for (int i = 0; i < sSufixRev.Length; i++) {
				if (sSufixRev[i] != sSlovoRev[i]) {
					//slova se shodují na oředchozí pozici (tj. pozice od konce)
					aiShoda[1] = i - 1;
					break;
				}
			}

			int iMaxShoda = aiShoda[1] == -1 ? 0 : aiShoda[1];
			string[] asRozdeleneSlovo = sSlovo.Substring(0, sSlovo.Length - iMaxShoda).Split(new char[] { sSufix[0] }, System.StringSplitOptions.None);
			if (asRozdeleneSlovo.Length <= 1) {
				aiShoda[0] = -1;
				return aiShoda;
			}

			Dictionary<int, int> gdc = new Dictionary<int, int>();
			for (int i = 0; i < sSlovo.Length - iMaxShoda; i++) {
				if (sSlovo[i] == sSufix[0]) {
					gdc.Add(i, sSlovo.Substring(i).PocetShodnychZnaku(sSufix));
				}
			}

			int iMax = 0;
			foreach (KeyValuePair<int, int> kvp in gdc) {
				if (kvp.Value >= iMax) {
					aiShoda[0] = kvp.Key;
				}
			}

			return aiShoda;

			//int iMax = gdc.Max().Key;

			/*
			int[] aiPoctyShodnychZnaku = new int[asRozdeleneSlovo.Length];

			//kontroluje se shoda následujících písmen ze sufixu
			for (int j = 0; j < asRozdeleneSlovo.Length; j++) {
				if (j == 0)
					aiPoctyShodnychZnaku[j] = -1;
				else {
					if (asRozdeleneSlovo[1].Length > 0)
						aiPoctyShodnychZnaku[j] = asRozdeleneSlovo[j].PocetShodnychZnaku(sSufix.Substring(1));
				}
			}

			//int iMax = aiPoctyShodnychZnaku.Max();
			int iDelka = 0;
			for (int i = 0; i < aiPoctyShodnychZnaku.Length; i++) {
				if (aiPoctyShodnychZnaku[i] == iMax) {
					for (int j = 0; j < i; j++) {
						iDelka += (string.IsNullOrEmpty(asRozdeleneSlovo[i]) ? 1 : asRozdeleneSlovo[i].Length);
					}
					aiShoda[0] = iDelka;
				}
			}
			*/

			/*
			for (int i = 0; i < sSufix.Length; i++) {

				for (int j = sSlovo.Length - 1; j >= 0; j--) {
					if (sSlovo[j] == sSufix[i]) {
						//TODO: nejspíš udělat matici shod
						aiShoda[0] = i;
						aiShoda[1] = j;
						return (aiShoda);
					}
				}
			}
			 */

			return aiShoda;
		}



		private static int[] ShodaSufixu(string sSufix, string sSlovo) {
			int[] aiShoda = new int[] { 0, 0 };
			for (int i = 0; i < sSufix.Length; i++) {

				for (int j = sSlovo.Length - 1; j >= 0; j--) {
					if (sSlovo[j] == sSufix[i]) {
						//TODO: nejspíš udělat matici shod
						aiShoda[0] = i;
						aiShoda[1] = j;
						return (aiShoda);
					}
				}
			}
			return (aiShoda);
		}
	}
}
