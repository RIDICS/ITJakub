using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Daliboris.Pomucky.Soubory.MetaInfo;
using NLog;

namespace Ujc.Ovj.Ooxml.Conversion.Test
{
	class Program
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			logger.Info("Conversion started at {0}.", DateTime.UtcNow);
			//MakeBulkConversion();
			MakeConversion();
			//TestMetadata();
			logger.Info("Conversion finished in {0}.", stopwatch.Elapsed);
			Console.ReadLine();
		}

		private static void TestMetadata()
		{
			string dataDirectory = GetDataDirectory();
			string inputDirectory = Path.Combine(dataDirectory, "Input");
			string outputDirectory = Path.Combine(dataDirectory, "Output");
			DirectoryInfo inputDirectoryInfo = new DirectoryInfo(inputDirectory);
			FileInfo[] files = inputDirectoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in files)
			{
				logger.Info("{0}", fileInfo.Name);
				string[] propertyNames = new[] { "Autor", "Kategorie", "Titul", "Předmět", "Komentář" };
				object[] properties = Metadata.NactiZabudovaneVlastnosti(fileInfo.FullName, propertyNames);

				string[] customPropertyNames = new[] { "htx_id" };
				object[] customProperties = Metadata.NactiUzivatelskeVlastnosti(fileInfo.FullName, customPropertyNames);

				for (int i = 0; i < propertyNames.Length; i++)
				{
					logger.Info("{0}:\t{1}", propertyNames[i], properties[i]);
				}

				for (int i = 0; i < customPropertyNames.Length; i++)
				{
					logger.Info("{0}:\t{1}", customPropertyNames[i], customProperties[i]);
				}
				
			}
		}

		private static void MakeBulkConversion()
		{
			string dataDirectory = GetDataDirectory();
			string inputDirectory = Path.Combine(dataDirectory, "Input");
			string outputDirectory = Path.Combine(dataDirectory, "Output");
			DirectoryInfo inputDirectoryInfo = new DirectoryInfo(inputDirectory);
			FileInfo[] files = inputDirectoryInfo.GetFiles("*.docx", SearchOption.TopDirectoryOnly);
			foreach (FileInfo fileInfo in files)
			{
				//Příprava souborů do dočasné složky
				DirectoryInfo outputDirectoryInfo = new DirectoryInfo(Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(fileInfo.Name)));
				if (!outputDirectoryInfo.Exists)
					Directory.CreateDirectory(outputDirectoryInfo.FullName);
				File.Copy(fileInfo.FullName, Path.Combine(outputDirectoryInfo.FullName, fileInfo.Name), true);
				string metadataFilePath = Path.Combine(inputDirectory, "Evidence.xml");


				MakeConversion(outputDirectoryInfo.FullName, fileInfo.Name, metadataFilePath);

				//MakeConversion(dataDirectory, Path.GetFileNameWithoutExtension(fileInfo.Name));
			}
		}

		//Ukázka volání konverze
		//Pro DocxToTeiConverterSettings stačí 3 hlavní parametry: 
			//cesta ke konvertovanému souboru
			//cesta k souboru s metadaty
			//funkce/delegát pro načítání verzí souboru
		private static void MakeConversion(string conversionDirectoryFullName, string documentName, string metadataFilePath)
		{
			DocxToTeiConverterSettings settings = new DocxToTeiConverterSettings();
			settings.InputFilePath = Path.Combine(conversionDirectoryFullName, documentName);
			settings.MetadataFilePath = metadataFilePath;
			settings.GetVersionList = GetVersions();
			String.Format("Úprava souboru k {0:g}", DateTime.Now);
			DoConversion(settings);
		}


		private static void MakeConversion(string dataDirectory, string documentName)
		{
			DocxToTeiConverterSettings settings = new DocxToTeiConverterSettings();
			settings.TempDirectoryPath = Path.Combine(dataDirectory, "Temp");
			settings.MetadataFilePath = Path.Combine(dataDirectory, "Input", "Evidence.xml");
			settings.InputFilePath = Path.Combine(dataDirectory, "Input", documentName + ".docx");
			settings.OutputFilePath = Path.Combine(dataDirectory, "Output", documentName + ".xml");
			String.Format("Úprava souboru k {0:g}", DateTime.Now);
			settings.GetVersionList = GetVersions();
			DocxToTeiConverter converter = new DocxToTeiConverter();
			ConversionResult result = converter.Convert(settings);
			if (result.IsConverted)
			{
				logger.Info("File {0} converted.", settings.InputFilePath);
			}
			else
			{
				logger.Info("File {0} not converted.", settings.InputFilePath);
				logger.Info("Errors: {0}", result.Errors);
			}
		}

		public static Func<string, List<VersionInfoSkeleton>> GetVersions()
		{
			List<VersionInfoSkeleton> versionList = new List<VersionInfoSkeleton>();
			versionList.Add(new VersionInfoSkeleton("Message 1", DateTime.UtcNow.AddDays(-2)));
			versionList.Add(new VersionInfoSkeleton("Message 2", DateTime.UtcNow));
			return s => versionList;
		}

		private static void MakeConversion()
		{
			string dataDirectory = GetDataDirectory();

			DocxToTeiConverterSettings settings = new DocxToTeiConverterSettings();
			string name = "Albetanus_Knizky_o_radnem_mluveni";
			settings.TempDirectoryPath = Path.Combine(dataDirectory, "Temp");
			settings.MetadataFilePath = Path.Combine(dataDirectory, "Input", "Evidence.xml");
			settings.InputFilePath = Path.Combine(dataDirectory, "Input", name + ".docx");
			settings.OutputFilePath = Path.Combine(dataDirectory, "Output", name + ".xml");
			String.Format("Úprava souboru k {0:g}", DateTime.Now);
			settings.SplitDocumentByPageBreaks = true;
			settings.GetVersionList = GetVersions();
			DoConversion(settings);
		}

		private static void DoConversion(DocxToTeiConverterSettings settings)
		{
			DocxToTeiConverter converter = new DocxToTeiConverter();
			ConversionResult result = converter.Convert(settings);
			if (result.IsConverted)
			{
				logger.Info("File {0} converted.", settings.InputFilePath);
			}
			else
			{
				logger.Info("File {0} not converted.", settings.InputFilePath);
				logger.Error("Errors: {0}", result.Errors.Count);
				foreach (Exception error in result.Errors)
				{
					logger.Error("\tError: {0}", error.Message);
				}
			}
		}

		private static string GetDataDirectory()
		{
			string dataDirectory = AssemblyDirectory;
			dataDirectory = dataDirectory.Substring(0,
				dataDirectory.LastIndexOf(String.Format("{0}bin{0}", Path.DirectorySeparatorChar)));

			dataDirectory = Path.Combine(dataDirectory, "Data");
			return dataDirectory;
		}

		public static string AssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}
	}
}
