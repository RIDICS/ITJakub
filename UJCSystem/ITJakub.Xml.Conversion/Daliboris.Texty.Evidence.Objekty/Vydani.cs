using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Daliboris.Texty.Evidence.Rozhrani;

namespace Daliboris.Texty.Evidence
{
    public class Vydani : IVydani
    {

        private IList<IIsbn> iisbnList;
        private List<Isbn> isbnList;

        private IList<IAudiosoubor> iaudiosoubory;
        private List<Audiosoubor> audiosoubory;


        public Vydani()
        {
            iisbnList = new List<IIsbn>();
            isbnList = new List<Isbn>();
            iaudiosoubory = new List<IAudiosoubor>();
            audiosoubory = new List<Audiosoubor>();
        }

        public Vydani(IList<IIsbn> isbn)
            : this()
        {
            iisbnList = isbn;
            foreach (IIsbn item in isbn)
            {
                Isbn isb = new Isbn();
                isb.Format = item.Format;
                isb.Cislo = item.Cislo;
                isbnList.Add(isb);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Vydani(IList<IIsbn> iisbnList, IList<IAudiosoubor> iaudiosoubory) : this(iisbnList)
        {
            this.iaudiosoubory = iaudiosoubory;
            foreach (IAudiosoubor audiosoubor in iaudiosoubory)
            {
                Audiosoubor audio = new Audiosoubor();
                audio.Delka = audiosoubor.Delka;
                audio.Format = audiosoubor.Format;
                audio.Nazev = audiosoubor.Nazev;
	            audio.Titul = audiosoubor.Titul;
                audiosoubory.Add(audio);
            }
        }


        public string Titul { get; set; }


        public List<Isbn> EvidencniCisla
        {
            get { return isbnList; }
            set { isbnList = value; }
        }

        IList<IIsbn> IVydani.EvidencniCisla
        {
            get
            {
                return iisbnList;
            }
            set { iisbnList = value; }
        }


        private void Synchronizovat(IList<IIsbn> rozhrani, List<Isbn> trida)
        {

        }

        public ZpusobVyuziti ZpusobVyuziti { get; set; }

        public string Vroceni { get; set; }

        public int PocetStran { get; set; }

        /// <summary>
        /// Rozsah stran, který byl ve výstupu publikován (zejména u úryvků v rámci audioknihy)
        /// </summary>
        public string Rozsah { get; set; }

        /// <summary>
        /// Interpreti, kteří se podíleli na vytvoření audioknihy.
        /// </summary>
        [XmlArrayItem("Interpret")]
        public List<string> Interpreti { get; set; }

        /// <summary>
        /// Seznam audiosoubrů obsahujících nahrávku.
        /// </summary>
        [XmlArrayItem("Audiosoubor")]
        public List<Audiosoubor> Audiosoubory
        {
            get { return audiosoubory; }
            set { audiosoubory = value; }
        }

        /// <summary>
        /// Pořadí textu v rámci audioknihy
        /// </summary>
        public int Poradi { get; set; }

        IList<IAudiosoubor> IVydani.Audiosoubory
        {
            get
            {
                if(iaudiosoubory == null)
                    iaudiosoubory = new List<IAudiosoubor>();
                if (audiosoubory.Count != iaudiosoubory.Count)
                {
                    iaudiosoubory = audiosoubory.Cast<IAudiosoubor>().ToList();
                }
                return iaudiosoubory;
            }
            set { iaudiosoubory = value; }
        }
    }
}
