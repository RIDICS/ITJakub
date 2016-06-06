using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;
using Microsoft.Win32;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
{
	class AltovaXsltExeTransformer : IXsltTransformer
	{

		private IXsltInformation _xsltInformation;
		private bool disposed = false; // to detect redundant calls
		private string _xsltVersion = null;
		private string _exePath = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		public AltovaXsltExeTransformer()
		{
			Name = "AltovaXMLExe";

		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

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
		/// Detailed informations about xslt transformation stylesheet
		/// </summary>
		public IXsltInformation XsltInformation
		{
			get { return _xsltInformation; }
		}

		/// <summary>
		/// Transformas input file using Xslt template.
		/// </summary>
		/// <param name="inputFile"></param>
		/// <param name="outputFile"></param>
		/// <param name="parameters"></param>
		public void Transform(string inputFile, string outputFile, NameValueCollection parameters)
		{
			Console.WriteLine("{0} > {1} ({2})", inputFile, outputFile, _xsltInformation.SourceFile);

			string parametersCmd = null;

			foreach (string parameter in parameters)
			{
				parametersCmd += String.Format("/param \"{0}='{1}'\" ", parameter, parameters[parameter]);
			}

			string arguments = string.Format("{0} {1} /in {2} {3} /out {4}", _xsltVersion, _xsltInformation.SourceFile, inputFile, parametersCmd, outputFile);
			
			Process process =  Process.Start(_exePath, arguments);
			process.WaitForExit();

		}

		/// <summary>
		/// Loads Xslt template for transformation
		/// </summary>
		/// <param name="xsltInformation"></param>
		/// <param name="settings"></param>
		public void Create(IXsltInformation xsltInformation, XsltTransformerSettings settings)
		{
			_xsltInformation = xsltInformation;
			if (settings.PreferredVersion == "1.0")
			{Version = "1.0";
			_xsltVersion = " /xslt1 ";
			}
			else
			{
				Version = "2.0";
				_xsltVersion = " /xslt2 ";
			}

			_exePath = GetCOMPath("AltovaXML.Application");


		}

		#endregion

		static string GetCOMPath(string comName)
		{
			RegistryKey comKey = Registry.ClassesRoot.OpenSubKey(comName + "\\CLSID");
			if (comKey == null)
				return null;
			string clsid = (string)comKey.GetValue("");
			comKey = Registry.ClassesRoot.OpenSubKey("CLSID\\" + clsid + "\\LocalServer32");
			if (comKey == null)
				comKey = Registry.ClassesRoot.OpenSubKey("Wow6432Node\\CLSID\\" + clsid + "\\LocalServer32");
			return (string)comKey.GetValue("");
		}
	}


}
