using System;
using System.Runtime.InteropServices;
namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	[Guid("09E44465-B15E-496D-A8A1-C9A601C79B96")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface IDoklady {
		void Add(IDoklad item);
		void Clear();
		bool Contains(IDoklad item);
		void CopyTo(IDoklad[] array, int arrayIndex);
		int Count { get; }
		IDoklad Get(IDoklad item);
		[ComVisible(false)]
		System.Collections.Generic.IEnumerator<IDoklad> GetEnumerator();
		bool IsReadOnly { get; }
		bool Remove(IDoklad item);
		void Sort();
		IDoklad this[int index] { get; set; }
	}
}
