namespace Daliboris.Texty.Evidence
{
	public enum TypTextovehoPrvku {
		/// <summary>
		/// Dosud neurčený nebo neurčitelný typ textového prvku
		/// </summary>
		Neurceno,

		/// <summary>
		/// Spojovací prvek mezi dvěma výrazy: a, nebo
		/// </summary>
		Konektor,

		/// <summary>
		/// Upřesnění datace: okolo, přelom, začátek, konec
		/// </summary>
		Upresneni,

		/// <summary>
		/// Označení uvedeného období: (30.) léta, století, (okolo) roku
		/// </summary>
		Obdobi,

		/// <summary>
		/// Označení části delšího období (století) pomocí zlomku: třetina, čtvrtina, (1.) polovina
		/// </summary>
		Zlomek,

		/// <summary>
		/// Jednomístné číslo, označení poloviny, třetiny, čtvrtiny
		/// </summary>
		JednomistneCislo,

		/// <summary>
		/// Dvojmístné číslo, označení desetiletí nebo století
		/// </summary>
		DvojmistneCislo,

		/// <summary>
		/// Čtyřmístné číslo, označení roku
		/// </summary>
		CtyrmistneCislo
	}
}