using System;

namespace Daliboris.Pomucky.Funkce.Textove {
	public static class Znaky {

		/// <summary>
		/// Zjišťuje, zda je zadaný znak vokál (v české abecedě)
		/// </summary>
		/// <param name="chZnak"></param>
		/// <returns></returns>
		public static bool JeVokal(char chZnak) {
			bool bJeVokal = false;
			switch (chZnak) {
				case 'a':
				case 'á':
				case 'e':
				case 'é':
				case 'ě':
				case 'i':
				case 'í':
				case 'o':
				case 'ó':
				case 'u':
				case 'ú':
				case 'ů':
					bJeVokal = true;
					break;
				default:
					break;
			}
			return bJeVokal;
		}

		/// <summary>
		/// Zjišťuje, zda zadaný znak patří k základní sadě ASCII
		/// </summary>
		/// <param name="chZnak"></param>
		/// <returns></returns>
		public static bool JeASCII(char chZnak) {
			return chZnak >= 0 && chZnak < 128;
		}

		/// <summary>
		/// Hexadecimální kód znaku
		/// </summary>
		/// <param name="chZnak"></param>
		/// <returns></returns>
		public static string HexadecimalniKod(char chZnak) {
			return String.Format("{0:x2}", ((int)chZnak));
		}

	}
}
