using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Daliboris.Texty.Evidence.Rozhrani;
using System.Xml.Xsl;
using Daliboris.Texty.Export.Rozhrani;

namespace Daliboris.Texty.Export
{
	internal partial class EdicniModulUpravy : Upravy
	{

		private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		const string cstrNazevVystupuFormat = "{0}_{1:00}.xml";
		private List<string> mglsVystupy;
		private string mstrDocasnaSlozka;


		public EdicniModulUpravy()
		{
		}

		public EdicniModulUpravy(IExportNastaveni emnNastaveni)
			: base(emnNastaveni)
		{
		}

		/*
			private void UpravPuvodni(IPrepis prepis) {



			 string strDocasnaSlozka = Nastaveni.DocasnaSlozka ?? Path.GetTempPath();

			 Dictionary<string, XslCompiledTransform> gdxc = NactiTransformacniKroky();
			 foreach (IPrepis prp in Nastaveni.Prepisy) {

				string sCesta = Path.Combine(Nastaveni.VstupniSlozka, prp.Soubor.Nazev);
				sCesta = sCesta.Replace(".docx", ".xml");
				FileInfo fi = new FileInfo(sCesta);
				if (!fi.Exists) {
				 throw new FileNotFoundException("Zadaný soubor '" + sCesta + "' neexistuje.");
				}

				List<string> glsVystupy = new List<string>(Nastaveni.TransformacniKroky.Count);
				string strNazev = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

				int iKrok = 0;
				string strVstup = sCesta;
				foreach (TransformacniKrok krok in Nastaveni.TransformacniKroky) {
				 iKrok++;

				 string strVystup = Path.Combine(strDocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, iKrok));
				 glsVystupy.Add(strVystup);

				 if (krok.Parametry != null) {
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
			 }
			}
		*/

		/// <summary>
		/// Načte jednotlivé transformační kroky (šablony XSLT)
		/// </summary>
		/// <returns>Vrací slovník se šablonami, které jsou přístupné prostřednictvím textového identifikátoru</returns>
		private Dictionary<string, XslCompiledTransform> NactiTransformacniKroky()
		{
			Dictionary<string, XslCompiledTransform> gdcx =
				new Dictionary<string, XslCompiledTransform>(Nastaveni.TransformacniKroky.Count);

			foreach (TransformacniKrok krok in Nastaveni.TransformacniKroky)
			{
				XsltSettings xsltSettings = new XsltSettings(true, false);
				XslCompiledTransform xct = new XslCompiledTransform();
				xct.Load(krok.Soubor, xsltSettings, null);
				gdcx.Add(krok.Sablona, xct);
			}
			return gdcx;
		}


		/// <summary>
		/// Upraví jeden přepis, transformuje ho do podoby pro ediční modul
		/// </summary>
		/// <param name="prp">Přepis (informace o něm)</param>
		/// <param name="emnNastaveni">Nastavení exportu pro ediční modul</param>
		/// <param name="glsTransformacniSablony">Seznam transformačních šablon XSLT, které se při úpravách použijí</param>
		public static void Uprav(IPrepis prp, IExportNastaveni emnNastaveni, List<string> glsTransformacniSablony)
		{

			//
			string sCesta = Path.Combine(emnNastaveni.VstupniSlozka, prp.Soubor.Nazev);
			sCesta = sCesta.Replace(".docx", ".xml").Replace(".doc", ".xml");
			FileInfo fi = new FileInfo(sCesta);
			if (!fi.Exists)
			{
				throw new FileNotFoundException("Zadaný soubor '" + sCesta + "' neexistuje.");
			}

			if (emnNastaveni.DocasnaSlozka == null)
				emnNastaveni.DocasnaSlozka = Path.GetTempPath();

			//Zpracovat hlavičku a front
			string strNazev = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);

			Console.WriteLine("Zpracovávám soubor '{0}'", strNazev);
			XmlWriter xw;


			string sFront = Path.Combine(emnNastaveni.SlozkaXslt, "Vytvorit_front_TEI.xsl");
			string sFrontVystup = Path.Combine(emnNastaveni.DocasnaSlozka, strNazev + "_front.xml");

			List<string> glsVystupy = new List<string>(glsTransformacniSablony.Count + 2);

			/*
			 UpravaSouboruXml uprava = new UpravaSouboruXml(UpravHlavicku);
			

			Dictionary<string, string> gdcParam = new Dictionary<string, string>();
			gdcParam.Add("soubor", prp.NazevSouboru);
			 TransformacniKrok tkh = new TransformacniKrok(Path.Combine(emnNastaveni.SlozkaXslt, "EM_Vytvorit_hlavicku_TEI.xsl"), emnNastaveni.SouborMetadat, gdcParam);
			*/

			string sHlavicka = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Vytvorit_hlavicku_TEI.xsl");
			string sHlavickaVystup = Path.Combine(emnNastaveni.DocasnaSlozka, strNazev + "_hlavicka.xml");

			XslCompiledTransform xctHlavick = new XslCompiledTransform();
			xctHlavick.Load(sHlavicka);
			XsltArgumentList xal = new XsltArgumentList();
			xal.AddParam("soubor", "", prp.NazevSouboru);

			xw = XmlWriter.Create(sHlavickaVystup);
			xctHlavick.Transform(emnNastaveni.SouborMetadat, xal, xw);
			xw.Close();

			glsVystupy.Add(sHlavickaVystup);

			XslCompiledTransform xctFront = new XslCompiledTransform();
			xctFront.Load(sFront);
			xw = XmlWriter.Create(sFrontVystup);
			xctFront.Transform(emnNastaveni.SouborMetadat, xal, xw);
			xw.Close();
			glsVystupy.Add(sFrontVystup);

			string strVstup = sCesta;
			string strVystup = null;
			for (int i = 0; i < glsTransformacniSablony.Count; i++)
			{

				strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, i));
				glsVystupy.Add(strVystup);

				XslCompiledTransform xslt = new XslCompiledTransform();
				Console.WriteLine("{0} = {1}", String.Format(cstrNazevVystupuFormat, strNazev, i), glsTransformacniSablony[i]);

				xslt.Load(glsTransformacniSablony[i]);
				xslt.Transform(strVstup, strVystup);
				strVstup = strVystup;
			}


			if (prp.LiterarniZanr == "biblický text")
			{
				strVstup = ZpracovatBiblickyText(emnNastaveni, strNazev, glsTransformacniSablony, glsVystupy, strVystup);
			}
			else
			{
				strVstup = ZpracovatOdkazyNaBiblickaMista(emnNastaveni, strNazev, glsTransformacniSablony, glsVystupy, strVystup);
			}



			string sSlouceni = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Spojit_s_hlavickou_a_front.xsl");//EBV


			strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, glsVystupy.Count));

			XsltSettings xsltSettings = new XsltSettings(true, false);
			XslCompiledTransform xctSlouceni = new XslCompiledTransform();

			xctSlouceni.Load(sSlouceni, xsltSettings, null);
			xal = new XsltArgumentList();
			xal.AddParam("hlavicka", "", sHlavickaVystup);
			xal.AddParam("zacatek", "", sFrontVystup);
			xw = XmlWriter.Create(strVystup);
			xctSlouceni.Transform(strVstup, xal, xw);
			xw.Close();
			strVstup = strVystup;
			glsVystupy.Add(strVystup);

			/*
			strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format("{0}_{1:00}.xml", strNazev, glsVystupy.Count));
			PresunoutMezeryVneTagu(strVstup, strVystup);
			glsVystupy.Add(strVystup);
			strVstup = strVystup;
		*/
			string sEdicniKomentar = Path.Combine(emnNastaveni.SlozkaXslt, "EM_Presunout_edicni_komentar.xsl");
			strVystup = Path.Combine(emnNastaveni.DocasnaSlozka, String.Format(cstrNazevVystupuFormat, strNazev, glsVystupy.Count));

			XmlUrlResolver xrl = new XmlUrlResolver();
			XslCompiledTransform xctEdicniKomentare = new XslCompiledTransform();
			xctEdicniKomentare.Load(sEdicniKomentar, xsltSettings, xrl);
			xw = XmlWriter.Create(strVystup);
			xctEdicniKomentare.Transform(strVstup, xal, xw);
			xw.Close();
			glsVystupy.Add(strVystup);

			strVstup = strVystup;
			strVystup = Path.Combine(emnNastaveni.VystupniSlozka, strNazev + ".xml");
			PresunoutMezeryVneTagu(strVstup, strVystup);


			//File.Copy(strVystup, Path.Combine(emnNastaveni.VystupniSlozka, strNazev + ".xml"),true);

		}


		internal static void PridatMezeryZaTagyPoInterpunkci(string vstup, string vystup)
		{
			XmlReaderSettings xrs = new XmlReaderSettings { ProhibitDtd = false };
			List<string> tagyPoInterpunkci = new List<string> { "note" };
			Pomucky.Xml.Transformace.PridatMezeryZaTagyPoInterpunkci(vstup, vystup, xrs, tagyPoInterpunkci, ZnakyInterpunkce);

		}

		internal static void PresunoutMezeryVneTagu(string strVstupniSoubor, string strVystupniSoubor)
		{
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.ProhibitDtd = false;
			List<string> vynechaneTagy = new List<string>();
			vynechaneTagy.Add("supplied");
			vynechaneTagy.Add("add");
			vynechaneTagy.Add("foreign");
			vynechaneTagy.Add("hi");
			vynechaneTagy.Add("rdg");

			List<string> preskoceneTagy = new List<string>();
			preskoceneTagy.Add("note");
			preskoceneTagy.Add("seg");
			preskoceneTagy.Add("c");

			List<string> tagyPredNimizSeMezeraNepresouva = new List<string> { "supplied", "add", "foreign", "hi", "rgd", "anchor", "app", "seg"/*,  "gap"*/, "ins", "del" };
			List<string> tagyKdeSeMezeraPresouvaZaZnacku = new List<string> { "note", /* "seg",  "c", */ "app" };
			List<string> tagyKdeSePresouvaniVynechava = new List<string> { "c", "unclear" /*, "del", "ins" */ /*, "gap" */};
			List<string> tagyKdeSePresouvaniIgnoruje = new List<string>() {"ins", "del"};

			Pomucky.Xml.Transformace.PresunoutMezeryVneTagu(strVstupniSoubor, 
				strVystupniSoubor, 
				xrs, 
				tagyPredNimizSeMezeraNepresouva, 
				tagyKdeSeMezeraPresouvaZaZnacku,
				tagyKdeSePresouvaniVynechava,
				tagyKdeSePresouvaniIgnoruje,
				ZnakyInterpunkce);
/*
			using (XmlReader xr = XmlReader.Create(strVstupniSoubor, xrs))
			{
				using (XmlWriter xw = XmlWriter.Create(strVystupniSoubor))
				{
					//Pomucky.Xml.Transformace.PresunoutMezeryVneTagu(xr, xw, vynechaneTagy, preskoceneTagy);
					//Pomucky.Xml.Transformace.PresunoutMezeryVneTagu(strVstup, strVystup, xrs, vynechaneTagy);

				}
			}
 * */
		}

		public override string DejNazevVystupu(IPrepis prepis)
		{
			return Path.Combine(mstrDocasnaSlozka, String.Format(cstrNazevVystupuFormat, prepis.Soubor.NazevBezPripony, PocetKroku));
		}

		/// <summary>
		/// Vrací název předchozího výstupu pro zadaný přepis.
		/// </summary>
		/// <param name="prepis">Objekt, pro nějž se vrací název výstupu</param>
		/// <returns>Název předchozího výstupu pro zadaný přepis</returns>
		/// <remarks>Název se určuje podle aktuálního počtu zpracovaných souborů.</remarks>
		public override string DejNazevPredchozihoVystupu(IPrepis prepis)
		{
			return Path.Combine(mstrDocasnaSlozka, String.Format(cstrNazevVystupuFormat, prepis.Soubor.NazevBezPripony, PocetKroku - 1));
		}


		public override void NastavVychoziHodnoty()
		{
			mglsVystupy = new List<string>();
			if (Nastaveni == null || Nastaveni.DocasnaSlozka == null)
				mstrDocasnaSlozka = Path.GetTempPath();
			else
				mstrDocasnaSlozka = Nastaveni.DocasnaSlozka;
		}

		public override string DocasnaSlozka
		{
			get { return mstrDocasnaSlozka; }
		}

		public override void Uprav(IPrepis prepis)
		{
			/*
			string strHlavicka = UpravHlavicku(prepis, null);
			string strUvod = UpravUvod(prepis, null);
			string strTelo = UpravTelo(prepis, null);
			string strZaver = UpravZaver(prepis, null);
			*/

		}

		/// <summary>
		/// Upraví závěr dokumentu (&gt;back&lt;), které obsahuje závěrečné pasáže, např. informaci a autorských právech.
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
		/// <returns>Vrací celou cestu k souboru se závěrem.</returns>
		/// <remarks>Pokud vrátí null, znamená to, že závěr nic neobsahuje.</remarks>

		public override string UpravZaver(IPrepis prepis, List<ITransformacniKrok> kroky)
		{
			return null;
		}

		/// <summary>
		/// Zkombinuje jednotlivé části textu.
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, pro nějž se budou kombinovat jednotlivé části.</param>
		/// <param name="strHlavicka">Soubor s textem hlavičky (&lt;teiHeader&gt;)</param><param name="strUvod">Soubor s textem úvodu (&lt;front&gt;)</param><param name="strTelo">Soubor s hlavním textem (&lt;body&gt;)</param><param name="strZaver">Soubor s textem na závěr (&lt;back&gt;)</param><param name="kroky">Trnasformační kroky, které se mají na slučované části aplikovat.</param>
		/// <returns>
		/// &gt;Pokud vrátí null, znamená to, že zkombinovaný soubor nebyl vytvořen.
		/// </returns>
		public override string ZkombinujCastiTextu(IPrepis prepis, string strHlavicka, string strUvod, string strTelo, string strZaver, IEnumerable<ITransformacniKrok> kroky)
		{
			string sVstup = DejNazevPredchozihoVystupu(prepis);
			string sVystup = UpravVstupniSoubor(prepis, kroky, sVstup);
			return sVystup;
		}

		public override string UpravPoSpojeni(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			string sVstup = DejNazevPredchozihoVystupu(prepis);
			string sVystup = UpravVstupniSoubor(prepis, kroky, sVstup);
			return sVystup;
		}

		/// <summary>
		/// Přiřadí identifikátory důležitým prvkům
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož závěr se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na závěr aplikovat.</param>
		/// <returns>Vrací celou cestu k přetransformovanému souboru.</returns>
		public override string PriradId(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			throw new NotImplementedException();
		}

		public override string ProvedUpravy(IPrepis prepis, string strVstup, IEnumerable<UpravaSouboruXml> upravy)
		{
			string strVystup = null;
			string sVstup = strVstup;
			foreach (UpravaSouboruXml upravaSouboruXml in upravy)
			{
				strVystup = DejNazevVystupu(prepis);
				_logger.Debug("Metoda={0};Vstup={1};Vystup={2}", upravaSouboruXml.Method.Name, sVstup, strVystup);
				//Console.WriteLine("{0} => {1}, {2}", sVstup, strVystup, upravaSouboruXml.Method.Name);
				upravaSouboruXml(sVstup, strVystup);
				mglsVystupy.Add(strVystup);
				sVstup = strVystup;
			}

			return strVystup;
		}

		public override int PocetKroku
		{
			get { return mglsVystupy == null ? 0 : mglsVystupy.Count; }
		}

		public override void SmazDocasneSoubory()
		{
			foreach (string strSoubor in mglsVystupy)
			{
				if (File.Exists(strSoubor))
					File.Delete(strSoubor);
			}
		}




		/// <summary>
		/// Upraví tělo dokumentu (&gt;body&lt;), které obsahuje hlavní text.
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož tělo se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na tělo aplikovat.</param>
		/// <returns>Vrací celou cestu k souboru s tělem.</returns>
		/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
		public override string UpravTelo(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			string sVstup = Path.Combine(Nastaveni.VstupniSlozka, prepis.Soubor.NazevBezPripony + ".xml");
			string sVystup = UpravVstupniSoubor(prepis, kroky, sVstup);
			return UpravBiblickaMista(prepis, sVystup);
		}

		/// <summary>
		/// Upraví úvod dokumentu (&gt;front&lt;), který obsahuje nadpis a autora.
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož úvod se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na úvod aplikovat.</param>
		/// <returns>Vrací celou cestu k souboru s úvodem.</returns>
		/// <remarks>Pokud vrátí null, znamená to, že úvod nic neobsahuje.</remarks>
		public override string UpravUvod(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky)
		{
			string sVstup = Nastaveni.SouborMetadat;
			return UpravVstupniSoubor(prepis, kroky, sVstup);
		}

		/// <summary>
		/// Upraví hlavičku dokumentu (&gt;teiHeader&lt;).
		/// </summary>
		/// <param name="prepis">Objekt s přepisem, jehož hlavička se má upravovat.</param>
		/// <param name="kroky">Trnasformační kroky, které se mají na hlavičku aplikovat.</param>
		/// <returns>Vrací celou cestu k vytvořenému souboru.</returns>
		/// <remarks>Pokud vrátí null, znamená to, že hlavička nic neobsahuje.</remarks>
		public override string UpravHlavicku(IPrepis prepis, List<ITransformacniKrok> kroky)
		{
			string sVstup = Nastaveni.SouborMetadat;
			return UpravVstupniSoubor(prepis, kroky, sVstup);
		}

		private string UpravVstupniSoubor(IPrepis prepis, IEnumerable<ITransformacniKrok> kroky, string sVstup)
		{

			string sVystup = DejNazevVystupu(prepis);
			foreach (ITransformacniKrok krok in kroky)
			{
				Dictionary<string, string> gdcParametry = VyplnitParametry(krok.Parametry, prepis);
				//Console.WriteLine("{0} => {1}, {2}", sVstup, sVystup, krok.Sablona);
				krok.Transformuj(sVstup, sVystup, gdcParametry);
				mglsVystupy.Add(sVystup);
				sVstup = sVystup;
				sVystup = Path.Combine(mstrDocasnaSlozka, String.Format(cstrNazevVystupuFormat, prepis.Soubor.NazevBezPripony, PocetKroku));
			}
			return mglsVystupy[PocetKroku - 1];
		}

		private static Dictionary<string, string> VyplnitParametry(Dictionary<string, string> gdcParametry, IPrepis prepis)
		{
			if (gdcParametry == null)
				return null;
			Dictionary<string, string> gdc = new Dictionary<string, string>(gdcParametry);
			foreach (KeyValuePair<string, string> kvp in gdcParametry)
			{
				if (kvp.Value != null)
					continue;
				switch (kvp.Key)
				{
					case "soubor":
						gdc[kvp.Key] = prepis.Soubor.Nazev;
						break;

				}
			}
			return gdc;
		}

		/// <summary>
		/// Upraví zpracování biblických míst: u biblických textů vytvoří oddíly pro knihy, kapitoly a verše,
		/// u nebiblických textů vytvoří odkazy na biblický text 
		/// </summary>
		/// <param name="prepis">Informace o přepisu, kterého se úprava týká</param>
		/// <param name="strVstup">Celá cesta vstupního souboru</param>
		/// <returns>Vrací celou cestu k výstupnímu souboru</returns>
		public string UpravBiblickaMista(IPrepis prepis, string strVstup)
		{
			string strVystup = DejNazevVystupu(prepis);
			if (prepis.LiterarniZanr == "biblický text")
			{
				RozdelitBibliNaKnihyAKapitoly(strVstup, strVystup);
				mglsVystupy.Add(strVystup);
				string sVstup = strVystup;
				strVystup = DejNazevVystupu(prepis);
				RozdelitBibliNaVerse(sVstup, strVystup);
			}
			else
			{
				OdkazyNaBiblickyText(strVstup, strVystup);
			}
			return strVystup;
		}
	}
}
