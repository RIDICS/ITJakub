namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IHlavicka
	{
		/// <summary>
		/// Autor díla
		/// </summary>
		string Autor { get; set; }

		/// <summary>
		/// Titul díla
		/// </summary>
		string Titul { get; set; }

		IDatace DataceDetaily { get; }

		/// <summary>
		/// DataceText pramene
		/// </summary>
		string DataceText { get; set; }

		/// <summary>
		/// Tiskař díla
		/// </summary>
		string Tiskar { get; set; }

		/// <summary>
		/// Místo tisku díla
		/// </summary>
		string MistoTisku { get; set; }

		/// <summary>
		/// Typ předlohy přepisu
		/// </summary>
		TypPredlohy TypPredlohy { get; set; }

		/// <summary>
		/// Typ předlohy přepisu (textová podoba)
		/// </summary>
		string TypPredlohyText { get; set; }

		/// <summary>
		/// Země uložení rukopisu/tisku
		/// </summary>
		string ZemeUlozeni { get; set; }

		/// <summary>
		/// Město uložení rukopisu/tisku
		/// </summary>
		string MestoUlozeni { get; set; }

		/// <summary>
		/// Instituce uložení rukopisu/tisku
		/// </summary>
		string InstituceUlozeni { get; set; }

		/// <summary>
		/// Signatura rukopisu/tisku
		/// </summary>
		string Signatura { get; set; }

		/// <summary>
		/// Foliace/paginace rukopisu/tisku
		/// </summary>
		string FoliacePaginace { get; set; }

		/// <summary>
		/// Titul edice
		/// </summary>
		string TitulEdice { get; set; }

		/// <summary>
		/// Editor edice
		/// </summary>
		string EditorEdice { get; set; }

		/// <summary>
		/// Místo vydání edice
		/// </summary>
		string MistoVydaniEdice { get; set; }

		/// <summary>
		/// Rok vydání edice
		/// </summary>
		string RokVydaniEdice { get; set; }

		/// <summary>
		/// Strany edice
		/// </summary>
		string StranyEdice { get; set; }

		/// <summary>
		/// Editoři přepisu (textová podoba)
		/// </summary>
		string EditoriPrepisuText { get; set; }

		/// <summary>
		/// Editoři přepisu
		/// </summary>
		string[] EditoriPrepisu { get; set; }

		/// <summary>
		/// Poznámka editora k přepisu
		/// </summary>
		string Poznamka { get; set; }

		string Ukazka { get; set; }
		string Knihopis { get; set; }



		/// <summary>
		/// Je-li autor díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		bool UzualniAutor { get; }

		/// <summary>
		/// Je-li titul díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		bool UzualniTitul { get; }

		string PopisStarehoTisku { get; }
		string PopisUlozeni { get; }
		string PopisRukopisu { get; }
		string PopisEdice { get; }
		bool Equals(object obj);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		bool Equals(IHlavicka other);

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		int GetHashCode();
	}
}