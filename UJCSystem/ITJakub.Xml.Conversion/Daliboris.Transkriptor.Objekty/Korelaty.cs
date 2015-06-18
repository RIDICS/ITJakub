using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty
{
	//[DebuggerDisplay("Korelat: {Transliterace.Text} == {Transkripce.Text}")]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Korelaty : ICollection<Korelat>
    {
        private bool mblnIsReadOnly = false;
        private List<Korelat> mglsKorelaty = new List<Korelat>();

		  public int PocetUsouvztaznenychZnaku(TypPrepisu tpTypPrepisu) {
			  int iPocet = 0;
			  foreach (Korelat kor in mglsKorelaty) {
				  switch (tpTypPrepisu) {
					  case TypPrepisu.Tansliterace:
						  iPocet += kor.Transliterace.PocetZnaku;
						  break;
					  case TypPrepisu.Transkripce:
						  iPocet += kor.Transkripce.PocetZnaku;
						  break;
					  default:
						  break;
				  }
			  }
			  return iPocet;
		  }

        #region ICollection<Korelat> Members

        public void Add(Korelat item)
        {
            mglsKorelaty.Add(item);
        }

        public void Clear()
        {
            mglsKorelaty.Clear();
        }

        public bool Contains(Korelat item)
        {
        	return mglsKorelaty.Contains(item);
        }

        public void CopyTo(Korelat[] array, int arrayIndex)
        {
            mglsKorelaty.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mglsKorelaty.Count; }
        }

        public bool IsReadOnly
        {
            get { return mblnIsReadOnly; }
        }

        public bool Remove(Korelat item)
        {
            return mglsKorelaty.Remove(item);
        }

        #endregion

				public void Sort() {
					mglsKorelaty.Sort();
				}

        #region IEnumerable<Korelat> Members

        public IEnumerator<Korelat> GetEnumerator()
        {
            return mglsKorelaty.GetEnumerator();
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
