using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	[XmlRoot("changedTokenSet", Namespace = "http://vokabular.ujc.cas.cz/schema/changeEngine/v1")]
	public class ChangedTokenSet : SetBase
	{

		[XmlArray("changedTokens")]
		[XmlArrayItem("changedToken")]
		public ChangedTokens ChangedTokens { get; set; }
	}
}