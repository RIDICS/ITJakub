using System;
using System.Xml.Serialization;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IExport
	{

        /// <summary>
        /// Do jakého výstupu byl export proveden
        /// </summary>
		ZpusobVyuziti ZpusobVyuziti { get; set; }

        /// <summary>
        /// Čas, kdy export proběhl 
        /// </summary>
		DateTime CasExportu { get; set; }

        /// <summary>
        /// Hash exporotvaného souboru
        /// </summary>
        string KontrolniSoucet { get; set; }

        /// <summary>
        /// Název souboru bez přípony
        /// </summary>
		[XmlIgnore]
		string NazevSouboru { get; set; }

		string ToString();

		bool Equals(IExport other);
	}
}