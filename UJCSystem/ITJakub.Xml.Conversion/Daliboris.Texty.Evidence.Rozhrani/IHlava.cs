using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IHlava
	{
		/// <summary>
		/// Památka zachycená v přepisu
		/// </summary>
		IPamatka Pamatka { get; set; }

		/// <summary>
		/// Předloha sloužící pro přepis (rukopis, tisk, edice)
		/// </summary>
		IPredloha Predloha { get; set; }

		/// <summary>
		/// Editoři elektronického přepisu
		/// </summary>
		IEditori Editori { get; set; }

		/// <summary>
		/// Poznámka editorů k přepisu
		/// </summary>
		string Poznamka { get; set; }

		/// <summary>
		/// Textová ukázka přepisu
		/// </summary>
		string Ukazka { get; set; }
	}
}