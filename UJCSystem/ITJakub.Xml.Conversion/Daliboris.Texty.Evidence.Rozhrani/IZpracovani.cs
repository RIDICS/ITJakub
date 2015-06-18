using System;
using System.Collections;
using System.Collections.Generic;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface IZpracovani
	{
		IExport ZjistiPosledniExport(ZpusobVyuziti zvZpusobVyuziti);
		IExport ZjistiPrvniExport(ZpusobVyuziti zvZpusobVyuziti);
		void ZaevidujExport(IExport exp);
		void ZaevidujExport(ZpusobVyuziti zvZpusobVyuziti, DateTime dtCasExportu, string kontrolniSoucet);
		string GUID { get; set; }
		FazeZpracovani FazeZpracovani { get; set; }
		CasoveZarazeni CasoveZarazeni { get; set; }
		bool Neexportovat { get; set; }
		List<IExport> Exporty { get; set; }
		ZpusobVyuziti ZpusobVyuziti { get; set; }
		string Komentar { get; set; }
		List<IExport> PrvniExporty { get; set; }
		long ObjemDat { get; set; }
		string ZkratkaPamatky { get; set; }
		string ZkratkaPramene { get; set; }
		string LiterarniDruh { get; set; }
		string LiterarniZanr { get; set; }
		Zabezpeceni Zabezpeceni { get; set; }

		/// <summary>
		/// Zda se má památka exportovat v transliterované podobě
		/// (pouze pro potřeby textové banky, nikoli Manuscriptoria).
		/// </summary>
		bool Transliterovane { get; set; }

		/// <summary>
		/// Zda jde o biblický text, jehož značení biblických knih se má exportovat.
		/// </summary>
		bool JeBiblickyText { get; }

	 /// <summary>
	 /// Zda přepis obsahuje text ediční poznámky.
	 /// </summary>
		bool MaEdicniPoznamku { get; set; }

		/// <summary>
		/// Publikace elektronické edice v různých kanálech (zejména jako e-kniha)
		/// </summary>
		IList<IVydani> Publikace { get; set; }


		/// <summary>
		/// Počet tokenů obsažených v elektronické edici
		/// </summary>
		int PocetTokenu { get; set; }

		/// <summary>
		/// Počet jedinečných tokenů obsažených v elektronické edici
		/// </summary>
		int PocetJedinecnychTokenu { get; set; }

		string GrantovaPodpora { get; set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		/// </param>
		bool Equals(IZpracovani other);

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
		///                 </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
		///                 </exception><filterpriority>2</filterpriority>
		bool Equals(object obj);

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		int GetHashCode();
	}
}