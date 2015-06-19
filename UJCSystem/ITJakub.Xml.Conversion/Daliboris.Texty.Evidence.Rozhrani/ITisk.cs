namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface ITisk : IPramen
	{
		/// <summary>
		/// Tiskař díla
		/// </summary>
		string Tiskar { get; set; }

		/// <summary>
		/// Místo tisku díla
		/// </summary>
		string MistoTisku { get; set; }

		/// <summary>
		/// Číslo knihopisu
		/// </summary>
		string Knihopis { get; set; }

		new string Popis();
	}
}