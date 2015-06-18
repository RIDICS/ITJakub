using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daliboris.Texty.Evidence
{
    /// <summary>
    /// Kompletní informace o přepisu textu a jeho zpracování uložené v databázi
    /// </summary>
    public class Zaznam : Prepis
    {

        public Zaznam() : base() { }
        public Zaznam(Soubor sbSoubor) : base(sbSoubor) { }
        public Zaznam(Hlavicka hlHlavicka, Soubor sbSoubor) : base(hlHlavicka, sbSoubor) { }
        public Zaznam(Hlavicka hlHlavicka, Soubor sbSoubor, Zpracovani zpZpracovani) : base(hlHlavicka, sbSoubor, zpZpracovani) { }
        public DateTime PosledniZmena { get; set; }

    }
}
