using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Daliboris.OOXML.Pomucky;
using Daliboris.OOXML.Word.Transform;
using Ujc.Ovj.Tools.Xml.XsltTransformation;
using Ujc.Ovj.Editing.TextProcessor.MsWord.TemplateBuilding.Builders;

namespace Test
{
	class Program
	{
		/// <summary>
		/// Složka, v níž jsou uloženy dokumentu s metadaty o mluvnicích ve formátu DOCX
		/// </summary>
		private const string SlozkaDocx = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Word\DOCX";

		/// <summary>
		/// Složka dropboxu, v níž jsou uloženy dokumentu s metadaty o mluvnicích ve formátu DOCX. Z této složky
		/// </summary>
		private const string SlozkaDocxOriginal = @"D:\Data\Dropbox\MDM\Docx\";

		/// <summary>
		/// Složka, v níž jsou uloženy transformační šablony Xslt, které zajišŤují převod ze základního Xml do strukturovaného
		/// </summary>
		private const string SlozkaXslt = @"V:\MDM\Evidence\Xslt\";

		/// <summary>
		/// Složka, v níž jsou umístěny soubory v základním Xml po transformaci z formátu DOCX
		/// </summary>
		private const string SlozkaXml = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Xml\";

		/// <summary>
		/// Složka, v níž jsou umístěny dokumenty s metadaty o mluvnicích ve formátu XML podle standardu TEI P5
		/// </summary>
		private const string SlozkaTei = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\TEI\";

		/// <summary>
		/// Složka, v níž jsou místěny dokumenty s daty o mluvnicích, které se budou importovat do modulu digitalizovaných mluvnic
		/// </summary>
		private const string SlozkaMdm = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\MDM\";

		/// <summary>
		/// Složka, v níž jsou uloženy dokumenty XML s anotacemi jendotlivých digitálních stránek mluvnic
		/// </summary>
		private const string SlozkaFaksimile = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Facsimile\";

        private const string SlozkaFaksimileCopied = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Facsimile\";

        ///<summary>
        /// Složka, v níž jsou uloženy dokumenty XML s anotacemi jendotlivých digitálních stránek mluvnic
        /// </summary>
        private const string SlozkaFaksimileOriginal = @"T:\Prameny\Fotokopie\MLUVNICE\Anotovane MLUVNICE\";

		/// <summary>
		/// TRansformační pravidla, která převedou z formátu DOCX všechny použité styly na jednoduché XML.
		/// </summary>
		private const string VsechnyStyly = @"V:\Projekty\Daliboris\OOXML\Data\Prevody\Vsechny_styly.2xml";

		private delegate void ZpracovatSouborXml(FileInfo souborXml);

		private delegate void ZpracovatSouboryXml(string slozka, string maska, ZpracovatSouborXml zpracovani);


		static void Main(string[] args)
		{

			//KontrolaExistenceSouboruAObrazku();
			// StazeniAktualnichFaksimili();
			//KontrolaFacsimileAObrazku();
			// ZkontrolujJedinecneId();

			// PrejmenovatSouboryCharakteristik();
			// PrejmenovatNazvySouboruVeFacsimile(@"V:\MDM\SkenyXml");
			//PrejmenovatNazvySouboruVeFacsimile(SlozkaFaksimile);
			//PrejmenovatSlozku();
			//PrejmenovatSlozku2();
			//VygenerovatSiFacsimile();



			TransformujZDocxNaKompletniXml();

		    //TestujCustomXmlPart();

			//ZjistitAktualniDavky();

			//TestOdlisitStejnePojmenovanaPaginy();

			//MdsVytvorDavkuProVodoznak();


			//ZkopirujObrazkyProDavku();
			//VytvorSlozkyProDavku();

			//ZkopirovatChybejiciSlozkyObrazkuProWebNaLokalniDisk();

			Console.WriteLine("Hotovo");

			Console.ReadLine();
		}

	    private static void TestujCustomXmlPart()
	    {
	        CustomXmlPartTest test = new CustomXmlPartTest();
            test.TestAdding();
            test.TestExtracting();
	    }

	    private static void ZjistitAktualniDavky()
		{
			string slozkaVW = @"W:\VWObrazky\MDM";
			string slozkaDavek = @"V:\MDM\Obrazky\_Watermark\Davky";

			DirectoryInfo vwInfo = new DirectoryInfo(slozkaVW);
			DirectoryInfo davkyInfo = new DirectoryInfo(slozkaDavek);

			FileInfo[] davky = davkyInfo.GetFiles("*.cmd", SearchOption.TopDirectoryOnly);
			foreach (FileInfo davka in davky)
			{
				string nazev = Path.GetFileNameWithoutExtension(davka.FullName);
				if (!Directory.Exists(Path.Combine(slozkaVW, nazev)))
				{
					Console.WriteLine(davka.FullName);
				}
			}

		}

		private static void TestOdlisitStejnePojmenovanaPaginy()
		{
			FileInfo file = new FileInfo(@"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\MDM\StajerZpusob1781_test.xml");
			//file = new FileInfo(@"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\MDM\HankaMluv1822_test.xml");
			OdlisitStejnePojmenovanaPaginy(file);
		}

		private static void MdsVytvorDavkuProVodoznak()
		{
			//string soubor = @"D:\Slovniky\JgSlov\Text\Xml\Import\Data\JgSlov_01_clean.xml";
			//FileInfo fileInfo = new FileInfo(soubor);
			//VytvorDavkuProVodoznak(fileInfo);

			string vychoziSlozka = @"D:\Slovniky\JgSlov\Obrazky\";


			string cilovaSlozka = @"D:\Slovniky\JgSlov\Obrazky\_Web\";

			string slozkaCmd = @"D:\Slovniky\JgSlov\Obrazky\_Watermark\";

			for (int i = 1; i <= 5; i++)
			{
				string nazev = String.Format("JgSlov{0:00}", i);

				VytvoritDavkuProVodoznak(vychoziSlozka + nazev, "Vodoznak_UJC", cilovaSlozka + nazev, slozkaCmd + nazev + ".cmd");

			}
		}

		private static void PrejmenovatSlozku2()
		{
			string vychoziSlozka = @"";
			vychoziSlozka = @"T:\Prameny\Fotokopie\MLUVNICE\Skeny - pod starymi zkratkami\";
			vychoziSlozka = @"T:\Prameny\Fotokopie\MLUVNICE\Anotovane MLUVNICE\";

			PrejmenovatObrazky(vychoziSlozka + "HanPravop1839", "HankaPrav1839");
			PrejmenovatObrazky(vychoziSlozka + "PelzelGrund1795", "PelclGrund1795");
			PrejmenovatObrazky(vychoziSlozka + "TomsaZeit1804", "TomsaBedeut1804");
		}

		/// <summary>
		/// Zkopíruje pro každý soubor faksimile složku s obrázky ze síťového disku (pokud ještě neexistuje).
		/// </summary>
		private static void ZkopirovatChybejiciSlozkyObrazkuProWebNaLokalniDisk()
		{
			DirectoryInfo tei = new DirectoryInfo(SlozkaTei);
			string sitovaSpolecna = @"T:\Prameny\Fotokopie\MLUVNICE\Anotovane MLUVNICE";

			FileInfo[] soubory = tei.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				string nazevBezPripony = NazevBezPripony(soubor.Name);
				DirectoryInfo obrazky = new DirectoryInfo(Path.Combine(@"V:\MDM\Obrazky\_Web", nazevBezPripony));
				if (!obrazky.Exists)
				{
					//Directory.CreateDirectory(obrazky.FullName);
					DirectoryCopy(Path.Combine(sitovaSpolecna, nazevBezPripony), obrazky.FullName, false);
					Console.WriteLine("Zkopírována složka {0}.", nazevBezPripony);
					//Console.WriteLine("!!! Složka s obrázky {0} neexistuje.", obrazky.FullName);
				}
			}

		}

		/// <summary>
		/// Provede všechny kroky nutné k vytvoření finálního Xml dokumentu pro import do databáze z dokumentů DOCX.
		/// </summary>
		private static void TransformujZDocxNaKompletniXml()
		{
			bool kopirovatDocx = true;
			if (kopirovatDocx)
				ZkopirovatOriginalniDocx();
			else
				VypsatUpozorneni("Kopírování originálních dokumentů s charakteristikami je nyní zakázáno.");

			ZkopirovatOriginalniFaksimile();
		    VlozitFaksimileDoDocx();

            TransformovatDocxNaXml(); //?
			TransformovatXmlNaTei();

		    ExtrahovatFaksimileZDocx();

			TransformovatTeiNaMdm();
			UpravitPerex();

			ZpracujSouboryXml(SlozkaMdm, "*.xml", ZkontrolujCislaPagin);
			ZpracujSouboryXml(SlozkaMdm, "*.xml", OdlisitStejnePojmenovanaPaginy);
			ZpracujSouboryXml(SlozkaMdm, "*.xml", UpravOdkazyNaPaginy);



			ZpracujSouboryXml(SlozkaMdm, "*.xml", VytvorDavkuProVodoznak);
			VypsatUpozorneni(String.Format("Zkopírujte soubory ze složky '{0}' na síťovou složku pro import:\r\n{1}.", SlozkaMdm, @"W:\Install\Vokabular\Moduly\Import\MDM\Data"));
		}

	    private static void ExtrahovatFaksimileZDocx()
	    {
            string fileMask = "*.docx";
            List<string> fileNames = GetFileNames(SlozkaDocx, fileMask);

            foreach (string fileName in fileNames)
            {
                string docxFile = Path.Combine(SlozkaDocx, fileName + ".docx");
                string facsimileFile = Path.Combine(SlozkaFaksimile, fileName + ".xml");
                CustomXmlPartExtractorSettings settings = new CustomXmlPartExtractorSettings();
                settings.ContentTypeId = "CD052837-B125-4941-B89D-25B5995A92D6";
                settings.DataStoreItemId = "5AD717C3-0CB1-42C1-BEB6-34308825C1B6";
                settings.DocxFilePath = docxFile;
                settings.CustomXmlFilePath = facsimileFile;
                CustomXmlPartExtractor extractor = new CustomXmlPartExtractor(settings);
                extractor.Extract();
            }
        }

	    /// <summary>
        /// Vloží do suoboru DOCX odpovídající XML s faksimile
        /// (na základě shodného jména souboru bez přípony).
        /// </summary>
	    private static void VlozitFaksimileDoDocx()
	    {
            string fileMask = "*.docx";
            List<string> fileNames = GetFileNames(SlozkaDocx, fileMask);

            foreach (string fileName in fileNames)
            {
                string docxFile = Path.Combine(SlozkaDocx, fileName + ".docx");
                string facsimileFile = Path.Combine(SlozkaFaksimile, fileName + ".xml");
                CustomXmlPartBuilderSettings settings = new CustomXmlPartBuilderSettings();
                settings.ContentTypeId = "CD052837-B125-4941-B89D-25B5995A92D6";
                settings.DataStoreItemId = "5AD717C3-0CB1-42C1-BEB6-34308825C1B6";
                settings.XmlPartFilePath = facsimileFile;
                settings.XsdFilePath = Path.Combine(SlozkaFaksimile, "TEI.xsd");

                CustomXmlPartBuilder builder = new CustomXmlPartBuilder(settings);
                builder.DocumentFilePath = docxFile;
                builder.Build();


            }



        }

	    private static void ZkopirovatOriginalniFaksimile()
		{
			string fileMask = "*.docx";
			List<string> fileNames = GetFileNames(SlozkaDocxOriginal, fileMask);

			string extension = ".xml";
			string sourceDirecotry = SlozkaFaksimileOriginal;
	        string destinationDirecory = SlozkaFaksimileCopied; //SlozkaFaksimile;

            CopyFilesToDirectory(fileNames, sourceDirecotry, extension, destinationDirecory);
		}

		private static void ZkopirovatOriginalniDocx()
		{

			string fileMask = "*.docx";
			List<string> fileNames = GetFileNames(SlozkaDocxOriginal, fileMask);

			string extension = ".docx";
			string sourceDirecotry = SlozkaDocxOriginal;
			string destinationDirecory = SlozkaDocx;


			CopyFilesToDirectory(fileNames, sourceDirecotry, extension, destinationDirecory);
		}

		/// <summary>
		/// Kopíruje soubory (podle seznamu názvů) z výchozí do cílové složky.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <param name="sourceDirecotry"></param>
		/// <param name="extension"></param>
		/// <param name="destinationDirecory"></param>
		private static void CopyFilesToDirectory(List<string> fileNames, string sourceDirecotry, string extension,
												 string destinationDirecory)
		{
			foreach (string fileName in fileNames)
			{
				FileInfo file = new FileInfo(Path.Combine(sourceDirecotry, fileName + extension));
				if (file.Exists)
				{
					FileInfo destination = new FileInfo(Path.Combine(destinationDirecory, fileName + extension));
					if (file.LastWriteTimeUtc > destination.LastWriteTimeUtc)
					{
						Console.WriteLine("Zkopírován soubor {0}", file.Name);
						file.CopyTo(destination.FullName, true);
					}
				}
				else
				{
					VypsatUpozorneni(String.Format("Soubor {0} neexistuje.", file.FullName));
				}
			}
		}

		public static List<string> GetFileNames(string directoryPath, string fileMask)
		{
			DirectoryInfo directory = new DirectoryInfo(directoryPath);
			FileInfo[] files = directory.GetFiles(fileMask, SearchOption.TopDirectoryOnly);
			List<string> names = new List<string>(fileMask.Length);
			foreach (FileInfo file in files)
			{
				names.Add(file.Name.Substring(0, file.Name.Length - file.Extension.Length));
			}
			return names;
		}

		/// <summary>
		/// Zkontroluje, zda soubory obsahují jedinečný identifikátor.
		/// </summary>
		private static void ZkontrolujJedinecneId()
		{
			DirectoryInfo facs = new DirectoryInfo(@"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\MDM");
			FileInfo[] fileInfos = facs.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
			XNamespace tei = "http://www.tei-c.org/ns/1.0";
			XNamespace xml = "http://www.w3.org/XML/1998/namespace";

			SortedDictionary<string, string> guids = new SortedDictionary<string, string>();

			foreach (FileInfo fileInfo in fileInfos)
			{
				XDocument document = XDocument.Load(fileInfo.FullName);
				//string guid = document.Root.Descendants(tei + "fileDesc").Attributes("n").FirstOrDefault().Value;
				//string guid = document.Root.Descendants(tei + "teiHeader").Attributes("n").FirstOrDefault().Value;
				string guid = document.Root.Descendants(tei + "teiHeader").Attributes(xml + "id").FirstOrDefault().Value;
				if (guids.ContainsKey(guid))
				{
					VypsatUpozorneni(String.Format("Identifikátor {0} již existuje: {1}, {2}", guid, fileInfo.Name, guids[guid]));
				}
				else
				{
					guids.Add(guid, fileInfo.Name);
				}
			}

			foreach (KeyValuePair<string, string> guid in guids)
			{
				Console.WriteLine("{0} = {1}", guid.Key, guid.Value);
			}
		}


		private static void VytvorSlozkyProDavku()
		{
			DirectoryInfo docx = new DirectoryInfo(SlozkaDocx);
			FileInfo[] soubory = docx.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				DirectoryInfo obrazky = new DirectoryInfo(Path.Combine(@"V:\MDM\Obrazky\_Web", NazevBezPripony(soubor.Name)));
				if (!obrazky.Exists)
				{
					Directory.CreateDirectory(obrazky.FullName);
					//Console.WriteLine("!!! Složka s obrázky {0} neexistuje.", obrazky.FullName);
				}
				else
				{
					//vymazat soubory ze složky?
					//DirectoryCopy(obrazky.FullName, @"V:\MDM\Obrazky\Aktualni\" + obrazky.Name, true);
				}
			}
		}

		private static void ZkopirujObrazkyProDavku()
		{
			DirectoryInfo docx = new DirectoryInfo(SlozkaDocx);
			FileInfo[] soubory = docx.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				DirectoryInfo obrazky = new DirectoryInfo(Path.Combine(@"V:\MDM\Prevod", NazevBezPripony(soubor.Name)));
				if (!obrazky.Exists)
				{
					chyba = true;
					Console.WriteLine("!!! Složka s obrázky {0} neexistuje.", obrazky.FullName);
				}
				else
				{
					DirectoryCopy(obrazky.FullName, @"V:\MDM\Obrazky\Aktualni\" + obrazky.Name, true);
				}
			}
		}


		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}


		/// <summary>
		/// Pro soubory XML s popisem mluvnice zkopíruje ze síťového disku dopovídající soubor s facsimile, pokud ještě na lokálním disku není.
		/// </summary>
		private static void StazeniAktualnichFaksimili()
		{
			DirectoryInfo docx = new DirectoryInfo(SlozkaDocx);
			string sitovaSlozkaFaksimile = @"T:\Prameny\Fotokopie\MLUVNICE\Anotovane MLUVNICE\";

			FileInfo[] soubory = docx.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				string souborXml = NazevBezPripony(soubor.Name) + ".xml";
				FileInfo lokalniFaksimile = new FileInfo(Path.Combine(SlozkaFaksimile, souborXml));
				FileInfo sitoveFaksimile = new FileInfo(Path.Combine(sitovaSlozkaFaksimile, souborXml));
				if (!lokalniFaksimile.Exists)
				{
					if (!sitoveFaksimile.Exists)
					{
						chyba = true;
						VypsatUpozorneni(String.Format("!!! Soubor faksimile {0} neexistuje ani na síti.", lokalniFaksimile.Name));
					}
					else
					{
						sitoveFaksimile.CopyTo(lokalniFaksimile.FullName);
						Console.WriteLine("Soubor faksimile {0} zkopírován ze sítě.", sitoveFaksimile.Name);
					}

				}
				else
				{
					if (lokalniFaksimile.LastWriteTime > sitoveFaksimile.LastWriteTime)
					{
						VypsatUpozorneni(String.Format("??? Soubor faksimile {0} na síti je novější:\r\n\t ({1} × {2})", souborXml,
							sitoveFaksimile.LastWriteTime, lokalniFaksimile.LastWriteTime));
					}
				}
			}
		}

		/// <summary>
		/// Kontroluje, zda ke každému souboru s charakteristickou (DOCX) existuje odpovídající soubor s facsimile (XML) a složka s obrázky
		/// </summary>
		private static void KontrolaExistenceSouboruAObrazku()
		{
			DirectoryInfo docx = new DirectoryInfo(SlozkaDocx);

			FileInfo[] soubory = docx.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				string souborXml = NazevBezPripony(soubor.Name) + ".xml";
				FileInfo xmlInfo = new FileInfo(Path.Combine(SlozkaFaksimile, souborXml));
				if (!xmlInfo.Exists)
				{
					chyba = true;
					VypsatUpozorneni(String.Format("!!! Soubor faksimile {0} neexistuje.", xmlInfo.FullName));
				}
				DirectoryInfo obrazky = new DirectoryInfo(Path.Combine(@"V:\MDM\Prevod", NazevBezPripony(soubor.Name)));
				if (!obrazky.Exists)
				{
					chyba = true;
					VypsatUpozorneni(String.Format("!!! Složka s obrázky {0} neexistuje.", obrazky.FullName));
				}
				if (!chyba)
					Console.WriteLine("OK! {0}", NazevBezPripony(soubor.Name));
				else
				{
					Console.WriteLine();
				}

			}
		}

		/// <summary>
		/// Kontroluje, zda soubor facsimile obsahuje všechny obrázky ve složce 
		/// a zda všechny obrázky ve složce mají odpovídající položku v souboru facsimile
		/// </summary>
		private static void KontrolaFacsimileAObrazku()
		{
			DirectoryInfo docx = new DirectoryInfo(SlozkaDocx);

			FileInfo[] soubory = docx.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo soubor in soubory)
			{
				bool chyba = false;
				string souborXml = NazevBezPripony(soubor.Name) + ".xml";
				FileInfo xmlInfo = new FileInfo(Path.Combine(SlozkaFaksimile, souborXml));
				if (!xmlInfo.Exists)
				{
					chyba = true;
					VypsatUpozorneni(String.Format("!!! Soubor faksimile {0} neexistuje.", xmlInfo.FullName));
				}
				DirectoryInfo obrazky = new DirectoryInfo(Path.Combine(@"V:\MDM\Prevod", NazevBezPripony(soubor.Name)));
				if (!obrazky.Exists)
				{
					chyba = true;
					VypsatUpozorneni(String.Format("!!! Složka s obrázky {0} neexistuje.", obrazky.FullName));
				}
				if (!chyba)
				{
					Console.Write("Kontrola souboru {0}:", xmlInfo.Name);

					List<string> obrazkySlozky = NactiSeznamObrazku(obrazky);
					List<string> obrazkyFacsimile = NactiSeznamObrazku(xmlInfo.FullName);
					List<string> zmeny1 = obrazkySlozky.Except(obrazkyFacsimile).ToList();
					List<string> zmeny2 = obrazkyFacsimile.Except(obrazkySlozky).ToList();
					if (zmeny1.Count > 0)
					{
						chyba = true;
						Console.WriteLine();
						VypsatUpozorneni("Soubory ve složce obrázků navíc: ");
						foreach (string s in zmeny1)
						{
							Console.WriteLine(s);
						}
					}
					if (zmeny2.Count > 0)
					{
						chyba = true;
						Console.WriteLine();
						VypsatUpozorneni("Soubory ve facsimile navíc: ");
						foreach (string s in zmeny2)
						{
							Console.WriteLine(s);
						}
					}

					Console.WriteLine();
				}
				else
				{
					Console.WriteLine();
				}

			}
		}

		private static List<string> NactiSeznamObrazku(DirectoryInfo slozka)
		{
			FileInfo[] files = slozka.GetFiles();

			List<string> seznam = new List<string>(files.Length);
			foreach (FileInfo file in files)
			{
				seznam.Add(file.Name.ToLower());
			}
			return seznam;
		}


		private static List<string> NactiSeznamObrazku(string souborXml)
		{
			XmlDocument xd = new XmlDocument();
			xd.Load(souborXml);

			XmlNamespaceManager nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);

			XmlNodeList graphics = xd.SelectNodes("/t:facsimile/t:surface/t:graphic", nmspc);
			List<string> seznam = new List<string>(graphics.Count);
			foreach (XmlNode graphic in graphics)
			{
				if (graphic.Attributes != null)
				{
					string url = graphic.Attributes["url"].Value;
					string nazev = url.Substring(url.LastIndexOf('\\') + 1);
					seznam.Add(nazev.ToLower());
				}
			}

			return seznam;
		}

		private static void PrejmenovatSouboryCharakteristik()
		{
			string seznam = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Zkratky\Prevodnik_zkratek.txt";
			string slozka = SlozkaFaksimile;
			using (StreamReader sr = new StreamReader(seznam))
			{
				string radek = null;
				while ((radek = sr.ReadLine()) != null)
				{
					if (radek == "*")
						break;
					if (radek.StartsWith(";"))
						continue;

					string[] polozky = radek.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
					FileInfo info = new FileInfo(Path.Combine(slozka, polozky[0] + ".xml"));
					if (info.Exists)
					{
						Console.WriteLine("{0} => {1}", polozky[0], polozky[1]);
						File.Move(info.FullName, Path.Combine(slozka, polozky[1] + ".xml"));
						FileInfo newFile = new FileInfo(Path.Combine(slozka, polozky[1] + ".xml"));
						newFile.LastWriteTime = DateTime.Now;
					}
					else
					{
						VypsatUpozorneni(String.Format("!!! {0}", polozky[0]));
					}
				}
			}
		}

		private static void VytvorDavkuProVodoznak(FileInfo souborxml)
		{

			Console.WriteLine("Vytváření dávky pro vodoznak  {0}", souborxml.Name);

			string vychoziSlozka = @"V:\MDM\Obrazky\Aktualni\";
			string cilovaSlozka = @"V:\MDM\Obrazky\_Web\";
			string slozkaCmd = @"V:\MDM\Obrazky\_Watermark\Davky\";


			XmlDocument xd = new XmlDocument();
			XmlNamespaceManager nmspc = GetMdmNamespaceManager();

			xd.Load(souborxml.FullName);
			XmlNode nd = xd.SelectSingleNode("/t:TEI/t:teiHeader/t:fileDesc/t:sourceDesc/t:msDesc/t:msIdentifier/t:repository", nmspc);
			string instituce = "SOUKROME";
			if (nd != null)
			{
				switch (nd.InnerText)
				{
					case "Knihovna Ústavu pro jazyk český AV ČR, v. v. i.":
					case "Ústav pro jazyk český AV ČR, v. v. i.":
						instituce = "UJC";
						break;
					case "Knihovna Národního muzea":
						instituce = "KNM";
						break;
					case "ze soukromé sbírky":
						instituce = "SOUKROME";
						break;
					case "Státní oblastní archiv v Třeboni":
						instituce = "TREBON";
						break;
					case "Vědecká knihovna v Olomouci":
						instituce = "VKOL";
						break;
					case "Strahovská knihovna":
						instituce = "STRAHOV";
						break;
					case "Moravská zemská knihovna":
						instituce = "MZK";
						break;
                    case "Bibliothek der Ludwig-Maximilians-Universität München":
				        instituce = "BLMU";
                        break;
					case "Národní knihovna České republiky":
						instituce = "NKCR";
						break;
					default:
						instituce = nd.InnerText;
						if (instituce.StartsWith("ze soukromé sbírky"))
							instituce = "SOUKROME";
						else
							VypsatUpozorneni(String.Format("Neznámá instituce '{0}' v souboru {1}", nd.InnerText, souborxml.Name));
						break;
				}
			}
			string nazev = NazevBezPripony(souborxml.Name);
			VytvoritDavkuProVodoznak(vychoziSlozka + nazev, "Vodoznak_" + instituce, cilovaSlozka + nazev, slozkaCmd + nazev + ".cmd");
		}

		private static void VytvoritDavkuProVodoznak(string slozka, string instituce, string vystupniSlozka, string ulozeni)
		{
			DirectoryInfo div = new DirectoryInfo(slozka);
			//složka s obrázky mluvnice neexistuje
			if (!div.Exists)
			{
				if (!Directory.Exists(vystupniSlozka))
					VypsatUpozorneni(String.Format("{0} (= výchozí) ani cílová složka neexistují. Zkopírujte do výchozí složky soubory (JPG) ze síťového úložiště (T:\\Prameny\\Fotokopie\\MLUVNICE\\Anotovane MLUVNICE).", slozka));
				return;
			}

			DirectoryInfo dic = new DirectoryInfo(vystupniSlozka);
			if (!dic.Exists)
				dic.Create();
			FileInfo[] fis = div.GetFiles();
			//Console.WriteLine("Vytvářím soubor {0}", ulozeni);
			using (StreamWriter sw = new StreamWriter(ulozeni, false))
			{
				Console.WriteLine(ulozeni);
				sw.WriteLine("SET IM=" + "\"" + @"D:\Programy\Multimedia\ImageMagick-6.7.3-5\" + "\"");
				sw.WriteLine();

				foreach (FileInfo fi in fis)
				{
					sw.WriteLine("{0} {1}.png {2} {3}", "%IM%composite.exe -gravity South -quality 70 ", instituce, fi.FullName, Path.Combine(vystupniSlozka, fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + fi.Extension.ToLower()));

				}
			}
		}
		private static void ZpracujSouboryXml(string slozka, string maska, ZpracovatSouborXml zpracovani)
		{
			DirectoryInfo di = new DirectoryInfo(slozka);
			FileInfo[] fis = di.GetFiles(maska, SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in fis)
			{
				if (zpracovani != null) zpracovani(fileInfo);
			}
		}


		private static void VygenerovatSiFacsimile()
		{
			string vzor = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Facsimile\DobrLehr1809.xml";
			DirectoryInfo directoryInfo = new DirectoryInfo(SlozkaDocx);
			FileInfo[] fileInfos = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in fileInfos)
			{
				FileInfo cil = new FileInfo(Path.Combine(SlozkaFaksimile, NazevBezPripony(fileInfo.Name) + ".xml"));
				if (!cil.Exists)
					File.Copy(vzor, cil.FullName);
				fileInfo.CreationTime = DateTime.Now;
			}
		}

		/// <summary>
		/// Přejmenuje složku na základě převodníku zkratek. Spolu se složkou přejmenuje obrázky ve sločce a cestu k souborům v XML.
		/// </summary>
		private static void PrejmenovatSlozku()
		{
			string spolecnaSlozka = @"V:\MDM\Obrazky";
			spolecnaSlozka = @"V:\MDM\Obrazky\Temp";
			spolecnaSlozka = @"V:\MDM\Skeny_nove";
			string prevodniSoubor = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Charakteristiky\Prevodnik_pojmenovani_mluvnic.txt";
			prevodniSoubor = @"D:\!UJC\OVJ\NAKI\Prace\MDM\Zkratky\Prevodnik_zkratek.txt";

			using (StreamReader sr = new StreamReader(prevodniSoubor))
			{
				string radek = null;
				while ((radek = sr.ReadLine()) != null)
				{
					if (radek.StartsWith(";")) continue;
					if (radek.StartsWith("*")) break;

					string[] slozky = radek.Split(new char[] { '|' });
					PrejmenujSlozku(spolecnaSlozka, slozky[0], slozky[1]);
				}
			}
			Console.ReadLine();
		}

		private static void PrejmenujSlozku(string vychoziSlozka, string puvodniNazev, string novyNazev)
		{
			FileInfo noveXml = new FileInfo(Path.Combine(vychoziSlozka, novyNazev + ".xml"));
			if (noveXml.Exists)
			{
				VypsatUpozorneni(String.Format("Soubor {0} již exituje.", noveXml.Name));
				return;
			}

			FileInfo file = new FileInfo(Path.Combine(vychoziSlozka, puvodniNazev + ".xml"));
			if (!file.Exists)
			{
				VypsatUpozorneni(String.Format("Soubor {0} neexituje.", file.Name));
				return;
			}
			DirectoryInfo slozka = new DirectoryInfo(Path.Combine(vychoziSlozka, puvodniNazev));
			if (!slozka.Exists)
			{
				VypsatUpozorneni(String.Format("Složka {0} neexituje.", slozka.Name));
				return;
			}
			Console.WriteLine("Zpracovávám složku {0} >> {1}", puvodniNazev, novyNazev);
			PrejmenovatObrazky(slozka, novyNazev);
			slozka.MoveTo(Path.Combine(slozka.Parent.FullName, novyNazev));

			PrejmenujSlozkuVXml(file, novyNazev);
		}

		private static void PrejmenovatObrazky(string slozka, string novyNazev)
		{
			DirectoryInfo info = new DirectoryInfo(slozka);
			PrejmenovatObrazky(info, novyNazev);
		}

		private static void PrejmenovatObrazky(DirectoryInfo slozka, string novyNazev)
		{
			FileInfo[] soubory = slozka.GetFiles("*.*", SearchOption.AllDirectories);
			foreach (FileInfo soubor in soubory)
			{
				string noveJmeno = NoveJmenoSouboru(soubor.Name, novyNazev);
				FileInfo novySoubor = new FileInfo(Path.Combine(soubor.DirectoryName, noveJmeno));
				if (!novySoubor.Exists)
					soubor.MoveTo(Path.Combine(soubor.DirectoryName, noveJmeno));
			}
		}

		private static string NoveJmenoSouboru(string soucasnyNazev, string novyNazev)
		{
			if (soucasnyNazev.IndexOf('_') == soucasnyNazev.LastIndexOf('_'))
			{
				int pomlcka = soucasnyNazev.LastIndexOf('-');
				if (pomlcka == -1)
					pomlcka = soucasnyNazev.LastIndexOf('_');
				return novyNazev + "_" + soucasnyNazev.Substring(pomlcka + 1);
			}
			else
			{
				int pomlcka = soucasnyNazev.IndexOf('-');
				if (pomlcka == -1)
					pomlcka = soucasnyNazev.IndexOf('_');
				return novyNazev + "_" + soucasnyNazev.Substring(pomlcka + 1);

			}
		}

		private static void PrejmenovatNazvySouboruVeFacsimile(string vychoziSlozka)
		{
			DirectoryInfo vychozi = new DirectoryInfo(vychoziSlozka);
			FileInfo[] files = vychozi.GetFiles("*.xml");
			foreach (FileInfo file in files)
			{
				PrejmenujNazvySouboruVeFacsimile(file.FullName, NazevBezPripony(file.Name));
			}
		}

		/// <summary>
		/// Přejmenuje názvy složek a souborů v XML podle nového názvu
		/// </summary>
		/// <param name="souborXml"></param>
		/// <param name="novyNazev"></param>
		private static void PrejmenujNazvySouboruVeFacsimile(string souborXml, string novyNazev)
		{
			XmlDocument xd = new XmlDocument();
			xd.Load(souborXml);

			XmlNamespaceManager nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);

			xd.DocumentElement.Attributes["n"].Value = novyNazev;

			XmlNodeList graphics = xd.SelectNodes("/t:facsimile/t:surface/t:graphic", nmspc);
			foreach (XmlNode graphic in graphics)
			{
				if (graphic.Attributes != null)
				{
					string url = graphic.Attributes["url"].Value;
					string noveUrl = NoveJmenoSouboru(url, novyNazev);
					graphic.Attributes["url"].Value = noveUrl;
				}
			}
			xd.Save(souborXml);

		}

		private static void PrejmenujSlozkuVXml(FileInfo souborXml, string novyNazev)
		{
			string temp = Path.GetTempFileName();
			XmlNamespaceManager nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
			xws.CloseOutput = true;


			XmlReaderSettings xrs = new XmlReaderSettings();


			using (XmlReader xr = XmlReader.Create(souborXml.FullName))
			{
				using (XmlWriter xw = XmlWriter.Create(temp, xws))
				{
					xw.WriteStartDocument();
					while (xr.Read())
					{
						string nazev = xr.Name;
						if (xr.NodeType == XmlNodeType.Element)
						{
							if (nazev == "facsimile")
							{
								xw.WriteStartElement(nazev, "http://www.tei-c.org/ns/1.0");
								xw.WriteAttributeString("xmlns", null, "http://www.tei-c.org/ns/1.0");
								for (int i = 0; i < xr.AttributeCount; i++)
								{
									xr.MoveToAttribute(i);
									switch (xr.Name)
									{
										case "n":
											xw.WriteAttributeString("n", novyNazev);
											break;
										case "xml:base":
											string text = xr.Value;
											int lomitko = text.LastIndexOf('\\');
											if (lomitko == -1)
												text = novyNazev;
											else
											{
												text = text.Substring(0, lomitko + 1) + novyNazev;
											}
											xw.WriteAttributeString("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace, text);
											break;
										case "xmlns":
											break;
										default:
											Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
											break;
									}
								}
								continue;
							}
							if (nazev == "graphic")
							{
								string url = xr.GetAttribute("url");
								string noveUrl = NoveJmenoSouboru(url, novyNazev);
								xw.WriteStartElement(nazev);
								xw.WriteAttributeString("url", noveUrl);
								xw.WriteEndElement();
								continue;
							}
						}
						Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
					}
				}
			}
			File.Move(temp, Path.Combine(souborXml.DirectoryName, novyNazev + ".xml"));
		}


		private static void ZkontrolujCislaPagin(FileInfo souborXml)
		{

			Console.WriteLine("Upravuji čísla pagin v souboru {0}", souborXml.Name);

			string temp = Path.GetTempFileName();


			XmlWriterSettings xws = new XmlWriterSettings();
			xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
			xws.CloseOutput = true;


			XmlReaderSettings xrs = new XmlReaderSettings();


			using (XmlReader xr = XmlReader.Create(souborXml.FullName))
			{
				using (XmlWriter xw = XmlWriter.Create(temp, xws))
				{
					xw.WriteStartDocument();
					while (xr.Read())
					{
					Zacatek:
						string nazev = xr.Name;
						if (xr.NodeType == XmlNodeType.Element && nazev == "surface")
						{
							string n = xr.GetAttribute("n");
							if (n == "xx" || n == "XX")
							{
								n = "××";
							}
							if (n.IndexOf('-') > -1)
							{
								n = n.Replace('-', '–');
							}
							if (n == "××") //obrázek označený ×× se vynechává
							{
								xr.Skip();
								if (xr.NodeType == XmlNodeType.Element)
									goto Zacatek;
								continue;
							}
							xw.WriteStartElement(nazev);
							xw.WriteAttributeString("n", n);
						}
						else
							Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
					}
				}
			}
			souborXml.Delete();
			File.Move(temp, souborXml.FullName);
		}

		private static XmlNamespaceManager GetMdmNamespaceManager()
		{
			XmlNamespaceManager nmspc;
			nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);
			return nmspc;
		}

		private static void TransformovatDocxNaXml()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(SlozkaDocx);
			FileInfo[] fileInfos = directoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			Parallel.ForEach(fileInfos, fileInfo =>
				{
					string souborXml = Path.Combine(SlozkaXml, NazevBezPripony(fileInfo.Name) + ".xml");

					KonverzeNastaveni konverzeNastaveni = new KonverzeNastaveni(fileInfo.FullName, VsechnyStyly, souborXml);
					GenerovatXml(konverzeNastaveni);
				}
				);

			/*
			foreach (FileInfo fileInfo in fileInfos)
			{
				string souborXml = Path.Combine(SlozkaXml, NazevBezPripony(fileInfo.Name) + ".xml");

				KonverzeNastaveni konverzeNastaveni = new KonverzeNastaveni(fileInfo.FullName, VsechnyStyly, souborXml);
				GenerovatXml(konverzeNastaveni);
			}
			 */
		}


		private static string NazevBezPripony(string nazev)
		{
			int tecka = nazev.LastIndexOf('.');
			if (tecka == -1)
				return nazev;
			return nazev.Substring(0, tecka);
		}


		private static void GenerovatXml(KonverzeNastaveni knvNastaveni)
		{
			DateTime dtn = DateTime.Now;
			Console.WriteLine("Generuju XML ze souboru: {0}", knvNastaveni.Docx);

			Settings stg = new Settings();
			stg.OutputIndent = !knvNastaveni.BezOdsazeni;
			XmlGenerator xg = new XmlGenerator(knvNastaveni.Docx, knvNastaveni.Doc2Xml, knvNastaveni.Xml, stg);
			xg.Read();
			Console.WriteLine("Čas zpracování: {0}", DateTime.Now - dtn);

		}

		private static void TransformovatTeiNaMdm()
		{

			DirectoryInfo directoryInfo = new DirectoryInfo(SlozkaTei);
			FileInfo[] fileInfos = directoryInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in fileInfos)
			{
				FileInfo souborFacsimile = new FileInfo(Path.Combine(SlozkaFaksimile, fileInfo.Name));
				if (!souborFacsimile.Exists)
				{
					VypsatUpozorneni(String.Format("Soubor facsimile neexistuje: {0}", souborFacsimile.Name));
					continue;
				}

				IList<IXsltTransformer> transformers = new List<IXsltTransformer>();
				IXsltTransformer transformer =
						XsltTransformerFactory.GetXsltTransformer(SlozkaXslt + "SloucitHlavickuPlusFacsimile.xsl");
				transformer.Parameters.Add("faksimile", Path.Combine(SlozkaFaksimile, fileInfo.Name));

				IXsltTransformer paginace = XsltTransformerFactory.GetXsltTransformer(SlozkaXslt + "UpravitPaginaciFacsimle.xsl");

				transformers.Add(transformer);
				transformers.Add(paginace);

				XsltTransformationProcess process = new XsltTransformationProcess(
					fileInfo.FullName,
					Path.Combine(SlozkaMdm, fileInfo.Name),
					transformers);

				Console.WriteLine("Transformuji soubor {0}", fileInfo.Name);

				process.Transform();
			}
		}

		private static void TransformovatXmlNaTei()
		{
			IList<IXsltTransformer> transformers = new List<IXsltTransformer>();
			transformers.Add(XsltTransformerFactory.GetXsltTransformer(SlozkaXslt + "Zakladni_xml_na_TEI.xsl"));
			XsltTransformationProcess process = new XsltTransformationProcess(SlozkaXml, "*.xml", SlozkaTei, "xml", transformers);
			process.Transform();
		}


		/// <summary>
		/// Odliší hypertextové odkazy na stejně pojmenované paginy, popř. lišící se velikostí písmen
		/// </summary>
		private static void OdlisitStejnePojmenovanaPaginy(FileInfo souborXml)
		{

			Console.WriteLine("Odlišuju sejně pojmenované paginy v souboru {0}", souborXml.Name);

			XmlNamespaceManager nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);
			bool dosloKeZmene = false;

			XmlDocument xd = new XmlDocument();
			xd.Load(souborXml.FullName);
			XmlNodeList odkazy = xd.SelectNodes("//t:surface/@n", nmspc);
			Dictionary<string, List<XmlNode>> pageNumbers = new Dictionary<string, List<XmlNode>>(odkazy.Count);

			char[] distinctChars = new char[] { 'º', '¹', '²', '³', '⁴' };


			foreach (XmlNode odkaz in odkazy)
			{
				string text = odkaz.Value.ToUpper(CultureInfo.CurrentCulture);
				if (!pageNumbers.ContainsKey(text))
				{
					pageNumbers.Add(text, new List<XmlNode>());
				}
				pageNumbers[text].Add(odkaz);
			}

			foreach (KeyValuePair<string, List<XmlNode>> pageNumber in pageNumbers)
			{
				if (pageNumber.Value.Count > 1)
				{
					dosloKeZmene = true;
					for (int i = 0; i < pageNumber.Value.Count; i++)
					{
						pageNumber.Value[i].Value += distinctChars[i];
					}
				}
			}

			if (dosloKeZmene)
			{
				Console.WriteLine("\tPaginy přejmenovány");
				xd.Save(souborXml.FullName);
			}

		}

		/// <summary>
		/// Upraví odkazy na exitující čísla paginy
		/// </summary>
		private static void UpravOdkazyNaPaginy(FileInfo souborXml)
		{

			Console.WriteLine("Upravuji odkazy v souboru {0}", souborXml.Name);

			XmlNamespaceManager nmspc = new XmlNamespaceManager(new NameTable());
			nmspc.AddNamespace("t", "http://www.tei-c.org/ns/1.0");
			nmspc.AddNamespace("a", "http://vokabular.ujc.cas.cz/ns/anotace");
			nmspc.AddNamespace("xml", Daliboris.Pomucky.Xml.Objekty.XmlNamespace);
			bool dosloKeZmene = false;

			XmlDocument xd = new XmlDocument();
			xd.Load(souborXml.FullName);
			XmlNodeList odkazy = xd.SelectNodes("//t:ref[@type='pagina']", nmspc);

			foreach (XmlNode node in odkazy)
			{
				string pagina = node.Attributes["target"].Value;
				if (PaginaExistuje(xd, nmspc, pagina))
					continue;
				if (PaginaExistuje(xd, nmspc, "–" + pagina))
					continue;
				if (PaginaExistuje(xd, nmspc, pagina + "–"))
					continue;

				dosloKeZmene = true;
				XmlAttribute xa = xd.CreateAttribute("subtype");
				xa.Value = "blind";
				node.Attributes.Append(xa);
			}
			if (dosloKeZmene)
				xd.Save(souborXml.FullName);
		}

		private static bool PaginaExistuje(XmlDocument xd, XmlNamespaceManager nmspc, string cislo)
		{
			string xpath = String.Format("/t:TEI/t:facsimile/t:surface[@n='{0}']", cislo);
			if (cislo[cislo.Length - 1] == '–')
				xpath = String.Format("/t:TEI/t:facsimile/t:surface[starts-with(@n, '{0}')]", cislo);
			if (cislo[0] == '–')
			{
				xpath = String.Format("/t:TEI/t:facsimile/t:surface[substring(@n, string-length(@n) - string-length('{0}') + 1) = '{0}']", cislo); //= ends-with
			}
			XmlNodeList xnl = xd.SelectNodes(xpath, nmspc);
			if (xnl.Count == 1)
				return true;
			foreach (XmlNode node in xnl)
				if (node.Attributes["n"].Value.IndexOf('²') > -1)
					return true;

			return false;
		}

		private static void UpravitPerex()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(SlozkaMdm);
			FileInfo[] fileInfos = directoryInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in fileInfos)
			{
				Console.WriteLine("Upravuji perex v souboru {0}", fileInfo.Name);
				UpravPerexSouboru(fileInfo);
			}
		}

		private static void UpravPerexSouboru(FileInfo souborXml)
		{
			string temp = Path.GetTempFileName();

			XmlWriterSettings xws = new XmlWriterSettings();
			xws.NamespaceHandling = NamespaceHandling.OmitDuplicates;
			xws.CloseOutput = true;


			XmlReaderSettings xrs = new XmlReaderSettings();


			using (XmlReader xr = XmlReader.Create(souborXml.FullName))
			{
				XmlReader xr2 = XmlReader.Create(souborXml.FullName);

				using (XmlWriter xw = XmlWriter.Create(temp, xws))
				{
					xw.WriteStartDocument();
					while (xr.Read())
					{
						if (xr2.ReadState != ReadState.Closed)
							xr2.Read();
						string nazev = xr.Name;
						if (xr.NodeType == XmlNodeType.Element)
						{
							if (nazev == "sourceDesc")
							{
								string typ = xr.GetAttribute("n");
								if (typ != "characteristic") goto Dalsi;

								ZpracujCharakteristikuNaPerex(xr2, xw);

								//XmlDocument xd = Daliboris.Pomucky.Xml.Objekty.ReadNodeAsXmlDocument(xr);
								//xd.Save(xw);
								goto Dalsi;
							}
						}
					Dalsi:
						Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
					}
					xw.WriteEndDocument();
					xw.Close();
				}

			}

			string newFile = souborXml.FullName;// +".xml";
			if (File.Exists(newFile))
				File.Delete(newFile);
			File.Move(temp, newFile);
		}

		private static void ZpracujCharakteristikuNaPerex(XmlReader xr, XmlWriter xw)
		{


			xw.WriteStartElement("sourceDesc");
			xw.WriteAttributeString("n", "perex");

			bool bylAnchor = false;
			bool bylOdstavec = false;
			while (xr.Read())
			{
				string nazev = xr.Name;
				if (xr.NodeType == XmlNodeType.Element)
					switch (nazev)
					{

						case "anchor":
							if (xr.GetAttribute("type") == "predel")
							{
								Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
								bylAnchor = true;
							}
							break;
						default:
							if (!bylAnchor)
								Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
							break;
					}
				else
				{
					if (xr.NodeType == XmlNodeType.EndElement && nazev == "sourceDesc")
					{
						xw.WriteEndElement(); //</sourceDesc>
						xr.Close();
						return;
					}
					if (xr.NodeType == XmlNodeType.EndElement && nazev == "p" && bylAnchor && !bylOdstavec)
					{
						Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw); //</p>
						bylOdstavec = true;
					}
					if (!bylAnchor)
						Daliboris.Pomucky.Xml.Transformace.SerializeNode(xr, xw);
				}
			}


		}

		private static void VypsatUpozorneni(string zprava)
		{
			string file = @"V:\MDM\Temp\Log.log";
			File.AppendAllLines(file, new List<string> { zprava }, Encoding.UTF8);

			ConsoleColor backgroundColor = Console.BackgroundColor;
			ConsoleColor foregroundColor = Console.ForegroundColor;

			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.ForegroundColor = ConsoleColor.DarkRed;

			Console.WriteLine(zprava);

			Console.BackgroundColor = backgroundColor;
			Console.ForegroundColor = foregroundColor;
		}

		/// <summary>
		/// A hard link is a way to represent a single file (i.e. data volume) by more than one path.
		/// In other words, it is a way to create multiple directory entries for a single file.
		/// A hard link is almost like a normal link (i.e. shortcut) but there is a big difference.
		/// If you delete the source file of a shortcut, the shortcut would be broken (points to a non-existing file.)
		/// A hard link on the other hand, would be still working fine if you delete the source file as if it was just a copy.
		/// Hard disk space is not multiplied for each hard link. Because they are share the same data volume, they share the same size.
		/// </summary>
		/// <param name="lpFileName"></param>
		/// <param name="lpExistingFileName"></param>
		/// <param name="lpSecurityAttributes"></param>
		/// <returns></returns>

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		static extern bool CreateHardLink(
			string lpFileName,
			string lpExistingFileName,
			IntPtr lpSecurityAttributes
		);

		/// <summary>
		/// Soft links (also called junctions,) are identical to hard links except that soft links are designated for directories not files.
		/// </summary>
		/// <param name="lpSymlinkFileName"></param>
		/// <param name="lpTargetFileName"></param>
		/// <param name="dwFlags"></param>
		/// <returns></returns>
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		static extern bool CreateSymbolicLink(
		string lpSymlinkFileName,
		string lpTargetFileName,
		uint dwFlags
		);

		const uint SYMBLOC_LINK_FLAG_FILE = 0x0;
		const uint SYMBLOC_LINK_FLAG_DIRECTORY = 0x1;
	}
}
