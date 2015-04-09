namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IEdice : IPredloha
	{
		/// <summary>
		/// Titul edice
		/// </summary>
		string Titul { get; set; }

		/// <summary>
		/// Editoři edice
		/// </summary>
		IEditori Editori { get; set; }

		/// <summary>
		/// Místo vydání edice
		/// </summary>
		string MistoVydani { get; set; }

		/// <summary>
		/// Rok vydání edice
		/// </summary>
		string RokVydani { get; set; }

		/// <summary>
		/// Strany edice
		/// </summary>
		string Strany { get; set; }

		new string Popis();
	}
}