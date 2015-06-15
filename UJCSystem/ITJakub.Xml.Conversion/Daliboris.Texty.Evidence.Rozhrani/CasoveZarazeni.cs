using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informace o základním časovém zařazení přepsaného pramene, popř. památky
	/// </summary>
	public enum CasoveZarazeni
	{
		/// <summary>
		/// Neznámé časové zařazení
		/// </summary>
		[EnumDescription("nezařazeno")]
		Nezarazeno = 0,
		/// <summary>
		/// Staročeská díla vzniklá do roku 1500
		/// </summary>
		/// 
		[EnumDescription("do roku 1500")]
		DoRoku1500 = 1500,
		/// <summary>
		/// Středočeská díla vzniklá mezi léty 1500 až 1800
		/// </summary>
		/// 
		[EnumDescription("do roku 1800")]
		DoRoku1800 = 1800,
		/// <summary>
		/// Novodobé texty
		/// </summary>
		/// 
		[EnumDescription("novodobé texty")]
		PoRoce2000 = 2000
	}
}