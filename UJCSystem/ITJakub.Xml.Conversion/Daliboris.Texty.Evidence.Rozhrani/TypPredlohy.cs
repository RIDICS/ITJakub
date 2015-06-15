using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informaco o předloze, která posloužila pro přepis textu
	/// </summary>
	public enum TypPredlohy
	{
		/// <summary>
		/// neznámý typ předlohy
		/// </summary>
		[EnumDescription("neznámý")]
		Neznamy = 0,
		/// <summary>
		/// předlohu přepisu tvoří rukopis
		/// </summary>
		[EnumDescription("rukopis")]
		Rukopis = 1,
		/// <summary>
		/// předlohu přepisu tvoří prvotisk (tisk do roku 1500)
		/// </summary>
		[EnumDescription("prvotisk")]
		Prvotisk = 2,
		/// <summary>
		/// předlohu přepisu tvoří starý tisk (z let 1500–1800)
		/// </summary>
		[EnumDescription("starý tisk")]
		StaryTisk = 4,
		/// <summary>
		/// předlohu přepisu tvoří novodobá edice
		/// </summary>
		[EnumDescription("edice")]
		Edice = 8
	}
}