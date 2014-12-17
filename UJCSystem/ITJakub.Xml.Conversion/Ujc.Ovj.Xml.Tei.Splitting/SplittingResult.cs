using System.Collections.Generic;

namespace Ujc.Ovj.Xml.Tei.Splitting
{
	public class SplittingResult
	{

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public SplittingResult(string inputFileFullPath, string outputDirectory) : this(inputFileFullPath)
		{
			OutputDirectory = outputDirectory;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public SplittingResult(string inputFileFullPath)
			: this()
		{
			InputFileFullPath = inputFileFullPath;
		}

		public SplittingResult()
		{
			PageBreaksSplitInfo = new List<PageBreakSplitInfo>();
		}

		#endregion

		#region Properties

		public string InputFileFullPath { get; set; }

		public string OutputDirectory { get; set; }

		public bool IsSplitted { get; set; }

		public IList<PageBreakSplitInfo> PageBreaksSplitInfo { get; set; }

		public string Errors { get; set; }

		#endregion

		#region Methods

		#endregion

		#region Helpers

		#endregion


		 
	}
}