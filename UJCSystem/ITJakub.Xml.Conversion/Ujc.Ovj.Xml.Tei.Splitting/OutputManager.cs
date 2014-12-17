// -----------------------------------------------------------------------
// <copyright file="OutputManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using System.Xml;

namespace Ujc.Ovj.Xml.Tei.Splitting
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	/// <summary>
	/// TODO: Update summary.
	/// </summary>
	public class OutputManager
	{

		private int currentChunk = 0;

		private string outputDirectory;

		private string fileName;

		private string fullPath;


		private DirectoryInfo outputDirectoryInfo;

		/// <summary>
		/// Složka, do níž se uloží jednotlivé části rozděleného souboru
		/// </summary>
		public string OutputDirectory
		{
			get { return outputDirectory; }
			set
			{
				outputDirectory = value;
				PrepareOutputDirectory();
			}
		}

		public string FileNameFormat { get; set; }

		public int CurrentChunk { get { return currentChunk; } }

		public string CurrentFileName { get { return fileName; } }

		public string CurrentFileFullPath { get { return fullPath; } }

		public XmlWriter GetXmlWriter()
		{
			currentChunk++;
			fileName = String.Format(FileNameFormat, CurrentChunk);
			fullPath = Path.Combine(OutputDirectory, fileName);
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.CloseOutput = true;
			settings.Indent = true;
			XmlWriter writer = XmlWriter.Create(fullPath, settings);
			return writer;
		}

		private void PrepareOutputDirectory()
		{
			outputDirectoryInfo = new DirectoryInfo(OutputDirectory);
			if (!outputDirectoryInfo.Exists)
				outputDirectoryInfo.Create();
		}

	}
}
