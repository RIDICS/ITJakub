// -----------------------------------------------------------------------
// <copyright file="XsltInformation.cs" company="Dalibor Lehečka">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Specialized;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation {
 using System;
 using System.Collections.Generic;
 using System.Text;

	/// <summary>
 /// TODO: Update summary.
 /// </summary>
 public class XsltInformation : IXsltInformation
	{
 	public XsltInformation()
 	{
 		Parameters = new NameValueCollection();
	 IncludedXslt = new List<IXsltInformation>();
 	}

 	public XsltInformation(string sourceFile) : this()
 	{
 		SourceFile = sourceFile;
 	}

 	public bool IsValid { get; set; }
 	public string Version { get; set; }

		/// <summary>
		/// Ouput method of the xslt transformation.
		/// Value of the @method attribute in &lt;xsl:output&gt; tag.
		/// </summary>
	public string Method { get; set; }
	public List<IXsltInformation> IncludedXslt { get; set; }
 	public string SourceFile { get; set; }
 	public string SourceXml { get; set; }
	public NameValueCollection Parameters { get; set; }
 	public List<string> Extensions { get; set; }
 	public List<string> Errors { get; set; }

		/// <summary>
		/// If xslt transformation generates result document(s) or not.
		/// If result document is generated, no single output is expected.
		/// </summary>
		public bool GeneratesResultDocument { get; set; }
	}
}
