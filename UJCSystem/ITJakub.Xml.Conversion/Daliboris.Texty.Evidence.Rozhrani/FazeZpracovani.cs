using Campari.Software.Reflection;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	/// <summary>
	/// Informace o fázi zpracování, v níž se přepis nachází
	/// </summary>
	public enum FazeZpracovani
	{
		[EnumDescription("odloženo")]
		Odlozeno = 0,
		[EnumDescription("zadáno")]
		Zadano = 10,
		[EnumDescription("přepsáno")]
		Prepsano = 20,
		[EnumDescription("textová kontrola")]
		TextovaKontrola = 30,
		[EnumDescription("formální kontrola")]
		FormalniKontrola = 40,
		[EnumDescription("odložený export")]
		OdlozitExport = 45,
		[EnumDescription("exportovat")]
		Exportovat = 50,
		[EnumDescription("exportováno")]
		Exportovano = 60
	}
}