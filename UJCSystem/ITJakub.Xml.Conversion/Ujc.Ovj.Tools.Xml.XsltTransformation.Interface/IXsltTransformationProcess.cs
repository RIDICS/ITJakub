using System.Collections.Generic;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
{
	public interface IXsltTransformationProcess
	{
		/// <summary>
		/// Gets or sets fullpath to the file which will be transformed.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací celou cestu k souboru, který se bude transformovat.
		/// </summary>
		string InputFilename { get; set; }


		/// <summary>
		/// Gets or sets fullpath to the file which will be at the of transformation process.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací celou cestu k souboru, který bude na konci transformačního procesu.
		/// </summary>
		string OutputFilename { get; set; }

		/// <summary>
		/// Gets or sets fullpath to the directory in which are files, which will be transformed.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací celou cestu ke složce, v níž jsou uloženy soubory, které se budou transformovat.
		/// </summary>
		string InputDirectory { get; set; }

		/// <summary>
		///  Gets or sets fullpath to the directory in which will be saved transformed files.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací celou cestu ke složce, v níž budou uloženy výsledné soubory po transformaci.
		/// </summary>
		string OutputDirectory { get; set; }

		/// <summary>
		/// Gets or sets mask of files in the input directory (<see cref="InputDirectory"/>), which will be transformed.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací masku souborů ve stupní složce (<see cref="InputDirectory"/>), které se budou transformovat. 
		/// </summary>
		string InputFileMask { get; set; }

		/// <summary>
		/// Gets or sets list of full paths to the files, which will be transformed.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací seznam plných cest k souborům, které se budou transformovat.
		/// </summary>
		IList<string> InputFiles { get; set; }

		/// <summary>
		/// Gets or sets extension for names of the transformed files.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací příponu pro pojmenování transformovaných souborů.
		/// Pokud je <value>null</value>, použije se jméno přípona <value>xml</value>.
		/// Použije se při transformaci seznamu souborů nebo souborů ve složce.
		/// </summary>
		string OutputFilenameExtension { get; set; }

		/// <summary>
		/// Gets or sets full path to the temp directory.
		/// If it is <value>null</value>,system temp directory is used.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací plnou cestu k dočasné složce.
		/// Je-li <value>null</value>, použe je systémová složka pro dočasné soubory.
		/// </summary>
		string TempDirectory { get; set; }


		/// <summary>
		/// Gets or sets if existing output files should be overwritten.
		/// Default value is <value>true</value>.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací informaci, jestli má být existující výstupní soubor přepsán.
		/// Výchozí hodnota je <value>true</value>.
		/// </summary>
		bool OverwriteExistingFiles { get; set; }

		/// <summary>
		/// Gets or sets list od transformers used during transformation process.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Nastavuje nebo vrací seznam transformačních procedur použitých při transformaci.
		/// </summary>
		IList<IXsltTransformer> XsltTransformers { get; set; }

		/// <summary>
		/// Performs transformation of the input and produce output.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Transformuje vstupní soubor nebo soubory a uloží výstupní soubor nebo soubory.
		/// </summary>
		void Transform();

		/// <summary>
		/// Performs transformation of the input file and produce output file.
		/// </summary>
		/// <summary xml:lang="cs">
		/// Transformuje vstupní soubor a uloží výstupní soubor.
		/// Při transformaci použije seznam transformačních procedur.
		/// </summary>
		void Transform(string inputFilename, string outputFilename, IList<IXsltTransformer> xsltTransformers);

	}
}
