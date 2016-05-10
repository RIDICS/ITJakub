using System;

namespace Daliboris.Pomucky.Rozsireni {


	namespace Chars {
		public static class ExtensionMethods {
			public static bool JeVokal(this char chZnak) {
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

			public static bool JeASCII(this char chZnak)
			{
				return chZnak >= 0 && chZnak < 128;
			}

			/// <summary>
			/// Hexadecimální kód znaku
			/// </summary>
			/// <param name="chZnak"></param>
			/// <returns></returns>
			public static string HexadecimalniKod(this char chZnak) {
				return String.Format("{0:x2}", ((int)chZnak));
			}
		}

	}
}
