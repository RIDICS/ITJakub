using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty {

	//XXXXXXXX
	[XmlRoot("pravidla")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Pravidla : ICollection<Pravidlo>, IPravidla {

		private bool mblnIsReadOnly = false;
		private List<Pravidlo> mglsPravidla = new List<Pravidlo>();
		#region ICollection<Pravidlo> Members

		public void Add(Pravidlo item) {
			mglsPravidla.Add(item);
		}

		public void Clear() {
			mglsPravidla.Clear();
		}

		public bool Contains(Pravidlo item) {
			return mglsPravidla.Contains(item);
		}

		public void CopyTo(Pravidlo[] array, int arrayIndex) {
			mglsPravidla.CopyTo(array, arrayIndex);
		}

		public int Count {
			get { return mglsPravidla.Count; }
		}

		public bool IsReadOnly {
			get { return mblnIsReadOnly; }
		}

		public bool Remove(Pravidlo item) {
			return mglsPravidla.Remove(item);
		}

		#endregion
		public Pravidlo this[int index] {
			get { return mglsPravidla[index]; }
			set { mglsPravidla[index] = value; }
		}
		public Pravidlo Get(Pravidlo item) {
			if (!this.Contains(item))
				return null;
			foreach (Pravidlo prv in mglsPravidla) {
				if (prv == item)
					return prv;
			}
			return null;
		}

		public void Sort() {
			mglsPravidla.Sort();
		}



		#region IEnumerable<Pravidlo> Members

		public IEnumerator<Pravidlo> GetEnumerator() {
			return mglsPravidla.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		#endregion



		#region IPravidla - členové

		public void Add(IPravidlo item) {
			this.Add(item as Pravidlo);
		}

		public bool Contains(IPravidlo item) {
			return this.Contains(item as Pravidlo);
		}

		public void CopyTo(IPravidlo[] array, int arrayIndex) {
			this.CopyTo(array as Pravidlo[], arrayIndex);
		}

		public IPravidlo Get(IPravidlo item) {
			return this.Get(item as Pravidlo);
		}

		IEnumerator<IPravidlo> IPravidla.GetEnumerator() {
			return (IEnumerator<IPravidlo>) this.GetEnumerator();
		}

		public bool Remove(IPravidlo item) {
			return this.Remove(item as Pravidlo);
		}

		IPravidlo IPravidla.this[int index] {
			get {
				return this[index] as Pravidlo;
			}
			set {
				this[index] = value as Pravidlo;
			}
		}

		#endregion
	}
}
