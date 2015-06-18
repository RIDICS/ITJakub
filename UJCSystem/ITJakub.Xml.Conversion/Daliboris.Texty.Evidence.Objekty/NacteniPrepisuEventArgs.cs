namespace Daliboris.Texty.Evidence
{
    using System;

    public class NacteniPrepisuEventArgs : EventArgs
    {

        public NacteniPrepisuEventArgs() : base() { }
        public NacteniPrepisuEventArgs(int iPrepisuCelkem)
            : base()
        {
            PrepisuCelkem = iPrepisuCelkem;
        }
        public NacteniPrepisuEventArgs(int iPrepisuCelkem, int iAktualniPrepis, string sNazevSouboru)
            : base()
        {
            PrepisuCelkem = iPrepisuCelkem;
            AktualniPrepis = iAktualniPrepis;
            NazevSouboru = sNazevSouboru;
        }

        public string NazevSouboru { get; set; }
        public int PrepisuCelkem { get; set; }
        public int AktualniPrepis { get; set; }
    }

}
