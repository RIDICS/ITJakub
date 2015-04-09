using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Daliboris.Transkripce.Objekty
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
		public class Prepisy : ICollection<Prepis>
		{
				private const bool cblnIsReadOnly = false;
				private readonly List<Prepis> mglsPrepisy = new List<Prepis>();


				#region ICollection<Prepis> Members

				public void Add(Prepis item)
				{
						mglsPrepisy.Add(item);
				}

				public void Clear()
				{
						mglsPrepisy.Clear();
				}

				public bool Contains(Prepis item)
				{
						return mglsPrepisy.Contains(item);
				}

				public void CopyTo(Prepis[] array, int arrayIndex)
				{
						mglsPrepisy.CopyTo(array, arrayIndex);
				}

				public int Count
				{
						get { return mglsPrepisy.Count; }
				}

				public bool IsReadOnly
				{
						get { return cblnIsReadOnly; }
				}

				public bool Remove(Prepis item)
				{
						return mglsPrepisy.Remove(item);
				}

				#endregion

				#region IEnumerable<Prepis> Members

				public IEnumerator<Prepis> GetEnumerator()
				{
						return mglsPrepisy.GetEnumerator();
				}

				#endregion

				#region IEnumerable Members

				System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
				{
						return this.GetEnumerator();
				}

				#endregion
		}
}
