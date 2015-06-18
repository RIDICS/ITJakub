using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Daliboris.Pomucky.Xml
{


	public static class Objekty
	{

		public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

		public static void WriteXmlIdAttribute(XmlWriter writer, string value)
		{
			writer.WriteAttributeString("id", Objekty.XmlNamespace, value);
		}

		public static XmlReader VytvorXmlReader(string strSoubor)
		{
			XmlTextReader treader = new XmlTextReader(strSoubor);
			XmlReaderSettings xrs = new XmlReaderSettings();
			xrs.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
			xrs.IgnoreWhitespace = false;
			XmlReader r = XmlReader.Create(treader, xrs);
			return (r);
		}
		public static XmlWriter VytvorXmlWriter(string strSoubor)
		{
			return VytvorXmlWriter(strSoubor, false);
		}

		public static XmlWriter VytvorXmlWriter(string strSoubor, bool blnBezOdsazeni)
		{
			XmlWriterSettings xws = new XmlWriterSettings();
			xws.Encoding = System.Text.Encoding.UTF8;
			if (!blnBezOdsazeni)
			{
				xws.IndentChars = " ";
				xws.Indent = true;
			}
			else
			{
				xws.Indent = false;
				xws.IndentChars = "";
				xws.NewLineChars = "";
			}
			XmlWriter xw = XmlWriter.Create(strSoubor, xws);

			return (xw);
		}

		public static string ReadCurrentNodeContentAsString(XmlReader reader)
		{
			StringBuilder sb = new StringBuilder();
			if (reader.NodeType == XmlNodeType.Attribute)
				reader.MoveToElement();
			if (reader.NodeType != XmlNodeType.Element)
				return null;
			string nodeName = reader.Name;
			int depth = reader.Depth;
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Text)
					sb.Append(reader.Value);
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == nodeName && reader.Depth == depth)
				{
					return sb.ToString();
				}
			}
			return sb.ToString();

		}

		/// <summary>
		/// Načte aktuální uzel jako <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="reader"><see cref="XmlReader"/> na pozici elementu, jehož obsah se má načíst do dokumetnu.</param>
		/// <returns>Vrací <see cref="XmlDocument"/> tvořený aktuálním uzlem nebo prázdný dokument.</returns>
		public static XmlDocument ReadNodeAsXmlDocument(XmlReader reader)
		{
			XmlDocument xd = new XmlDocument();

			if (reader.NodeType == XmlNodeType.Attribute)
				reader.MoveToElement();
			if (reader.NodeType != XmlNodeType.Element)
				return null;
			string nodeName = reader.Name;
			int depth = reader.Depth;

			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.OmitXmlDeclaration = true;

				using (XmlWriter xw = XmlWriter.Create(memoryStream, settings))
				{
						string text = null;
						bool isEmpty = reader.IsEmptyElement;
					try
					{
						Transformace.SerializeNode(reader, xw);
						if (isEmpty && reader.Name == nodeName && reader.Depth == depth)
						{
							LoadXmlString(xw, memoryStream, xd);
							return xd;
						}
						while (reader.Read())
						{
							isEmpty = reader.IsEmptyElement;
							Transformace.SerializeNode(reader, xw);
							if (isEmpty && reader.Name == nodeName && reader.Depth == depth)
							{
								LoadXmlString(xw, memoryStream, xd);
								break;
							}
							if (reader.NodeType == XmlNodeType.EndElement && reader.Name == nodeName && reader.Depth == depth)
							{
								LoadXmlString(xw, memoryStream, xd);
								break;
							}
						}

					}
					catch (Exception e)
					{
						string error = e.Message;
						
					}

				}
			}

			return xd;
		}

		private static void LoadXmlString(XmlWriter xw, MemoryStream memoryStream, XmlDocument xd)
		{
			string text;
			xw.Close();
			text = Encoding.UTF8.GetString(memoryStream.ToArray());
			//na 1. pozici se vyskytuje BOM; 65279
			xd.LoadXml(text.Substring(1));
		}
	}
}
