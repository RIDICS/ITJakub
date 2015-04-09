using System.Collections.Generic;
using System.Collections.Specialized;

namespace Daliboris.Pomucky.Xml.XsltTransformation
{
	public interface IXsltInformation
	{
		bool IsValid { get; set; }
		string Version { get; set; }
		List<IXsltInformation> IncludedXslt { get; set; }
		string SourceFile { get; set; }
		string SourceXml { get; set; }
		NameValueCollection Parameters { get; set; }
		List<string> Extensions { get; set; }
		List<string> Errors { get; set; }
	}
}