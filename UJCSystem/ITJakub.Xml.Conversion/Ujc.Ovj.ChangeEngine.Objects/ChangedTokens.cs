using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
	[XmlRoot("changedTokens", Namespace = "http://vokabular.ujc.cas.cz/schema/changeEngine/v1")]
	public class ChangedTokens : Collection<ChangedToken>
	{
		
	}
}
