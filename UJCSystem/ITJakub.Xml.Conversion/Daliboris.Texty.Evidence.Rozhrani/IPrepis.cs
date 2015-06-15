using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IPrepis
	{
		event PropertyChangedEventHandler PropertyChanged;
		IHlava Hlava { get; set; }

		/// <summary>
		/// Informace o přepsaném díle a prameni
		/// </summary>
		/// 
//[XmlElement("hlavicka")]
		IHlavicka Hlavicka { get; set; }

		/// <summary>
		/// Informace o formátu a uložení dokumentu na disku
		/// </summary>
		/// 
//[XmlElement("soubor")]
		ISoubor Soubor { get; set; }

		/// <summary>
		/// Informace o stavu zpracování dokumentu
		/// </summary>
		/// 
//[XmlElement("zpracovani")]
		IZpracovani Zpracovani { get; set; }

		string Autor { get; }
		string Titul { get; }
		string Signatura { get; }

		[XmlIgnore]
		ZpusobVyuziti ZpusobVyuziti { get; set; }

		[XmlIgnore]
		FazeZpracovani FazeZpracovani { get; set; }

		[XmlIgnore]
		CasoveZarazeni CasoveZarazeni { get; set; }

		[XmlIgnore]
		string AutorTextu { get; set; }

		[XmlIgnore]
		string TitulTextu { get; set; }

		string TitulBezZavorek { get; }

		/// <summary>
		/// Období vzniku pramane (s periodou 50 let)
		/// </summary>
		string ObdobiVzniku { get; set; }

		string NazevSouboru { get; }
		DateTime Zmeneno { get; }
		string KontrolniSoucet { get; }
		string DataceText { get; }
		IDatace DataceDetaily { get; }
		int Identifikator { get; set; }

		[XmlIgnore]
		StatusAktualizace Status { get; set; }

		[XmlIgnore]
		IPrepis PuvodniPodoba { get; set; }

		[XmlIgnore]
		string GUID { get; set; }

		[XmlIgnore]
		string Komentar { get; set; }

		[XmlIgnore]
		bool Neexportovat { get; set; }

		[XmlIgnore]
		string ZkratkaPamatky { get; set; }

		[XmlIgnore]
		string ZkratkaPramene { get; set; }

		[XmlIgnore]
		string LiterarniDruh { get; set; }

		[XmlIgnore]
		string LiterarniZanr { get; set; }


		/// <summary>
		/// Zda se jedná o edici, slovník, mluvnici, odbornou literaturu
		/// </summary>
		string TypPrepisu { get; set; }

		bool Equals(object obj);
		int GetHashCode();
		object Clone();
		void PromitniZmenyDoHlavy();
	}
}