using System;
using System.Runtime.InteropServices;
namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	[Guid("BFA2AB21-1811-4949-B9B4-5F1FCC4C69A7")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IPravidlo {
		int CompareTo(IPravidlo other);
		IDoklady Doklady { get; set; }
		bool DokladySpecified { get; }
		bool Equals(IPravidlo other);
		bool Equals(object obj);
		int GetHashCode();
		string Jazyk { get; set; }
		string Transkripce { get; set; }
		string Transliterace { get; set; }
		string Urceni { get; set; }
        bool IsRegularExpression { get; set; }
	}
}
