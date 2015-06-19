namespace Daliboris.Texty.Export
{
	public class KrajniStrukturyBible
	{
		public KrajniStrukturyBible() {}

		public KrajniStrukturyBible(StrukturaBible sbZacatek, StrukturaBible sbKonec)
		{
			Zacatek = sbZacatek;
			Konec = sbKonec;
		}
		public StrukturaBible Zacatek { get; set; }
		public StrukturaBible Konec { get; set; }
	}
}