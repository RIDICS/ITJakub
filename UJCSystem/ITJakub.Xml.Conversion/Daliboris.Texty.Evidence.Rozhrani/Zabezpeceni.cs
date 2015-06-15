using Campari.Software.Reflection;
using System;

namespace Daliboris.Texty.Evidence.Rozhrani {

 /// <summary>
 /// Informace o zabezpečení dokumentu, pro jaký typ uživatelů je přístupný
 /// </summary>
 [Flags]
 public enum Zabezpeceni {

	/// <summary>
	/// Veřejný
	/// </summary>
	[EnumDescription("Veřejné")]
	Verejne = 1,

	/// <summary>
	/// Spolupracovníci
	/// </summary>
	[EnumDescription("Spolupracovníci")]
	Spolupracovnici = 2,

	/// <summary>
	/// Hodnotitelé (GA ČR, AV ČR)
	/// </summary>
	[EnumDescription("Hodnotitelé")]
	Hodnotitele = 4,

	/// <summary>
	/// Interní
	/// </summary>
	[EnumDescription("Interní")]
	Interni = 128

 }
}
