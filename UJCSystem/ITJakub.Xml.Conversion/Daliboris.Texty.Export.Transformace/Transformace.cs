using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export
{
  public class Transformace
  {
	private ITransformaceNastaveni _mnstNastaveni;



	public ITransformaceNastaveni Nastaveni
	{
	  get { return _mnstNastaveni; }
	  set { _mnstNastaveni = value; }
	}

	public Transformace() { }
	public Transformace(ITransformaceNastaveni nstNastaveni)
	{
	  Nastaveni = nstNastaveni;
	}

	public void Transformuj(TransformaceNastaveni nstNastaveni)
	{
	  Nastaveni = nstNastaveni;
	  Transformuj();
	}

      /// <summary>
      /// Vrací seznam transformačních souborů (XSLT) pro daný identifikátor.
      /// </summary>
      /// <param name="identifikator">Indentifikátor oblasti, jejíž šablony se mají načíst.</param>
      /// <returns></returns>
    public List<string> DejTransformacniSoubory(string identifikator)
      {
          if (Nastaveni.TransformacniSoubor == null) return null;
          List<string> kroky = new List<string>();

          XmlDocument document = new XmlDocument();
          document.Load(Nastaveni.TransformacniSoubor);
          XmlNode transformattion = document.SelectSingleNode("//transformation[@xml:id='" + identifikator  + "']");
          if (transformattion == null) return null;
          if (transformattion.Attributes != null)
          {
              string slozka = transformattion.Attributes["directory"].Value;
              XmlNodeList nodeList = transformattion.SelectNodes(".//step");
              if (nodeList != null)
                  foreach (XmlNode node in nodeList)
                  {
                      if (node.Attributes != null) kroky.Add(Path.Combine(slozka, node.Attributes["file"].Value));
                  }
          }
          return kroky;
      }

	public void Transformuj()
	{
	  string strDocasnaSlozka = _mnstNastaveni.DocasnaSlozka ?? Path.GetTempPath();

	  Dictionary<string, XslCompiledTransform> gdxc = NactiTransformacniKroky();
	  DirectoryInfo di = new DirectoryInfo(_mnstNastaveni.VstupniSlozka);
	  const string csMaskaVsechnySoubory = "*.*";
	  FileInfo[] fis = di.GetFiles(_mnstNastaveni.MaskaSoubru ?? csMaskaVsechnySoubory);

	  foreach (FileInfo fi in fis)
	  {

		List<string> glsVystupy = new List<string>(_mnstNastaveni.TransformacniKroky.Count);
		string strNazev = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
		Console.WriteLine(strNazev);
		int iKrok = 0;
		string strVstup = fi.FullName;
		foreach (TransformacniKrok krok in _mnstNastaveni.TransformacniKroky)
		{
		  iKrok++;

		  string strVystup = Path.Combine(strDocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, iKrok));
		  glsVystupy.Add(strVystup);

		  if (krok.Parametry != null)
		  {
			//foreach (KeyValuePair<string, string> kvp in krok.Parametry)
			//{
			//  switch (kvp.Key)
			//  {
			//    case "soubor":

			//  }
			//}
		  }
		  gdxc[krok.Sablona].Transform(strVstup, strVystup);
		  strVstup = strVystup;

		}

		File.Copy(strVstup, Path.Combine(_mnstNastaveni.VystupniSlozka, strNazev + ".xml"), true);
		if (_mnstNastaveni.SmazatDocasneSoubory)
		{
		  OdstranitSouboryZDisku(glsVystupy);
		}
	  }

	}

	private void OdstranitSouboryZDisku(List<string> glsVystupy)
	{
	  foreach (string sVystup in glsVystupy)
	  {
		try
		{
		  if (sVystup != null) File.Delete(sVystup);
		}
		catch (FileNotFoundException e)
		{
		}
	  }
	}

	private Dictionary<string, XslCompiledTransform> NactiTransformacniKroky()
	{
	  Dictionary<string, XslCompiledTransform> gdcx =
		  new Dictionary<string, XslCompiledTransform>(_mnstNastaveni.TransformacniKroky.Count);

	  foreach (TransformacniKrok krok in _mnstNastaveni.TransformacniKroky)
	  {
		XsltSettings xsltSettings = new XsltSettings(true, false);
		XmlUrlResolver resolver = new XmlUrlResolver();
		resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

		XslCompiledTransform xct = new XslCompiledTransform();
	  	xct.OutputSettings.CloseOutput = true;
		xct.Load(krok.Soubor, xsltSettings, resolver);
		gdcx.Add(krok.Sablona, xct);
	  }
	  return gdcx;
	}

  }
}
