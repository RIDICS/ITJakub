using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informace o změnách přepisu (aktuální verze versus verze na disku)
	/// </summary>
	public enum StatusAktualizace
	{
		[EnumDescription("beze změn")]
		BezeZmen,
		[EnumDescription("nové")]
		Nove,
		[EnumDescription("odstraněno")]
		Odstraneno,
		[EnumDescription("změněno")]
		Zmeneno
	}
}