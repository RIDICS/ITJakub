using System.Collections.Generic;

namespace Daliboris.Slovniky {
  public class ZdrojeInfo : List<ZdrojInfo> {

	public ZdrojInfo DejZdrojPodleIdentifikatoru(int identifikator) {
	  foreach (ZdrojInfo zdrojInfo in this) {
		if (zdrojInfo.Identifikator == identifikator)
		  return zdrojInfo;
	  }
	  return null;
	}

	public ZdrojInfo DejZdrojPodleZkratky(string zkratka)
	{
	  foreach (ZdrojInfo zdrojInfo in this) {
		if (zdrojInfo.Zkratka == zkratka)
		  return zdrojInfo;
	  }
	  return null;
	}

	public ZdrojInfo DejZdrojPodleAkronymu(string akronym) {
	  foreach (ZdrojInfo zdrojInfo in this) {
		if (zdrojInfo.Zkratka == akronym)
		  return zdrojInfo;
	  }
	  return null;
	}

  	public ZdrojeInfo DejZakladniZdroje()
  	{
  		ZdrojeInfo zdroje = new ZdrojeInfo();
  		foreach (ZdrojInfo zdrojInfo in this)
  		{
  			if(!zdrojInfo.Pomocny) zdroje.Add(zdrojInfo);
  		}
  		return zdroje;
  	}

  }

}
