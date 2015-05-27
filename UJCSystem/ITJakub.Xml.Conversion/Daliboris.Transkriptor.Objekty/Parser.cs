using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	/// <summary>
	/// Třída, která z transliterované podoby vytvoří na základě pravidel podobu transkribovanou.
	/// </summary>
	public class Parser {

		private Pravidla mprvPravidla;
		private Prepisy mprpPrepisy;

		public Parser() { }
		public Parser(Prepisy prpPrepisy, Pravidla prvPravidla) {
			mprpPrepisy = prpPrepisy;
			mprvPravidla = prvPravidla;
		}

		public Prepisy Prepisy {
			get { return mprpPrepisy; }
			set { mprpPrepisy = value; }
		}

		public Pravidla Pravidla {
			get { return mprvPravidla; }
			set { mprvPravidla = value; }
		}
		/// <summary>
		/// Projde seznam přepisů a pokusí se na základě transliterované podoby a sady pravidel
		/// vytvořit transkribovanou podobu, tj. vytvořit sadu korelátů.
		/// </summary>
		/// <param name="prpPrepisy">Přepisy určené k transkripci.</param>
		/// <param name="prvPravidla">Pravidla, která se mají na transliteraci použít.</param>
		public void Parsuj(Prepisy prpPrepisy, Pravidla prvPravidla) {
			mprpPrepisy = prpPrepisy;
			mprvPravidla = prvPravidla;
			Parsuj();
		}
		/// <summary>
		/// Projde seznam přepisů a pokusí se na základě transliterované podoby a sady pravidel
		/// vytvořit transkribovanou podobu, tj. vytvořit sadu korelátů.
		/// </summary>
		public void Parsuj() { 

		}
	}
}
