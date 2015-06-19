namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IPamatka
	{
		/// <summary>
		/// Autor díla
		/// </summary>
		string Autor { get; set; }

		/// <summary>
		/// Titul díla
		/// </summary>
		/// <summary>
		/// Je-li autor díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		bool UzualniAutor { get; }

		/// <summary>
		/// Titul díla
		/// </summary>
		string Titul { get; set; }

		/// <summary>
		/// Je-li titul díla uzuální, uvádí se v hranatých závorkách
		/// </summary>
		bool UzualniTitul { get; }

		/// <summary>
		/// Uzuální zkratka památky
		/// </summary>
		string Zkratka { get; set; }

		/// <summary>
		/// Literární druh popisující památku
		/// <example>próza, verš</example>
		/// </summary>
		string LiterarniDruh { get; set; }

		/// <summary>
		/// Literární žánr památky
		/// <example>cestopis</example>
		/// </summary>
		string LiterarniZanr { get; set; }

		/// <summary>
		/// Datace díla
		/// </summary>
		IDatace Datace { get; set; }

		/// <summary>
		/// Pramen památky
		/// </summary>
		IPramen Pramen { get; set; }

		/// <summary>
		/// Edice památky (nebo spíše pramene)
		/// </summary>
		IEdice Edice { get; set; }
	}
}