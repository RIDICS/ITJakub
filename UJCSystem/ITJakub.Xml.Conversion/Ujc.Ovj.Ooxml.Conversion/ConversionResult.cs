namespace Ujc.Ovj.Ooxml.Conversion
{
	public class ConversionResult
	{

		#region Constructors

		public ConversionResult()
		{
		}

		#endregion

		#region Properties

		public bool IsConverted { get; set; }

		public string Errors { get; set; }

		/// <summary>
		/// Path to the file with metadata about converted texts.
		/// </summary>
		public string MetadataFilePath { get; set; }


		#endregion

		#region Methods

		#endregion

		#region Helpers

		#endregion


		 
	}
}