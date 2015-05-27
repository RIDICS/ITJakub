// -----------------------------------------------------------------------
// <copyright file="AltovaXsltTransformer.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Specialized;

namespace Daliboris.Pomucky.Xml.XsltTransformation {
 using System;
 using System.Collections.Generic;
 using System.Text;
 using Altova.AltovaXML;

 /// <summary>
 /// TODO: Update summary.
 /// </summary>
 public class AltovaXsltTransformer : IXsltTransformer {

	private bool disposed = false; // to detect redundant calls
	Application application;
	IXSLT2 xslt2;
 	IXSLT1 xslt1;

 	public AltovaXsltTransformer()
 	{
 		Name = "AltovaXML";
 	}

	#region Implementation of IXsltTransformer

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

	/// <summary>
	/// Transformas input file using Xslt template.
	/// </summary>
	/// <param name="inputFile"></param>
	/// <param name="outputFile"></param>
	/// <param name="parameters"></param>
	public void Transform(string inputFile, string outputFile, NameValueCollection parameters) {

	 if(xslt1 != null)
	 {
		xslt1.InputXMLFileName = inputFile;
		xslt1.ClearExternalParameterList();
		foreach (string parameter in parameters) {
		 xslt1.AddExternalParameter(parameter, parameters[parameter]);
		}
		xslt1.Execute(outputFile);
	 }

	 if(xslt2 != null)
	 {
		xslt2.InputXMLFileName = inputFile;
		xslt2.ClearExternalParameterList();
		foreach (string parameter in parameters) {
		 xslt2.AddExternalParameter(parameter, parameters[parameter]);
		}
		xslt2.Execute(outputFile);
	 }


	}

	/// <summary>
	/// Loads Xslt template for transformation
	/// </summary>
	/// <param name="xsltInformation"></param>
	/// <param name="settings"></param>
	public void Create(IXsltInformation xsltInformation, XsltTransformerSettings settings) {
	 application = new Application();
	 if(settings.PreferredVersion == "1.0")
	 {
		Version = "1.0";
		xslt1 = application.XSLT1;
	 	xslt1.XSLFileName = xsltInformation.SourceFile;
	 }
	 else
	 {
		Version = "2.0";
		xslt2 = application.XSLT2;
		xslt2.XSLFileName = xsltInformation.SourceFile;
	 }
	}

	#endregion

 	private void Dispose(bool disposing) {
	 if (!disposed) {
		if (disposing) {
		 // Dispose managed resources.
		}

		// There are no unmanaged resources to release, but
		// if we add them, they need to be released here.
		if(xslt2 != null)
		{
			System.Runtime.InteropServices.Marshal.ReleaseComObject(xslt2);
	 	 xslt2 = null;
		}

		if(xslt1 != null)
		{
			
		}

	 	System.Runtime.InteropServices.Marshal.ReleaseComObject(application);}
	 	application = null;
	 disposed = true;
	}
	

 	#region Implementation of IDisposable

 	/// <summary>
 	/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
 	/// </summary>
 	/// <filterpriority>2</filterpriority>
 	public void Dispose()
 	{
 		Dispose(true);
	 GC.SuppressFinalize(this);
 	}

 	#endregion
 }
}
