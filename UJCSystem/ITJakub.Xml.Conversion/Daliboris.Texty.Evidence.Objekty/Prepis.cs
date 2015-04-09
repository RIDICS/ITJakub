using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Xml.Serialization;
	using System.Diagnostics;

	/// <summary>
	/// Informace elektronickém o dokumentu, který obsahuje přepis textu
	/// </summary>
	//[XmlRoot("prepis")]
	[XmlRoot(Namespace = "http://www.daliboris.cz/schemata/prepisy.xsd")]
	[DebuggerDisplay("{Soubor.Nazev}, {TitulTextu}")]
	public class Prepis : ICloneable, INotifyPropertyChanged, IPrepis {

		private Hlava mhlHlava = new Hlava();
		private Hlavicka mhlHlavicka;
		private ISoubor msbSoubor;
		private IZpracovani mzpZpracovani;
		private string mstrObdobiVzniku = null;
		private string _typPrepisu;

		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion


		public Prepis() { }
		public Prepis(Hlavicka hlHlavicka, ISoubor sbSoubor) {
			this.Soubor = (Soubor)sbSoubor;
			this.Hlavicka = hlHlavicka;
		}
		public Prepis(ISoubor sbSoubor) {
			this.Soubor = (Soubor)sbSoubor;
		}
		public Prepis(Hlavicka hlHlavicka, ISoubor sbSoubor, Zpracovani zpZpracovani) {
			this.Soubor = (Soubor)sbSoubor;
			this.Hlavicka = hlHlavicka;
			this.Zpracovani = zpZpracovani;
		}


		public void PromitniZmenyDoHlavy()
		{
				IPamatka pm = this.Hlava.Pamatka;
				pm.Autor = this.Autor;
				pm.LiterarniDruh = this.LiterarniDruh;
				pm.LiterarniZanr = this.LiterarniZanr;
				pm.Titul = this.Titul;
				//pm.UzualniTitul = !(this.TitulBezZavorek = this.Titul);
				pm.Zkratka = this.ZkratkaPamatky;


				IPramen pr = this.Hlava.Pamatka.Pramen;
				pr.Datace = this.DataceDetaily;
				pr.Zkratka = this.ZkratkaPramene;
				
			Ulozeni u = new Ulozeni(Hlavicka.ZemeUlozeni, 
				Hlavicka.MestoUlozeni, Hlavicka.InstituceUlozeni, 
				Hlavicka.Signatura, Hlavicka.FoliacePaginace);

			pr.Ulozeni = u;
			
			/*
				IUlozeni ul = pr.Ulozeni;
				
				ul.Mesto = this.Hlavicka.MestoUlozeni;
				ul.Instituce = this.Hlavicka.InstituceUlozeni;
				ul.Zeme = this.Hlavicka.ZemeUlozeni;
				ul.Signatura = this.Hlavicka.FoliacePaginace;
				ul.FoliacePaginace = this.Hlavicka.FoliacePaginace;
			*/
		}
		public Hlava Hlava {
			get { return mhlHlava; }
			set {
				mhlHlava = (Hlava)value;
				//mhlHlava.Pamatka.Zkratka = this.ZkratkaPamatky;
				//mhlHlava.Pamatka.Pramen.Zkratka = this.ZkratkaPramene;
				//mhlHlava.Pamatka.LiterarniDruh = this.LiterarniDruh;
				//mhlHlava.Pamatka.LiterarniZanr = this.LiterarniZanr;
				OnPropertyChanged(new PropertyChangedEventArgs("Hlava"));
			}
		}

		/// <summary>
		/// Informace o přepsaném díle a prameni
		/// </summary>
		/// 
		//[XmlElement("hlavicka")]
		public Hlavicka Hlavicka {
			get { return (mhlHlavicka); }
			set {
				mhlHlavicka = (Hlavicka)value;
				OnPropertyChanged(new PropertyChangedEventArgs("Hlavicka"));
			}
		}

		/// <summary>
		/// Informace o formátu a uložení dokumentu na disku
		/// </summary>
		/// 
		//[XmlElement("soubor")]
		public Soubor Soubor { get { return (Soubor)msbSoubor; } set { msbSoubor = value; OnPropertyChanged(new PropertyChangedEventArgs("Soubor")); } }
		/// <summary>
		/// Informace o stavu zpracování dokumentu
		/// </summary>
		/// 
		//[XmlElement("zpracovani")]
		public Zpracovani Zpracovani { get { return (Zpracovani)mzpZpracovani; } set { mzpZpracovani = value; OnPropertyChanged(new PropertyChangedEventArgs("Zpracovani")); } }

		public string Autor { get { return (Hlavicka.Autor); } }
		public string Titul { get { return (Hlavicka.Titul); } }
		public string Signatura { get { return (Hlavicka.Signatura); } }
		[XmlIgnore]
		public ZpusobVyuziti ZpusobVyuziti {
			get { return (mzpZpracovani.ZpusobVyuziti); }
			set {
				mzpZpracovani.ZpusobVyuziti = value;
				OnPropertyChanged(new PropertyChangedEventArgs("ZpusobVyuziti"));
				/*Set(this, "ZpusobVyuziti", ref mzpZpracovani.ZpusobVyuziti, value, PropertyChanged);*/
			}
		}
		[XmlIgnore]
		public FazeZpracovani FazeZpracovani {
			get { return (mzpZpracovani.FazeZpracovani); }
			set { mzpZpracovani.FazeZpracovani = value; OnPropertyChanged(new PropertyChangedEventArgs("FazeZpracovani")); }
		}
		[XmlIgnore]
		public CasoveZarazeni CasoveZarazeni {
			get { return (mzpZpracovani.CasoveZarazeni); }
			set { mzpZpracovani.CasoveZarazeni = value; OnPropertyChanged(new PropertyChangedEventArgs("CasoveZarazeni")); }
		}
		//[XmlIgnore]
		//public string NazevSouboru {
		//  get { return (Soubor.Nazev); }
		//  set { Soubor.Nazev = value; OnPropertyChanged(new PropertyChangedEventArgs("NazevSouboru")); }
		//}

		[XmlIgnore]
		public string AutorTextu {
			get { return (Hlavicka.Autor); }
			set { Hlavicka.Autor = value; OnPropertyChanged(new PropertyChangedEventArgs("AutorTextu")); }
		}
		[XmlIgnore]
		public string TitulTextu {
			get { return (Hlavicka.Titul); }
			set { Hlavicka.Titul = value; OnPropertyChanged(new PropertyChangedEventArgs("TitulTextu")); }
		}
		//[XmlIgnore]
		public string TitulBezZavorek {
			get {
				if (Hlavicka.UzualniTitul) {
					return Hlavicka.Titul.Substring(1, Titul.Length - 2);
				}
				else {
					return Hlavicka.Titul;
				}
			}
		}



		/// <summary>
		/// Období vzniku pramane (s periodou 50 let)
		/// </summary>
		public string ObdobiVzniku {
			get {
				if (mstrObdobiVzniku == null)
					mstrObdobiVzniku = AnalyzatorDatace.UrcitObdobiVzniku(mhlHlavicka.DataceDetaily);
				return mstrObdobiVzniku;
			}
			set { mstrObdobiVzniku = value; OnPropertyChanged(new PropertyChangedEventArgs("ObdobiVzniku")); }
		}
		public string NazevSouboru { get { return (Soubor.Nazev); } }
		public DateTime Zmeneno { get { return (Soubor.Zmeneno); } }
		public string KontrolniSoucet { get { return Soubor.KontrolniSoucet; } }
		public string DataceText { get { return (Hlavicka.DataceText); } }
		public IDatace DataceDetaily { get { return (Hlavicka.DataceDetaily); } }
		public int Identifikator { get; set; }
		[XmlIgnore]
		public StatusAktualizace Status { get; set; }
		[XmlIgnore]
		public IPrepis PuvodniPodoba { get; set; }
		[XmlIgnore]
		public string GUID {
			get { return mzpZpracovani.GUID; }
			set { mzpZpracovani.GUID = value; OnPropertyChanged(new PropertyChangedEventArgs("GUID")); }
		}
		[XmlIgnore]
		public string Komentar {
			get { return mzpZpracovani.Komentar; }
			set { mzpZpracovani.Komentar = value; OnPropertyChanged(new PropertyChangedEventArgs("Komentar")); }
		}

		[XmlIgnore]
		public bool Transliterovane
		{
		  get { return mzpZpracovani.Transliterovane; }
		  set { mzpZpracovani.Transliterovane = value; OnPropertyChanged(new PropertyChangedEventArgs("Transliterovane")); }
		}

		[XmlIgnore]
		public bool Neexportovat {
			get { return mzpZpracovani.Neexportovat; }
			set { mzpZpracovani.Neexportovat = value; OnPropertyChanged(new PropertyChangedEventArgs("Neexportovat")); }
		}

		[XmlIgnore]
		public string ZkratkaPamatky {
			get { return mzpZpracovani.ZkratkaPamatky; }
			set { mzpZpracovani.ZkratkaPamatky = value; OnPropertyChanged(new PropertyChangedEventArgs("ZkratkaPamatky")); }
		}

		[XmlIgnore]
		public string ZkratkaPramene {
			get { return mzpZpracovani.ZkratkaPramene; }
			set { mzpZpracovani.ZkratkaPramene = value; OnPropertyChanged(new PropertyChangedEventArgs("ZkratkaPramene")); }
		}

		[XmlIgnore]
		public string LiterarniDruh {
			get { return mzpZpracovani.LiterarniDruh; }
			set { mzpZpracovani.LiterarniDruh = value; OnPropertyChanged(new PropertyChangedEventArgs("LiterarniDruh")); }
		}

		[XmlIgnore]
		public string LiterarniZanr {
			get { return mzpZpracovani.LiterarniZanr; }
			set { mzpZpracovani.LiterarniZanr = value; OnPropertyChanged(new PropertyChangedEventArgs("LiterarniZanr")); }
		}



		/// <summary>
		/// Zda se jedná o edici, slovník, mluvnici, odbornou literaturu
		/// </summary>
		public string TypPrepisu
		{
			get { return _typPrepisu; }
			set { _typPrepisu = value; OnPropertyChanged(new PropertyChangedEventArgs("TypPrepisu")); }
		}


		// override object.Equals
		public override bool Equals(object obj) {
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			// TODO: write your implementation of Equals() here
			Prepis lhs = (Prepis)obj;
			if (this.Hlavicka == lhs.Hlavicka && this.Soubor == lhs.Soubor &&
					 this.Zpracovani == lhs.Zpracovani)
				return true;
			else
				return false;
		}

		// override object.GetHashCode
		public override int GetHashCode() {
			// TODO: write your implementation of GetHashCode() here
			//throw new NotImplementedException();
			return base.GetHashCode();
		}

		//public static bool operator ==(Prepis rhs, Prepis lhs) {
		//   if (rhs == null && lhs == null)
		//      return true;
		//   if (rhs == null | lhs == null)
		//      return false;
		//   if (rhs.Hlavicka == lhs.Hlavicka && rhs.Soubor == lhs.Soubor &&
		//      rhs.Zpracovani == lhs.Zpracovani)
		//      return true;
		//   else
		//      return false;
		//}

		//public static bool operator !=(Prepis rhs, Prepis lhs) {
		//   return !(rhs == lhs);
		//}



		#region ICloneable Members

		public object Clone() {
			//Prepis prp = new Prepis(this.Hlavicka, this.Soubor, this.Zpracovani);
			//return prp;
			Prepis prp = (Prepis)this.MemberwiseClone();

			return prp;
		}

		

		#endregion

		public Prepis DeepCopy() {
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();

			binaryFormatter.Serialize(memoryStream, this);
			memoryStream.Seek(0, SeekOrigin.Begin);

			return (Prepis)binaryFormatter.Deserialize(memoryStream);

		}

		private void OnPropertyChanged(PropertyChangedEventArgs e) {
			if (PropertyChanged != null) {
				PropertyChanged(this, e);
			}
		}

		public static bool Set<T>(object owner, string propName,
			ref T oldValue, T newValue, PropertyChangedEventHandler eventHandler) {
			// make sure the property name really exists
			if (owner.GetType().GetProperty(propName) == null) {
				throw new ArgumentException("Neexistuje vlastnost s názvem '" + propName + "' u objektu " + owner.GetType().FullName);
			}
			// we only raise an event if the value has changed
			if (!Equals(oldValue, newValue)) {
				oldValue = newValue;
				if (eventHandler != null) {
					eventHandler(owner, new PropertyChangedEventArgs(propName));
				}
			}
			return true;
		}


		#region IPrepis Members


		IHlava IPrepis.Hlava {
			get {
				return Hlava;
			}
			set {
				Hlava = value as Hlava;
			}
		}

		IHlavicka IPrepis.Hlavicka {
			get {
				return Hlavicka;
			}
			set {
				Hlavicka = value as Hlavicka;
			}
		}

		ISoubor IPrepis.Soubor {
			get {
				return Soubor;
			}
			set {
				Soubor = value as Soubor;
			}
		}

		IZpracovani IPrepis.Zpracovani {
			get {
				return Zpracovani;
			}
			set {
				Zpracovani = value as Zpracovani;
			}
		}

		#endregion
	}

}
