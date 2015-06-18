using System;
using System.Collections.Generic;

namespace Daliboris.Texty.Evidence.Rozhrani
{
    /// <summary>
    /// Vlastnosti audiosouboru
    /// </summary>
    public interface IAudiosoubor
    {
        /// <summary>
        /// Název souboru včetně přípony
        /// </summary>
        string Nazev { get; set; }
        /// <summary>
        /// Název formátu (MP3, OGG ap.)
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Délka audionahrávky
        /// </summary>
        TimeSpan Delka { get; set; }

        /// <summary>
        /// Interpreti, kteří se podíleli na vytvoření audioknihy.
        /// </summary>
        List<string> Interpreti { get; set; }

			/// <summary>
			/// Titul audiostopy, pokud je znám
			/// </summary>
				string Titul { get; set; }

    }
}