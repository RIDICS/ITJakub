using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{

	/// <summary>
	/// Základní informace o dokumentu, který obsahuje přepis textu
	/// </summary>
	public class Soubor : IEquatable<ISoubor>, ISoubor
	{

		public Soubor()
		{
			
		}

		public Soubor(FileInfo fiFileInfo)
		{
			Nazev = fiFileInfo.Name;
			Adresar = fiFileInfo.DirectoryName;
			Zmeneno = fiFileInfo.LastWriteTime;
			Velikost = fiFileInfo.Length;
			switch (fiFileInfo.Extension.ToLower())
			{
				case ".docm":
				case ".docx":
					FormatWordu = FormatSouboru.Docx;
					break;
				case ".doc":
					FormatWordu = FormatSouboru.Doc;
					break;
				default:
					FormatWordu = FormatSouboru.Neznamy;
					break;
			}
			
		}
		private bool mblnZmenilSe = false;

		public string Nazev { get; set; }
        public string NazevArchive { get; set; }
		public string Adresar { get; set; }
		public string CelaCesta { get { return this.Adresar + "\\" + this.Nazev; } }
		public FormatSouboru FormatWordu { get; set; }
		public DateTime Zmeneno { get; set; }
		public long Velikost { get; set; }
		public long CisloRevize { get; set; }
		public bool ZmenilSe { get { return mblnZmenilSe; } }
		public string KontrolniSoucet { get; set; }
		public string NazevBezPripony
		{
			get
			{
				int iTecka = Nazev.LastIndexOf('.');
				if(iTecka == -1) return Nazev;
				return Nazev.Substring(0, iTecka);
			}
		}

        public string Pripona
        {
            get
            {
                int iTecka = Nazev.LastIndexOf('.');
                if (iTecka == -1) return null;
                return Nazev.Substring(iTecka + 1, Nazev.Length - iTecka - 1);
            }
        }

        public static bool operator ==(Soubor rhs, Soubor lhs)
		{
			if (rhs.Adresar == lhs.Adresar && rhs.FormatWordu == lhs.FormatWordu && rhs.Nazev == lhs.Nazev &&
				  rhs.Zmeneno == lhs.Zmeneno)
				return true;
			else
				return false;


		}

		public static bool operator !=(Soubor rhs, Soubor lhs)
		{
			return !(rhs == lhs);
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.
		///                 </param>
		public bool Equals(ISoubor other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Nazev, Nazev) && Equals(other.FormatWordu, FormatWordu) && other.Zmeneno.Equals(Zmeneno) && Equals(other.KontrolniSoucet, KontrolniSoucet) && Equals(other.Adresar, Adresar);
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
			if (obj.GetType() != typeof (ISoubor)) return false;
			return Equals((ISoubor) obj);
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
				int result = (Nazev != null ? Nazev.GetHashCode() : 0);
				result = (result*397) ^ FormatWordu.GetHashCode();
				result = (result*397) ^ Zmeneno.GetHashCode();
				result = (result*397) ^ (KontrolniSoucet != null ? KontrolniSoucet.GetHashCode() : 0);
				result = (result*397) ^ (Adresar != null ? Adresar.GetHashCode() : 0);
				return result;
			}
		}
	}

}
