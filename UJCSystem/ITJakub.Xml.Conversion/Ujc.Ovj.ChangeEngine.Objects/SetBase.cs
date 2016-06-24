using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	public class SetBase
	{
		/// <summary>
		/// Jedinečný identifikátor pravidel. Slouží pro kombinaci více pravidel.
		/// </summary>
		[XmlAttribute(AttributeName = "identifier")]
		public string Identifier { get; set; }

		/// <summary>
		/// Jazyk pravidel, tj. na jaký jazyk má cenu pravidla aplikovat.
		/// </summary>
		[XmlAttribute(AttributeName = "language")]
		public string Language { get; set; }

		/// <summary>
		/// Zaměření; k čemu se pravidla hodí. Slouží k rozčlenění pravidel do větších celků.
		/// </summary>
		[XmlElement(ElementName = "target")]
		public string Target { get; set; }

		/// <summary>
		/// Podrobnější popis pravidel. Jejich obsah, autor ap.
		/// </summary>
		[XmlElement(ElementName = "description")]
		public string Description { get; set; } 
	}
}