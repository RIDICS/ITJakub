using System.Collections.Generic;

namespace Daliboris.Texty.Export.Rozhrani
{
  public interface ITransformacniKrok
  {

	  

	/// <summary>
	/// Pojmenování šablony XSLT
	/// </summary>
	string Sablona { get; set; }

	/// <summary>
	/// Cesta k souboru šablony XSLT
	/// </summary>
	string Soubor { get; set; }

	/// <summary>
	/// Seznam parametrů a jejich hodnot, které se budou předávat šabloně XSLT
	/// </summary>
	Dictionary<string, string> Parametry { get; set; }

	/// <summary>
	/// Provede transformaci vstupního souboru a výsledek uloží do výstupního souboru
	/// </summary>
	/// <param name="strVstup">Vstupní soubor, který se bude transformovat</param>
	/// <param name="strVystup">Celá cesta k výstupnímu souboru, do něhož se uloží výsledek transformace</param>
  	void Transformuj(string strVstup, string strVystup);

	/// <summary>
	/// Provede transformaci vstupního souboru a výsledek uloží do výstupního souboru
	/// </summary>
	/// <param name="strVstup">Vstupní soubor, který se bude transformovat</param>
	/// <param name="strVystup">Celá cesta k výstupnímu soubru, do něhož se uloží výsledek transformace</param>
	/// <param name="gdcParametry">Seznam parametrů, které se předají transformační šabloně</param>
  	void Transformuj(string strVstup, string strVystup, Dictionary<string, string> gdcParametry);

	 
  }
}