using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Identifikace metadat ukládaných s dokumentem ve Windows
	/// </summary>
	public enum Metaudaje
	{
		[EnumDescription("identifikátor (GUID)")]
		htx_id,
		[EnumDescription("čas posledního exportu do MNS")]
		htx_posledniExport_1,
		[EnumDescription("čas posledního exportu do STB")]
		htx_posledniExport_2,
		[EnumDescription("čas posledního exportu do střč. korpusu")]
		htx_posledniExport_4,
		[EnumDescription("čas posledního exportu do...")]
		htx_posledniExport_8,
		[EnumDescription("aktuální fáze zpracování dokumentu")]
		htx_fazeZpracovani,
		[EnumDescription("základní časové zařzení pramene")]
		ovj_casoveZarazeni,
		[EnumDescription("způsoby využití přepisu")]
		ovj_zpusobVyuziti
	}
}