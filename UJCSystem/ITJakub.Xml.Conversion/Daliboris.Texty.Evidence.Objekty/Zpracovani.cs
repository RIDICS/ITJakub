#region

using System;
using System.Collections.Generic;
using Daliboris.Texty.Evidence.Rozhrani;
using System.Linq;

#endregion

namespace Daliboris.Texty.Evidence {

	/// <summary>
	/// Základní informace o aktuálním a budoucím zpracování přepisu
	/// </summary>
	public class Zpracovani : IEquatable<IZpracovani>, IZpracovani
	{
		private List<IExport> milstExporty;
		private List<IExport> milstPrvniExporty;

		private List<Export> mglsExporty;
		private List<Export> mglsPrvniExporty;

		private IList<IVydani> milstPublikace;
		private List<Vydani> mglstPublikace;

		public Zpracovani()
		{
			mglsPrvniExporty = new List<Export>();
			mglsExporty = new List<Export>();

			milstExporty = new List<IExport>();
			milstPrvniExporty = new List<IExport>();

			milstPublikace = new List<IVydani>();
			mglstPublikace = new List<Vydani>();
		}

		private CasoveZarazeni mcszCasoveZarazeni;
/*
		private readonly string mstrObdobiVzniku;
*/

		public IExport ZjistiPosledniExport(ZpusobVyuziti zvZpusobVyuziti) {
			Export exp = new Export(zvZpusobVyuziti, DateTime.MinValue);
			foreach (Export ex in mglsExporty) {
				if (ex.ZpusobVyuziti == zvZpusobVyuziti)
					if (exp.CasExportu < ex.CasExportu) {
						exp.CasExportu = ex.CasExportu;
					    ex.KontrolniSoucet = ex.KontrolniSoucet;
					}
			}
			if (exp.CasExportu == DateTime.MinValue)
				return null;
			return exp;
		}


		public IExport ZjistiPrvniExport(ZpusobVyuziti zvZpusobVyuziti) {
			foreach (Export ex in milstPrvniExporty) {
				if (ex.ZpusobVyuziti == zvZpusobVyuziti)
					return ex;
			}
			foreach (Export ex in mglsPrvniExporty) {
				if (ex.ZpusobVyuziti == zvZpusobVyuziti)
					return ex;
			}
			return null;
		}
		public void ZaevidujExport(IExport exp) {
			Export e = null;
			if (!milstExporty.Contains<IExport>(exp))
				milstExporty.Add(exp);
			foreach (Export ex in milstPrvniExporty) {
				if (ex.ZpusobVyuziti == exp.ZpusobVyuziti)
					e = ex;
			}
			if (e == null)
				milstPrvniExporty.Add(exp);
			else {
				if (e.CasExportu > exp.CasExportu) {
					milstPrvniExporty.Remove(e);
					milstPrvniExporty.Add(exp);
				}
			}

			e = null;
			Export exp2 = new Export(exp.ZpusobVyuziti, exp.CasExportu, exp.KontrolniSoucet);
			if (!mglsExporty.Contains<Export>(exp2))
				mglsExporty.Add(exp2);
			foreach (Export ex in mglsPrvniExporty) {
				if (ex.ZpusobVyuziti == exp2.ZpusobVyuziti)
					e = ex;
			}
			if (e == null)
				mglsPrvniExporty.Add(exp2);
			else {
				if (e.CasExportu > exp.CasExportu) {
					mglsPrvniExporty.Remove(e);
					mglsPrvniExporty.Add(exp2);
				}
			}

		}

		public void ZaevidujExport(ZpusobVyuziti zvZpusobVyuziti, DateTime dtCasExportu, string kontrolniSoucet)
		{
			ZaevidujExport(new Export(zvZpusobVyuziti, dtCasExportu));
		}

		public string GUID { get; set; }
		public FazeZpracovani FazeZpracovani {
			get;
			set;
		}
		public CasoveZarazeni CasoveZarazeni {
			get { return mcszCasoveZarazeni; }
			set { mcszCasoveZarazeni = value; }
		}

		public bool Neexportovat { get; set; }


		//[XmlArray("Exporter")]
		//[XmlArrayItem("Exportik")]
		public List<Export> Exporty {
			//get { return ((IZpracovani)this).Exporty.Cast<Export>().ToList(); }
			//set { ((IZpracovani)this).Exporty = value.Cast<IExport>().ToList(); }

			get
			{
				if (milstExporty.Count > mglsExporty.Count)
				{
					mglsExporty = new List<Export>();
					foreach (IExport export in milstExporty)
					{
						Export e = new Export(export.ZpusobVyuziti, export.CasExportu, export.KontrolniSoucet);
						e.NazevSouboru = export.NazevSouboru;
						mglsExporty.Add(e);
					}
				}
				return mglsExporty;
			}
			set
			{
				mglsExporty = value;
				//((IZpracovani)this).Exporty = value.Cast<IExport>().ToList();
				milstExporty = value.Cast<IExport>().ToList();
			}

		}

		//private Exporty mexpExporty;
		//public Exporty Exportovani
		//{
		//  get
		//  {
		//    if(mexpExporty == null)
		//      mexpExporty = new Exporty(milstExporty);
		//    return mexpExporty;
		//  }
		//  set { mexpExporty = value;}
		//}

		List<IExport> IZpracovani.Exporty {
			get {
				if (milstExporty == null)
					milstExporty = new List<IExport>();
				if (mglsExporty.Count != milstExporty.Count)
				{
				  milstExporty = mglsExporty.Cast<IExport>().ToList();
				}
				return milstExporty;
			}
			set { milstExporty = value; }
		}
		public ZpusobVyuziti ZpusobVyuziti { get; set; }
		public string Komentar { get; set; }


		public List<Export> PrvniExporty {
			//get { return ((IZpracovani)this).PrvniExporty.Cast<Export>().ToList(); }
			//set { ((IZpracovani)this).PrvniExporty = value.Cast<IExport>().ToList(); }

			get { return mglsPrvniExporty; }
			set { mglsPrvniExporty = value; }

		}

		List<IExport> IZpracovani.PrvniExporty {
			get {
				if (milstPrvniExporty == null)
					milstPrvniExporty = new List<IExport>();
				if (milstPrvniExporty.Count != mglsPrvniExporty.Count)
				{
					milstPrvniExporty = mglsPrvniExporty.Cast<IExport>().ToList();
				}
				return milstPrvniExporty;
			}
		  set { milstPrvniExporty = value.Cast<IExport>().ToList(); }
		}
		public long ObjemDat { get; set; }
		public string ZkratkaPamatky { get; set; }
		public string ZkratkaPramene { get; set; }
		public string LiterarniDruh { get; set; }
		public string LiterarniZanr { get; set; }

		/// <summary>
		/// Zda se má památka exportovat v transliterované podobě
		/// (pouze pro potřeby textové banky, nikoli Manuscriptoria)¨.
		/// </summary>
		public bool Transliterovane { get; set; }

		/// <summary>
		/// Zda jde o biblický text, jehož značení biblických knih se má exportovat.
		/// </summary>
		public bool JeBiblickyText
		{
			get { return LiterarniZanr == "biblický text"; }
		}

		/// <summary>
		/// Zda přepis obsahuje text ediční poznámky.
		/// </summary>
		public bool MaEdicniPoznamku { get; set; }
		

		public Zabezpeceni Zabezpeceni { get; set; }

		public int PocetTokenu { get; set; }

		public int PocetJedinecnychTokenu { get; set; }

		public string GrantovaPodpora { get; set; }

/*
		private void UrciCasoveZarazeni(Datace dtcDatace) {
			if (mcszCasoveZarazeni == CasoveZarazeni.Nezarazeno) {
				if (dtcDatace.Stoleti <= 1500)
					mcszCasoveZarazeni = CasoveZarazeni.DoRoku1500;
				else if (dtcDatace.Stoleti <= 1800)
					mcszCasoveZarazeni = CasoveZarazeni.DoRoku1800;
				else if (dtcDatace.Stoleti > 1800)
					mcszCasoveZarazeni = CasoveZarazeni.PoRoce2000;
			}
			mstrObdobiVzniku = AnalyzatorDatace.UrcitObdobiVzniku(dtcDatace);

		}
*/

		public static bool operator ==(Zpracovani rhs, Zpracovani lhs)
		{
			if (rhs.CasoveZarazeni == lhs.CasoveZarazeni && rhs.Exporty.Equals(lhs.Exporty) &&
								rhs.FazeZpracovani == lhs.FazeZpracovani && rhs.GUID == lhs.GUID &&
								rhs.Komentar == lhs.Komentar && rhs.Neexportovat == lhs.Neexportovat &&
								rhs.ZpusobVyuziti == lhs.ZpusobVyuziti && 
								rhs.PocetTokenu == lhs.PocetTokenu && rhs.PocetJedinecnychTokenu == lhs.PocetJedinecnychTokenu && 
				rhs.GrantovaPodpora == lhs.GrantovaPodpora)
				return true;
			return false;
		}

		public static bool operator !=(Zpracovani rhs, Zpracovani lhs) {
			return !(rhs == lhs);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		/// </param>
		public bool Equals(IZpracovani other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Exporty, milstExporty) &&
			       Equals(other.PrvniExporty, milstPrvniExporty) &&
			       Equals(other.CasoveZarazeni, mcszCasoveZarazeni) &&
			       Equals(other.GUID, GUID) &&
			       Equals(other.FazeZpracovani, FazeZpracovani) &&
			       other.Neexportovat.Equals(Neexportovat) &&
			       Equals(other.ZpusobVyuziti, ZpusobVyuziti) &&
			       Equals(other.Komentar, Komentar) &&
			       other.ObjemDat == ObjemDat &&
			       Equals(other.ZkratkaPamatky, ZkratkaPamatky) &&
			       Equals(other.ZkratkaPramene, ZkratkaPramene) &&
			       Equals(other.LiterarniDruh, LiterarniDruh) &&
			       Equals(other.LiterarniZanr, LiterarniZanr) &&
						 Equals(other.PocetTokenu, PocetTokenu) && 
						 Equals(other.PocetJedinecnychTokenu, PocetJedinecnychTokenu) &&
						 Equals(other.GrantovaPodpora, GrantovaPodpora);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
		///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
		///                 </exception><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Zpracovani)) return false;
			return Equals((Zpracovani) obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			unchecked
			{
				int result = (milstExporty != null ? milstExporty.GetHashCode() : 0);
				result = (result*397) ^ (milstPrvniExporty != null ? milstPrvniExporty.GetHashCode() : 0);
				result = (result*397) ^ mcszCasoveZarazeni.GetHashCode();
				//result = (result*397) ^ (mstrObdobiVzniku != null ? mstrObdobiVzniku.GetHashCode() : 0);
				result = (result*397) ^ (GUID != null ? GUID.GetHashCode() : 0);
				result = (result*397) ^ FazeZpracovani.GetHashCode();
				result = (result*397) ^ Neexportovat.GetHashCode();
				result = (result*397) ^ ZpusobVyuziti.GetHashCode();
				result = (result*397) ^ (Komentar != null ? Komentar.GetHashCode() : 0);
				result = (result*397) ^ ObjemDat.GetHashCode();
				result = (result*397) ^ (ZkratkaPamatky != null ? ZkratkaPamatky.GetHashCode() : 0);
				result = (result*397) ^ (ZkratkaPramene != null ? ZkratkaPramene.GetHashCode() : 0);
				result = (result*397) ^ (LiterarniDruh != null ? LiterarniDruh.GetHashCode() : 0);
				result = (result*397) ^ (LiterarniZanr != null ? LiterarniZanr.GetHashCode() : 0);
				result = (result * 397) ^ PocetJedinecnychTokenu.GetHashCode();
				result = (result * 397) ^ PocetTokenu.GetHashCode();
				result = (result * 397) ^ (GrantovaPodpora != null ? GrantovaPodpora.GetHashCode() : 0);
				return result;
			}
		}

		//http://devlicious.com/blogs/louhaskett/archive/2007/06/13/how-to-cast-between-list-t-s.aspx
/*
		/// <summary>
/// This method adds the items in the first list to the second list.
/// This method is  helpful because even when X inherits from Y, List&lt;X&gt; does not inherit from List&lt;Y&gt;, and so you cannot cast between them, only copy.
/// </summary>
/// <typeparam name="TFromType"></typeparam>
/// <typeparam name="TToType"></typeparam>
/// <param name="listToCopyFrom"></param>
/// <param name="listToCopyTo"></param>
/// <returns></returns>
public static List<TToType> AddRange <TFromType, TToType>( IEnumerable<TFromType> listToCopyFrom, List<TToType> listToCopyTo ) where TFromType : TToType
{
		// loop through the list to copy, and
		foreach ( TFromType item in listToCopyFrom )
		{
	// add items to the copy tolist
	listToCopyTo.Add( item );
		}
		
		// return the copy to list
		return listToCopyTo;
}
*/


		IList<IVydani> IZpracovani.Publikace {
		  get {
			if (milstPublikace == null)
			  milstPublikace = new List<IVydani>();
			if (mglstPublikace.Count != milstPublikace.Count) {
			  milstPublikace = mglstPublikace.Cast<IVydani>().ToList();
			}
			return milstPublikace;
		  }
		  set { milstPublikace = value; }
		}

		public List<Vydani> Publikace {
		 get
			{
				if (milstPublikace.Count > mglstPublikace.Count)
				{
					mglstPublikace = new List<Vydani>();
					foreach (IVydani vydani in milstPublikace)
					{
					  Vydani e = new Vydani(vydani.EvidencniCisla, vydani.Audiosoubory);
					  e.Titul = vydani.Titul;
					  e.ZpusobVyuziti = e.ZpusobVyuziti;
						mglstPublikace.Add(e);
					}
				}
				return mglstPublikace;
			}
			set
			{
				mglstPublikace = value;
				milstPublikace = value.Cast<IVydani>().ToList();
			}

		}
		
	}

}
