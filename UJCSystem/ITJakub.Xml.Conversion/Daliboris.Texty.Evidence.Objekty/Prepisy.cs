namespace Daliboris.Texty.Evidence
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections;
    using System.ComponentModel;
    using System.Xml.Serialization;

    /// <summary>
    /// Kolekce přepisů
    /// </summary>
    /// 
    [XmlRoot("Prepisy", Namespace = "http://www.daliboris.cz/schemata/prepisy.xsd")]
    public class Prepisy : IBindingListView, IList<Prepis>
    {
        //public class Prepisy : ObservableCollection<Prepis>, ICollection<Prepis>, IBindingList, IList<Prepis> {

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [field: NonSerialized]
        public event ListChangedEventHandler ListChanged;

        private List<Prepis> mglPrepisy = new List<Prepis>();
        private List<Prepis> mglFiltrovanePrepisy = new List<Prepis>();
        private bool mblnJeSerazen = false;
        private bool mblnJeFiltrovan = false;
        PropertyDescriptor sortProperty = null;
        private ListSortDirection listSortDirection = ListSortDirection.Ascending;
        private int mintIdentifikator = 1;
        private Dictionary<string, int> mdcSouborZmena = new Dictionary<string, int>();


        private string mstrFiltr = "";

        public void Replace(Prepis prp, int intIdentifikator)
        {
            if (mblnJeFiltrovan)
            {
                ReplacePrepis(mglFiltrovanePrepisy, prp, intIdentifikator);
            }
            ReplacePrepis(mglPrepisy, prp, intIdentifikator);

        }

        private void ReplacePrepis(List<Prepis> prepisy, Prepis prp, int intIdentifikator)
        {
            for (int i = 0; i < prepisy.Count; i++)
            {
                if (prepisy[i].Identifikator == intIdentifikator)
                {
                    prp.Identifikator = intIdentifikator;
                    prepisy[i] = prp;
                }
            }
        }

        public Prepis Find(Predicate<Prepis> match)
        {
            if (mblnJeFiltrovan)
                return mglFiltrovanePrepisy.Find(match);
            else
                return mglPrepisy.Find(match);
        }

        public Prepisy FindAll(Predicate<Prepis> match)
        {
            Prepisy prp = new Prepisy();

            List<Prepis> glp = mglPrepisy.FindAll(match);
            if (glp.Count > 0)
            {
                prp.GLstPrepisy = glp;
                return prp;
            }
            else
                return null;
        }

        private List<Prepis> GLstPrepisy
        {
            get { return mglPrepisy; }
            set { mglPrepisy = value; }
        }
        //[XmlArray("prepis")]
        public Prepis this[int index]
        {
            get
            {
                if (mblnJeFiltrovan)
                    return mglFiltrovanePrepisy[index];
                else
                    return mglPrepisy[index];
            }
            set
            {
                if (mblnJeFiltrovan)
                    mglFiltrovanePrepisy[index] = value;
                mglPrepisy[index] = value;
            }
        }

        public Prepis this[string soubor]
        {
            get
            {
                if (mblnJeFiltrovan)
                    return mglFiltrovanePrepisy[mdcSouborZmena[soubor]];
                else
                    return mglPrepisy[mdcSouborZmena[soubor]];
            }
            set
            {
                if (mblnJeFiltrovan)
                    mglFiltrovanePrepisy[mdcSouborZmena[soubor]] = value;
                mglPrepisy[mdcSouborZmena[soubor]] = value;
            }
        }

        public bool ExistujeSoubor(string strNazevSouboru)
        {
            return mdcSouborZmena.ContainsKey(strNazevSouboru);
        }

        #region ICollection<Prepis> Members

        public void Add(Prepis item)
        {
            item.Identifikator = mintIdentifikator++;
            mglPrepisy.Add(item);
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Add(item);
            mdcSouborZmena.Add(item.NazevSouboru, mglPrepisy.Count - 1);
        }

        public void Clear()
        {
            mglPrepisy.Clear();
            mdcSouborZmena.Clear();
            mglFiltrovanePrepisy.Clear();
        }

        public bool Contains(Prepis item)
        {
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Contains(item);
            return mglPrepisy.Contains(item);
        }

        public void CopyTo(Prepis[] array, int arrayIndex)
        {
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.CopyTo(array, arrayIndex);
            else
                mglPrepisy.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                if (mblnJeFiltrovan)
                    return mglFiltrovanePrepisy.Count;
                else
                    return mglPrepisy.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Prepis item)
        {
            mdcSouborZmena.Remove(item.Soubor.Nazev);
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Remove(item);
            return mglPrepisy.Remove(item);
        }

        #endregion

        #region IEnumerable<Prepis> Members

        public IEnumerator<Prepis> GetEnumerator()
        {
            if (mblnJeFiltrovan)
                return mglFiltrovanePrepisy.GetEnumerator();
            else
                return mglPrepisy.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mglPrepisy.GetEnumerator();
        }

        #endregion

        #region IBindingList Members

        public void AddIndex(PropertyDescriptor property)
        {
            mblnJeSerazen = true;
            sortProperty = property;
        }

        public object AddNew()
        {
            object o = null;

            try
            {
                o = this.Add(o);
            }

            catch (System.Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return o;
        }

        public bool AllowEdit
        {
            get { return true; }
        }

        public bool AllowNew
        {
            get { return true; }
        }

        public bool AllowRemove
        {
            get { return true; }
        }

        public void Seradit(string nazevVlastnosti, ListSortDirection smerRazeni)
        {
            mblnJeSerazen = true;
            listSortDirection = smerRazeni;

            //////ArrayList a = new ArrayList();
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Sort(new PrepisComparer<Prepis>(nazevVlastnosti));
            else
                mglPrepisy.Sort(new PrepisComparer<Prepis>(nazevVlastnosti));
            //this.Sort(new ObjectPropertyComparer(property.Name));
            if (smerRazeni == ListSortDirection.Descending)
                if (mblnJeFiltrovan)
                    mglFiltrovanePrepisy.Reverse();
                else
                    mglPrepisy.Reverse();

        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            sortProperty = property;
            Seradit(sortProperty.Name, direction);
        }

        public int Find(PropertyDescriptor property, object key)
        {
            //throw new NotImplementedException();
            return 0;
        }

        public bool IsSorted
        {
            get { return mblnJeSerazen; }
        }


        public void RemoveIndex(PropertyDescriptor property)
        {
            //throw new NotImplementedException();
            return;
        }

        public void RemoveSort()
        {
            mblnJeSerazen = false;
            sortProperty = null;
        }

        public ListSortDirection SortDirection
        {
            get { return listSortDirection; }
        }

        public PropertyDescriptor SortProperty
        {
            get { return sortProperty; }
        }

        public bool SupportsChangeNotification
        {
            get { return true; }
        }

        public bool SupportsSearching
        {
            get { return true; }
        }

        public bool SupportsSorting
        {
            get { return true; }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            //throw new NotImplementedException();
	        Prepis prepis = value as Prepis;
					if(prepis == null)
            return -1;
					else
					{
						Add(prepis);
						return Count;
					}
        }

        public bool Contains(object value)
        {
            //throw new NotImplementedException();
            Prepis prp = value as Prepis;
            if (prp == null)
                return false;
            else
                return this.Contains(prp);
        }

        public int IndexOf(object value)
        {
            //throw new NotImplementedException();
            Prepis prp = value as Prepis;
            if (prp == null)
                return -1;
            else
            {
                if (mblnJeFiltrovan)
                    return mglFiltrovanePrepisy.IndexOf(prp);
                else
                    return mglPrepisy.IndexOf(prp);
            }
        }

        public void Insert(int index, object value)
        {
            //throw new NotImplementedException();
            return;
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Remove(object value)
        {
            //throw new NotImplementedException();
            Prepis prp = value as Prepis;
            if (prp == null)
                return;
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Remove(prp);
            mglPrepisy.Remove(prp);
            return;
        }

        public void RemoveAt(int index)
        {
            //throw new NotImplementedException();
            return;
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (Prepis)value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            //throw new NotImplementedException();
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.ToArray().CopyTo(array, index);
            else
                mglPrepisy.ToArray().CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            //get { return this.IsSynchronized; }
            get { return false; }
        }

        public object SyncRoot
        {
            //get { throw new NotImplementedException(); }

            get
            {
                if (mblnJeFiltrovan)
                    return mglFiltrovanePrepisy;
                else
                    return mglPrepisy;
            }
        }

        #endregion

        #region IList<Prepis> Members

        public int IndexOf(Prepis item)
        {
            if (mblnJeFiltrovan)
                return mglFiltrovanePrepisy.IndexOf(item);
            else
                return mglPrepisy.IndexOf(item);
        }

        public void Insert(int index, Prepis item)
        {
            if (mblnJeFiltrovan)
                mglFiltrovanePrepisy.Insert(index, item);
            mglPrepisy.Insert(index, item);
        }

        #endregion

        #region IBindingListView Members

        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            throw new NotImplementedException();
        }

        public string Filter
        {
            get
            {
                return mstrFiltr;
            }
            set
            {
                mstrFiltr = value;
                Prepisy prps = null;
                if (mstrFiltr.StartsWith("AutorSouborNazev"))
                {
                    mblnJeFiltrovan = true;
                    FiltrPrepisu flp = new FiltrPrepisu();
                    flp.Text = mstrFiltr.Split(new char[] { ' ' })[1];
                    prps = FindAll(flp.ObsahujeText);
                }
                else if (String.IsNullOrEmpty(mstrFiltr))
                {
                    mblnJeFiltrovan = false;
                }
                else
                {
                    Predicate<Prepis> pp = null;
                    switch (mstrFiltr)
                    {
                        case "odstraněné přepisy":
                            pp = FiltrPrepisu.OdstranenePrepisy;
                            break;
                        default:
                            break;
                    }
                    prps = FindAll(pp);
                }
                if (mblnJeFiltrovan)
                {
                    if (prps == null)
                        mglFiltrovanePrepisy = new List<Prepis>();
                    else
                        mglFiltrovanePrepisy = prps.GLstPrepisy;
                }

                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                //zpracovat filtr
            }
        }

        public void RemoveFilter()
        {
            Filter = "";
            mblnJeFiltrovan = false;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            //odstranit filtr
        }

        public ListSortDescriptionCollection SortDescriptions
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsAdvancedSorting
        {
            get { return false; }
        }

        public bool SupportsFiltering
        {
            get { return true; }
        }

        #endregion

        #region Události

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        protected void OnInsertComplete(int index, object value)
        {
            OnListChanged(new ListChangedEventArgs(
            ListChangedType.ItemAdded, index));
        }

        protected void OnRemoveComplete(int index, object value)
        {
            OnListChanged(new ListChangedEventArgs(
            ListChangedType.ItemDeleted, index));
        }

        protected void OnSetComplete(int index, object value)
        {
            OnListChanged(new ListChangedEventArgs(
            ListChangedType.ItemChanged, index));
        }

        #endregion
    }

}
