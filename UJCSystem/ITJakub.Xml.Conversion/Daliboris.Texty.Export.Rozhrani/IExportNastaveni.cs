using System;
using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Export.Rozhrani
{
	public interface IExportNastaveni : ITransformaceNastaveni
	{
        
        /// <summary>
        /// Přepis, který se bude exportovat
        /// </summary>
        IPrepis Prepis { get; set; }

	 /// <summary>
	 /// Soubor s metadaty o přepisech
	 /// </summary>
		string SouborMetadat { get; set; }

	 /// <summary>
	 /// Schéma XSD, které slouží k validaci vygenerovaného souboru
	 /// </summary>
		string ValidacniXsd { get; set; }

	 /// <summary>
	 /// Čas exportu, který se má nastavit (slouží pro nastavení stejného času u více dokumentů)
	 /// </summary>
		DateTime CasExportu { get; set; }

	 /// <summary>
	 /// Zda se má zaevidovat provedený export
	 /// </summary>
		bool Evidovat { get; set; }

		/// <summary>
		/// Zda se má vypisovat na konzolu/do logu všechny informace. Pokud je <value>false</value>, vypisují se jenom chyby.
		/// </summary>
		bool VypisovatVse { get; set; }

		/// <summary>
		/// Soubor popisující Xslt transformace využívané knihovnou XsltTransformation.
		/// </summary>
		string SouborTransformaci { get; set; }
	}
}