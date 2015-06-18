using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IPredloha
	{
		[XmlAttribute("typ")]
		TypPredlohy Typ { get; set; }

		string Popis();
	}
}