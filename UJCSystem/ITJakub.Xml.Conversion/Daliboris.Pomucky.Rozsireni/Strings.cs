using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Daliboris.Pomucky.Rozsireni.Chars;

namespace Daliboris.Pomucky.Rozsireni {
	namespace Strings {

		public static class ExtensionMethods {
			public static char PosledniZnak(this string sText) {
				if (String.IsNullOrEmpty(sText))
					return (new char());
				return (sText[sText.Length - 1]);
			}
			public static char PrvniZnak(this string sText) {
				if (String.IsNullOrEmpty(sText))
					return (new char());
				return (sText[0]);
			}
			public static bool ObsahujeZavorkyUprostredSlova(this string sText, string sZavorky) {
				if (String.IsNullOrEmpty(sText))
					return false;
				if ((sText.Contains(sZavorky[0])) & !((sText.PrvniZnak() == sZavorky[0]) & (sText.PosledniZnak() == (sZavorky[1]))))
					//pro případ, že text obsahuje jenom jednu závorku
					if (sText.IndexOf(sZavorky[0]) == -1 | sText.IndexOf(sZavorky[1]) == -1)
						return false;
					else
						return true;
				return false;
			}

			/// <summary>
			/// Odstraní diakritická znaménka písmen z textu
			/// </summary>
			/// <param name="sText">Text, z něhož se odstraní diakritická znaménka</param>
			/// <returns>Vrací text s písmeny bez diakritických znamének</returns>
			public static string OdstranitDiakritiku(this string sText) {
				String normalizedString = sText.Normalize(NormalizationForm.FormD);
				StringBuilder stringBuilder = new StringBuilder();

				for (int i = 0; i < normalizedString.Length; i++) {
					Char c = normalizedString[i];
					if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
						stringBuilder.Append(c);
				}

				return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
			}

			public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm, Func<char, char> customFolding)
			{
				foreach (char c in src.Normalize(compatNorm ? NormalizationForm.FormKD : NormalizationForm.FormD))
					switch (CharUnicodeInfo.GetUnicodeCategory(c))
					{
						case UnicodeCategory.NonSpacingMark:
						case UnicodeCategory.SpacingCombiningMark:
						case UnicodeCategory.EnclosingMark:
							//do nothing
							break;
						default:
							yield return customFolding(c);
							break;
					}
			}
			public static IEnumerable<char> RemoveDiacriticsEnum(string src, bool compatNorm)
			{
				return RemoveDiacritics(src, compatNorm, c => c);
			}
			public static string RemoveDiacritics(string src, bool compatNorm, Func<char, char> customFolding)
			{
				StringBuilder sb = new StringBuilder();
				foreach (char c in RemoveDiacriticsEnum(src, compatNorm, customFolding))
					sb.Append(c);
				return sb.ToString();
			}

			public static string RemoveDiacritics(string src, bool compatNorm)
			{
				return RemoveDiacritics(src, compatNorm, c => c);
			}


			/// <summary>
			/// Převede řetězec na znaky ASCII.
			/// </summary>
			/// <param name="sText">Text, který se má převést na znaky ASCII</param>
			/// <param name="chNahradniZnak">Znak ASCII, který nahradí nepodporované znaky ve vstupním textu</param>
			/// <returns>Vrací řetězec obsahující pouze znaky ASCII</returns>
			public static string PrevestNaASCII(this string sText, char chNahradniZnak) {
				for (int i = 0; i < sText.Length; i++) {
					if (sText[i] < 33 || sText[i] > 127)
						sText = sText.Replace(sText[i], chNahradniZnak);
				}
				return sText;
			}

			/// <summary>
			/// Určuje, zda řetězec obsahuje pouze znaky ASCII
			/// </summary>
			/// <param name="sText">Text, který se kontroluje na přítomnot znaků ASCII</param>
			/// <returns>Vrací True, pokud řetězec obsahuje pouze znaky ASCII, False, pokud řetězec obsahuje alespoň jeden znak mimo tabulku ASCII.</returns>
			public static bool JeASCII(this string sText) {
				foreach (char ch in sText) {
					if (!ch.JeASCII())
						return false;
				}
				return true;
			}

			public static string Reverze(this string sText, bool bZachovatSprezky) {
				StringBuilder sb = new StringBuilder(sText.Length);

				if (sText.ToLower().Contains("ch") & bZachovatSprezky) {
					for (int i = sText.Length - 1; i >= 0; i--) {
						if (i > 0 && sText[i] == 'h' | sText[i] == 'H') {
							if (sText[i - 1] == 'c' | sText[i - 1] == 'C') {
								sb.Append(sText[i - 1]);
								sb.Append(sText[i]);
								i--;
							}
							else
								sb.Append(sText[i]);
						}
						else
							sb.Append(sText[i]);
					}
				}
				else {
					//zkontrolovat ch, závorky ve slovech
					for (int i = sText.Length - 1; i >= 0; i--) {
						sb.Append(sText[i]);
					}
				}
				return sb.ToString();
			}

			public static string Reverze(this string sText) {
				return sText.Reverze(false);
			}

			public static int PocetShodnychZnaku(this string sText, string sSCim) {
				int iPocet = 0;
				for (int i = 0; i < (sText.Length < sSCim.Length ? sText.Length : sSCim.Length); i++) {
					if (sText[i] == sSCim[i])
						iPocet = i + 1;
					else
						break;
				}
				return iPocet;
			}

			/// <summary>
			/// Připojí k textu interpunkci včetně mezery, pokud následující text není prázdný
			/// </summary>
			/// <param name="sText">Text, k němuž se připojuje interpunkce</param>
			/// <param name="sInterpunkce">Interpunkční znaménko připojované za text</param>
			/// <param name="sNasledujiciText">Text, který bude následovat za aktuálním textem</param>
			/// <returns>Pokud následující text není prázdný, vrátí text s interpunkcí a mezerou, v opačném případě vrátí text</returns>
			public static string PropojitInterpunkci(this string sText, string sInterpunkce, string sNasledujiciText) {
				if (sInterpunkce == null)
					throw new NullReferenceException("Parametr sInterpunkce nemůže být null");
				if (String.IsNullOrEmpty(sNasledujiciText))
					return sText + sInterpunkce.Trim();
				return sText + sInterpunkce.Trim() + ' ';
			}
			public static int PocetVyskytu(this string strText, char chZnak) {
				int iPocet = 0;
				for (int i = 0; i < strText.Length; i++) {
					if (strText[i] == chZnak)
						iPocet++;
				}
				return iPocet;
			}
			public static int PocetVyskytu(this string strText, char[] chZnaky) {
				int iPocet = 0;
				for (int i = 0; i < strText.Length; i++) {
					if (chZnaky.Contains(strText[i]))
						iPocet++;
				}
				return iPocet;
			}
		}
	}
}