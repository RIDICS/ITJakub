using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Xsl;
using Daliboris.Texty.Export.Rozhrani;
using NLog;
using Ujc.Ovj.Tools.Xml.XsltTransformation;

//using Ujc.Ovj.Tools.Xml.XsltTransformation;

//using Ujc.Ovj.Tools.Xml.XsltTransformation;

namespace Daliboris.Texty.Export {
 public class TransformacniKrok : ITransformacniKrok {


	private static Logger _logger = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// Cesta k souboru šablony XSLT
	/// </summary>
	public string Sablona { get; set; }

	private string mstrSouborXslt;

	/// <summary>
	/// Cesta k souboru XML, který se bude transformovat
	/// </summary>
	public string Soubor {
	 get { return mstrSouborXslt; }
	 set {
		mstrSouborXslt = value;
		NactiTransformacniSablonu();
	 }
	}

	private XslCompiledTransform mxsltSablona;
	private IXsltTransformer xsltTransformer;

/*
	private XslCompiledTransform TransformacniSablona {
	 get { return mxsltSablona; }
	 set { mxsltSablona = value; }
	}
*/

	/// <summary>
	/// Seznam parametrů a jejich hodnot, které se budou předávat šabloně XSLT
	/// </summary>
	public Dictionary<string, string> Parametry { get; set; }

	/// <summary>
	/// Provede transformaci vstupního souboru a výsledek uloží do výstupního souboru
	/// </summary>
	/// <param name="strVstup">Vstupní soubor, který se bude transformovat</param>
	/// <param name="strVystup">Celá cesta k výstupnímu soubru, do něhož se uloží výsledek transformace</param>
	public void Transformuj(string strVstup, string strVystup) {
	 Transformuj(strVstup, strVystup, Parametry);
	}

	/// <summary>
	/// Provede transformaci vstupního souboru a výsledek uloží do výstupního souboru
	/// </summary>
	/// <param name="strVstup">Vstupní soubor, který se bude transformovat</param>
	/// <param name="strVystup">Celá cesta k výstupnímu soubru, do něhož se uloží výsledek transformace</param>
	/// <param name="gdcParametry">Seznam parametrů, které se předají transformační šabloně</param>
	public void Transformuj(string strVstup, string strVystup, Dictionary<string, string> gdcParametry) {

	 _logger.Debug("Sablona={0}; Nazev={1}; Verze={2}; Vstup={3}; Vystup={4}", Sablona, xsltTransformer.Name, xsltTransformer.Version, strVstup, strVystup);

	 NameValueCollection parameters = new NameValueCollection();
	 if (gdcParametry != null)
	 {
		foreach (KeyValuePair<string, string> keyValuePair in gdcParametry) {
		 parameters.Add(keyValuePair.Key, keyValuePair.Value);
		}
	 }
	 xsltTransformer.Transform(strVstup, strVystup, parameters);

	 /*
	 XmlWriterSettings xws = new XmlWriterSettings();
	 xws.CloseOutput = true;

	 XmlWriter xw = XmlWriter.Create(strVystup, xws);
	 XsltArgumentList xal = new XsltArgumentList();
	 Dictionary<string, string> gdcParam = gdcParametry;

	 if (gdcParam != null && gdcParam.Count > 0) {

		foreach (KeyValuePair<string, string> kvp in gdcParam) {
		 xal.AddParam(kvp.Key, "", kvp.Value);
		}
	 }
	 mxsltSablona.Transform(strVstup, xal, xw);
	 xw.Close();
	 */ 
	}

	public TransformacniKrok() { }

	/// <summary>
	/// Konstruktor objektu
	/// </summary>
	/// <param name="strSablona">Seznam parametrů a jejich hodnot, které se budou předávat šabloně XSLT</param>
	public TransformacniKrok(string strSablona) {
	 Sablona = strSablona;
	}

	/// <summary>
	/// Konstruktor objektu
	/// </summary>
	/// <param name="strSablona">Cesta k souboru šablony XSLT</param>
	/// <param name="gdcParametry">Seznam parametrů a jejich hodnot, které se budou předávat šabloně XSLT</param>
	public TransformacniKrok(string strSablona, Dictionary<string, string> gdcParametry)
	 : this(strSablona) {
	 Parametry = gdcParametry;
	}

	/// <summary>
	/// Konstruktor objektu
	/// </summary>
	/// <param name="strSablona">Cesta k souboru šablony XSLT</param>
	/// <param name="strSoubor">Cesta k souboru XML, který se bude transformovat</param>
	public TransformacniKrok(string strSablona, string strSoubor)
	 : this(strSablona) {
	 Soubor = strSoubor;
	}

	/// <summary>
	/// Konstruktor objektu
	/// </summary>
	/// <param name="strSablona">Cesta k souboru šablony XSLT</param>
	/// <param name="strSoubor">Cesta k souboru XML, který se bude transformovat</param>
	/// <param name="gdcParametry">Seznam parametrů a jejich hodnot, které se budou předávat šabloně XSLT</param>
	public TransformacniKrok(string strSablona, string strSoubor, Dictionary<string, string> gdcParametry)
	 : this(strSablona, strSoubor) {
	 Parametry = gdcParametry;
	}

	private void NactiTransformacniSablonu() {
	 if (Soubor == null)
		throw new ArgumentNullException("Soubor", "Cesta k souboru šablony není nastavena.");
	 if (!File.Exists(Soubor))
		throw new FileNotFoundException("Šablona XSLT '" + Soubor + "' neexistuje.");

		//xsltTransformer = XsltTransformerFactory.GetXsltTransformer(Soubor);
	 /*
	 XsltSettings xsltSettings = new XsltSettings(true, false);
	 XmlResolver xrr = new XmlUrlResolver();
	 mxsltSablona = new XslCompiledTransform();
	 mxsltSablona.Load(Soubor, xsltSettings, xrr);
	 */
	}

 }
}
