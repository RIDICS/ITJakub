using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IDatace
	{
		[XmlAttribute("slovniPopis")]
		string SlovniPopis { get; set; }
		[XmlAttribute("stoleti")]
		int Stoleti { get; set; }
		[XmlAttribute("polovinaStoleti")]
		int PolovinaStoleti { get; set; }
		[XmlAttribute("desetileti")]
		int Desetileti { get; set; }
		[XmlAttribute("rok")]
		int Rok { get; set; }
		[XmlAttribute("upresneni")]
		string Upresneni { get; set; }
		[XmlAttribute("relativniChronologie")]
		int RelativniChronologie { get; set; }
		[XmlAttribute("nePredRokem")]
		int NePredRokem { get; set; }
		[XmlAttribute("nePoRoce")]
		int NePoRoce { get; set; }

		/// <summary>
		/// Přibližné umístění na časové ose (v rozmezí 25 let)
		/// </summary>
		int CasovyOtisk { get; }

		string ToString();
		void AnalyzovatSlovniPopis(string sSlovniPopis);
		int CompareTo(object obj);
		object Clone();
	}
}