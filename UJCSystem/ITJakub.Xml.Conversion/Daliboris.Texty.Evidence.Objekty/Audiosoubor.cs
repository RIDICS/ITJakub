using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
    public class Audiosoubor : IAudiosoubor
    {
        private TimeSpan _delka;

        #region Implementation of IAudiosoubor

        /// <summary>
        /// Název souboru včetně přípony
        /// </summary>
        public string Nazev { get; set; }

        /// <summary>
        /// Název formátu (MP3, OGG ap.)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Délka audionahrávky
        /// </summary>
        [XmlIgnore]
        public TimeSpan Delka
        {
            get { return _delka; }
            set { _delka = value; }
        }

        /// <summary>
        /// Interpreti, kteří se podíleli na vytvoření audioknihy.
        /// </summary>
        [XmlArrayItem("Interpret")]
        public List<string> Interpreti { get; set; }

        /// <summary>
        /// Délka audionahrávky v milisekundách
        /// </summary>
        [XmlAttribute("milisekund")]
        public long DelkaTicks
        {
            get { return _delka.Ticks; }
            set { _delka = new TimeSpan(value); }
        }

        public string Stopaz
        {
            get { return _delka.ToString(); }
            set { ; }
        }

				public string Titul { get; set; }

        #endregion
    }
}