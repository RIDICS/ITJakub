﻿using System;
using System.Collections.Generic;

namespace Ujc.Ovj.Ooxml.Conversion
{
	public class DocxToTeiConverterSettings
	{

		#region Constructors

		public DocxToTeiConverterSettings()
		{
			SplitDocumentByPageBreaks = true;
#if(DEBUG)
			Debug = true;
#endif
		}


		#endregion

		#region Properties

		/// <summary>
		/// Full path to input file (DOCX).
		/// </summary>
		public string InputFilePath { get; set; }

		/// <summary>
		/// List of full paths to input files (DOCX). 
		/// In the case when one work is divided into multiple files (e.g. dictionaries divided by letter).
		/// </summary>
		public List<string> InputFilesPaths { get; set; }

		/// <summary>
		/// Full path to output file (XML).
		/// </summary>
		public string OutputFilePath { get; set; }

        /// <summary>
        /// Output full path for metadata file (XMD)
        /// </summary>
        public string OutputMetadataFilePath { get; set; }

        /// <summary>
        /// Path to the file with metadata about converted texts.
        /// </summary>
        public string MetadataFilePath { get; set; }

		/// <summary>
		/// Path of temp directory. Used to store temporary files.
		/// </summary>
		public string TempDirectoryPath { get; set; }

		/// <summary>
		/// If set to <value>true</value>, temporary files are not deleted.
		/// </summary>
		public bool Debug { get; set; }

		/// <summary>
		/// If set to <value>true</value>, document is divided to fragments by page breaks (&lt;pb&gt;).
		/// </summary>
		/// <remarks>Default value is <value>true</value>.</remarks>
		public bool SplitDocumentByPageBreaks { get; set; }

        public string DataDirectoryPath { get; set; }

		/// <summary>
		/// Function which returns list of version for document (by document id).
		/// </summary>
		public Func<string, List<VersionInfoSkeleton>> GetVersionList { get; set; }

	    

	    #endregion

		#region Methods

		#endregion

		#region Helpers

		#endregion



	}
}