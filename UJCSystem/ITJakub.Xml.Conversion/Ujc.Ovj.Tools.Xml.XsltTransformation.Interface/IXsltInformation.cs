using System.Collections.Generic;
using System.Collections.Specialized;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
{
	public interface IXsltInformation
	{
		bool IsValid { get; set; }
		string Version { get; set; }
		/// <summary>
		/// Ouput method of the xslt transformation.
		/// Value of the @method attribute in &lt;xsl:output&gt; tag.
		/// </summary>
		string Method { get; set; }
		List<IXsltInformation> IncludedXslt { get; set; }
		string SourceFile { get; set; }
		string SourceXml { get; set; }
		NameValueCollection Parameters { get; set; }
		List<string> Extensions { get; set; }
		List<string> Errors { get; set; }
		/// <summary>
		/// If xslt transformation generates result document(s) or not.
		/// If result document is generated, no single output is expected.
		/// </summary>
		bool GeneratesResultDocument { get; set; }
	}
}