using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Daliboris.Transkripce.Objekty {
	[ComVisible(true)]
	public class AplikovanePravidlo : Pravidlo {
		public AplikovanePravidlo() : base()
		{
			
		}

		public AplikovanePravidlo(IPravidlo prvPravidlo, bool  blnJeAplikovane) : base(prvPravidlo.Transliterace, prvPravidlo.Transkripce, prvPravidlo.Jazyk)
		{
			JeAplikovane = blnJeAplikovane;
		}
		public bool JeAplikovane { get; set; }
	}
}
