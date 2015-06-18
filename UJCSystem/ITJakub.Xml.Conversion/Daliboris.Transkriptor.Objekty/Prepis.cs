using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	public class Prepis {
		private string mstrTranskripce = null;
		private string mstrTransliterace = null;
		private Korelaty mkrKorelaty = new Korelaty();
		private string mstrPoznamka = null;
		private bool mblnJeHotovy = false;
		private bool mblnNepouzivatPravidla = false;
		private int mintPocet = 0;
	    private List<ITransformacniKrok> transformacniKroky = new List<ITransformacniKrok>();


		public bool JeKompletni() {
			if (mblnNepouzivatPravidla) {
				if (!String.IsNullOrEmpty(mstrTranskripce) || mblnJeHotovy)
					return true;
			}
			else {
				int iPocetZnaku = mkrKorelaty.PocetUsouvztaznenychZnaku(TypPrepisu.Tansliterace);
				if (iPocetZnaku != mstrTransliterace.Length)
					return false;
				iPocetZnaku = mkrKorelaty.PocetUsouvztaznenychZnaku(TypPrepisu.Transkripce);
				if (iPocetZnaku != mstrTranskripce.Length)
					return false;
				return true;
			}
			return false;
		}

		public bool NepouzivatPravidla {
			get { return mblnNepouzivatPravidla; }
			set { mblnNepouzivatPravidla = value; }
		}

		public bool JeHotovy {
			get { return mblnJeHotovy; }
			set { mblnJeHotovy = value; }
		}

		public string Poznamka {
			get { return mstrPoznamka; }
			set { mstrPoznamka = value; }
		}

		public Prepis() { }
		public Prepis(string strTransliterace, string strTranskripce) {
			mstrTransliterace = strTransliterace;
			mstrTranskripce = strTranskripce;
		}
		public Prepis(string strTransliterace, string strTranskripce, int intPocet) : this(strTransliterace, strTranskripce) {
			mintPocet = intPocet;
		}

        public Prepis(string strTransliterace, string strTranskripce, int intPocet, List<ITransformacniKrok> transformacniKroky) :
            this(strTransliterace, strTranskripce, intPocet)
	    {
	        this.TransformacniKroky = transformacniKroky;
	    }

	    /// <summary>
		/// Transliterované znìní pøepisu (originál)
		/// </summary>
		public string Transliterace {
			get { return mstrTransliterace; }
			set { mstrTransliterace = value; }
		}

		/// <summary>
		/// Transkribované znìní pøpeisu
		/// </summary>
		public string Transkripce {
			get { return mstrTranskripce; }
			set { mstrTranskripce = value; }
		}

		/// <summary>
		/// Poèet výskytù danéh pøepisu ve vìtším textovém celku
		/// </summary>
		public int Pocet {
			get { return mintPocet; }
			set { mintPocet = value; }
		}

		public Korelaty Korelaty {
			get { return mkrKorelaty; }
			set { mkrKorelaty = value; }
		}

	    public List<ITransformacniKrok> TransformacniKroky
	    {
	        get { return transformacniKroky; }
	        set { transformacniKroky = value; }
	    }
	}
}
