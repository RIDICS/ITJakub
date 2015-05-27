using System;
using System.Xml.Serialization;
using System.Xml;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	public static class Spravce {
		public static void Serializuj(object objekt, string strSoubor, bool blnOdsadit) {
			if (objekt == null)
				throw new NullReferenceException("Objekt je nastaven na hodnotu null");
			XmlSerializer xs = new XmlSerializer(objekt.GetType());
			XmlWriterSettings xws = new XmlWriterSettings();
			xws.CloseOutput = true;
			xws.Indent = true;
			XmlWriter xw = XmlWriter.Create(strSoubor, xws);
			if (xw == null) return;
			xs.Serialize(xw, objekt);
			xw.Close();
		}
		public static void Serializuj(object objekt, string strSoubor) {
			Serializuj(objekt, strSoubor, true);
		}
		public static object Deserializuj(Type tpTyp, String strSoubor) {
			XmlSerializer xs = new XmlSerializer(tpTyp);
			XmlReader xr = XmlReader.Create(strSoubor);
			Object obj = xs.Deserialize(xr);
			xr.Close();
			return obj;

		}
	}
}
