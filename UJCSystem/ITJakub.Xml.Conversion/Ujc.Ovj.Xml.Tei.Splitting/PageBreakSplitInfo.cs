namespace Ujc.Ovj.Xml.Tei.Splitting
{
	public class PageBreakSplitInfo
	{

		#region Constructors

	    /// <summary>
	    /// Initializes a new instance of the <see cref="T:System.Object"/> class.
	    /// </summary>
	    public PageBreakSplitInfo(SourceDocumentInfo sourceDocumentInfo)
	    {
	        SourceDocumentInfo = sourceDocumentInfo;
	    }

	    public PageBreakSplitInfo()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public PageBreakSplitInfo(string fileName, string fullPath)
		{
			FileName = fileName;
			FullPath = fullPath;
		}

	    /// <summary>
	    /// Initializes a new instance of the <see cref="T:System.Object"/> class.
	    /// </summary>

	    /// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public PageBreakSplitInfo(string id, string number, string fileName, string fullPath)
		{
			Id = id;
			Number = number;
			FileName = fileName;
			FullPath = fullPath;
		}

		#endregion

		#region Properties

		public string Id { get; set; }
		public string Number { get; set; }
		public string FileName { get; set; }
		public string FullPath { get; set; }
		//File name with facsimile picture of the page.
		public string Facsimile { get; set; }
        /// <summary>
        /// Identifier and version identifier of the source document
        /// </summary>
        public SourceDocumentInfo SourceDocumentInfo { get; set; }
		#endregion

		#region Methods

		#endregion

		#region Helpers

		#endregion


		 
	}
}