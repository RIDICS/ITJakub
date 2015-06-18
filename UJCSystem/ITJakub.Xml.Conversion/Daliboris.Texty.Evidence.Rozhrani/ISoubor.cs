using System;

namespace Daliboris.Texty.Evidence.Rozhrani
{
	public interface ISoubor
	{
		string Nazev { get; set; }
		string NazevBezPripony { get; }
		string Adresar { get; set; }
		string CelaCesta { get; }
		FormatSouboru FormatWordu { get; set; }
		DateTime Zmeneno { get; set; }
		long Velikost { get; set; }
		long CisloRevize { get; set; }
		bool ZmenilSe { get; }
		string KontrolniSoucet { get; set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		bool Equals(ISoubor other);

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