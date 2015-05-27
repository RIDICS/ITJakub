using System;
using System.Collections.Specialized;

namespace Daliboris.Pomucky.Xml.XsltTransformation {
 public interface IXsltTransformer : IDisposable {

	/// <summary>
	/// Name of Xslt transformer
	/// </summary>
	string Name { get; }

 	/// <summary>
	/// Version of the Xslt engine 
	/// </summary>
	string Version { get; set; }

	/// <summary>
	/// Parameters used in the template
	/// </summary>
	NameValueCollection Parameters { get; set; }

	/// <summary>
	/// Transformas input file using Xslt template.
	/// </summary>
	/// <param name="inputFile"></param>
	/// <param name="outputFile"></param>
	/// <param name="parameters"></param>
	void Transform(string inputFile, string outputFile, NameValueCollection parameters);

	/// <summary>
	/// Loads Xslt template for transformation
	/// </summary>
	/// <param name="xsltInformation"></param>
	/// <param name="settings"></param>
	void Create(IXsltInformation xsltInformation, XsltTransformerSettings settings);

 }
}