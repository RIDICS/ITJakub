using System;
using System.Runtime.InteropServices;
namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	[Guid("7F1AC534-D0A0-4F21-9DEC-1A85DD0928A1")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IDoklad {
		int CompareTo(IDoklad other);
		bool Equals(IDoklad other);
		string Transkripce { get; set; }
		string Transliterace { get; set; }
	}
}
