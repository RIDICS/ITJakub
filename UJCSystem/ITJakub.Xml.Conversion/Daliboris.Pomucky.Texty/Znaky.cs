namespace Daliboris.Pomucky.Texty {
	public class Znaky {
		/// <summary>
		/// Označuje začátek hlavičky souboru.
		/// </summary>
		public const char ZacatekHlavicky = '\u0001';
		/// <summary>
		/// Vyznačuje začátek samotného obsahu v souboru; následuje po hlavičce.
		/// </summary>
		public const char ZacatekObsahu = '\u0002';
		/// <summary>
		/// Vyznačuje konec obsahu v souboru.
		/// </summary>
		public const char KonecObsahu = '\u0003';

		/// <summary>
		/// Znak oddělující jednotlivá pole na jednom řádku; např. slovo a jeho frekvenci.
		/// </summary>
		public const char OddelovacPoli = '\u001d';
		/// <summary>
		/// Znak oddělující jednotlivé hodnoty v rámci jednoho pole; např. doklady na jeden trigram.
		/// </summary>
		public const char OddelovacHodnot = '\u001f';



	}

}
