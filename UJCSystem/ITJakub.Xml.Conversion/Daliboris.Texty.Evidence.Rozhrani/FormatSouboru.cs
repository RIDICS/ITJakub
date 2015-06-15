using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informace o formátu dokumentu, který obsahuje přepis tetu
	/// </summary>
	public enum FormatSouboru
	{
		[EnumDescription("neznámý")]
		Neznamy = 0,
		[EnumDescription("Word 97–2003")]
		Doc = 1,
		[EnumDescription("Word 2007")]
		Docx
	}
}