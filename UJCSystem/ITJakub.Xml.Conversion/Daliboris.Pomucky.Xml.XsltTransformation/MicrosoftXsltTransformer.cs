// -----------------------------------------------------------------------
// <copyright file="MicrosoftXsltTransformer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Specialized;

namespace Daliboris.Pomucky.Xml.XsltTransformation {
 using System;
 using System.Collections.Generic;
 using System.Text;
 using System.Xml.Xsl;
 using System.Xml;

 /// <summary>
 /// TODO: Update summary.
 /// </summary>
 public class MicrosoftXsltTransformer : IXsltTransformer {

	private bool disposed = false; // to detect redundant calls

	private XslCompiledTransform xslCompiledTransform;

 	public MicrosoftXsltTransformer()
 	{
 		Name = "Microsoft XslCompiledTransform";
		Version = "2.0";
 	}

 	/// <summary>
 	/// Name of Xslt transformer
 	/// </summary>
 	public string Name { get; private set; }

 	/// <summary>
	/// Version of the Xslt engine 
	/// </summary>
	public string Version { get; set; }

	/// <summary>
	/// Parameters used in the template
	/// </summary>
	public NameValueCollection Parameters { get; set; }


	public void Transform(string inputFile, string outputFile, NameValueCollection parameters) {
	 XsltArgumentList argumentList = new XsltArgumentList();
	 foreach (string parameter in parameters) {
		 if (parameters[parameter] != null)
			argumentList.AddParam(parameter, String.Empty, parameters[parameter]);
	 }

	 XmlWriterSettings xws = new XmlWriterSettings();
	 xws.CloseOutput = true;

		using (XmlWriter xw = XmlWriter.Create(outputFile, xws))
		{
			xslCompiledTransform.Transform(inputFile, argumentList, xw);
		}
	}

	/// <summary>
	/// Loads Xslt template for transformation
	/// </summary>
	/// <param name="xsltInformation"></param>
	/// <param name="settings"></param>
	public void Create(IXsltInformation xsltInformation, XsltTransformerSettings settings)
	{
	 XsltSettings xsltSettings = new XsltSettings(true, false);
	 XmlUrlResolver resolver = new XmlUrlResolver();
	 xslCompiledTransform = new XslCompiledTransform(false);
	 xslCompiledTransform.Load(xsltInformation.SourceFile, xsltSettings, resolver);
	}

	private void Dispose(bool disposing) {
	 if (!disposed) {
		if (disposing) {
		 // Dispose managed resources.
		 xslCompiledTransform = null;
		}

		// There are no unmanaged resources to release, but
		// if we add them, they need to be released here.
	 }
	 disposed = true;
	}

	#region Implementation of IDisposable

	/// <summary>
	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	/// </summary>
	/// <filterpriority>2</filterpriority>
	public void Dispose() {
	 Dispose(true);
	 GC.SuppressFinalize(this);
	}

	#endregion
 }
}
