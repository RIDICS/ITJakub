using System;
using System.Runtime.InteropServices;
namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	[Guid("CC3FB34E-7FB7-4399-AAF4-BCC0BA8EFFAF")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IPravidla {
		void Add(IPravidlo item);
		void Clear();
		bool Contains(IPravidlo item);
		void CopyTo(IPravidlo[] array, int arrayIndex);
		int Count { get; }
		IPravidlo Get(IPravidlo item);
		[ComVisible(false)]
		System.Collections.Generic.IEnumerator<IPravidlo> GetEnumerator();
		bool IsReadOnly { get; }
		bool Remove(IPravidlo item);
		void Sort();
		IPravidlo this[int index] { get; set; }
	}
}
