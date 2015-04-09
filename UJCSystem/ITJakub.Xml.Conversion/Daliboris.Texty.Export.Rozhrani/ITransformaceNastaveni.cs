using System;
using System.Collections.Generic;

namespace Daliboris.Texty.Export.Rozhrani
{
	public interface ITransformaceNastaveni
	{
		string VstupniSlozka { get; set; }
		string MaskaSoubru { get; set; }
		string VystupniSlozka { get; set; }
		string DocasnaSlozka { get; set; }
		string SlozkaXslt { get; set; }
		bool SmazatDocasneSoubory { get; set; }
		List<ITransformacniKrok> TransformacniKroky { get; set; }
    string TransformacniSoubor { get; set; }
		string TranskripcniSoubor { get; set; }
	}
}