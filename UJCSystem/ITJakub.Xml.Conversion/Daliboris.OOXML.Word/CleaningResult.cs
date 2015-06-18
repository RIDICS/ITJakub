using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.OOXML.Word
{
	public class CleaningResult
	{
		public int NumerOfChanges { get; set; }
		public bool Success { get; set; }
		public Exception Exception { get; set; }
		public string Output { get; set; }
	}
}
