using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
	public class KonverzeNastaveni
	{

		public KonverzeNastaveni(string strDocx, string strDoc2Xml, string strXml)
		{
			Docx = strDocx;
			Doc2Xml = strDoc2Xml;
			Xml = strXml;
		}

		public string Docx { get; set; }
		public string Doc2Xml { get; set; }
		public string Xml { get; set; }
		public bool BezOdsazeni { get; set; }

	}
}
