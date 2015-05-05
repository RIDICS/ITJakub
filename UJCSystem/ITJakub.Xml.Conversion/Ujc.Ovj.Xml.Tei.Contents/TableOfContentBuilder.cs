using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ujc.Ovj.Xml.Tei.Contents
{
	public class TableOfContentBuilder
	{
		private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
		private const string TeiNamespace = "http://www.tei-c.org/ns/1.0";
		private const string ItJakubTeiNamespace = "http://vokabular.ujc.cas.cz/ns/it-jakub/tei/1.0";


		#region Vlastnosti

		/// <summary>
		/// Soubor XML, pro nějž se má vytvořit obsah
		/// </summary>
		public string XmlFile { get; set; }

		/// <summary>
		/// Složka, do níž se uloží jednotlivé části rozděleného souboru
		/// </summary>
		public string OutputDirectory { get; set; }

		/// <summary>
		/// Počáteční element, od něhož začne rozdělování souboru na menší části
		/// </summary>
		public string StartingElement { get; set; }

		#endregion

		#region Metody

		public TableOfContentResult MakeTableOfContent()
		{

			TableOfContentResult result = new TableOfContentResult();

			bool splittingStarted = (StartingElement == null);

			FileInfo xmlFileInfo = new FileInfo(XmlFile);

			string elementName = null;

			using (XmlReader reader = XmlReader.Create(XmlFile))
			{
				reader.MoveToContent();
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element)
					{
						elementName = reader.Name;
						bool isEmpty = reader.IsEmptyElement;

						if (!splittingStarted)
							if (reader.Name == StartingElement)
							{
								splittingStarted = true;
							}
						if (!splittingStarted)
							continue;

					}
				}

				return result;
			}

			#endregion

		}
	}
}
