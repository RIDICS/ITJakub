using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
		/// <summary>
		/// Základní údaje o přepisovaném textu a o přepisu. Tyto informace se nacházejí v dokumentu s přepisem v podobě tabulky, dále ve vlastnostech dokumentu a v databázi s evidencí textů.
		/// </summary>
		public class Hlavicka : IEquatable<IHlavicka>, IHlavicka
		{

				private string mstrTypPredlohyText;
				private TypPredlohy mtpTypPredlohy;
				private string mstrEditoriPrepisuText;
				private string[] mastrEditoriPrepisu;
				private readonly Datace mdtcDatace = new Datace();
				private string mstrDatace;
/*
                            private static Dictionary<TypPredlohy, string> mgdcTypPredlohyText = new Dictionary<TypPredlohy, string>();
                */


			public Datace DataceDetaily {get { return mdtcDatace; } }

				/// <summary>
				/// Autor díla
				/// </summary>
				public string Autor { get; set; }
				/// <summary>
				/// Titul díla
				/// </summary>
				public string Titul { get; set; }

				IDatace IHlavicka.DataceDetaily { get { return mdtcDatace; } }
				/// <summary>
				/// DataceText pramene
				/// </summary>
				public string DataceText
				{
						get { return mstrDatace; }
						set
						{
								mstrDatace = value;
								mdtcDatace.SlovniPopis = mstrDatace;
								//mstrObdobiVzniku = AnalyzatorDatace.UrcitObdobiVzniku(mdtcDatace);
						}
				}
				/// <summary>
				/// Tiskař díla
				/// </summary>
				public string Tiskar { get; set; }
				/// <summary>
				/// Místo tisku díla
				/// </summary>
				public string MistoTisku { get; set; }
				/// <summary>
				/// Typ předlohy přepisu
				/// </summary>
				public TypPredlohy TypPredlohy
				{
						get { return mtpTypPredlohy; }
						set { mtpTypPredlohy = value; mstrTypPredlohyText = TypPredlohyNaText(mtpTypPredlohy); }
				}
				/// <summary>
				/// Typ předlohy přepisu (textová podoba)
				/// </summary>
				public string TypPredlohyText
				{
						get { return mstrTypPredlohyText; }
						set { mstrTypPredlohyText = value; mtpTypPredlohy = TextNaTypPredlohy(mstrTypPredlohyText); }
				}
				/// <summary>
				/// Země uložení rukopisu/tisku
				/// </summary>
				public string ZemeUlozeni { get; set; }
				/// <summary>
				/// Město uložení rukopisu/tisku
				/// </summary>
				public string MestoUlozeni { get; set; }
				/// <summary>
				/// Instituce uložení rukopisu/tisku
				/// </summary>
				public string InstituceUlozeni { get; set; }
				/// <summary>
				/// Signatura rukopisu/tisku
				/// </summary>
				public string Signatura { get; set; }
				/// <summary>
				/// Foliace/paginace rukopisu/tisku
				/// </summary>
				public string FoliacePaginace { get; set; }
				/// <summary>
				/// Titul edice
				/// </summary>
				public string TitulEdice { get; set; }
				/// <summary>
				/// Editor edice
				/// </summary>
				public string EditorEdice { get; set; }
				/// <summary>
				/// Místo vydání edice
				/// </summary>
				public string MistoVydaniEdice { get; set; }
				/// <summary>
				/// Rok vydání edice
				/// </summary>
				public string RokVydaniEdice { get; set; }
				/// <summary>
				/// Strany edice
				/// </summary>
				public string StranyEdice { get; set; }

				/// <summary>
				/// Editoři přepisu (textová podoba)
				/// </summary>
				public string EditoriPrepisuText
				{
						get { return mstrEditoriPrepisuText; }
						set
						{
								mstrEditoriPrepisuText = value;
							if(mstrEditoriPrepisuText != null)
								mastrEditoriPrepisu = GetCleanEditori(mstrEditoriPrepisuText);
							else
							{
								mastrEditoriPrepisu = null;
							}
						}
				}
				/// <summary>
				/// Editoři přepisu
				/// </summary>
				public string[] EditoriPrepisu
				{
						get { return mastrEditoriPrepisu; }
						set
						{
						    mastrEditoriPrepisu = value;
						    mstrEditoriPrepisuText = mastrEditoriPrepisu != null ? String.Join("; ", mastrEditoriPrepisu) : null;
						}
				}
				/// <summary>
				/// Poznámka editora k přepisu
				/// </summary>
				public string Poznamka { get; set; }
				public string Ukazka { get; set; }
				public string Knihopis { get; set; }

				/// <summary>
				/// Je-li autor díla uzuální, uvádí se v hranatých závorkách
				/// </summary>
				public bool UzualniAutor
				{
						get
						{
								if (String.IsNullOrEmpty(this.Autor))
										return false;
								else
										return (this.Autor.Contains("["));
						}
				}
				/// <summary>
				/// Je-li titul díla uzuální, uvádí se v hranatých závorkách
				/// </summary>
				public bool UzualniTitul
				{
						get
						{
								if (String.IsNullOrEmpty(this.Titul))
										return false;
								else
										return (this.Titul[0]  == '[');
						}
				}

				public string PopisStarehoTisku
				{
						get
						{
								StringBuilder sb = new StringBuilder();


								if (this.MistoTisku != null)
								{
										sb.Append(this.MistoTisku);
										if (this.DataceText != null)
												sb.Append(", " + this.DataceText + "; ");
								}
								if (this.Tiskar != null)
										sb.Append(this.Tiskar);
								if (this.Knihopis != null)
										sb.Append("; Knihopis: " + this.Knihopis);
								return sb.ToString();
						}
				}


				public string PopisUlozeni
				{
						get
						{
								StringBuilder sb = new StringBuilder();
								if (!String.IsNullOrEmpty(this.InstituceUlozeni))
										sb.Append(this.InstituceUlozeni + ", ");
								if (!String.IsNullOrEmpty(this.MestoUlozeni))
										sb.Append(this.MestoUlozeni + ", ");
								if (!String.IsNullOrEmpty(this.ZemeUlozeni))
										sb.Append(this.ZemeUlozeni);
								return sb.ToString();
						}
				}
				public string PopisRukopisu
				{
						get
						{

								StringBuilder sb = new StringBuilder();


								if (this.ZemeUlozeni != null)
										sb.Append(this.ZemeUlozeni + ", ");
								if (this.MestoUlozeni != null)
										sb.Append(this.MestoUlozeni + ", ");
								if (this.InstituceUlozeni != null)
										sb.Append(this.InstituceUlozeni);
								if (!String.IsNullOrEmpty(this.Signatura))
										sb.Append(", sign.: " + this.Signatura);
								if (this.FoliacePaginace != null)
										sb.Append(", " + this.FoliacePaginace);

								return sb.ToString();
						}
				}



				public string PopisEdice
				{
						get
						{
								StringBuilder sb = new StringBuilder();
								if (this.TitulEdice != null)
								{
										sb.Append(this.TitulEdice + ", ");
								}
								if (this.MistoVydaniEdice != null)
								{
										sb.Append(this.MistoVydaniEdice);
								}
								if (this.RokVydaniEdice != null)
								{
										sb.Append(" " + this.RokVydaniEdice);
								}
								if (this.StranyEdice != null)
								{
										sb.Append(", s. " + this.StranyEdice + ", ");
								}
								if (this.EditorEdice != null)
								{
										sb.Append("ed. " + this.EditorEdice);
								}
								return sb.ToString();
						}
				}


				private static TypPredlohy TextNaTypPredlohy(string sText)
				{
						TypPredlohy tpTyp;
						switch (sText)
						{
								case "rukopis":
										tpTyp = TypPredlohy.Rukopis;
										break;
								case "prvotisk":
										tpTyp = TypPredlohy.Prvotisk;
										break;
								case "starý tisk":
										tpTyp = TypPredlohy.StaryTisk;
										break;
								case "edice":
										tpTyp = TypPredlohy.Edice;
										break;
								default:
										tpTyp = TypPredlohy.Neznamy;
										break;
						}
						return tpTyp;
				}
				private static string TypPredlohyNaText(TypPredlohy tpTyp)
				{
						string sTyp = null;
						switch (tpTyp)
						{
								case TypPredlohy.Rukopis:
										sTyp = "rukopis";
										break;
								case TypPredlohy.Prvotisk:
										sTyp = "prvotisk";
										break;
								case TypPredlohy.StaryTisk:
										sTyp = "starý tisk";
										break;
								case TypPredlohy.Edice:
										sTyp = "edice";
										break;
								default:
										sTyp = "neznámý";
										break;
						}
						return sTyp;
				}

				public static bool operator ==(Hlavicka lhs, Hlavicka rhs)
				{
						if (lhs.Autor == rhs.Autor && lhs.DataceText == rhs.DataceText && rhs.EditorEdice == lhs.EditorEdice &&
								 rhs.EditoriPrepisuText == lhs.EditoriPrepisuText && rhs.FoliacePaginace == lhs.FoliacePaginace &&
								 rhs.InstituceUlozeni == lhs.InstituceUlozeni && rhs.Knihopis == lhs.Knihopis &&
								 rhs.MestoUlozeni == lhs.MestoUlozeni && rhs.MistoTisku == lhs.MistoTisku && rhs.MistoVydaniEdice == lhs.MistoVydaniEdice &&
								 rhs.Poznamka == lhs.Poznamka && rhs.RokVydaniEdice == lhs.RokVydaniEdice && rhs.Signatura == lhs.Signatura &&
								 rhs.StranyEdice == lhs.StranyEdice && rhs.Tiskar == lhs.Tiskar && rhs.Titul == lhs.Titul &&
								 rhs.TitulEdice == lhs.TitulEdice && rhs.TypPredlohy == lhs.TypPredlohy && rhs.Ukazka == lhs.Ukazka &&
								 rhs.UzualniAutor == lhs.UzualniAutor && rhs.UzualniTitul == lhs.UzualniTitul && rhs.ZemeUlozeni == lhs.ZemeUlozeni)
								return true;
						else
								return false;
				}

				public static bool operator !=(Hlavicka lhs, Hlavicka rhs)
				{
						return !(lhs == rhs);
				}

				public override bool Equals(object obj)
				{
					if (ReferenceEquals(null, obj)) return false;
					if (ReferenceEquals(this, obj)) return true;
					if (obj.GetType() != typeof (Hlavicka)) return false;
					return Equals((Hlavicka) obj);
				}

			/// <summary>
			/// Indicates whether the current object is equal to another object of the same type.
			/// </summary>
			/// <returns>
			/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
			/// </returns>
			/// <param name="other">An object to compare with this object.
			/// </param>
			public bool Equals(IHlavicka other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Equals(other.TypPredlohyText, mstrTypPredlohyText) && Equals(other.TypPredlohy, mtpTypPredlohy) && Equals(other.EditoriPrepisuText, mstrEditoriPrepisuText) && Equals(other.EditoriPrepisu, mastrEditoriPrepisu) && Equals(other.DataceDetaily, mdtcDatace) && Equals(other.DataceText, mstrDatace) && Equals(other.Autor, Autor) && Equals(other.Titul, Titul) && Equals(other.Tiskar, Tiskar) && Equals(other.MistoTisku, MistoTisku) && Equals(other.ZemeUlozeni, ZemeUlozeni) && Equals(other.MestoUlozeni, MestoUlozeni) && Equals(other.InstituceUlozeni, InstituceUlozeni) && Equals(other.Signatura, Signatura) && Equals(other.FoliacePaginace, FoliacePaginace) && Equals(other.TitulEdice, TitulEdice) && Equals(other.EditorEdice, EditorEdice) && Equals(other.MistoVydaniEdice, MistoVydaniEdice) && Equals(other.RokVydaniEdice, RokVydaniEdice) && Equals(other.StranyEdice, StranyEdice) && Equals(other.Poznamka, Poznamka) && Equals(other.Ukazka, Ukazka) && Equals(other.Knihopis, Knihopis);
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
					int result = (mstrTypPredlohyText != null ? mstrTypPredlohyText.GetHashCode() : 0);
					result = (result*397) ^ mtpTypPredlohy.GetHashCode();
					result = (result*397) ^ (mstrEditoriPrepisuText != null ? mstrEditoriPrepisuText.GetHashCode() : 0);
					result = (result*397) ^ (mastrEditoriPrepisu != null ? mastrEditoriPrepisu.GetHashCode() : 0);
					result = (result*397) ^ (mdtcDatace != null ? mdtcDatace.GetHashCode() : 0);
					result = (result*397) ^ (mstrDatace != null ? mstrDatace.GetHashCode() : 0);
					result = (result*397) ^ (Autor != null ? Autor.GetHashCode() : 0);
					result = (result*397) ^ (Titul != null ? Titul.GetHashCode() : 0);
					result = (result*397) ^ (Tiskar != null ? Tiskar.GetHashCode() : 0);
					result = (result*397) ^ (MistoTisku != null ? MistoTisku.GetHashCode() : 0);
					result = (result*397) ^ (ZemeUlozeni != null ? ZemeUlozeni.GetHashCode() : 0);
					result = (result*397) ^ (MestoUlozeni != null ? MestoUlozeni.GetHashCode() : 0);
					result = (result*397) ^ (InstituceUlozeni != null ? InstituceUlozeni.GetHashCode() : 0);
					result = (result*397) ^ (Signatura != null ? Signatura.GetHashCode() : 0);
					result = (result*397) ^ (FoliacePaginace != null ? FoliacePaginace.GetHashCode() : 0);
					result = (result*397) ^ (TitulEdice != null ? TitulEdice.GetHashCode() : 0);
					result = (result*397) ^ (EditorEdice != null ? EditorEdice.GetHashCode() : 0);
					result = (result*397) ^ (MistoVydaniEdice != null ? MistoVydaniEdice.GetHashCode() : 0);
					result = (result*397) ^ (RokVydaniEdice != null ? RokVydaniEdice.GetHashCode() : 0);
					result = (result*397) ^ (StranyEdice != null ? StranyEdice.GetHashCode() : 0);
					result = (result*397) ^ (Poznamka != null ? Poznamka.GetHashCode() : 0);
					result = (result*397) ^ (Ukazka != null ? Ukazka.GetHashCode() : 0);
					result = (result*397) ^ (Knihopis != null ? Knihopis.GetHashCode() : 0);
					return result;
				}
			}

			private static string[] GetCleanEditori(string editoriText)
			{
				string[] editori = editoriText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < editori.Length; i++)
				{
					editori[i] = editori[i].Trim();
				}
				return editori;
			}
		}
}
