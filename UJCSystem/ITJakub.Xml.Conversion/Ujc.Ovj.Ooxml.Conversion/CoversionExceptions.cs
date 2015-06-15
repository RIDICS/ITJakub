using System;

namespace Ujc.Ovj.Ooxml.Conversion
{
	public class DocumentSplittingException : Exception
	{
		public DocumentSplittingException(string message) : base(message)
		{
		}

		public DocumentSplittingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class NotSupportedFileFormatException : Exception
	{
		public NotSupportedFileFormatException(string message) : base(message)
		{
		}

		public NotSupportedFileFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public class DocumentNotInEvidenceException : Exception
	{
		public DocumentNotInEvidenceException(string message) : base(message)
		{
		}

		public DocumentNotInEvidenceException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}