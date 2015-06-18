using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Export.Rozhrani {
 public delegate void UpravaSouboruXml(string strVstup, string strVystup);

 public interface IUpravy {

	void Uprav(IPrepis prepis);

	IExportNastaveni Nastaveni { get; set; }

	/// <summary>
	/// Upraví hlavičku dokumentu (&lt;teiHeader&gt;).
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož hlavička se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na hlavičku aplikovat.</param>
	/// <returns>Vrací celou cestu k vytvořenému souboru.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že hlavička nic neobsahuje.</remarks>
	string UpravHlavicku(IPrepis prepis, List<ITransformacniKrok> kroky);

	/// <summary>
	/// Upraví úvod dokumentu (&lt;front&gt;), který obsahuje nadpis a autora.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož úvod se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na úvod aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru s úvodem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
	string UpravUvod(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// Upraví tělo dokumentu (&lt;body&gt;), které obsahuje hlavní text.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož tělo se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na tělo aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru s tělem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
	string UpravTelo(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);


	/// <summary>
	/// Upraví závěr dokumentu (&lt;back&gt;), které obsahuje závěrečné pasáže, např. informaci a autorských právech.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru se závěrem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že závěr nic neobsahuje.</remarks>
	string UpravZaver(IPrepis prepis, List<ITransformacniKrok> kroky);

	/// <summary>
	/// Úpraví spojený dokument, slouží zejména k přesunům elementů mezi částmi &lt;front&gt;, &lt;body&gt; a &lt;back&gt;.
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož sloučené části se mají upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na sloučené části aplikovat.</param>
	/// <returns>Vrací celou cestu k souboru se obměněným souborem.</returns>
	/// <remarks>Pokud vrátí null, znamená to, že obměněný soubor nebyl vytvořen.</remarks>
	string UpravPoSpojeni(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

 	/// <summary>
 	/// Zkombinuje jednotlivé části textu.
 	/// </summary>
 	/// <param name="strHlavicka">Soubor s textem hlavičky (&lt;teiHeader&gt;)</param>
 	/// <param name="strUvod">Soubor s textem úvodu (&lt;front&gt;)</param>
 	/// <param name="strTelo">Soubor s hlavním textem (&lt;body&gt;)</param>
 	/// <param name="strZaver">Soubor s textem na závěr (&lt;back&gt;)</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na slučované části aplikovat.</param>
 	/// <returns>>Pokud vrátí null, znamená to, že zkombinovaný soubor nebyl vytvořen.</returns>
	string ZkombinujCastiTextu(IPrepis prepis, string strHlavicka, string strUvod, string strTelo, string strZaver, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// Přiřadí identifikátory důležitým prvkům
	/// </summary>
	/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
	/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
	/// <returns>Vrací celou cestu k přetransformovanému souboru.</returns>
	string PriradId(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="prepis"></param>
	/// <param name="strVstup"></param>
	/// <param name="upravy"></param>
	/// <returns></returns>
	string ProvedUpravy(IPrepis prepis, string strVstup, IEnumerable<UpravaSouboruXml> upravy);

	/// <summary>
	/// Vrací název výstupu pro zadaný přepis.
	/// </summary>
	/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
	/// <returns>Název výstupu pro zadaný přepis</returns>
	/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
 	string DejNazevVystupu(IPrepis prepis);

	/// <summary>
	/// Vrací název předchozího výstupu pro zadaný přepis.
	/// </summary>
	/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
	/// <returns>Název předchozího výstupu pro zadaný přepis</returns>
	/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
	string DejNazevPredchozihoVystupu(IPrepis prepis);

	int PocetKroku { get; }

	void SmazDocasneSoubory();

	void NastavVychoziHodnoty();

	string DocasnaSlozka { get; }
 }

}
