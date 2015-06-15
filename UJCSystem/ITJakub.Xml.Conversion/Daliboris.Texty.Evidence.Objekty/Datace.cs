using System;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	/// <summary>
	/// Informace o obecném a podrobném časovém zařazení přepisu
	/// </summary>
	public class Datace : IComparable, ICloneable, IDatace, IEquatable<Datace>
	{
		/*

						private const string csOtaznik = "(?)";
						private const string csOkoloRoku = "okolo roku";
						private const string csPoRoce = "po roce";
						private const string csPrelom = "přelom";
						private const string csPolovina = "polovina";
						private const string cs1polovina = "1. polovina";
						private const string cs2polovina = "2. polovina";
						private const string csLeta = ". léta ";
						private const string csStoleti = ". století";
						private const string csA = " a ";
						private const string csZacatek = "začátek";
						private const string csKonec = "konec";

		*/

		public Datace() { }

		public Datace(string strSlovniPopis) {
			SlovniPopis = strSlovniPopis;
		}

		private string mstrSlovniPopis;

		//[XmlAttribute("slovniPopis")]
		public string SlovniPopis {
			get { return mstrSlovniPopis; }
			set {
				mstrSlovniPopis = value;
				AnalyzovatSlovniPopis(mstrSlovniPopis);
			}
		}
		public int Stoleti { get; set; }
		public int PolovinaStoleti { get; set; }
		public int Desetileti { get; set; }
		public int Rok { get; set; }
		public string Upresneni { get; set; }
		public int RelativniChronologie { get; set; }
		
		public int NePredRokem { get; set;}
		public int NePoRoce {get; set;}

		//private void UrciKoncovyRok(Datace datace) {
		// if (datace.Rok != 0)
		//  return 0;
		// if (datace.Desetileti != 0) {
		//  return datace.Stoleti + (datace.Desetileti * 10);
		// }
		//}

		//private int UrciPocatecniRok(Datace datace) {
		// if (datace.Rok != 0)
		//  return 0;

		//}


		/// <summary>
		/// Přibližné umístění na časové ose (v rozmezí 25 let)
		/// </summary>
		public int CasovyOtisk {

			get {
				int iCasovyOtisk = this.Rok;
				if (iCasovyOtisk == 0)
					return this.Stoleti + (this.PolovinaStoleti * 50) + this.RelativniChronologie;
				return iCasovyOtisk;
			}
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Datace other)
		{
			return this.Desetileti == other.Desetileti &&
			       this.NePoRoce == other.NePoRoce &&
			       this.NePredRokem == other.NePredRokem &&
			       this.PolovinaStoleti == other.PolovinaStoleti &&
			       this.RelativniChronologie == other.RelativniChronologie &&
			       this.Rok == other.Rok &&
			       this.SlovniPopis == other.SlovniPopis &&
			       this.Stoleti == other.Stoleti &&
			       this.Upresneni == other.Upresneni;

		}

		#region Equality members

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Datace) obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (mstrSlovniPopis != null ? mstrSlovniPopis.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ Stoleti;
				hashCode = (hashCode*397) ^ PolovinaStoleti;
				hashCode = (hashCode*397) ^ Desetileti;
				hashCode = (hashCode*397) ^ Rok;
				hashCode = (hashCode*397) ^ (Upresneni != null ? Upresneni.GetHashCode() : 0);
				hashCode = (hashCode*397) ^ RelativniChronologie;
				hashCode = (hashCode*397) ^ NePredRokem;
				hashCode = (hashCode*397) ^ NePoRoce;
				return hashCode;
			}
		}

		public static bool operator ==(Datace left, Datace right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Datace left, Datace right)
		{
			return !Equals(left, right);
		}

		#endregion

		public override string ToString() {
			//return base.ToString();
			return String.Format("století: {0}, polovina: {1}, desetiletí: {2}, rok: {3}, upřesnění: {4}, relativní chronologie: {5}",
																								Stoleti, PolovinaStoleti, Desetileti, Rok, Upresneni, RelativniChronologie);
		}

		public void AnalyzovatSlovniPopis(string sSlovniPopis) {

			if (String.IsNullOrEmpty(sSlovniPopis))
				return;
			Datace dt = AnalyzatorDatace.AnalyzovatDataci(sSlovniPopis);
			Stoleti = dt.Stoleti;
			PolovinaStoleti = dt.PolovinaStoleti;
			Desetileti = dt.Desetileti;
			Rok = dt.Rok;
			Upresneni = dt.Upresneni;
			RelativniChronologie = dt.RelativniChronologie;
			NePredRokem = dt.NePredRokem;
			NePoRoce = dt.NePoRoce;

			return;
		}

		/*
								const int cintPoceTextu = 12;

								Dictionary<string, string> mgdcTexty = new Dictionary<string, string>(cintPoceTextu);
								mgdcTexty.Add(cs1polovina, cs1polovina);
								mgdcTexty.Add(cs2polovina, cs2polovina);
								mgdcTexty.Add(csA, csA);
								mgdcTexty.Add(csKonec, csKonec);
								mgdcTexty.Add(csLeta, csLeta);
								mgdcTexty.Add(csOkoloRoku, csOkoloRoku);
								mgdcTexty.Add(csOtaznik, csOtaznik);
								mgdcTexty.Add(csPolovina, csPolovina);
								mgdcTexty.Add(csPoRoce, csPoRoce);
								mgdcTexty.Add(csPrelom, csPrelom);
								mgdcTexty.Add(csStoleti, csStoleti);
								mgdcTexty.Add(csZacatek, csZacatek);
								StringBuilder sbSlovniPopis = new StringBuilder(sSlovniPopis);

								foreach (string sval in mgdcTexty.Values)
								{
										while (sbSlovniPopis.ToString().Contains(sval))
										{
												int i = sbSlovniPopis.ToString().IndexOf(sval);
												sbSlovniPopis.Remove(i, sval.Length);
										}
								}

								const string csPismena = "0123456789/–.";
								string[] asSlova = sbSlovniPopis.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
								List<string> glNeznamaSlova = new List<string>(asSlova.Length);
								foreach (string sSlovo in asSlova)
								{
										for (int i = 0; i < sSlovo.Length; i++)
										{
												if (!(csPismena.Contains(sSlovo[i].ToString())))
												{
														glNeznamaSlova.Add(sSlovo);
														break;
												}
										}
								}

								if (glNeznamaSlova.Count > 0)
								{
										Upresneni = "neznámé výrazy (" + String.Join("; ", glNeznamaSlova.ToArray()) + ")";
										return;
								}
								//throw new ArgumentException("Popis datace obsahuje neznámé výrazy: " + String.Join("; ", glNeznamaSlova.ToArray()));
								//nelze použít kvůli serializaci


								//nedjříve kontrola, jestli popis obsahuje pouze identifikovatelné výrazy, tj. číslice + určitý repertoár slov

								mstrSlovniPopis = sSlovniPopis;
								string sPopis = mstrSlovniPopis;

								RozebratUpresneniVPopisu(ref sPopis, csOtaznik);
								RozebratUpresneniVPopisu(ref sPopis, csOkoloRoku);
								RozebratUpresneniVPopisu(ref sPopis, csPoRoce);
								if (sPopis.Contains(csPrelom))
								{
										this.RelativniChronologie = 1;
										RozebratUpresneniVPopisu(ref sPopis, csPrelom);
								}

								//datace typu 1432/1433
								if (sPopis.Contains('/'))
								{
										string[] asRozhrani = sPopis.Split(new char[] { '/' });
										sPopis = asRozhrani[asRozhrani.Length - 1];
								}
								//datace typu 1502–1503
								if (sPopis.Contains('–'))
								{
										string[] asRozhrani = sPopis.Split(new char[] { '–' });
										sPopis = asRozhrani[asRozhrani.Length - 1];
								}

								int iRok;
								if (Int32.TryParse(sPopis, out iRok))
								{
										Rok = iRok;
										Stoleti = (iRok / 100) * 100;
										int iDesetileti = Rok - Stoleti;
										PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(iDesetileti);
										Desetileti = iDesetileti / 10;
										RelativniChronologie = RelativniChronologieNaZakladeRoku(iRok);
								}
								else
								{
										sPopis = mstrSlovniPopis;
								}

								if (sPopis.Contains(csPrelom))
								{
										PolovinaStoleti = 1;
										sPopis = sPopis.Replace(csPrelom, "");
										if (sPopis.Contains(csA))
										{
												sPopis = sPopis.Substring(sPopis.IndexOf(csA) + csA.Length);
										}
										RelativniChronologie = 1;
								}

								if (sPopis.Contains(cs1polovina))
								{
										PolovinaStoleti = 1;
										RelativniChronologie = 3;
										sPopis = sPopis.Replace(cs1polovina, "");
								}
								if (sPopis.Contains(cs2polovina))
								{
										PolovinaStoleti = 2;
										//Desetileti = 5;???
										RelativniChronologie = 7;
										sPopis = sPopis.Replace(cs2polovina, "");
								}
								int iPozice = sPopis.IndexOf(csLeta);
								if (iPozice > 0)
								{
										string sText = sPopis.Substring(iPozice - 2, 2);
										int iDesetileti;
										if (Int32.TryParse(sText, out iDesetileti))
										{
												PolovinaStoleti = PolovinaStoletiNaZakladeDesetileti(iDesetileti);
												Desetileti = iDesetileti / 10;
												RelativniChronologie = RelativniChronologieNaZakladeRoku(Desetileti * 10 + 9);
												sPopis = sPopis.Remove(iPozice - 2, csLeta.Length + 2);
										}
								}

								iPozice = sPopis.IndexOf(csStoleti);
								if (iPozice > 0)
								{
										//předpokládá se dvoumístný údaj o století - rozšířit i na 9. století a níže
										string sText = sPopis.Substring(iPozice - 2, 2);
										int iStoleti;
										if (Int32.TryParse(sText, out iStoleti))
										{
												Stoleti = (iStoleti - 1) * 100;
												if (RelativniChronologie == 0)
														RelativniChronologie = 9;
												sPopis = sPopis.Remove(iPozice - 2, csStoleti.Length + 2);
										}
								}

								sPopis = sPopis.Trim();
								if (sPopis == csPolovina)
								{
										Desetileti = 5;
										PolovinaStoleti = 2;
										RelativniChronologie = 5;
										sPopis = null;
								}

								switch (sPopis)
								{
										case csZacatek:
												RelativniChronologie = 2;
												break;
										case csKonec:
												RelativniChronologie = 8;
												break;
										case csPolovina:
												RelativniChronologie = 5;
												break;
								}

								if (Stoleti == 0)
								{
										if (!String.IsNullOrEmpty(sPopis))
										{
												Upresneni = sPopis;
										}
										else
										{
												int iCislo;
												if (!Int32.TryParse(sPopis, out iCislo))
														Upresneni += " " + sPopis;
										}
								}

						}
		 */
		/*
						private int PolovinaStoletiNaZakladeDesetileti(int iDesetileti)
						{
								return iDesetileti < 50 ? 1 : 2;
						}

						int RelativniChronologieNaZakladeRoku(int iRok)
						{
								int iStoleti = (iRok / 100) * 100;
								int iDesetileti = Rok - Stoleti;
								int iTemp = (int)(Math.Truncate(iDesetileti / 12.5)) + 1; //snad to bude fuingovat; ve VB Fix
								return iTemp;
						}

						void RozebratUpresneniVPopisu(ref string sPopis, string sUpresneni)
						{
								if (sPopis.Contains(sUpresneni))
								{
										this.Upresneni = String.IsNullOrEmpty(this.Upresneni) ? sUpresneni : this.Upresneni + " " + sUpresneni;
										sPopis = sPopis.Replace(csOkoloRoku, "");
								}
						}
		 */


		#region IComparable Members

		public int CompareTo(object obj) {
			/*
			menší než 0 => This instance is less than obj.
			0 => This instance is equal to obj.
			větší než 0 => This instance is greater than obj. 
			 * 
			 */
			if (obj == null)
				return 1;
			if (!(obj is Datace))
				throw new ArgumentException("Parametr není typu Datace");

			Datace dt = (Datace)obj;


			//položky s neznámou datací se zařadí až na konec
			if (this.NePoRoce == 0 && this.NePredRokem == 0)
			{
				if (dt.NePoRoce == 0 && dt.NePredRokem == 0)
					return 0;
				else
					return 1;
			}
			//položky s neznámou datací se zařadí až na konec
			if (dt.NePoRoce == 0 && dt.NePredRokem == 0)
			{
				if (this.NePoRoce == 0 && this.NePredRokem == 0)
					return 0;
				else
					return -1;
			}
			if (dt.NePoRoce == this.NePoRoce)
				return this.NePredRokem.CompareTo(dt.NePredRokem);
			return this.NePoRoce.CompareTo(dt.NePoRoce);
			//if (this.NePredRokem < dt.NePredRokem)
			//  return 1;
			//if (this.NePoRoce < dt.NePredRokem)
			//  return 1;
			//return -1;
			
			/*
			int iVysledekPorovnani = -1;
			iVysledekPorovnani = this.Stoleti.CompareTo(dt.Stoleti);
			if (iVysledekPorovnani == 0) {
				iVysledekPorovnani = this.RelativniChronologie.CompareTo(dt.RelativniChronologie);
				if (iVysledekPorovnani == 0) {
					iVysledekPorovnani = this.PolovinaStoleti.CompareTo(dt.PolovinaStoleti);
					if (iVysledekPorovnani == 0) {
						iVysledekPorovnani = this.Desetileti.CompareTo(dt.Desetileti);
						if (iVysledekPorovnani == 0) {
							iVysledekPorovnani = this.Rok.CompareTo(dt.Rok);
							if (iVysledekPorovnani == 0) {
								if (this.Upresneni == null) {
									if (dt.Upresneni == null)
										iVysledekPorovnani = 0;
									else
										iVysledekPorovnani = -1;
								}
								else
									iVysledekPorovnani = this.Upresneni.CompareTo(dt.Upresneni);
							}
						}
					}
				}
			}

			return iVysledekPorovnani;
			*/
		}

		#endregion

		#region ICloneable Members

		public object Clone() {
			throw new NotImplementedException();
		}

		#endregion
	}

}
