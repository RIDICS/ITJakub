using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace Daliboris.Pomucky.Xml.XsltTransformation
{
	public class XsltTransformationProcess : IXsltTransformationProcess
	{

		public XsltTransformationProcess() {
			OverwriteExistingFiles = true;
		}

		public XsltTransformationProcess(IList<IXsltTransformer> xsltTransformers) : this() {
			XsltTransformers = xsltTransformers;
		}

		public XsltTransformationProcess(string inputFilename, string outputFilename, IList<IXsltTransformer> xsltTransformers) : this(xsltTransformers)
		{
			InputFilename = inputFilename;
			OutputFilename = outputFilename;
		}

		public XsltTransformationProcess(string inputDirectory, string inputFileMask, string outputDirectory, string outputFilenameExtension, IList<IXsltTransformer> xsltTransformers)
			: this(xsltTransformers)
		{
			InputDirectory = inputDirectory;
			InputFileMask = inputFileMask;
			OutputDirectory = outputDirectory;
			OutputFilenameExtension = outputFilenameExtension;

		}

		private void SetDefaultValues()
		{
			if (OutputFilenameExtension == null)
				OutputFilenameExtension = "xml";
			if (InputFileMask == null)
				InputFileMask = "*.*";
			if (TempDirectory == null)
				TempDirectory = Path.GetTempPath();
			else
			{
			    DirectoryInfo temp = new  DirectoryInfo(TempDirectory);
                if(!temp.Exists)
                    temp.Create();
			}
		}

		public string InputFilename { get; set; }

		public string OutputFilename { get; set; }

		public string InputDirectory { get; set; }

		public string OutputDirectory { get; set; }

		public string InputFileMask { get; set; }

		public IList<string> InputFiles { get; set; }

		public string OutputFilenameExtension { get; set; }

		public string TempDirectory { get; set; }

		public bool OverwriteExistingFiles {get; set; }

		public IList<IXsltTransformer> XsltTransformers { get; set; }

        public bool IncludeSubdirectories { get; set; }

		public void Transform()
		{
			SetDefaultValues();
			if (InputDirectory != null && OutputDirectory != null)
			{
				Transform(InputDirectory, InputFileMask, OutputDirectory, OutputFilenameExtension);
				return;
			}
			if (InputFilename != null && OutputFilename != null)
			{
				Transform(InputFilename, OutputFilename, XsltTransformers);
			}
		}

		public void Transform(string inputFilename, string outputFilename, IList<IXsltTransformer> xsltTransformers)
		{
		    Console.WriteLine("{0} => {1}", inputFilename, outputFilename);
			SetDefaultValues();
			string input = inputFilename;
			string output = outputFilename;
		    FileInfo fi = new FileInfo(inputFilename);
		    string fileName = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
		    int i = 0;
			
			foreach (IXsltTransformer transformer in XsltTransformers)
			{
			    output = Path.Combine(TempDirectory, String.Format("{0}_{1:00}{2}", fileName, i++, fi.Extension));
                Console.WriteLine("{0} => {1}", input, output);
				transformer.Transform(input, output, transformer.Parameters);
				input = output;
			}
			File.Copy(output, outputFilename, OverwriteExistingFiles);
		}

		private void Transform(string inputDirectory, string inputFileMask, string outputDirectory, string outputFilenameExtension)
		{
			SetDefaultValues();

			DirectoryInfo directoryInfo = new DirectoryInfo(inputDirectory);
		    SearchOption searchOption = IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			FileInfo[] fileInfos = directoryInfo.GetFiles(inputFileMask, searchOption);
			Dictionary<string, string> inputOutputFiles = new Dictionary<string, string>(fileInfos.Length);

			foreach (FileInfo fileInfo in fileInfos)
			{
			    if (IncludeSubdirectories)
			        CreateOutputDirectory(fileInfo, inputDirectory, outputDirectory);
			    string subdirectory = Path.Combine(GetSubdirecotryPath(fileInfo, inputDirectory), "\\");
			    if (subdirectory == "\\") subdirectory = string.Empty;

			    inputOutputFiles.Add(fileInfo.FullName,
			                         Path.Combine(outputDirectory, subdirectory) +
			                         FileNameWithoutExtension(fileInfo) + "." + outputFilenameExtension);

			}
			Transform(inputOutputFiles);
		}

        private string GetSubdirecotryPath(FileInfo fileInfo, string inputDirectory)
        {
            if ((fileInfo.FullName.Length - fileInfo.Name.Length - inputDirectory.Length) == 0)
                return String.Empty;
            string newDirectory = fileInfo.FullName.Substring(inputDirectory.Length,
                                                              fileInfo.FullName.Length - fileInfo.Name.Length -
                                                              inputDirectory.Length - 1);
            return newDirectory;
        }

	    private void CreateOutputDirectory(FileInfo fileInfo, string inputDirectory, string outputDirectory)
	    {
            string newDirectory = GetSubdirecotryPath(fileInfo, inputDirectory);
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(outputDirectory, newDirectory));
	        if (!directoryInfo.Exists)
	            Directory.CreateDirectory(directoryInfo.FullName);
	    }


	    private void Transform(IList<string> inputFiles, string outputDirectory, string outputFilenameExtension)
		{
			Dictionary<string, string> inputOutputFiles = new Dictionary<string, string>(inputFiles.Count);
			foreach (string file in inputFiles)
			{
				inputOutputFiles.Add(file, Path.Combine(outputDirectory, FileNameWithoutExtension(file) + "." + outputFilenameExtension));
			}

			Transform(inputOutputFiles);
		}

		private void Transform(Dictionary<string, string> inputOutputFiles)
		{
			
			foreach (KeyValuePair<string, string> kvp in inputOutputFiles)
			{
				Transform(kvp.Key, kvp.Value, XsltTransformers);
			}
		}



		private string FileNameWithoutExtension(FileInfo filenInfo)
		{
			if (filenInfo.Extension.Length == 0)
				return filenInfo.Name;
			return filenInfo.Name.Substring(0, filenInfo.Name.Length - filenInfo.Extension.Length);
		}

		private string FileNameWithoutExtension(string fileName)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			return FileNameWithoutExtension(fileInfo);
		}
	}
}
