using System.IO;

namespace Daliboris.Texty.Export {
 public class AdresarovaStruktura
 {
	public const string Xml = "XML";
	public const string Test = "Test";
	public const string Etalon = "Etalon";
	public const string Odeslano = "Odeslano";
	public const string Temp = "Temp";
	public const string Data = "Data";
	public const string Vystup = "Vystup";
	public const string SpolecneData = "_Data";
	public const string SpolecneTemp = "_Temp";
	public const string SpolecneDocXml = "_DocXml";

   /// <summary>
   /// Název složky s dokumenty po exportu z XML
   /// </summary>
	public const string DocXml = "DocXml";

   /// <summary>
   /// Výchozí složka společná pro všechny exporty
   /// </summary>
	public string VychoziSlozka { get; set; }

   /// <summary>
   /// Cíl exportu, např. Manuscriptorium, STB atp.
   /// </summary>
	public string Cil { get; set; }


	public AdresarovaStruktura(string vychoziSlozka, string strCil)
	{
		VychoziSlozka = vychoziSlozka;
		Cil = strCil;
	}

	 /// <summary>
	 /// Vytvoří podsložky ve výchozí složce, pokud neexistují
	 /// </summary>
	 public void VytvorStrukturu()
	 {
		 VytvorSlozku(DejXml);
		 VytvorSlozku(DejTest);
		 VytvorSlozku(DejEtalon);
		 VytvorSlozku(DejOdeslano);
		 VytvorSlozku(DejTemp);
		 VytvorSlozku(DejData);
		 VytvorSlozku(DejCil);
		 VytvorSlozku(DejVystup);
		 VytvorSlozku(DejSpolecneDocXml);
	 }

	 private void VytvorSlozku(string slozka)
	 {
		 if (!Directory.Exists(slozka))
			 Directory.CreateDirectory(slozka);
	 }

	public string DejXml { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil,Xml)); } }
	public string DejTest { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil,Test)); } }
	public string DejEtalon { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil,Etalon)); } }
	public string DejOdeslano { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil,Odeslano)); } }
	public string DejTemp { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil, Temp)); } }
	public string DejData { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil, Data)); } }
	public string DejCil { get { return Path.Combine(VychoziSlozka, Cil); } }
	public string DejVystup { get { return Path.Combine(VychoziSlozka, Path.Combine(Cil, Vystup)); } }
	


	public string DejSpolecneData { get { return Path.Combine(VychoziSlozka, SpolecneData); } }
	public string DejSpolecneTemp { get { return Path.Combine(VychoziSlozka, SpolecneTemp); } }
	public string DejSpolecneDocXml { get { return Path.Combine(VychoziSlozka, SpolecneDocXml); } }


 }
}
