using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IPramen
	{
		/// <summary>
		/// Uzuální zkratka pramene
		/// </summary>
		string Zkratka { get; set; }

		/// <summary>
		/// Uložení tisku
		/// </summary>
		IUlozeni Ulozeni { get; set; }

		/// <summary>
		/// Datace tisku
		/// </summary>
		IDatace Datace { get; set; }

		[XmlAttribute("typ")]
		TypPredlohy Typ { get; set; }

		string Popis();
	}
}